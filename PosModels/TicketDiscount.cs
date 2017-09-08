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
    public class TicketDiscount : DataModelBase
    {
        #region Licensed Access Only
        static TicketDiscount()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketDiscount).Assembly.GetName().GetPublicKeyToken(),
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
        public int DiscountId
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

        [ModeledData()]
        public double? Amount
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? PseudoEmployeeId
        {
            get;
            private set;
        }

        private TicketDiscount(YearId primaryKey, int discountId, int ticketId,
            int? ticketItemId, double? amount, int? pseudoEmployeeId)
        {
            PrimaryKey = primaryKey;
            DiscountId = discountId;
            TicketId = ticketId;
            TicketItemId = ticketItemId;            
            Amount = amount;
            PseudoEmployeeId = pseudoEmployeeId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the TicketDiscount table, for a Ticket
        /// </summary>
        public static TicketDiscount Add(int discountId, YearId ticketPrimaryKey,
            double? amount, int? pseudoEmployeeId)
        {
            return Add(discountId, ticketPrimaryKey, null, amount, pseudoEmployeeId);
        }

        /// <summary>
        /// Add a new entry to the TicketDiscount table, for a TicketItem
        /// </summary>
        public static TicketDiscount Add(int discountId, YearId ticketPrimaryKey,
            int? ticketItemId, double? amount, int? pseudoEmployeeId)
        {
            TicketDiscount result = null;
            DateTime purchaseTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddTicketDiscount";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketDiscountYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketDiscountDiscountId", SqlDbType.Int, discountId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketDiscountTicketItemId", SqlDbType.Int, ticketItemId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountPseudoEmployeeId", SqlDbType.Int, pseudoEmployeeId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@TicketDiscountId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketDiscount(
                        new YearId(ticketPrimaryKey.Year,
                        Convert.ToInt32(sqlCmd.Parameters["@TicketDiscountId"].Value)),
                        discountId, ticketPrimaryKey.Id, ticketItemId, amount, pseudoEmployeeId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketDiscount,RESEED,0)";
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
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketDiscountId FROM TicketDiscount", cn))
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
        /// Delete an entry from the TicketDiscount table
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            TicketDiscount ticketDiscount = Get(cn, primaryKey);
            if (ticketDiscount != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM TicketDiscount WHERE (TicketDiscountId=" + primaryKey.Id + " AND TicketDiscountYear=" + primaryKey.Year + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the TicketDiscount table for a specific ticketItem
        /// </summary>
        public static bool DeleteForTicketItem(YearId ticketItemPrimaryKey)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketDiscount WHERE (" +
                    "TicketDiscountYear=" + ticketItemPrimaryKey.Year + " AND " +
                    "TicketDiscountTicketItemId=" + ticketItemPrimaryKey.Id + ")";
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the TicketDiscount table
        /// </summary>
        public static TicketDiscount Get(YearId primaryKey)
        {
            TicketDiscount result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketDiscount Get(SqlConnection cn, YearId primaryKey)
        {
            TicketDiscount result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDiscount WHERE TicketDiscountId=" + primaryKey.Id + " AND TicketDiscountYear=" + primaryKey.Year, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketDiscount(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the TicketDiscount table (Don't use)
        /// </summary>
        private static IEnumerable<TicketDiscount> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDiscount", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketDiscount(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketDiscount table for a particular ticket
        /// </summary>
        public static IEnumerable<TicketDiscount> GetAll(YearId ticketPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDiscount WHERE (" +
                    "TicketDiscountYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketDiscountTicketId=" + ticketPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketDiscount(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the TicketDiscount table for the specified tickets
        /// </summary>
        public static IEnumerable<TicketDiscount> GetAllForTickets(IEnumerable<Ticket> tickets)
        {
            if (tickets != null)
            {
                foreach (Ticket ticket in tickets)
                {
                    IEnumerable<TicketDiscount> discounts = TicketDiscount.GetAll(ticket.PrimaryKey);
                    if (discounts == null)
                        continue;
                    foreach (TicketDiscount discount in discounts)
                    {
                        yield return discount;
                    }
                }
            }
        }

        /// <summary>
        /// Get all the entries in the TicketDiscount table for a particular ticket
        /// </summary>
        public static IEnumerable<TicketDiscount> GetAllForTicketItem(YearId ticketItemPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketDiscount WHERE (" +
                    "TicketDiscountYear=" + ticketItemPrimaryKey.Year + " AND " +
                    "TicketDiscountTicketItemId=" + ticketItemPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketDiscount(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the TicketDiscount table
        /// </summary>
        public static bool Update(TicketDiscount ticketDiscount)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, ticketDiscount);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, TicketDiscount ticketDiscount)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketDiscount SET TicketDiscountDiscountId=@TicketDiscountDiscountId,TicketDiscountTicketId=@TicketDiscountTicketId,TicketDiscountTicketItemId=@TicketDiscountTicketItemId,TicketDiscountPseudoEmployeeId=@TicketDiscountPseudoEmployeeId,TicketDiscountAmount=@TicketDiscountAmount WHERE (TicketDiscountId=@TicketDiscountId AND TicketDiscountYear=@TicketDiscountYear)";

                BuildSqlParameter(sqlCmd, "@TicketDiscountYear", SqlDbType.SmallInt, ticketDiscount.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketDiscountId", SqlDbType.Int, ticketDiscount.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketDiscountDiscountId", SqlDbType.Int, ticketDiscount.DiscountId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountTicketId", SqlDbType.Int, ticketDiscount.TicketId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountTicketItemId", SqlDbType.Int, ticketDiscount.TicketItemId);
                BuildSqlParameter(sqlCmd, "@TicketDiscountAmount", SqlDbType.Float, ticketDiscount.Amount);
                BuildSqlParameter(sqlCmd, "@TicketDiscountPseudoEmployeeId", SqlDbType.Int, ticketDiscount.PseudoEmployeeId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketDiscount))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketDiscount WHERE TicketDiscountId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketDiscount object from a SqlDataReader object
        /// </summary>
        private static TicketDiscount BuildTicketDiscount(SqlDataReader rdr)
        {
            return new TicketDiscount(
                GetPrimaryKey(rdr),
                GetDiscountId(rdr),
                GetTicketPrimaryKey(rdr),
                GetTicketItemId(rdr),
                GetAmount(rdr),
                GetPseudoEmployeeId(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return new YearId(rdr.GetInt16(0), rdr.GetInt32(1));
        }

        private static int GetDiscountId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetTicketPrimaryKey(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int? GetTicketItemId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(4))
                return null;
            return rdr.GetInt32(4);
        }

        private static double? GetAmount(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(5))
                return null;
            return rdr.GetDouble(5);
        }

        private static int? GetPseudoEmployeeId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(6))
                return null;
            return rdr.GetInt32(6);
        }
        #endregion

    }
}
