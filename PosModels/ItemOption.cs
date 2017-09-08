using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class ItemOption : DataModelBase
    {
        #region Licensed Access Only
        static ItemOption()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemOption).Assembly.GetName().GetPublicKeyToken(),
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

        [ModeledData("ItemOptionSetId")]
        public int SetId
        {
            get;
            private set;
        }

        [ModeledData("Value")]
        public string Name
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? CostForExtra
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool UsesIngredient
        {
            get;
            private set;
        }

        [ModeledData()]
        public int? ProductId
        {
            get;
            private set;
        }

        [ModeledData()]
        public double? ProductAmount
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataType("SMALLINT")]
        [ModeledDataNullable()]
        public MeasurementUnit? ProductMeasurementUnit
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsDiscontinued
        {
            get;
            private set;
        }
        // End of modeled properties

        public bool UsesItem
        {
            get
            {
                return (!UsesIngredient && ProductId != null);
            }
        }

        private ItemOption(int id, int setId, string name, bool isDiscontinued)
        {
            Id = id;
            SetId = setId;
            Name = name;
            CostForExtra = null;
            ProductId = null;
            ProductAmount = null;
            ProductMeasurementUnit = null;
            UsesIngredient = false;
            IsDiscontinued = isDiscontinued;
        }

        private ItemOption(int id, int setId, string name, double? costForExtra,
            bool usesIngredient, int? productId, double? productAmount,
            MeasurementUnit? productMeasurementUnit, bool isDiscontinued)
            : this(id, setId, name, isDiscontinued)
        {
            CostForExtra = costForExtra;
            ProductId = productId;
            ProductAmount = productAmount;
            ProductMeasurementUnit = productMeasurementUnit;
            UsesIngredient = usesIngredient;
        }

        public void Discontinue()
        {
            IsDiscontinued = true;
            Update(this);
        }

        public void SetSetId(int setId)
        {
            SetId = setId;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetCostForExtra(double? costForExtra)
        {
            CostForExtra = costForExtra;
        }

        public void SetUsesIngredient(bool usesIngredient)
        {
            UsesIngredient = usesIngredient;
        }

        public void SetProductId(int? productId)
        {
            ProductId = productId;
        }

        public void SetProductAmount(double? productAmount)
        {
            ProductAmount = productAmount;
        }

        public void SetProductMeasurementUnit(MeasurementUnit? measurementUnit)
        {
            ProductMeasurementUnit = measurementUnit;
        }

        public bool Update()
        {
            return ItemOption.Update(this);
        }

        #region static
        public static ItemOption Add(int itemOptionSetId, string optionValue,
            double? costForExtra, bool usesIngredient, int? productId,
            double? productAmount, MeasurementUnit? productMeasurementUnit)
        {
            ItemOption result = null;
            bool isDiscontinued = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemOption", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemOptionItemOptionSetId", SqlDbType.Int, itemOptionSetId);
                BuildSqlParameter(sqlCmd, "@ItemOptionValue", SqlDbType.Text, optionValue);
                BuildSqlParameter(sqlCmd, "@ItemOptionCostForExtra", SqlDbType.Float, costForExtra);
                BuildSqlParameter(sqlCmd, "@ItemOptionUsesIngredient", SqlDbType.Bit, usesIngredient);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductId", SqlDbType.Int, productId);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductAmount", SqlDbType.Float, productAmount);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductMeasurementUnit", SqlDbType.SmallInt, productMeasurementUnit);
                BuildSqlParameter(sqlCmd, "@ItemOptionIsDiscontinued", SqlDbType.Bit, isDiscontinued);
                BuildSqlParameter(sqlCmd, "@ItemOptionId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemOption(Convert.ToInt32(sqlCmd.Parameters["@ItemOptionId"].Value),
                        itemOptionSetId, optionValue, costForExtra, usesIngredient,
                        productId, productAmount, productMeasurementUnit,
                        isDiscontinued);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<ItemOption> GetInSet(int itemOptionSetId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOption WHERE ItemOptionItemOptionSetId=" + itemOptionSetId + " AND ItemOptionIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemOption(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static ItemOption Get(int itemOptionId)
        {
            ItemOption result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, itemOptionId);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemOption Get(SqlConnection cn, int itemOptionId)
        {
            ItemOption result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOption WHERE ItemOptionId=" + itemOptionId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItemOption(rdr);
                    }
                }
            }
            return result;
        }

        public static ItemOption Get(int itemOptionSetId, string optionName)
        {
            ItemOption result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, itemOptionSetId, optionName);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemOption Get(SqlConnection cn, int itemOptionSetId, string optionName)
        {
            ItemOption result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOption WHERE ItemOptionItemOptionSetId=" + itemOptionSetId + " AND ItemOptionIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        //result = new Category();
                        string lineName = rdr[2].ToString();
                        if (optionName == lineName)
                        {
                            result = BuildItemOption(rdr);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update an entry in the ItemOption table
        /// </summary>
        public static bool Update(ItemOption itemOption)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, itemOption);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, ItemOption itemOption)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ItemOption SET ItemOptionItemOptionSetId=@ItemOptionItemOptionSetId,ItemOptionValue=@ItemOptionValue,ItemOptionCostForExtra=@ItemOptionCostForExtra,ItemOptionUsesIngredient=@ItemOptionUsesIngredient,ItemOptionProductId=@ItemOptionProductId,ItemOptionProductAmount=@ItemOptionProductAmount,ItemOptionProductMeasurementUnit=@ItemOptionProductMeasurementUnit,ItemOptionIsDiscontinued=@ItemOptionIsDiscontinued WHERE ItemOptionId=@ItemOptionId";

                BuildSqlParameter(sqlCmd, "@ItemOptionId", SqlDbType.Int, itemOption.Id);
                BuildSqlParameter(sqlCmd, "@ItemOptionItemOptionSetId", SqlDbType.Int, itemOption.SetId);
                BuildSqlParameter(sqlCmd, "@ItemOptionValue", SqlDbType.Text, itemOption.Name);
                BuildSqlParameter(sqlCmd, "@ItemOptionCostForExtra", SqlDbType.Float, itemOption.CostForExtra);
                BuildSqlParameter(sqlCmd, "@ItemOptionUsesIngredient", SqlDbType.Bit, itemOption.UsesIngredient);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductId", SqlDbType.Int, itemOption.ProductId);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductAmount", SqlDbType.Float, itemOption.ProductAmount);
                BuildSqlParameter(sqlCmd, "@ItemOptionProductMeasurementUnit", SqlDbType.SmallInt, itemOption.ProductMeasurementUnit);
                BuildSqlParameter(sqlCmd, "@ItemOptionIsDiscontinued", SqlDbType.Bit, itemOption.IsDiscontinued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        private static ItemOption BuildItemOption(SqlDataReader rdr)
        {
            return new ItemOption(
                GetId(rdr),
                GetSetId(rdr),
                GetName(rdr),
                GetCostForExtra(rdr),
                GetUsesIngredient(rdr),
                GetProductId(rdr),
                GetProductAmount(rdr),
                GetMeasurementUnit(rdr),
                GetIsDiscontinued(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetSetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static double? GetCostForExtra(SqlDataReader rdr)
        {
            double? costForExtra = null;
            if (!rdr.IsDBNull(3))
                costForExtra = rdr.GetDouble(3);
            return costForExtra;
        }

        private static bool GetUsesIngredient(SqlDataReader rdr)
        {
            return rdr.GetBoolean(4);
        }

        private static int? GetProductId(SqlDataReader rdr)
        {
            int? productId = null;
            if (!rdr.IsDBNull(5))
                productId = rdr.GetInt32(5);
            return productId;
        }

        private static double? GetProductAmount(SqlDataReader rdr)
        {
            double? productAmount = null;
            if (!rdr.IsDBNull(6))
                productAmount = rdr.GetDouble(6);
            return productAmount;
        }

        private static MeasurementUnit? GetMeasurementUnit(SqlDataReader rdr)
        {
            MeasurementUnit? productMeasurementUnit = null;
            if (!rdr.IsDBNull(7))
                productMeasurementUnit = rdr.GetInt16(7).GetMeasurementUnit();
            return productMeasurementUnit;
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(8);
        }
        #endregion

    }
}
