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
    public class TicketItemReturn : DataModelBase
    {
        #region Licensed Access Only
        static TicketItemReturn()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketItemReturn).Assembly.GetName().GetPublicKeyToken(),
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
        public int TicketId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int ItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public int ItemQuantity
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

        [ModeledData()]
        public DateTime When
        {
            get;
            private set;
        }

        private TicketItemReturn(YearId primaryKey, int registerDrawerId, int employeeId,
            int ticketId, int itemId, int itemQuantity, double amount, DateTime when)
        {
            PrimaryKey = primaryKey;
            RegisterDrawerId = registerDrawerId;
            EmployeeId = employeeId;
            TicketId = ticketId;
            ItemId = itemId;
            ItemQuantity = itemQuantity;
            Amount = amount;
            When = when;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the TicketItemReturn table
        /// </summary>
        public static TicketItemReturn Add(int registerDrawerId, int employeeId,
            YearId ticketPrimaryKey, int itemId, int itemQuantity, double amount)
        {
            TicketItemReturn result = null;
            DateTime when = DateTime.Now;

            itemQuantity = itemQuantity.Clamp(1, short.MaxValue);

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketItemReturn";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemReturnYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnItemQuantity", SqlDbType.SmallInt, itemQuantity);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnWhen", SqlDbType.DateTime, when);
                BuildSqlParameter(sqlCmd, "@TicketItemReturnId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketItemReturn(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketItemReturnId"].Value)),
                        registerDrawerId, employeeId, ticketPrimaryKey.Id, itemId, itemQuantity,
                        amount, when);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketItemReturn,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketItemReturnId FROM TicketItemReturn", cn))
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
        /// Get an entry from the TicketItemReturn table
        /// </summary>
        public static TicketItemReturn Get(YearId primaryKey)
        {
            TicketItemReturn result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketItemReturn Get(SqlConnection cn, YearId primaryKey)
        {
            TicketItemReturn result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItemReturn WHERE (TicketItemReturnId=" + primaryKey.Id + " AND TicketItemReturnYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketItemReturn(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketItemReturn table
        /// </summary>
        public static IEnumerable<TicketItemReturn> GetAllForTicket(YearId ticketPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd =
                    new SqlCommand("SELECT * FROM TicketItemReturn WHERE (" +
                        "TicketItemReturnYear=" + ticketPrimaryKey.Year + " AND " +
                        "TicketItemReturnTicketId=" + ticketPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItemReturn(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketItemReturn table
        /// </summary>
        public static IEnumerable<TicketItemReturn> GetAll(int? registerDrawerId = null)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                if (registerDrawerId == null)
                    cmd = new SqlCommand("SELECT * FROM TicketItemReturn", cn);
                else
                    cmd = new SqlCommand("SELECT * FROM TicketItemReturn WHERE TicketItemReturnRegisterDrawerId=" + registerDrawerId.Value, cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItemReturn(rdr);
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                FinishedWithConnection(cn);
            }
        }

        /// <summary>
        /// Get all the entries in the TicketItemReturn table
        /// </summary>
        public static IEnumerable<TicketItemReturn> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItemReturn WHERE (TicketItemReturnWhen >= @TicketItemReturnSearchStartTime AND TicketItemReturnWhen <= @TicketItemReturnSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@TicketItemReturnSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemReturnSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItemReturn(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketItemReturn))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketItemReturn WHERE TicketItemReturnId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketItemReturn object from a SqlDataReader object
        /// </summary>
        private static TicketItemReturn BuildTicketItemReturn(SqlDataReader rdr)
        {
            return new TicketItemReturn(
                GetPrimaryKey(rdr),
                GetRegisterDrawerId(rdr),
                GetEmployeeId(rdr),
                GetTicketPrimaryKey(rdr),
                GetItemId(rdr),
                GetItemQuantity(rdr),
                GetAmount(rdr),
                GetWhen(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetRegisterDrawerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int GetTicketPrimaryKey(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }

        private static int GetItemQuantity(SqlDataReader rdr)
        {
            return rdr.GetInt16(6);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(7);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(8);
        }
        #endregion

    }
}
