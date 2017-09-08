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
    public class TicketCoupon : DataModelBase
    {
        #region Licensed Access Only
        static TicketCoupon()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketCoupon).Assembly.GetName().GetPublicKeyToken(),
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
        public int CouponId
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

        private TicketCoupon(YearId primaryKey, int couponId, int ticketId,
            int? ticketItemId)
        {
            PrimaryKey = primaryKey;
            CouponId = couponId;
            TicketId = ticketId;
            TicketItemId = ticketItemId;
        }

        /// <summary>
        /// Get the coupon description of this TicketCoupon.
        /// </summary>
        public string GetCouponDescription()
        {
            string result = null;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT CouponDescription FROM Coupon WHERE CouponId=" + CouponId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = rdr[0].ToString();
                    }
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        #region static
        /// <summary>
        /// Add a new entry to the TicketCoupon table, for a Ticket
        /// </summary>
        public static TicketCoupon AddForTicket(int couponId, YearId ticketPrimaryKey)
        {
            TicketCoupon result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketCoupon";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketCouponYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketCouponCouponId", SqlDbType.Int, couponId);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketItemId", SqlDbType.Int, null);
                BuildSqlParameter(sqlCmd, "@TicketCouponId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketCoupon(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketCouponId"].Value)),
                        couponId, ticketPrimaryKey.Id, 0);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Add a new entry to the TicketCoupon table, for a TicketItem
        /// </summary>
        public static TicketCoupon AddForTicketItem(int couponId, YearId ticketPrimaryKey, int ticketItemId)
        {
            TicketCoupon result = null;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketCoupon";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketCouponYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketCouponCouponId", SqlDbType.Int, couponId);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketItemId", SqlDbType.Int, ticketItemId);
                BuildSqlParameter(sqlCmd, "@TicketCouponId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketCoupon(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketCouponId"].Value)),
                        couponId, ticketPrimaryKey.Id, ticketItemId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketCoupon,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketCouponId FROM TicketCoupon", cn))
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
        /// Delete an entry from the TicketCoupon table
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            TicketCoupon ticketCoupon = Get(cn, primaryKey);
            if (ticketCoupon != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM TicketCoupon WHERE (TicketCouponId=" + primaryKey.Id + " AND TicketCouponYear=" + primaryKey.Year + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the TicketCoupon table
        /// </summary>
        public static bool DeleteForTicketItem(YearId ticketItemPrimaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketCoupon WHERE (" +
                    "TicketCouponTicketItemId=" + ticketItemPrimaryKey.Id + " AND " +
                    "TicketCouponYear=" + ticketItemPrimaryKey.Year + ")";
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the TicketCoupon table
        /// </summary>
        public static TicketCoupon Get(YearId primaryKey)
        {
            TicketCoupon result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketCoupon Get(SqlConnection cn, YearId primaryKey)
        {
            TicketCoupon result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketCoupon WHERE (TicketCouponId=" + primaryKey.Id + " AND TicketCouponYear=" + primaryKey.Year + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketCoupon(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketCoupon table
        /// </summary>
        public static IEnumerable<TicketCoupon> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketCoupon", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketCoupon(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketCoupon table for a specific ticket
        /// </summary>
        public static IEnumerable<TicketCoupon> GetAll(YearId ticketPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketCoupon WHERE " +
                    "(TicketCouponYear=" + ticketPrimaryKey.Year +
                    " AND TicketCouponTicketId=" + ticketPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketCoupon(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketCoupon table for a specififc tickets
        /// </summary>
        public static IEnumerable<TicketCoupon> GetAllForTickets(IEnumerable<Ticket> tickets)
        {
            if (tickets != null)
            {
                foreach (Ticket ticket in tickets)
                {
                    IEnumerable<TicketCoupon> coupons = TicketCoupon.GetAll(ticket.PrimaryKey);
                    if (coupons == null)
                        continue;
                    foreach (TicketCoupon coupon in coupons)
                    {
                        yield return coupon;
                    }
                }
            }
        }

        /// <summary>
        /// Get all the entries in the TicketCoupon table for a specififc ticket item
        /// </summary>
        public static IEnumerable<TicketCoupon> GetAllForTicketItem(YearId ticketItemPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketCoupon WHERE " +
                    "(TicketCouponYear=" + ticketItemPrimaryKey.Year + " AND " +
                    "TicketCouponTicketItemId=" + ticketItemPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketCoupon(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the TicketCoupon table
        /// </summary>
        public static bool Update(TicketCoupon ticketCoupon)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, ticketCoupon);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, TicketCoupon ticketCoupon)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketCoupon SET TicketCouponCouponId=@TicketCouponCouponId,TicketCouponTicketId=@TicketCouponTicketId,TicketCouponTicketItemId=@TicketCouponTicketItemId WHERE (TicketCouponId=@TicketCouponId AND TicketCouponYear=@TicketCouponYear)";

                BuildSqlParameter(sqlCmd, "@TicketCouponYear", SqlDbType.Int, ticketCoupon.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketCouponId", SqlDbType.Int, ticketCoupon.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketCouponCouponId", SqlDbType.Int, ticketCoupon.CouponId);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketId", SqlDbType.Int, ticketCoupon.TicketId);
                BuildSqlParameter(sqlCmd, "@TicketCouponTicketItemId", SqlDbType.Int, ticketCoupon.TicketItemId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketCoupon))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketCoupon WHERE TicketCouponId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketCoupon object from a SqlDataReader object
        /// </summary>
        private static TicketCoupon BuildTicketCoupon(SqlDataReader rdr)
        {
            return new TicketCoupon(
                GetPrimaryKey(rdr),
                GetCouponId(rdr),
                GetTicketId(rdr),
                GetTicketItemId(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetCouponId(SqlDataReader rdr)
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
        #endregion

    }
}
