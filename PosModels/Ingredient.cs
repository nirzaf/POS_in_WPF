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
    public class Ingredient : DataModelBase, IComparable, IEquatable<Ingredient>
    {
        #region Licensed Access Only
        static Ingredient()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Ingredient).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("Name")]
        public string FullName
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string ShortName
        {
            get;
            private set;
        }

        [ModeledData()]
        public double InventoryAmount
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

        [ModeledData("ExtendedYield")]
        public double? ExtendedIngredientYield
        {
            get;
            private set;
        }

        [ModeledData()]
        public double CostPerUnit
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? ParQuantity
        {
            get;
            private set;
        }

        private Ingredient(int id, string fullname, string shortname,
            double inventoryAmount, MeasurementUnit unit, double? extendedIngredientYield,
            double costPerUnit, double? parQuantity)
        {
            Id = id;
            FullName = fullname;
            ShortName = shortname;
            InventoryAmount = inventoryAmount;
            MeasurementUnit = unit;
            ExtendedIngredientYield = extendedIngredientYield;
            CostPerUnit = costPerUnit;
            ParQuantity = parQuantity;
        }

        /// <summary>
        /// Checks to see if this ingredient contains another ingredient
        /// </summary>
        /// <param name="ingredientId"></param>
        /// <returns></returns>
        public bool ContainsIngredient(int ingredientId)
        {
            if (!IngredientSet.HasEntries(Id))
                return false;
            foreach (IngredientSet ingredientSet in IngredientSet.GetAll(Id))
            {
                Ingredient ingredient = Ingredient.Get(ingredientSet.IngredientId);
                if ((ingredientId == ingredient.Id) || (ingredient.ContainsIngredient(ingredientId)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Will either use the cost of its ingredients, if this is
        /// a prepared ingredient, else returns this.CostPerUnit
        /// </summary>
        /// <returns></returns>
        public double GetActualCostPerUnit()
        {
            if (!IngredientSet.HasEntries(Id))
                return CostPerUnit;
            double result = 0;
            foreach (IngredientSet ingredientSet in IngredientSet.GetAll(Id))
            {
                Ingredient ingredient = Ingredient.Get(ingredientSet.IngredientId);
                double amountInInventoryUnits = UnitConversion.Convert(ingredientSet.Amount,
                    ingredientSet.MeasurementUnit, ingredient.MeasurementUnit);
                result += (amountInInventoryUnits * ingredient.GetActualCostPerUnit());
            }
            return result;
        }

        public void SetFullName(string name)
        {
            FullName = name;
        }

        public void SetShortName(string shortName)
        {
            ShortName = shortName;
        }

        public void SetInventoryAmount(double amount)
        {
            InventoryAmount = amount;
        }

        public void SetMeasurementUnit(MeasurementUnit unit)
        {
            MeasurementUnit = unit;
        }

        public void SetExtendedIngredientYield(double? yield)
        {
            ExtendedIngredientYield = yield;
        }

        public void UpdateExtendedIngredientYield(double? yield)
        {
            ExtendedIngredientYield = yield;
            UpdateYield(this);
        }

        public void SetCostPerUnit(double costPerUnit)
        {
            CostPerUnit = costPerUnit;
        }

        public void SetParQuantity(double? parQuantity)
        {
            ParQuantity = parQuantity;
        }

        public bool Update()
        {
            return Ingredient.Update(this);
        }

        public int CompareTo(object obj)
        {
            if (obj is Ingredient)
            {
                Ingredient ingredientObj = obj as Ingredient;
                return Id.CompareTo(ingredientObj.Id);
            }
            throw new InvalidOperationException("Failed to compare two elements in the array");
        }

        public bool Equals(Ingredient other)
        {
            return (Id == other.Id);
        }

        #region static
        public static Ingredient Add(string ingredientName, string ingredientShortName,
            double amount, MeasurementUnit measureUnit, double costPerUnit, double? parQuantity)
        {
            return Add(ingredientName, ingredientShortName, amount, (short)measureUnit,
                costPerUnit, parQuantity);
        }

        private static Ingredient Add(string ingredientName, string ingredientShortName,
            double amount, short measureUnitId, double costPerUnit, double? parQuantity)
        {
            Ingredient result = null;
            SqlConnection cn = GetConnection();
            string cmd = "AddIngredient";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@IngredientName", SqlDbType.Text, ingredientName);
                BuildSqlParameter(sqlCmd, "@IngredientShortName", SqlDbType.Text, ingredientShortName);
                BuildSqlParameter(sqlCmd, "@IngredientInventoryAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@IngredientMeasurementUnit", SqlDbType.SmallInt, measureUnitId);
                BuildSqlParameter(sqlCmd, "@IngredientCostPerUnit", SqlDbType.Float, costPerUnit);
                BuildSqlParameter(sqlCmd, "@IngredientParQuantity", SqlDbType.Float, parQuantity);
                BuildSqlParameter(sqlCmd, "@IngredientId", SqlDbType.Int, ParameterDirection.ReturnValue);

                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Ingredient(Convert.ToInt32(sqlCmd.Parameters["@IngredientId"].Value),
                        ingredientName, ingredientShortName, amount,
                        measureUnitId.GetMeasurementUnit(), null, costPerUnit, parQuantity);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<Ingredient> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ingredient", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildIngredient(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static Ingredient Get(int id)
        {
            Ingredient result = null;
            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static Ingredient Get(SqlConnection cn, int id)
        {
            Ingredient result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ingredient WHERE IngredientId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildIngredient(rdr);
                    }
                }
            }
            return result;
        }

        public static Ingredient Get(string ingredientName)
        {
            Ingredient result = null;

            if (!String.IsNullOrEmpty(ingredientName))
            {
                SqlConnection cn = GetConnection();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Ingredient WHERE IngredientName LIKE @IngredientName", cn))
                {
                    BuildSqlParameter(cmd, "@IngredientName", SqlDbType.Text, ingredientName);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            result = BuildIngredient(rdr);
                        }
                    }
                }
                FinishedWithConnection(cn);
            }
            return result;
        }

        public static MeasurementUnit GetMeasurementUnit(int ingredientId)
        {
            MeasurementUnit result = MeasurementUnit.None;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT IngredientMeasurementUnit FROM Ingredient WHERE IngredientId=" + ingredientId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        result = rdr.GetInt16(0).GetMeasurementUnit();
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        private static bool UpdateYield(Ingredient ingredient)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Ingredient SET IngredientExtendedYield=@IngredientExtendedYield WHERE IngredientId=@IngredientId";

                BuildSqlParameter(sqlCmd, "@IngredientId", SqlDbType.Int, ingredient.Id);
                if (ingredient.ExtendedIngredientYield.HasValue)
                    BuildSqlParameter(sqlCmd, "@IngredientExtendedYield", SqlDbType.Float, ingredient.ExtendedIngredientYield.Value);
                else
                    BuildSqlParameter(sqlCmd, "@IngredientExtendedYield", SqlDbType.Float, null);
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public static bool Update(Ingredient ingredient)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, ingredient);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, Ingredient ingredient)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Ingredient SET IngredientName=@IngredientName,IngredientShortName=@IngredientShortName,IngredientInventoryAmount=@IngredientInventoryAmount,IngredientMeasurementUnit=@IngredientMeasurementUnit,IngredientExtendedYield=@IngredientExtendedYield,IngredientCostPerUnit=@IngredientCostPerUnit,IngredientParQuantity=@IngredientParQuantity WHERE IngredientId=@IngredientId";

                BuildSqlParameter(sqlCmd, "@IngredientId", SqlDbType.Int, ingredient.Id);
                BuildSqlParameter(sqlCmd, "@IngredientName", SqlDbType.Text, ingredient.FullName);
                BuildSqlParameter(sqlCmd, "@IngredientShortName", SqlDbType.Text, ingredient.ShortName);
                BuildSqlParameter(sqlCmd, "@IngredientInventoryAmount", SqlDbType.Float, ingredient.InventoryAmount);
                BuildSqlParameter(sqlCmd, "@IngredientMeasurementUnit", SqlDbType.SmallInt, ingredient.MeasurementUnit);
                if (ingredient.ExtendedIngredientYield.HasValue)
                    BuildSqlParameter(sqlCmd, "@IngredientExtendedYield", SqlDbType.Float, ingredient.ExtendedIngredientYield.Value);
                else
                    BuildSqlParameter(sqlCmd, "@IngredientExtendedYield", SqlDbType.Float, null);
                BuildSqlParameter(sqlCmd, "@IngredientCostPerUnit", SqlDbType.Float, ingredient.CostPerUnit);
                BuildSqlParameter(sqlCmd, "@IngredientParQuantity", SqlDbType.Float, ingredient.ParQuantity);
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM Ingredient WHERE IngredientId=" + id;
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public static Ingredient Find(IEnumerable<Ingredient> ingredients, int id)
        {
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.Id == id)
                    return ingredient;
            }

            return null;
        }

        public static IEnumerable<Ingredient> GetIngredients(IEnumerable<IngredientSet> ingredientSets)
        {
            foreach (IngredientSet set in ingredientSets)
            {
                yield return Ingredient.Get(set.IngredientId);
            }
        }

        private static Ingredient BuildIngredient(SqlDataReader rdr)
        {
            return new Ingredient(
                GetId(rdr),
                GetName(rdr),
                GetShortName(rdr),
                GetInventoryAmount(rdr),
                GetMeasurementUnit(rdr),
                GetYield(rdr),
                GetCostPerUnit(rdr),
                GetParQuantity(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static string GetShortName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static double GetInventoryAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static MeasurementUnit GetMeasurementUnit(SqlDataReader rdr)
        {
            return rdr.GetInt16(4).GetMeasurementUnit();
        }

        private static double? GetYield(SqlDataReader rdr)
        {
            double? yeild = null;
            if (!rdr.IsDBNull(5))
                yeild = rdr.GetDouble(5);
            return yeild;
        }

        private static double GetCostPerUnit(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }

        private static double? GetParQuantity(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(7))
                return null;
            return rdr.GetDouble(7);
        }
        #endregion

    }
}
