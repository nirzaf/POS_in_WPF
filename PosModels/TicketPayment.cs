using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;
using TemposLibrary;

namespace PosModels
{
    [ModeledDataClass()]
    public class TicketPayment : DataModelBase
    {
        #region Licensed Access Only
        static TicketPayment()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketPayment).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData("Year", "Id")]
        [ModeledDataType("SMALLINT", "INT")]
        public YearId PrimaryKey
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TicketId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int RegisterDrawerId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Amount
        {
            get;
            private set;
        }

        [ModeledData("Type")]
        [ModeledDataType("TINYINT")]
        public PaymentSource PaymentType
        {
            get;
            private set;
        }

        [ModeledData("Time")]
        public DateTime TransactionTime
        {
            get;
            private set;
        }

        [ModeledData("CardInfo")]
        [ModeledDataType("VARBINARY")]
        [ModeledDataNullable()]
        public byte[] SerializedCardInfo
        {
            get;
            private set;
        }

        private TicketPayment(YearId primaryKey, int ticketId, int registerDrawerId,
            int employeeId, double amount, PaymentSource type, DateTime transactionTime,
            byte[] serializedCardInfo)
        {
            PrimaryKey = primaryKey;
            TicketId = ticketId;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            Amount = amount;
            PaymentType = type;
            TransactionTime = transactionTime;
            SerializedCardInfo = serializedCardInfo;
        }

        public void SetCreditCardInfo(CreditCardInfo creditCardInfo)
        {
#if !DEMO
            byte[] decryptedBytes = creditCardInfo.SerializeObject();
            SerializedCardInfo = AESHelper.Encrypt(decryptedBytes, "ThePriceIsRight");
#else
            SerializedCardInfo = creditCardInfo.SerializeObject();
#endif
        }

        public CreditCardInfo GetCreditCardInfo()
        {
#if !DEMO
            byte[] decryptedBytes = AESHelper.Decrypt(SerializedCardInfo, "ThePriceIsRight");
            return decryptedBytes.DeserializeObject() as CreditCardInfo;
#else
            return SerializedCardInfo.DeserializeObject() as CreditCardInfo;
#endif
        }

        #region static
        /// <summary>
        /// Add a new entry to the TicketPayment table
        /// </summary>
        public static TicketPayment Add(YearId ticketPrimaryKey, int registerDrawerId,
            int employeeId, double amount, PaymentSource type, CreditCardInfo cardInfo = null)
        {
            TicketPayment result = null;
            DateTime now = DateTime.Now;
            byte[] encryptedCardInfo = null;
            if (cardInfo != null)
            {
#if !DEMO
                byte[] decryptedBytes = cardInfo.SerializeObject();
                encryptedCardInfo = AESHelper.Encrypt(decryptedBytes, "ThePriceIsRight");
#else
                encryptedCardInfo = cardInfo.SerializeObject();
#endif
            }

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketPayment";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketPaymentYear", SqlDbType.Int, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketPaymentTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketPaymentRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@TicketPaymentEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@TicketPaymentAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@TicketPaymentType", SqlDbType.TinyInt, type);
                BuildSqlParameter(sqlCmd, "@TicketPaymentTime", SqlDbType.DateTime, now);
                BuildSqlParameter(sqlCmd, "@TicketPaymentCardInfo", SqlDbType.VarBinary, encryptedCardInfo);
                BuildSqlParameter(sqlCmd, "@TicketPaymentId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketPayment(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketPaymentId"].Value)),
                        ticketPrimaryKey.Id, registerDrawerId, employeeId, amount,
                        type, now, encryptedCardInfo);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketPayment,RESEED,0)";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool TableIsEmpty()
        {
            bool foundEntry = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketPaymentId FROM TicketPayment", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundEntry = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return !foundEntry;
        }

        /// <summary>
        /// Returns the number of table entries for the specified year/id
        /// </summary>
        public static int Count(YearId ticketPrimaryKey)
        {
            int count = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TicketPayment WHERE (" +
                    "TicketPaymentYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketPaymentTicketId=" + ticketPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        count = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);
            return count;
        }

        /// <summary>
        /// Delete an entry from the TicketPayment table
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            TicketPayment ticketPayment = Get(cn, primaryKey);
            if (ticketPayment != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM TicketPayment WHERE (TicketPaymentId=" + primaryKey.Id + " AND TicketPaymentYear=" + primaryKey.Year + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public static void DeleteAll(YearId ticketPrimaryKey)
        {
            IEnumerable<TicketPayment> payments = TicketPayment.GetAll(ticketPrimaryKey);
            foreach (TicketPayment payment in payments)
            {
                TicketPayment.Delete(payment.PrimaryKey);
            }
        }

        /// <summary>
        /// Get an entry from the TicketPayment table
        /// </summary>
        public static TicketPayment Get(YearId primaryKey)
        {
            TicketPayment result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketPayment Get(SqlConnection cn, YearId primaryKey)
        {
            TicketPayment result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketPayment WHERE (TicketPaymentId=" + primaryKey.Id + " AND TicketPaymentYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketPayment(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketPayment table
        /// </summary>
        public static IEnumerable<TicketPayment> GetAll(YearId ticketPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketPayment WHERE (" +
                    "TicketPaymentYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketPaymentTicketId=" + ticketPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketPayment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<TicketPayment> GetAllForTickets(IEnumerable<Ticket> tickets)
        {
            foreach (Ticket ticket in tickets)
            {
                foreach (TicketPayment ticketPayment in TicketPayment.GetAll(ticket.PrimaryKey))
                {
                    yield return ticketPayment;
                }
            }
        }

        /// <summary>
        /// Get all the entries in the TicketPayment table
        /// </summary>
        public static IEnumerable<TicketPayment> GetAllForRegisterDrawer(int registerDrawerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketPayment WHERE TicketPaymentRegisterDrawerId=" + registerDrawerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketPayment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the TicketPayment table
        /// </summary>
        public static bool Update(TicketPayment ticketPayment)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, ticketPayment);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, TicketPayment ticketPayment)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketPayment SET TicketPaymentTicketId=@TicketPaymentTicketId,TicketPaymentRegisterDrawerId=@TicketPaymentRegisterDrawerId,TicketPaymentEmployeeId=@TicketPaymentEmployeeId,TicketPaymentAmount=@TicketPaymentAmount,TicketPaymentType=@TicketPaymentType,TicketPaymentTime=@TicketPaymentTime,TicketPaymentCardInfo=@TicketPaymentCardInfo WHERE (TicketPaymentId=@TicketPaymentId AND TicketPaymentYear=@TicketPaymentYear)";

                BuildSqlParameter(sqlCmd, "@TicketPaymentYear", SqlDbType.SmallInt, ticketPayment.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketPaymentId", SqlDbType.Int, ticketPayment.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketPaymentTicketId", SqlDbType.Int, ticketPayment.TicketId);
                BuildSqlParameter(sqlCmd, "@TicketPaymentRegisterDrawerId", SqlDbType.Int, ticketPayment.RegisterDrawerId);
                BuildSqlParameter(sqlCmd, "@TicketPaymentEmployeeId", SqlDbType.Int, ticketPayment.EmployeeId);
                BuildSqlParameter(sqlCmd, "@TicketPaymentAmount", SqlDbType.Float, ticketPayment.Amount);
                BuildSqlParameter(sqlCmd, "@TicketPaymentType", SqlDbType.TinyInt, ticketPayment.PaymentType);
                BuildSqlParameter(sqlCmd, "@TicketPaymentTime", SqlDbType.DateTime, ticketPayment.TransactionTime);
                BuildSqlParameter(sqlCmd, "@TicketPaymentCardInfo", SqlDbType.VarBinary, ticketPayment.SerializedCardInfo);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketPayment))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketPayment WHERE TicketPaymentId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketPayment object from a SqlDataReader object
        /// </summary>
        private static TicketPayment BuildTicketPayment(SqlDataReader rdr)
        {
            return new TicketPayment(
                GetPrimaryKey(rdr),
                GetTicketId(rdr),
                GetRegisterId(rdr),
                GetEmployeeId(rdr),
                GetAmount(rdr),
                GetPaymentSource(rdr),
                GetTransactionTime(rdr),
                GetSerializedCardInfo(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetTicketId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetRegisterId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(5);
        }

        private static PaymentSource GetPaymentSource(SqlDataReader rdr)
        {
            return rdr.GetByte(6).GetPaymentSource();
        }

        private static DateTime GetTransactionTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(7);
        }

        private static byte[] GetSerializedCardInfo(SqlDataReader rdr)
        {
            SqlBytes serialData = rdr.GetSqlBytes(8);
            if (serialData.Buffer == null)
                return null;
            return (byte[])serialData.Buffer.Clone();
        }
        #endregion

    }
}
