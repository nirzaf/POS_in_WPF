using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;
using System.Data.SqlClient;
using PosModels.Managers;
using PosModels.Helpers;
using System.Data;

namespace PosModels
{
    [ModeledDataClass()]
    public class ItemOptionAdjustment : DataModelBase
    {
        [ModeledData()]
        public int Id
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

        [ModeledData("EntryType")]
        [ModeledDataType("TINYINT")]
        public ItemOptionAdjustmentType Type
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? ItemOptionSetId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? ItemOptionId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? OldProductId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? NewProductId
        {
            get;
            private set;
        }

        [ModeledData()]
        public byte? OldTinyIntValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public byte? NewTinyIntValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? OldFloatValue
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? NewFloatValue
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

        private ItemOptionAdjustment(int id, int employeeId, ItemOptionAdjustmentType type,
            int? itemOptionSetId, int? itemOptionId, int? oldProductId, int? newProductId,
            byte? oldTinyIntValue, byte? newTinyIntValue, double? oldFloatValue,
            double? newFloatValue, DateTime when)
        {
            Id = id;
            EmployeeId = employeeId;
            Type = type;
            ItemOptionSetId = itemOptionSetId;
            ItemOptionId = itemOptionId;
            OldProductId = oldProductId;
            NewProductId = newProductId;
            OldTinyIntValue = oldTinyIntValue;
            NewTinyIntValue = newTinyIntValue;
            OldFloatValue = oldFloatValue;
            NewFloatValue = newFloatValue;
        }

        public static ItemOptionAdjustment Add(int employeeId, ItemOptionAdjustmentType type,
            int? itemOptionSetId, int? itemOptionId, int? oldProductId, int? newProductId,
            byte? oldTinyIntValue, byte? newTinyIntValue, double? oldFloatValue,
            double? newFloatValue)
        {
            ItemOptionAdjustment result = null;
            SqlConnection cn = GetConnection();
            DateTime when = DateTime.Now;

            string cmd = "AddItemOptionAdjustment";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentItemOptionSetId", SqlDbType.Int, itemOptionSetId);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentItemOptionId", SqlDbType.Int, itemOptionId);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentType", SqlDbType.TinyInt, type);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentOldProductId", SqlDbType.Int, oldProductId);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentNewProductId", SqlDbType.Int, newProductId);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentOldTinyIntValue", SqlDbType.TinyInt, oldTinyIntValue);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentNewTinyIntValue", SqlDbType.TinyInt, newTinyIntValue);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentOldFloatValue", SqlDbType.Float, oldFloatValue);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentNewFloatValue", SqlDbType.Float, newFloatValue);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentWhen", SqlDbType.DateTime, when);
                BuildSqlParameter(sqlCmd, "@ItemOptionAdjustmentId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemOptionAdjustment(Convert.ToInt32(sqlCmd.Parameters["@ItemAdjustmentId"].Value),
                        employeeId, type, itemOptionSetId, itemOptionId, oldProductId, newProductId, oldTinyIntValue,
                        newTinyIntValue, oldFloatValue, newFloatValue, when);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<ItemOptionAdjustment> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOptionAdjustment", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemOptionAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<ItemOptionAdjustment> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOptionAdjustment " +
                "WHERE ((ItemOptionAdjustmentWhen >= @ItemOptionAdjustmentWhenStart) AND (ItemOptionAdjustmentWhen <= @ItemOptionAdjustmentWhenEnd))", cn))
            {
                BuildSqlParameter(cmd, "@ItemOptionAdjustmentWhenStart", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@ItemOptionAdjustmentWhenEnd", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemOptionAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static ItemOptionAdjustment Get(int id)
        {
            ItemOptionAdjustment result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemOptionAdjustment Get(SqlConnection cn, int id)
        {
            ItemOptionAdjustment result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemAdjustment WHERE ItemAdjustmentId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemOptionAdjustment(rdr);
                    }
                }
            }
            return result;
        }

        private static ItemOptionAdjustment BuildItemOptionAdjustment(SqlDataReader rdr)
        {
            return new ItemOptionAdjustment(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetType(rdr),
                GetItemOptionSetId(rdr),
                GetItemOptionId(rdr),
                GetOldProductId(rdr),
                GetNewProductId(rdr),
                GetOldTinyIntValue(rdr),
                GetNewTinyIntValue(rdr),
                GetOldFloatValue(rdr),
                GetNewFloatValue(rdr),
                GetWhen(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static ItemOptionAdjustmentType GetType(SqlDataReader rdr)
        {
            return (ItemOptionAdjustmentType)rdr.GetByte(2);
        }

        private static int? GetItemOptionSetId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(3))
                return null;
            return rdr.GetInt32(3);
        }

        private static int? GetItemOptionId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(4))
                return null;
            return rdr.GetInt32(4);
        }

        private static int? GetOldProductId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(5))
                return null;
            return rdr.GetInt32(5);
        }

        private static int? GetNewProductId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(6))
                return null;
            return rdr.GetInt32(6);
        }

        private static byte? GetOldTinyIntValue(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(7))
                return null;
            return rdr.GetByte(7);
        }

        private static byte? GetNewTinyIntValue(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(8))
                return null;
            return rdr.GetByte(8);
        }

        private static double? GetOldFloatValue(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(9))
                return null;
            return rdr.GetDouble(9);
        }

        private static double? GetNewFloatValue(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(10))
                return null;
            return rdr.GetDouble(10);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(11);
        }


    }
}
