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
    public class TicketItemOption : DataModelBase
    {
        #region Licensed Access Only
        static TicketItemOption()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketItemOption).Assembly.GetName().GetPublicKeyToken(),
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
        public int ItemOptionId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TicketItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public TicketItemOptionType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// This value is used for extra-extra type TicketItemOptions. It's
        /// value is, by default, 1 when using normal options.
        /// </summary>
        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public int ChangeCount
        {
            get;
            private set;
        }

        private TicketItemOption(YearId primaryKey, int itemOptionId, int ticketItemId,
            TicketItemOptionType type, int changeCount)
        {
            PrimaryKey = primaryKey;
            ItemOptionId = itemOptionId;
            TicketItemId = ticketItemId;
            Type = type;
            ChangeCount = changeCount;     
        }

        #region static
        #region Waste Handling
        public static void AdjustInventory(TicketItemOption ticketItemOption,
            double portionSize,
            bool increaseInventory)
        {
            ItemOption itemOption = ItemOption.Get(ticketItemOption.ItemOptionId);
            if (itemOption.UsesItem && (itemOption.ProductId != null) &&
                (itemOption.ProductAmount != null))
            {
                PosModelHelper.AdjustInventoryByItem(
                    itemOption.ProductId.Value, itemOption.ProductAmount.Value * portionSize,
                    increaseInventory);
            }
            else if (itemOption.UsesIngredient && (itemOption.ProductId != null) &&
                (itemOption.ProductAmount != null) && (itemOption.ProductMeasurementUnit != null))
            {
                PosModelHelper.AdjustInventoryByIngredient(
                    itemOption.ProductId.Value,
                    increaseInventory, itemOption.ProductAmount.Value * portionSize,
                    itemOption.ProductMeasurementUnit.Value);
            }
        }
        #endregion

        public static TicketItemOption Add(int ticketItemId, int itemOptionId,
            TicketItemOptionType type, int changeCount)
        {
            TicketItemOption result = null;
            short year = DayOfOperation.CurrentYear;
            YearId ticketItemPrimaryKey = new YearId(year, ticketItemId);

            changeCount = changeCount.Clamp(0, 255);

            // Cache old before making a change
            TicketItem ticketItem = TicketItem.Get(ticketItemPrimaryKey);
            if (!ticketItem.IsTicketItemOptionsChanged)
                TicketItem.CacheTicketItemOptions(ticketItem);

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddTicketItemOption", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemOptionItemOptionId", SqlDbType.Int, itemOptionId);
                BuildSqlParameter(sqlCmd, "@TicketItemOptionTicketItemId", SqlDbType.Int, ticketItemId);
                BuildSqlParameter(sqlCmd, "@TicketItemOptionType", SqlDbType.TinyInt, type);
                BuildSqlParameter(sqlCmd, "@TicketItemOptionChangeCount", SqlDbType.TinyInt, changeCount);
                BuildSqlParameter(sqlCmd, "@TicketItemOptionYear", SqlDbType.SmallInt, year);
                BuildSqlParameter(sqlCmd, "@TicketItemOptionId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new TicketItemOption(new YearId(year, Convert.ToInt32(sqlCmd.Parameters["@TicketItemOptionId"].Value)),
                        itemOptionId, ticketItemId, type, changeCount);
                }
            }
            FinishedWithConnection(cn);

            if (result != null)
                SetTicketItemIsChanged(ticketItem);

            // Reduce the inventory
            TicketItemOption.AdjustInventory(result, ticketItem.Quantity, false);

            return result;
        }

        public static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            string cmd = "DBCC CHECKIDENT (TicketItemOption,RESEED,0)";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool YearHasTicketItemOptions(short year)
        {
            bool foundTicket = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketItemOptionYear FROM TicketItemOption WHERE TicketItemOptionYear=" + year, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundTicket = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return foundTicket;
        }

        public static bool Delete(TicketItem ticketItem, int itemOptionId,
            bool adjustInventory)
        {
            Int32 rowsAffected = 0;


            // Cache old before making a change
            if (!ticketItem.IsTicketItemOptionsChanged)
                TicketItem.CacheTicketItemOptions(ticketItem);

            SqlConnection cn = GetConnection();
            List<TicketItemOption> ticketItemOptions = new List<TicketItemOption>();
            if (adjustInventory)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText =
                        "SELECT * FROM TicketItemOption WHERE (" +
                        "TicketItemOptionItemOptionId=" + itemOptionId + " AND " +
                        "TicketItemOptionYear=" + ticketItem.PrimaryKey.Year + " AND " +
                        "TicketItemOptionTicketItemId=" + ticketItem.PrimaryKey.Id + ")";
                    using (SqlDataReader rdr = sqlCmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ticketItemOptions.Add(BuildTicketItemOption(rdr));
                        }
                    }
                }
            }            
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText =
                    "DELETE FROM TicketItemOption WHERE (" +
                    "TicketItemOptionItemOptionId=" + itemOptionId + " AND " +
                    "TicketItemOptionYear=" + ticketItem.PrimaryKey.Year + " AND " +
                    "TicketItemOptionTicketItemId=" + ticketItem.PrimaryKey.Id + ")";
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);

            if ((rowsAffected != 0) && (ticketItem.PrimaryKey.Id > 0))
                SetTicketItemIsChanged(ticketItem);

            // Increase the inventory
            if (adjustInventory)
            {
                foreach (TicketItemOption ticketItemOption in ticketItemOptions)
                {
                    AdjustInventory(ticketItemOption, ticketItem.Quantity, true);
                }
            }

            return (rowsAffected != 0);
        }

        public static bool DeleteAll(TicketItem ticketItem, bool adjustInventory)
        {
            Int32 rowsAffected = 0;

            // Cache old before making a change
            if ((ticketItem != null) && !ticketItem.IsTicketItemOptionsChanged)
                TicketItem.CacheTicketItemOptions(ticketItem);

            SqlConnection cn = GetConnection();
            List<TicketItemOption> ticketItemOptions = new List<TicketItemOption>();
            if (adjustInventory)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText =
                        "SELECT * FROM TicketItemOption WHERE " +
                        "(TicketItemOptionYear=" + ticketItem.PrimaryKey.Year +
                        " AND TicketItemOptionTicketItemId=" +
                        ticketItem.PrimaryKey.Id + ")";
                    using (SqlDataReader rdr = sqlCmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ticketItemOptions.Add(BuildTicketItemOption(rdr));
                        }
                    }
                }
            }
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = 
                    "DELETE FROM TicketItemOption WHERE " +
                    "(TicketItemOptionYear=" + ticketItem.PrimaryKey.Year +
                    " AND TicketItemOptionTicketItemId=" +
                    ticketItem.PrimaryKey.Id + ")";
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);

            if (rowsAffected != 0)
                SetTicketItemIsChanged(ticketItem);

            // Increase the inventory of for the ticketitem
            if (adjustInventory)
            {
                foreach (TicketItemOption ticketItemOption in ticketItemOptions)
                {
                    AdjustInventory(ticketItemOption, ticketItem.Quantity, true);
                }
            }

            return (rowsAffected != 0);
        }

        public static IEnumerable<TicketItemOption> GetAll(YearId ticketItemPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItemOption WHERE (" +
                    "TicketItemOptionYear=" + ticketItemPrimaryKey.Year + " AND " +
                    "TicketItemOptionTicketItemId=" + ticketItemPrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItemOption(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static bool HasOption(YearId primaryKey, int optionItemId)
        {

            bool found = false;
            foreach (TicketItemOption tio in GetAll(primaryKey))
            {
                if (tio.ItemOptionId == optionItemId)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static TicketItemOption Get(YearId primaryKey)
        {
            TicketItemOption result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, primaryKey);
            FinishedWithConnection(cn);

            return result;
        }

        private static TicketItemOption Get(SqlConnection cn, YearId primaryKey)
        {
            TicketItemOption result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItemOption WHERE " +
                "(TicketItemOptionYear=" + primaryKey.Year + " AND TicketItemOptionId=" +
                primaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketItemOption(rdr);
                    }
                }
            }
            return result;
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketItemOption))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketItemOption WHERE TicketItemOptionId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        private static void SetTicketItemIsChanged(TicketItem ticketItem)
        {
            if (ticketItem != null)
                ticketItem.SetTicketItemOptionsChanged();
        }

        /// <summary>
        /// Build a TicketItemOption object from a SqlDataReader object
        /// </summary>
        private static TicketItemOption BuildTicketItemOption(SqlDataReader rdr)
        {
            return new TicketItemOption(
                GetPrimaryKey(rdr),
                GetItemOptionId(rdr),
                GetTicketItemId(rdr),
                GetTicketItemOptionType(rdr),
                GetChangeCount(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return YearId.Create(
                rdr.GetInt16(0),
                rdr.GetInt32(1));
        }

        private static int GetItemOptionId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetTicketItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static TicketItemOptionType GetTicketItemOptionType(SqlDataReader rdr)
        {
            return rdr.GetByte(4).GetTicketItemOptionType();
        }

        private static int GetChangeCount(SqlDataReader rdr)
        {
            return rdr.GetByte(5);
        }
        #endregion

    }
}
