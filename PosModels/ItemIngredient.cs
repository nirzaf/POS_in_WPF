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
    public class ItemIngredient : DataModelBase
    {
        #region Licensed Access Only
        static ItemIngredient()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemIngredient).Assembly.GetName().GetPublicKeyToken(),
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
        public int IngredientId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Amount
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

        private ItemIngredient(int id, int itemId, int ingredientId, double amount,
            MeasurementUnit measurementUnit)
        {
            Id = id;
            ItemId = itemId;
            IngredientId = ingredientId;
            Amount = amount;
            MeasurementUnit = measurementUnit;
        }

        public void SetItemId(int itemId)
        {
            ItemId = itemId;
        }

        public void SetIngredientId(int ingredientId)
        {
            IngredientId = ingredientId;
        }

        public void SetAmount(double amount)
        {
            Amount = amount;
        }

        public void SetMeasurementUnit(MeasurementUnit measurementUnit)
        {
            MeasurementUnit = measurementUnit;
        }

        public bool Update()
        {
            return ItemIngredient.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the ItemIngredient table
        /// </summary>
        public static ItemIngredient Add(int itemId, int ingredientId, double amount,
            MeasurementUnit measurementUnit)
        {
            ItemIngredient result = null;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemIngredient", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemIngredientItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAmount", SqlDbType.Float, amount);
                BuildSqlParameter(sqlCmd, "@ItemIngredientMeasurementUnit", SqlDbType.SmallInt, measurementUnit);
                BuildSqlParameter(sqlCmd, "@ItemIngredientId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemIngredient(Convert.ToInt32(sqlCmd.Parameters["@ItemIngredientId"].Value),
                        itemId, ingredientId, amount, measurementUnit);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the ItemIngredient table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            ItemIngredient itemIngredient = Get(cn, id);
            if (itemIngredient != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM ItemIngredient WHERE ItemIngredientId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the ItemIngredient table
        /// </summary>
        public static ItemIngredient Get(int id)
        {
            ItemIngredient result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemIngredient Get(SqlConnection cn, int id)
        {
            ItemIngredient result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredient WHERE ItemIngredientId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemIngredient(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ItemIngredient table
        /// </summary>
        public static IEnumerable<ItemIngredient> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredient", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemIngredient(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all the entries in the ItemIngredient table, for an item
        /// </summary>
        public static IEnumerable<ItemIngredient> GetAll(int itemId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemIngredient WHERE ItemIngredientItemId=" + itemId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemIngredient(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the ItemIngredient table
        /// </summary>
        public static bool Update(ItemIngredient itemIngredient)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, itemIngredient);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, ItemIngredient itemIngredient)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ItemIngredient SET ItemIngredientItemId=@ItemIngredientItemId,ItemIngredientIngredientId=@ItemIngredientIngredientId,ItemIngredientAmount=@ItemIngredientAmount,ItemIngredientMeasurementUnit=@ItemIngredientMeasurementUnit WHERE ItemIngredientId=@ItemIngredientId";

                BuildSqlParameter(sqlCmd, "@ItemIngredientId", SqlDbType.Int, itemIngredient.Id);
                BuildSqlParameter(sqlCmd, "@ItemIngredientItemId", SqlDbType.Int, itemIngredient.ItemId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientIngredientId", SqlDbType.Int, itemIngredient.IngredientId);
                BuildSqlParameter(sqlCmd, "@ItemIngredientAmount", SqlDbType.Float, itemIngredient.Amount);
                BuildSqlParameter(sqlCmd, "@ItemIngredientMeasurementUnit", SqlDbType.SmallInt, (int)itemIngredient.MeasurementUnit);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a ItemIngredient object from a SqlDataReader object
        /// </summary>
        private static ItemIngredient BuildItemIngredient(SqlDataReader rdr)
        {
            return new ItemIngredient(
                GetId(rdr),
                GetItemId(rdr),
                GetIngredientId(rdr),
                GetAmount(rdr),
                GetMeasurementUnit(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static int GetIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static double GetAmount(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static MeasurementUnit GetMeasurementUnit(SqlDataReader rdr)
        {
            return rdr.GetInt16(4).GetMeasurementUnit();
        }
        #endregion

    }
}
