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
    public class VendorItem : DataModelBase
    {
        #region Licensed Access Only
        static VendorItem()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(VendorItem).Assembly.GetName().GetPublicKeyToken(),
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
        public int VendorId
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Description
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Cost
        {
            get;
            private set;
        }

        [ModeledData()]
        [ModeledDataNullable()]
        public string VendorItemNumber
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
        public int ItemId
        {
            get;
            private set;
        }

        private VendorItem(int id, int vendorId, string description, double cost,
            string vendorNumber, int ingredientId, int itemId)
        {
            Id = id;
            VendorId = vendorId;
            Description = description;
            Cost = cost;
            VendorItemNumber = vendorNumber;
            IngredientId = ingredientId;
            ItemId = itemId;
        }

        #region static
        /// <summary>
        /// Add a new entry to the VendorItem table, for Ingredients
        /// </summary>
        public static VendorItem AddIngredient(int vendorId, string description, double cost,
            string vendorNumber, int ingredientId)
        {
            VendorItem result = null;
            DateTime purchaseTime = DateTime.Now;
            int itemId = 0;

            SqlConnection cn = GetConnection();
            string cmd = "AddVendorItem";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@VendorItemVendorId", SqlDbType.Int, vendorId);
                BuildSqlParameter(sqlCmd, "@VendorItemDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@VendorItemCost", SqlDbType.Float, cost);
                BuildSqlParameter(sqlCmd, "@VendorItemVendorItemNumber", SqlDbType.Text, vendorNumber);
                BuildSqlParameter(sqlCmd, "@VendorItemIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@VendorItemItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@VendorItemId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new VendorItem(Convert.ToInt32(sqlCmd.Parameters["@VendorItemId"].Value),
                        vendorId, description, cost, vendorNumber, ingredientId, itemId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Add a new entry to the VendorItem table, for Items
        /// </summary>
        public static VendorItem AddItem(int vendorId, string description, double cost,
            string vendorNumber, int itemId)
        {
            VendorItem result = null;
            DateTime purchaseTime = DateTime.Now;
            int ingredientId = 0;

            SqlConnection cn = GetConnection();
            string cmd = "AddVendorItem";
            using (SqlCommand sqlCmd = new SqlCommand(cmd, cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@VendorItemVendorId", SqlDbType.Int, vendorId);
                BuildSqlParameter(sqlCmd, "@VendorItemDescription", SqlDbType.Text, description);
                BuildSqlParameter(sqlCmd, "@VendorItemCost", SqlDbType.Float, cost);
                BuildSqlParameter(sqlCmd, "@VendorItemVendorItemNumber", SqlDbType.Text, vendorNumber);
                BuildSqlParameter(sqlCmd, "@VendorItemIngredientId", SqlDbType.Int, ingredientId);
                BuildSqlParameter(sqlCmd, "@VendorItemItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@VendorItemId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new VendorItem(Convert.ToInt32(sqlCmd.Parameters["@VendorItemId"].Value),
                        vendorId, description, cost, vendorNumber, ingredientId, itemId);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Delete an entry from the VendorItem table
        /// </summary>
        public static bool Delete(int id)
        {
            Int32 rowsAffected = 0;
            SqlConnection cn = GetConnection();
            VendorItem vendorItem = Get(cn, id);
            if (vendorItem != null)
            {
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM VendorItem WHERE VendorItemId=" + id;
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Get an entry from the VendorItem table
        /// </summary>
        public static VendorItem Get(int id)
        {
            VendorItem result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, id);
            FinishedWithConnection(cn);

            return result;
        }

        private static VendorItem Get(SqlConnection cn, int id)
        {
            VendorItem result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorItem WHERE VendorItemId=" + id, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildVendorItem(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the VendorItem table
        /// </summary>
        public static IEnumerable<VendorItem> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM VendorItem", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildVendorItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the VendorItem table
        /// </summary>
        public static bool Update(VendorItem vendorItem)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, vendorItem);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, VendorItem vendorItem)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE VendorItem SET VendorItemVendorId=@VendorItemVendorId,VendorItemDescription=@VendorItemDescription,VendorItemCost=@VendorItemCost,VendorItemVendorItemNumber=@VendorItemVendorItemNumber,VendorItemIngredientId=@VendorItemIngredientId,VendorItemItemId=@VendorItemItemId WHERE VendorItemId=@VendorItemId";

                BuildSqlParameter(sqlCmd, "@VendorItemId", SqlDbType.Int, vendorItem.Id);
                BuildSqlParameter(sqlCmd, "@VendorItemVendorId", SqlDbType.Int, vendorItem.VendorId);
                BuildSqlParameter(sqlCmd, "@VendorItemDescription", SqlDbType.Text, vendorItem.Description);
                BuildSqlParameter(sqlCmd, "@VendorItemCost", SqlDbType.Float, vendorItem.Cost);
                BuildSqlParameter(sqlCmd, "@VendorItemVendorItemNumber", SqlDbType.Text, vendorItem.VendorItemNumber);
                BuildSqlParameter(sqlCmd, "@VendorItemIngredientId", SqlDbType.Int, vendorItem.IngredientId);
                BuildSqlParameter(sqlCmd, "@VendorItemItemId", SqlDbType.Int, vendorItem.ItemId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a VendorItem object from a SqlDataReader object
        /// </summary>
        private static VendorItem BuildVendorItem(SqlDataReader rdr)
        {
            return new VendorItem(
                GetId(rdr),
                GetVendorId(rdr),
                GetDescription(rdr),
                GetCost(rdr),
                GetVendorItemNumber(rdr),
                GetIngredientId(rdr),
                GetItemId(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetVendorId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetDescription(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static double GetCost(SqlDataReader rdr)
        {
            return rdr.GetDouble(3);
        }

        private static string GetVendorItemNumber(SqlDataReader rdr)
        {
            string vendorItemNumber = null;
            if (!rdr.IsDBNull(4))
            {
                string value = rdr.GetString(4);
                if (!value.Equals(""))
                    vendorItemNumber = value;
            }
            return vendorItemNumber;
        }

        private static int GetIngredientId(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }
        #endregion

    }
}
