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
    public class ItemPricingChangeLog : DataModelBase
    {
        #region Licensed Access Only
        static ItemPricingChangeLog()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemPricingChangeLog).Assembly.GetName().GetPublicKeyToken(),
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
        public int EmployeeId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? ItemPricingId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? OldPrice
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? NewPrice
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public Days? OldDayOfWeek
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public Days? NewDayOfWeek
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? OldStartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? NewStartTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? OldEndTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public TimeSpan? NewEndTime
        {
            get;
            private set;
        }

        [ModeledData("DateTime")]
        public DateTime ChangedTime
        {
            get;
            private set;
        }

        private ItemPricingChangeLog(int id, int itemId, int employeeId, int? itemPricingId,
            double? oldPrice, double? newPrice, Days? oldDayOfTheWeek, Days? newDayOfTheWeek,
            TimeSpan? oldStartTime, TimeSpan? newStartTime, TimeSpan? oldEndTime,
            TimeSpan? newEndTime, DateTime changedTime)
        {
            Id = id;
            ItemId = itemId;
            EmployeeId = employeeId;
            ItemPricingId = itemPricingId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            OldDayOfWeek = oldDayOfTheWeek;
            NewDayOfWeek = newDayOfTheWeek;
            OldStartTime = oldStartTime;
            NewStartTime = newStartTime;
            OldEndTime = oldEndTime;
            NewEndTime = newEndTime;
            ChangedTime = changedTime;
        }

        #region Static
        /// <summary>
        /// Add a new entry to the ItemPricingChangeLog table
        /// </summary>
        public static ItemPricingChangeLog Add(int itemId, int employeeId, int? itemPricingId, 
            double? oldPrice, double? newPrice, Days? oldDayOfTheWeek = null, Days? newDayOfTheWeek = null,
            TimeSpan? oldStartTime = null, TimeSpan? newStartTime = null, TimeSpan? oldEndTime = null,
            TimeSpan? newEndTime = null)
        {
            ItemPricingChangeLog result = null;
            DateTime changedTime = DateTime.Now;

            SqlConnection cn = GetConnection();
            string cmd = "AddItemPricingChangeLog";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogItemPricingId", SqlDbType.Int, itemPricingId);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogOldPrice", SqlDbType.Float, oldPrice);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogNewPrice", SqlDbType.Float, newPrice);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogOldDayOfWeek", SqlDbType.TinyInt, oldDayOfTheWeek);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogNewDayOfWeek", SqlDbType.TinyInt, newDayOfTheWeek);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogOldStartTime", SqlDbType.Time, oldStartTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogNewStartTime", SqlDbType.Time, newStartTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogOldEndTime", SqlDbType.Time, oldEndTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogNewEndTime", SqlDbType.Time, newEndTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogDateTime", SqlDbType.DateTime, changedTime);
                BuildSqlParameter(sqlCmd, "@ItemPricingChangeLogId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemPricingChangeLog(Convert.ToInt32(sqlCmd.Parameters["@ItemPricingChangeLogId"].Value),
                        itemId, employeeId, itemPricingId, oldPrice, newPrice, oldDayOfTheWeek, newDayOfTheWeek,
                        oldStartTime, newStartTime, oldEndTime, newEndTime, changedTime);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the ItemPricingChangeLog table
        /// </summary>
        public static ItemPricingChangeLog Get(int id)
        {
            ItemPricingChangeLog result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static ItemPricingChangeLog Get(SqlConnection cn, int id)
        {
            ItemPricingChangeLog result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemPricingChangeLog WHERE ItemPricingChangeLogId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemPricingChangeLog(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ItemPricingChangeLog table
        /// </summary>
        public static IEnumerable<ItemPricingChangeLog> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemPricingChangeLog", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemPricingChangeLog(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<ItemPricingChangeLog> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemPricingChangeLog " +
                "WHERE ((ItemPricingChangeLogDateTime >= @ItemPricingChangeLogStart) AND (ItemPricingChangeLogDateTime <= @ItemPricingChangeLogEnd))", cn))
            {
                BuildSqlParameter(cmd, "@ItemPricingChangeLogStart", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@ItemPricingChangeLogEnd", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemPricingChangeLog(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Build a ItemPricingChangeLog object from a SqlDataReader object
        /// </summary>
        private static ItemPricingChangeLog BuildItemPricingChangeLog(SqlDataReader rdr)
        {
            return new ItemPricingChangeLog(
                GetId(rdr),
                GetItemId(rdr),
                GetEmployeeId(rdr),
                GetItemPricingId(rdr),
                GetOldCost(rdr),
                GetNewCost(rdr),
                GetOldDayOfWeek(rdr),
                GetNewDayOfWeek(rdr),
                GetOldStartTime(rdr),
                GetNewStartTime(rdr),
                GetOldEndTime(rdr),
                GetNewEndTime(rdr),
                GetChangedTime(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int? GetItemPricingId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(3))
                return null;
            return rdr.GetInt32(3);
        }

        private static double? GetOldCost(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(4))
                return null;
            return rdr.GetDouble(4);
        }

        private static double? GetNewCost(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(5))
                return null;
            return rdr.GetDouble(5);
        }

        private static Days? GetOldDayOfWeek(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(6))
                return null;
            return (Days)rdr.GetByte(6);
        }

        private static Days? GetNewDayOfWeek(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(7))
                return null;
            return (Days)rdr.GetByte(7);
        }

        private static TimeSpan? GetOldStartTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(8))
                return null;
            return rdr.GetTimeSpan(8);
        }

        private static TimeSpan? GetNewStartTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(9))
                return null;
            return rdr.GetTimeSpan(9);
        }

        private static TimeSpan? GetOldEndTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(10))
                return null;
            return rdr.GetTimeSpan(10);
        }

        private static TimeSpan? GetNewEndTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(11))
                return null;
            return rdr.GetTimeSpan(11);
        }

        private static DateTime GetChangedTime(SqlDataReader rdr)
        {
            return rdr.GetDateTime(12);
        }
        #endregion
    }
}
