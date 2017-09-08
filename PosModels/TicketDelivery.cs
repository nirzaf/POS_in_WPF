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
    public class TicketDelivery : DataModelBase
    {
        #region Licensed Access Only
        static TicketDelivery()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketDelivery).Assembly.GetName().GetPublicKeyToken(),
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
        public int DeliveryDriverId
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
        public DateTime DepartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? ReturnTime
        {
            get;
            private set;
        }

        private TicketDelivery(YearId primaryKey, int ticketId, int employeeId,
            DateTime departTime, DateTime? returnTime)
        {
            PrimaryKey = primaryKey;
            DeliveryDriverId = employeeId;
            TicketId = ticketId;
            DepartTime = departTime;
            ReturnTime = returnTime;
        }

        #region static
        /// <summary>
        /// Add a new entry to the TicketDelivery table
        /// </summary>
        public static TicketDelivery Add(YearId ticketPrimaryKey, int deliveryDriverId)
        {
            TicketDelivery result = null;
            DateTime departTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketDelivery";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketDeliveryYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryDeliveryDriverId", SqlDbType.Int, deliveryDriverId);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryDepartTime", SqlDbType.DateTime, departTime);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketDelivery(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketDeliveryId"].Value)),
                        ticketPrimaryKey.Id, deliveryDriverId, departTime, null);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the TicketDelivery table
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            TicketDelivery deliveryDispatch = Get(cn, primaryKey);
            if (deliveryDispatch != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM TicketDelivery WHERE (TicketDeliveryId=" + primaryKey.Id + " AND TicketDeliveryYear=" + primaryKey.Year + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the TicketDelivery table, by ticket id
        /// </summary>
        public static void DeleteByTicket(YearId ticketPrimaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketDelivery WHERE (" +
                    "TicketDeliveryYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketDeliveryTicketId=" + ticketPrimaryKey.Id + ")";
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return;
        }

        /// <summary>
        /// Get an entry from the TicketDelivery table
        /// </summary>
        public static TicketDelivery Get(YearId primaryKey)
        {
            TicketDelivery result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketDelivery Get(SqlConnection cn, YearId primaryKey)
        {
            TicketDelivery result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDelivery WHERE (TicketDeliveryId=" + primaryKey.Id + " AND TicketDeliveryYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketDelivery(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketDelivery table
        /// </summary>
        public static IEnumerable<TicketDelivery> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDelivery", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketDelivery(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketDelivery table
        /// </summary>
        public static IEnumerable<TicketDelivery> GetAllActive()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDelivery WHERE TicketDeliveryReturnTime IS NULL", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketDelivery(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the TicketDelivery table
        /// </summary>
        public static bool Update(TicketDelivery deliveryDispatch)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, deliveryDispatch);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, TicketDelivery deliveryDispatch)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketDelivery SET TicketDeliveryEmployeeId=@TicketDeliveryEmployeeId,TicketDeliveryTicketId=@TicketDeliveryTicketId,TicketDeliveryDepartTime=@TicketDeliveryDepartTime,TicketDeliveryReturnTime=@TicketDeliveryReturnTime WHERE (TicketDeliveryId=@TicketDeliveryId AND TicketDeliveryYear=@TicketDeliveryYear)";

                BuildSqlParameter(sqlCmd, "@TicketDeliveryYear", SqlDbType.SmallInt, deliveryDispatch.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryId", SqlDbType.Int, deliveryDispatch.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryDeliveryDriverId", SqlDbType.Int, deliveryDispatch.DeliveryDriverId);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryTicketId", SqlDbType.Int, deliveryDispatch.TicketId);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryDepartTime", SqlDbType.DateTime, deliveryDispatch.DepartTime);
                BuildSqlParameter(sqlCmd, "@TicketDeliveryReturnTime", SqlDbType.DateTime, deliveryDispatch.ReturnTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a TicketDelivery object from a SqlDataReader object
        /// </summary>
        private static TicketDelivery BuildTicketDelivery(SqlDataReader rdr)
        {
            return new TicketDelivery(
                GetPrimaryKey(rdr),
                GetTicketId(rdr),
                GetDeliveryDriverId(rdr),
                GetDepartureTime(rdr),
                GetReturnTime(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetTicketId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetDeliveryDriverId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static DateTime GetDepartureTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(4);
        }

        private static DateTime? GetReturnTime(SqlDataReader rdr)
        {
            DateTime? returnTime = null;
            if (rdr.IsDBNull(5))
                returnTime = rdr.GetDateTime(5);
            return returnTime;
        }
        #endregion

    }
}
