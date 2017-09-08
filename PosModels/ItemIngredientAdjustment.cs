using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;
using System.Data.SqlClient;
using PosModels.Helpers;
using System.Data;
using PosModels.Managers;

namespace PosModels
{
    [ModeledDataClass()]
    public class ItemIngredientAdjustment : DataModelBase
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
        public int ItemId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int IngredientId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? OldAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? NewAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        public MeasurementUnit MeasurementUnit
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

        private ItemIngredientAdjustment(int id, int employeeId, int itemId,
            int ingredientId, double? oldAmount, double? newAmount,
            MeasurementUnit measurementUnit, DateTime when)
        {
            Id = id;
            EmployeeId = employeeId;
            ItemId = itemId;
            IngredientId = ingredientId;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            MeasurementUnit = measurementUnit;
            When = when;
        }

        public static ItemIngredientAdjustment Add(int employeeId, int itemId,
            int ingredientId, double? oldAmount, double? newAmount,
            MeasurementUnit measurementUnit, DateTime? when = null)
        {
            ItemIngredientAdjustment result = null;
            SqlConnection cn = GetConnection();

            if (when == null)
                when = DateTime.Now;

            string cmd = "AddItemIngredientAdjustment";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentOriginalValue", SqlDbType.Float, oldAmount);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentNewValue", SqlDbType.Float, newAmount);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentMeasurementUnit", SqlDbType.SmallInt, measurementUnit);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentWhen", SqlDbType.DateTime, when.Value);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAdjustmentId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemIngredientAdjustment(Convert.ToInt32(sqlCmd.Parameters["@ItemIngredientAdjustmentId"].Value),
                        employeeId, itemId, ingredientId, oldAmount, newAmount, measurementUnit, when.Value);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<ItemIngredientAdjustment> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredientAdjustment", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemIngredientAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<ItemIngredientAdjustment> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredientAdjustment " +
                "WHERE ((ItemIngredientAdjustmentWhen >= @ItemIngredientAdjustmentWhenStart) AND (ItemIngredientAdjustmentWhen <= @ItemIngredientAdjustmentWhenEnd))", cn))
            {
                BuildSqlParameter(cmd, "@ItemIngredientAdjustmentWhenStart", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@ItemIngredientAdjustmentWhenEnd", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemIngredientAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static ItemIngredientAdjustment Get(int id)
        {
            ItemIngredientAdjustment result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemIngredientAdjustment Get(SqlConnection cn, int id)
        {
            ItemIngredientAdjustment result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredientAdjustment WHERE ItemIngredientAdjustmentId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemIngredientAdjustment(rdr);
                    }
                }
            }
            return result;
        }

        private static ItemIngredientAdjustment BuildItemIngredientAdjustment(SqlDataReader rdr)
        {
            return new ItemIngredientAdjustment(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetItemId(rdr),
                GetIngredientId(rdr),
                GetOriginalAmount(rdr),
                GetNewAmount(rdr),
                GetMeasurementUnit(rdr),
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

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static double? GetOriginalAmount(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(4))
                return rdr.GetDouble(4);
            return null;
        }

        private static double? GetNewAmount(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(5))
                return rdr.GetDouble(5);
            return null;
        }

        private static MeasurementUnit GetMeasurementUnit(SqlDataReader rdr)
        {
            return (MeasurementUnit)rdr.GetInt16(6);
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(7);
        }

    }
}
