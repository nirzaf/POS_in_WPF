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
    public class TicketVoid : DataModelBase
    {
        #region Licensed Access Only
        static TicketVoid()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DirectDepositTransaction).Assembly.GetName().GetPublicKeyToken(),
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
        public int? TicketItemId
        {
            get;
            private set;
        }

        [ModeledData("Time")]
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

        private TicketVoid(YearId primaryKey, int employeeId, int ticketId,
            int? ticketItemId, DateTime when, double amount)
        {
            PrimaryKey = primaryKey;
            EmployeeId = employeeId;
            TicketId = ticketId;
            TicketItemId = ticketItemId;
            When = when;
            Amount = amount;
        }

        #region static
        /// <summary>
        /// Add a new entry to the TicketVoid table
        /// </summary>
        public static TicketVoid Add(int employeeId, YearId ticketPrimaryKey,
            int? ticketItemId, double amount)
        {
            TicketVoid result = null;
            DateTime voidTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketVoid";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketVoidYear", SqlDbType.Int, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketVoidEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@TicketVoidTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketVoidTicketItemId", SqlDbType.Int, ticketItemId);
                BuildSqlParameter(sqlCmd, "@TicketVoidTime", SqlDbType.DateTime, voidTime);
                BuildSqlParameter(sqlCmd, "@TicketVoidAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@TicketVoidId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketVoid(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketVoidId"].Value)),
                        employeeId, ticketPrimaryKey.Id, ticketItemId, voidTime, amount);
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketVoid,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketVoidId FROM TicketVoid", cn))
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
        /// Get an entry from the TicketVoid table
        /// </summary>
        public static TicketVoid Get(YearId primaryKey)
        {
            TicketVoid result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketVoid Get(SqlConnection cn, YearId primaryKey)
        {
            TicketVoid result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketVoid WHERE (TicketVoidId=" + primaryKey.Id + " AND TicketVoidYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketVoid(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketVoid table
        /// </summary>
        public static IEnumerable<TicketVoid> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketVoid", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketVoid(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketVoid table
        /// </summary>
        public static IEnumerable<TicketVoid> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketVoid WHERE (TicketVoidTime >= @TicketVoidSearchStartTime AND TicketVoidTime <= @TicketVoidSearchEndTime)", cn))
            {
                BuildSqlParameter(cmd, "@TicketVoidSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketVoidSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketVoid(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketVoid))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketVoid WHERE TicketVoidId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketVoid object from a SqlDataReader object
        /// </summary>
        private static TicketVoid BuildTicketVoid(SqlDataReader rdr)
        {
            return new TicketVoid(
                GetPrimaryKey(rdr),
                GetEmployeeId(rdr),
                GetTicketId(rdr),
                GetTicketItemId(rdr),
                GetWhen(rdr),
                GetAmount(rdr));
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

        private static int? GetTicketItemId(SqlDataReader rdr)
        {
            int? ticketItemId = null;
            if (!rdr.IsDBNull(4))
                ticketItemId = rdr.GetInt32(4);
            return ticketItemId;
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(5);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }
        #endregion

    }
}
