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
    public class ItemPricing : DataModelBase
    {
        #region Licensed Access Only
        static ItemPricing()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemPricing).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData()]
        public int ItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public Days DayOfWeek
        {
            get;
            private set;
        }

        [ModeledData("StartDate")]
        public TimeSpan? StartTime
        {
            get;
            private set;
        }

        [ModeledData("EndDate")]
        public TimeSpan? EndTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Price
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? AdditionalDiscountMin
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? AdditionalDiscountMax
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? AdditionalDiscountPrice
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsEnabled
        {
            get;
            private set;
        }

        private ItemPricing(int id, int itemId, Days day, TimeSpan? startTime, TimeSpan? endTime, double price,
            int? additionalDiscountMin, int? additionalDiscountMax, double? additionalDiscountPrice, bool isEnabled)
            : this(id, itemId, day, price, additionalDiscountMin, additionalDiscountMax, additionalDiscountPrice, isEnabled)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        private ItemPricing(int id, int itemId, Days day, double price,
            int? additionalDiscountMin, int? additionalDiscountMax, double? additionalDiscountPrice,
            bool isEnabled)
        {
            Id = id;
            ItemId = itemId;
            StartTime = null;
            EndTime = null;
            DayOfWeek = day;
            Price = price;
            AdditionalDiscountMin = additionalDiscountMin;
            AdditionalDiscountMax = additionalDiscountMax;
            AdditionalDiscountPrice = additionalDiscountPrice;
            IsEnabled = isEnabled;
        }


        public void SetItemId(int itemId)
        {
            ItemId = itemId;
        }

        public void SetDayOfTheWeek(Days day)
        {
            DayOfWeek = day;
        }

        public void SetStartTime(TimeSpan? start)
        {
            StartTime = start;
        }

        public void SetEndTime(TimeSpan? end)
        {
            EndTime = end;
        }

        public void SetPrice(double price)
        {
            Price = price;
        }

        public void SetAdditionalDiscountMin(int? min)
        {
            AdditionalDiscountMin = min;
        }

        public void SetAdditionalDiscountMax(int? max)
        {
            AdditionalDiscountMax = max;
        }

        public void SetAdditionalDiscountPrice(double? price)
        {
            AdditionalDiscountPrice = price;
        }

        public void SetIsEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public bool Update()
        {
            return ItemPricing.Update(this);
        }

        #region static
        public static ItemPricing Add(int itemId, TimeSpan? startTime, TimeSpan? endTime, double price,
            int? additionalDiscountMin, int? additionalDiscountMax, double? additionalDiscountPrice, bool isEnabled)
        {
            ItemPricing result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemPricing", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemPricingItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemPricingDayOfWeek", SqlDbType.TinyInt, Days.Any);
                BuildSqlParameter(sqlCmd, "@ItemPricingStartDate", SqlDbType.Time, startTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingEndDate", SqlDbType.Time, endTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingPrice", SqlDbType.Float, price);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMin", SqlDbType.Float, additionalDiscountMin);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMax", SqlDbType.Float, additionalDiscountMax);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountPrice", SqlDbType.Float, additionalDiscountPrice);
                BuildSqlParameter(sqlCmd, "@ItemPricingIsEnabled", SqlDbType.Bit, isEnabled);
                BuildSqlParameter(sqlCmd, "@ItemPricingId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemPricing(Convert.ToInt32(sqlCmd.Parameters["@ItemPricingId"].Value),
                        itemId, Days.Any, startTime, endTime, price, additionalDiscountMin,
                        additionalDiscountMax, additionalDiscountPrice, isEnabled);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static ItemPricing Add(int itemId, Days day, TimeSpan? startTime, TimeSpan? endTime, double price,
            int? additionalDiscountMin, int? additionalDiscountMax, double? additionalDiscountPrice,
            bool isEnabled)
        {
            ItemPricing result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemPricing", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemPricingItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemPricingDayOfWeek", SqlDbType.TinyInt, day);
                BuildSqlParameter(sqlCmd, "@ItemPricingStartDate", SqlDbType.Time, startTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingEndDate", SqlDbType.Time, endTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingPrice", SqlDbType.Float, price);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMin", SqlDbType.Float, additionalDiscountMin);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMax", SqlDbType.Float, additionalDiscountMax);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountPrice", SqlDbType.Float, additionalDiscountPrice);
                BuildSqlParameter(sqlCmd, "@ItemPricingIsEnabled", SqlDbType.Bit, isEnabled);
                BuildSqlParameter(sqlCmd, "@ItemPricingId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemPricing(Convert.ToInt32(sqlCmd.Parameters["@ItemPricingId"].Value),
                        itemId, day, startTime, endTime, price, additionalDiscountMin,
                        additionalDiscountMax, additionalDiscountPrice, isEnabled);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the ItemPricing table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            ItemPricing itemPricing = Get(cn, id);
            if (itemPricing != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM ItemPricing WHERE ItemPricingId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Delete an entry from the ItemPricing table, by ItemId
        /// </summary>
        public static bool DeleteByItemId(int itemId)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM ItemPricing WHERE ItemPricingItemId=" + itemId;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the ItemPricing table
        /// </summary>
        public static ItemPricing Get(int id)
        {
            ItemPricing result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static ItemPricing Get(SqlConnection cn, int id)
        {
            ItemPricing result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemPricing WHERE ItemPricingId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemPricing(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ItemPricing table
        /// </summary>
        public static IEnumerable<ItemPricing> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemPricing", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemPricing(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the ItemPricing table, with the specified ItemId
        /// </summary>
        public static IEnumerable<ItemPricing> GetAll(int itemId, bool nowOnly, bool enableRequired)
        {
            string cmdText = "SELECT * FROM ItemPricing WHERE (ItemPricingItemId=" + itemId + ")";
            if (nowOnly)
                cmdText += " AND (ItemPricingDayOfWeek=@ItemPricingDayOfWeek)" +
                   " AND ((ItemPricingStartDate IS NULL) OR (ItemPricingStartDate >= @CurrentTime))" +
                   " AND ((ItemPricingEndDate IS NULL) OR (ItemPricingEndDate <= @CurrentTime))";
            if (enableRequired)
                cmdText += " AND (ItemPricingIsEnabled=@ItemPricingIsEnabled)";
            DateTime now = DateTime.Now;

            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                int dayOfWeek = (int)DayHelper.ConvertToDay(now.DayOfWeek);
                cmd = new SqlCommand(cmdText, cn);
                if (nowOnly)
                {
                    BuildSqlParameter(cmd, "@ItemPricingDayOfWeek", SqlDbType.TinyInt, dayOfWeek);
                    BuildSqlParameter(cmd, "@CurrentTime", SqlDbType.Time, now.TimeOfDay);
                }
                if (enableRequired)
                    BuildSqlParameter(cmd, "@ItemPricingIsEnabled", SqlDbType.Bit, enableRequired);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemPricing(rdr);
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
        /// Update an entry in the ItemPricing table
        /// </summary>
        public static bool Update(ItemPricing itemPricing)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, itemPricing);
            FinishedWithConnection(cn);

            return result;
        }

        public static ItemPricing FindById(ItemPricing[] itemPricingCollection, int id)
        {
            foreach (ItemPricing pricing in itemPricingCollection)
            {
                if (pricing.Id == id)
                    return pricing;
            }
            return null;
        }

        private static bool Update(SqlConnection cn, ItemPricing itemPricing)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ItemPricing SET ItemPricingItemId=@ItemPricingItemId,ItemPricingDayOfWeek=@ItemPricingDayOfWeek,ItemPricingStartDate=@ItemPricingStartDate,ItemPricingEndDate=@ItemPricingEndDate,ItemPricingPrice=@ItemPricingPrice,ItemPricingAdditionalDiscountMin=@ItemPricingAdditionalDiscountMin,ItemPricingAdditionalDiscountMax=@ItemPricingAdditionalDiscountMax,ItemPricingAdditionalDiscountPrice=@ItemPricingAdditionalDiscountPrice,ItemPricingIsEnabled=@ItemPricingIsEnabled WHERE ItemPricingId=@ItemPricingId";

                BuildSqlParameter(sqlCmd, "@ItemPricingId", SqlDbType.Int, itemPricing.Id);
                BuildSqlParameter(sqlCmd, "@ItemPricingItemId", SqlDbType.Int, itemPricing.ItemId);
                BuildSqlParameter(sqlCmd, "@ItemPricingDayOfWeek", SqlDbType.TinyInt, itemPricing.DayOfWeek);
                BuildSqlParameter(sqlCmd, "@ItemPricingStartDate", SqlDbType.Time, itemPricing.StartTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingEndDate", SqlDbType.Time, itemPricing.EndTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingPrice", SqlDbType.Float, itemPricing.Price);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMin", SqlDbType.Float, itemPricing.AdditionalDiscountMin);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountMax", SqlDbType.Float, itemPricing.AdditionalDiscountMax);
                BuildSqlParameter(sqlCmd, "@ItemPricingAdditionalDiscountPrice", SqlDbType.Float, itemPricing.AdditionalDiscountPrice);
                BuildSqlParameter(sqlCmd, "@ItemPricingIsEnabled", SqlDbType.Bit, itemPricing.IsEnabled);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a ItemPricing object from a SqlDataReader object
        /// </summary>
        private static ItemPricing BuildItemPricing(SqlDataReader rdr)
        {
            return new ItemPricing(
                GetId(rdr),
                GetItemId(rdr),
                GetDay(rdr),
                GetStartTime(rdr),
                GetEndTime(rdr),
                GetPrice(rdr),
                GetAdditionalMin(rdr),
                GetAdditionalMax(rdr),
                GetAdditionalPrice(rdr),
                GetIsEnabled(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static Days GetDay(SqlDataReader rdr)
        {
            return (Days)rdr.GetByte(2);
        }

        private static TimeSpan? GetStartTime(SqlDataReader rdr)
        {
            TimeSpan? startTime = null;
            if (!rdr.IsDBNull(3))
                startTime = rdr.GetTimeSpan(3);
            return startTime;
        }

        private static TimeSpan? GetEndTime(SqlDataReader rdr)
        {
            TimeSpan? endTime = null;
            if (!rdr.IsDBNull(4))
                endTime = rdr.GetTimeSpan(4);
            return endTime;
        }

        private static double GetPrice(SqlDataReader rdr)
        {
            return rdr.GetDouble(5);
        }

        private static int? GetAdditionalMin(SqlDataReader rdr)
        {
            int? additionalMin = null;
            if (!rdr.IsDBNull(6))
                additionalMin = rdr.GetInt32(6);
            return additionalMin;
        }

        private static int? GetAdditionalMax(SqlDataReader rdr)
        {
            int? additionalMax = null;
            if (!rdr.IsDBNull(7))
                additionalMax = rdr.GetInt32(7);
            return additionalMax;
        }

        private static double? GetAdditionalPrice(SqlDataReader rdr)
        {
            double? additionalPrice = null;
            if (!rdr.IsDBNull(8))
                additionalPrice = rdr.GetDouble(8);
            return additionalPrice;
        }

        private static bool GetIsEnabled(SqlDataReader rdr)
        {
            return rdr.GetBoolean(9);
        }
        #endregion

    }
}
