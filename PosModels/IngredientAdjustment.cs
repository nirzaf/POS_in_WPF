using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;
using PosModels.Managers;
using System.Data.SqlClient;
using PosModels.Helpers;
using System.Data;

namespace PosModels
{
    [ModeledDataClass()]
    public class IngredientAdjustment : DataModelBase
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
        public int IngredientId
        {
            get;
            private set;
        }

        /// <summary>
        /// Only null when specifing a RecipeIngredientId, and when null indicates that a
        /// recipe ingredient was added. Both OriginalValue and NewValue must not both be
        /// null.
        /// </summary>
        [ModeledData()]
        public double? OriginalValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Only null when specifing a RecipeIngredientId, and when null indicates that a
        /// recipe ingredient eas removed. Both OriginalValue and NewValue must not both be
        /// null.
        /// </summary>
        [ModeledData()]
        public double? NewValue
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

        /// <summary>
        /// When specified, the IngredientAdjustment is a recipe change. When value is
        /// null, the IngredientAdjustment is a change in Ingredient.InventoryAmount
        /// </summary>
        [ModeledData()]
        public int? RecipeIngredientId
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

        private IngredientAdjustment(int id, int employeeId, int ingredientId, 
            double? originalValue, double? newValue, MeasurementUnit measurementUnit,
            int? recipeIngredientId, DateTime when)
        {
            Id = id;
            IngredientId = ingredientId;
            EmployeeId = employeeId;
            OriginalValue = originalValue;
            NewValue = newValue;
            MeasurementUnit = measurementUnit;
            RecipeIngredientId = recipeIngredientId;
            When = when;
        }

        #region static
        public static IngredientAdjustment Add(int employeeId, int ingredientId,
            double? originalValue, double? newValue, MeasurementUnit measurementUnit, 
            int? recipeIngredientId = null, DateTime? when = null)
        {
            IngredientAdjustment result = null;
            SqlConnection cn = GetConnection();

            if (when == null)
                when = DateTime.Now;

            string cmd = "AddIngredientAdjustment";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentEmployeeId", SqlDbType.Int, employeeId);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentOriginalValue", SqlDbType.Float, originalValue);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentNewValue", SqlDbType.Float, newValue);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentMeasurementUnit", SqlDbType.SmallInt, measurementUnit);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentRecipeIngredientId", SqlDbType.Int, recipeIngredientId);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentWhen", SqlDbType.DateTime, when.Value);
                BuildSqlParameter(sqlCmd, "@IngredientAdjustmentId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new IngredientAdjustment(Convert.ToInt32(sqlCmd.Parameters["@IngredientAdjustmentId"].Value),
                        employeeId, ingredientId, originalValue, newValue, measurementUnit, recipeIngredientId, when.Value);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<IngredientAdjustment> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientAdjustment", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildIngredientAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<IngredientAdjustment> GetAll(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientAdjustment " + 
                "WHERE ((IngredientAdjustmentWhen >= @IngredientAdjustmentWhenStart) AND (IngredientAdjustmentWhen <= @IngredientAdjustmentWhenEnd))", cn))
            {
                BuildSqlParameter(cmd, "@IngredientAdjustmentWhenStart", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@IngredientAdjustmentWhenEnd", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildIngredientAdjustment(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IngredientAdjustment Get(int id)
        {
            IngredientAdjustment result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static IngredientAdjustment Get(SqlConnection cn, int id)
        {
            IngredientAdjustment result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM IngredientAdjustment WHERE IngredientAdjustmentId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildIngredientAdjustment(rdr);
                    }
                }
            }
            return result;
        }

        private static IngredientAdjustment BuildIngredientAdjustment(SqlDataReader rdr)
        {
            return new IngredientAdjustment(
                GetId(rdr),
                GetEmployeeId(rdr),
                GetIngredientId(rdr),
                GetOriginalAmount(rdr),
                GetNewAmount(rdr),
                GetMeasurementUnit(rdr),
                GetRecipeIngredientId(rdr),
                GetWhen(rdr));

        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetEmployeeId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static double? GetOriginalAmount(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(3))
                return rdr.GetDouble(3);
            return null;
        }

        private static double? GetNewAmount(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(4))
                return rdr.GetDouble(4);
            return null;
        }

        private static MeasurementUnit GetMeasurementUnit(SqlDataReader rdr)
        {
            return (MeasurementUnit)rdr.GetInt16(5);
        }

        private static int? GetRecipeIngredientId(SqlDataReader rdr)
        {
            if (!rdr.IsDBNull(6))
                return rdr.GetInt32(6);
            return null;
        }

        private static DateTime GetWhen(SqlDataReader rdr)
        {
            return rdr.GetDateTime(7);
        }
        #endregion
    }
}
