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
    public class ItemAdjustment : DataModelBase
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

        [ModeledData()]
        [ModeledDataType("TINYINT")]
        public ItemAdjustmentType Type
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
        public int? ItemOptionSetId
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

        private ItemAdjustment(int id, int employeeId, ItemAdjustmentType type,
            int itemId, int? itemOptionSetId, DateTime when)
        {
            Id = id;
            EmployeeId = employeeId;
            Type = type;
            ItemId = itemId;
            ItemOptionSetId = itemOptionSetId;
            When = when;
        }

        public static ItemAdjustment Add(int employeeId, ItemAdjustmentType type,
            int itemId, int? itemOptionSetId = null)
        {
            ItemAdjustment result = null;
            SqlConnection cn = GetConnection();
            DateTime when = DateTime.Now;

            string cmd = "AddItemAdjustment";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentItemOptionSetId", SqlDbType.Int, itemOptionSetId);
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentType", SqlDbType.TinyInt, type);
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentWhen", SqlDbType.DateTime, when);
                BuildSqlParameter(sqlCmd, "@ItemAdjustmentId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemAdjustment(Convert.ToInt32(sqlCmd.Parameters["@ItemAdjustmentId"].Value),
                        employeeId, type, itemId, itemOptionSetId, when);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<ItemAdjustment> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemAdjustment", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<ItemAdjustment> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemAdjustment " +
                "WHERE ((ItemAdjustmentWhen >= @ItemAdjustmentWhenStart) AND (ItemAdjustmentWhen <= @ItemAdjustmentWhenEnd))", cn))
            {
                BuildSqlParameter(cmd, "@ItemAdjustmentWhenStart", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@ItemAdjustmentWhenEnd", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static ItemAdjustment Get(int id)
        {
            ItemAdjustment result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemAdjustment Get(SqlConnection cn, int id)
        {
            ItemAdjustment result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemAdjustment WHERE ItemAdjustmentId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemAdjustment(rdr);
                    }
                }
            }
            return result;
        }

        private static ItemAdjustment BuildItemAdjustment(SqlDataReader rdr)
        {
            return new ItemAdjustment(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetType(rdr),
                GetItemId(rdr),
                GetItemOptionSetId(rdr),
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

        private static ItemAdjustmentType GetType(SqlDataReader rdr)
        {
            return (ItemAdjustmentType)rdr.GetByte(2);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int? GetItemOptionSetId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(4))
                return null;
            return rdr.GetInt32(4);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(5);
        }

    }
}
