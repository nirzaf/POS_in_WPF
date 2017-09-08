using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class GiftCard : DataModelBase
    {
        #region Licensed Access Only
        static GiftCard()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(GiftCard).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData("Name")]
        public string CustomerName
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Balance
        {
            get;
            private set;
        }

        [ModeledData()]
        public int AuthorizingEmployeeId
        {
            get;
            private set;
        }

        [ModeledData("PurchaseTicketId")]
        public int PurchasingTicketId
        {
            get;
            private set;
        }

        [ModeledData()]
        public byte[] ScanData
        {
            get;
            private set;
        }

        private GiftCard(int id, string customerName, double balance, int authorizingEmployeeId,
            int purchasingTicketId, byte[] scanData)
        {
            Id = id;
            CustomerName = customerName;
            Balance = balance;
            AuthorizingEmployeeId = authorizingEmployeeId;
            PurchasingTicketId = purchasingTicketId;
            ScanData = scanData;
        }

        #region static
        /// <summary>
        /// Add a new entry to the GiftCard table
        /// </summary>
        public static GiftCard Add(string customerName, double balance, int authorizingEmployeeId,
            int purchasingTicketId)
        {
            GiftCard result = null;
            byte[] scanCode = GenerateScanCodeData();

            SqlConnection cn = GetConnection();
            string cmd = "AddGiftCard";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@GiftCardName", SqlDbType.Text, customerName);
                BuildSqlParameter(sqlCmd, "@GiftCardBalance", SqlDbType.Float, balance);
                BuildSqlParameter(sqlCmd, "@GiftCardAuthorizingEmployeeId", SqlDbType.Int, authorizingEmployeeId);
                BuildSqlParameter(sqlCmd, "@GiftCardPurchaseTicketId", SqlDbType.Int, purchasingTicketId);
                BuildSqlParameter(sqlCmd, "@GiftCardScanData", SqlDbType.Binary, scanCode);
                BuildSqlParameter(sqlCmd, "@GiftCardId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new GiftCard(Convert.ToInt32(sqlCmd.Parameters["@GiftCardId"].Value),
                        customerName, balance, authorizingEmployeeId, purchasingTicketId, scanCode);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the GiftCard table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            GiftCard giftCard = Get(cn, id);
            if (giftCard != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM GiftCard WHERE GiftCardId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the GiftCard table
        /// </summary>
        public static GiftCard Get(int id)
        {
            GiftCard result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static GiftCard Get(SqlConnection cn, int id)
        {
            GiftCard result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM GiftCard WHERE GiftCardId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildGiftCard(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the GiftCard table
        /// </summary>
        public static IEnumerable<GiftCard> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM GiftCard", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildGiftCard(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the GiftCard table
        /// </summary>
        public static bool Update(GiftCard giftCard)
        {
            bool result = false;
            SqlConnection cn = GetConnection();
            result = Update(cn, giftCard);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, GiftCard giftCard)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE GiftCard SET GiftCardName=@GiftCardName,GiftCardBalance=@GiftCardBalance,GiftCardAuthorizingEmployeeId=@GiftCardAuthorizingEmployeeId,GiftCardPurchaseTicketId=@GiftCardPurchaseTicketId,GiftCardScanData=@GiftCardScanData WHERE GiftCardId=@GiftCardId";

                BuildSqlParameter(sqlCmd, "@GiftCardId", SqlDbType.Int, giftCard.Id);
                BuildSqlParameter(sqlCmd, "@GiftCardName", SqlDbType.Text, giftCard.CustomerName);
                BuildSqlParameter(sqlCmd, "@GiftCardBalance", SqlDbType.Float, giftCard.Balance);
                BuildSqlParameter(sqlCmd, "@GiftCardAuthorizingEmployeeId", SqlDbType.Int, giftCard.AuthorizingEmployeeId);
                BuildSqlParameter(sqlCmd, "@GiftCardPurchaseTicketId", SqlDbType.Int, giftCard.PurchasingTicketId);
                BuildSqlParameter(sqlCmd, "@GiftCardScanData", SqlDbType.Binary, giftCard.ScanData);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static byte[] GenerateScanCodeData()
        {
            byte[] scanCode = new byte[520];
            Random random = new Random();
            random.NextBytes(scanCode);
            return scanCode;
        }

        /// <summary>
        /// Build a GiftCard object from a SqlDataReader object
        /// </summary>
        private static GiftCard BuildGiftCard(SqlDataReader rdr)
        {
            return new GiftCard(
                GetId(rdr),
                GetCustomerName(rdr),
                GetBalance(rdr),
                GetEmployeeId(rdr),
                GetTicketId(rdr),
                GetScanData(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetCustomerName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static double GetBalance(SqlDataReader rdr)
        {
            return rdr.GetDouble(2);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int GetTicketId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static byte[] GetScanData(SqlDataReader rdr)
        {
            return rdr.GetSqlBinary(5).Value;
        }
        #endregion

    }
}
