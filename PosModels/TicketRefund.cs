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
    public class TicketRefund : DataModelBase
    {
        #region Licensed Access Only
        static TicketRefund()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketRefund).Assembly.GetName().GetPublicKeyToken(),
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
        public int RegisterDrawerId
        {
            get;
            private set;
        }

        [ModeledData("Date")]
        public DateTime When
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

        [ModeledData("Status")]
        [ModeledDataType("TINYINT")]
        public TicketRefundType Type
        {
            get;
            private set;
        }

        private TicketRefund(YearId primaryKey, int employeeId, int ticketId,
            int registerDrawerId, DateTime when, double amount,
            TicketRefundType refundType)
        {
            PrimaryKey = primaryKey;
            EmployeeId = employeeId;
            TicketId = ticketId;
            RegisterDrawerId = registerDrawerId;
            When = when;
            Amount = amount;
            Type = refundType;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the TicketRefund table
        /// </summary>
        public static TicketRefund Add(YearId ticketPrimaryKey, int employeeId,
            int registerDrawerId, double amount, TicketRefundType refundType)
        {
            TicketRefund result = null;
            DateTime refundTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketRefund";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketRefundYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketRefundEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@TicketRefundTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketRefundRegisterDrawerId", SqlDbType.Int, registerDrawerId);
                BuildSqlParameter(sqlCmd, "@TicketRefundDate", SqlDbType.DateTime, refundTime);
                BuildSqlParameter(sqlCmd, "@TicketRefundAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@TicketRefundStatus", SqlDbType.TinyInt, refundType);
                BuildSqlParameter(sqlCmd, "@TicketRefundId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketRefund(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketRefundId"].Value)),
                        employeeId, ticketPrimaryKey.Id, registerDrawerId, refundTime, amount, refundType);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketRefund,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketRefundId FROM TicketRefund", cn))
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
        /// Get an entry from the TicketRefund table
        /// </summary>
        public static TicketRefund Get(YearId primaryKey)
        {
            TicketRefund result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketRefund Get(SqlConnection cn, YearId primaryKey)
        {
            TicketRefund result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketRefund WHERE (TicketRefundId=" + primaryKey.Id + " AND TicketRefundYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketRefund(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketRefund table
        /// </summary>
        public static IEnumerable<TicketRefund> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketRefund", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketRefund(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketRefund table
        /// </summary>
        public static IEnumerable<TicketRefund> GetAll(int registerDrawerId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketRefund WHERE TicketRefundRegisterDrawerId=" + registerDrawerId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketRefund(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<TicketRefund> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketRefund WHERE (TicketRefundDate >= @TicketRefundSearchStartTime AND TicketRefundDate <= @TicketRefundSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@TicketRefundSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketRefundSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketRefund(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketRefund))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketRefund WHERE TicketRefundId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketRefund object from a SqlDataReader object
        /// </summary>
        private static TicketRefund BuildTicketRefund(SqlDataReader rdr)
        {
            return new TicketRefund(
                GetPrimaryKey(rdr),
                GetEmployeeId(rdr),
                GetTicketId(rdr),
                GetRegisterDrawerId(rdr),
                GetWhen(rdr),
                GetAmount(rdr),
                GetRefundType(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetTicketId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int GetRegisterDrawerId(SqlDataReader rdr)
        {
            return rdr.GetInt32(4);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(5);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }

        private static TicketRefundType GetRefundType(SqlDataReader rdr)
        {
            return rdr.GetByte(7).GetTicketRefundType();
        }
        #endregion

    }
}
