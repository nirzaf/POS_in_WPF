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
    public class ItemOptionSet : DataModelBase
    {
        #region Licensed Access Only
        static ItemOptionSet()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ItemOptionSet).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        private IEnumerable<ItemOption> options = null;

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Name
        {
            get;
            private set;
        }

        [ModeledData("NumMin")]
        [ModeledDataType("TINYINT")]
        public int SelectedMinimum
        {
            get;
            private set;
        }

        [ModeledData("NumFree")]
        [ModeledDataType("TINYINT")]
        public int SelectedFree
        {
            get;
            private set;
        }

        [ModeledData("NumMax")]
        [ModeledDataType("TINYINT")]
        public int SelectedMaximum
        {
            get;
            private set;
        }

        [ModeledData()]
        public double ExcessUnitCost
        {
            get;
            private set;
        }

        [ModeledData("PizzaStyle")]
        public bool IsPizzaStyle
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

        private ItemOptionSet(int id, string name, int min, int free, int max,
            double excessUnitCost, bool isPizzaStyle, bool isDiscontinued)
        {
            Id = id;
            Name = name;
            SelectedMinimum = min;
            SelectedFree = free;
            SelectedMaximum = max;
            ExcessUnitCost = excessUnitCost;
            IsPizzaStyle = isPizzaStyle;
            IsDiscontinued = isDiscontinued;
        }

        public bool ContainsItemOptionUsingItem(int itemId)
        {
            foreach (ItemOption itemOption in ItemOption.GetInSet(Id))
            {
                if ((itemOption.UsesItem) && (itemOption.ProductId != null) &&
                    (itemOption.ProductId.Value == itemId))
                    return true;
            }
            return false;
        }

        public void Discontinue()
        {
            foreach (ItemOption itemOption in ItemOption.GetInSet(Id))
            {
                itemOption.Discontinue();
            }
            IsDiscontinued = true;
            Update(this);
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetSelectedMinimum(int min)
        {
            SelectedMinimum = MathHelper.Clamp(min, 0, 255);
        }

        public void SetSelectedMaximum(int max)
        {
            SelectedMaximum = MathHelper.Clamp(max, 0, 255);
        }

        public void SetSelectedFree(int free)
        {
            SelectedFree = MathHelper.Clamp(free, 0, 255);
        }

        public void SetExcessUnitCost(double excessUnitCost)
        {
            ExcessUnitCost = excessUnitCost;
        }

        public void SetIsPizzaStyle(bool isPizzaStyle)
        {
            IsPizzaStyle = isPizzaStyle;
        }

        public IEnumerable<ItemOption> GetOptions()
        {
            if (options == null)
                options = ItemOption.GetInSet(Id);
            return options;
        }

        public bool Update()
        {
            return ItemOptionSet.Update(this);
        }

        #region static
        /// <summary>
        /// Add a new entry to the ItemOptionSet table
        /// </summary>
        public static ItemOptionSet Add(string optionSetName, int min, int free, int max,
            double excessUnitCost, bool isPizzaStyle)
        {
            ItemOptionSet result = null;
            bool isDiscontinued = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItemOptionSet", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemOptionSetName", SqlDbType.Text, optionSetName);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumMin", SqlDbType.TinyInt, min);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumFree", SqlDbType.TinyInt, free);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumMax", SqlDbType.TinyInt, max);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetExcessUnitCost", SqlDbType.Float, excessUnitCost);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetPizzaStyle", SqlDbType.Bit, isPizzaStyle);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetIsDiscontinued", SqlDbType.Bit, isDiscontinued);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new ItemOptionSet(Convert.ToInt32(sqlCmd.Parameters["@ItemOptionSetId"].Value),
                        optionSetName, min, free, max, excessUnitCost, isPizzaStyle,
                        isDiscontinued);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Get an entry from the ItemOptionSet table
        /// </summary>
        public static ItemOptionSet Get(int optionSetId)
        {
            ItemOptionSet result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, optionSetId);
            FinishedWithConnection(cn);
            return result;
        }

        private static ItemOptionSet Get(SqlConnection cn, int optionSetId)
        {
            ItemOptionSet result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOptionSet WHERE ItemOptionSetId=" + optionSetId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result = BuildItemOptionSet(rdr);
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get an entry from the ItemOptionSet table, using the option set's name
        /// </summary>
        public static ItemOptionSet Get(string optionSetName)
        {
            ItemOptionSet result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, optionSetName);
            FinishedWithConnection(cn);
            return result;
        }
        private static ItemOptionSet Get(SqlConnection cn, string optionSetName)
        {
            ItemOptionSet result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOptionSet WHERE ItemOptionSetIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        //result = new Category();
                        string lineName = rdr[1].ToString();
                        if (optionSetName == lineName)
                        {
                            result = BuildItemOptionSet(rdr);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all the entries in the ItemOptionSet table
        /// </summary>
        public static IEnumerable<ItemOptionSet> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM ItemOptionSet WHERE ItemOptionSetIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItemOptionSet(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Update an entry in the ItemOptionSet table
        /// </summary>
        public static bool Update(ItemOptionSet itemOptionSet)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, itemOptionSet);
            FinishedWithConnection(cn);

            return result;
        }

        private static bool Update(SqlConnection cn, ItemOptionSet itemOptionSet)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE ItemOptionSet SET ItemOptionSetName=@ItemOptionSetName,ItemOptionSetNumMin=@ItemOptionSetNumMin,ItemOptionSetNumFree=@ItemOptionSetNumFree,ItemOptionSetNumMax=@ItemOptionSetNumMax,ItemOptionSetExcessUnitCost=@ItemOptionSetExcessUnitCost,ItemOptionSetPizzaStyle=@ItemOptionSetPizzaStyle,ItemOptionSetIsDiscontinued=@ItemOptionSetIsDiscontinued WHERE ItemOptionSetId=@ItemOptionSetId";

                BuildSqlParameter(sqlCmd, "@ItemOptionSetId", SqlDbType.Int, itemOptionSet.Id);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetName", SqlDbType.Text, itemOptionSet.Name);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumMin", SqlDbType.TinyInt, itemOptionSet.SelectedMinimum);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumFree", SqlDbType.TinyInt, itemOptionSet.SelectedFree);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetNumMax", SqlDbType.TinyInt, itemOptionSet.SelectedMaximum);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetExcessUnitCost", SqlDbType.Float, itemOptionSet.ExcessUnitCost);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetPizzaStyle", SqlDbType.Bit, itemOptionSet.IsPizzaStyle);
                BuildSqlParameter(sqlCmd, "@ItemOptionSetIsDiscontinued", SqlDbType.Bit, itemOptionSet.IsDiscontinued);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Build a ItemOptionSet object from a SqlDataReader object
        /// </summary>
        private static ItemOptionSet BuildItemOptionSet(SqlDataReader rdr)
        {
            return new ItemOptionSet(
                GetId(rdr),
                GetName(rdr),
                GetMin(rdr),
                GetFree(rdr),
                GetMax(rdr),
                GetExcessUnitCost(rdr),
                GetIsPizzaStyle(rdr),
                GetIsDiscontinued(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static string GetName(SqlDataReader rdr)
        {
            return rdr.GetString(1);
        }

        private static int GetMin(SqlDataReader rdr)
        {
            return rdr.GetByte(2);
        }

        private static int GetFree(SqlDataReader rdr)
        {
            return rdr.GetByte(3);
        }

        private static int GetMax(SqlDataReader rdr)
        {
            return rdr.GetByte(4);
        }

        private static double GetExcessUnitCost(SqlDataReader rdr)
        {
            return rdr.GetDouble(5);
        }

        private static bool GetIsPizzaStyle(SqlDataReader rdr)
        {
            return rdr.GetBoolean(6);
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(7);
        }

        public static ItemOptionSet Find(ItemOptionSet[] all, int id)
        {
            foreach (ItemOptionSet set in all)
            {
                if (set.Id == id)
                    return set;
            }
            return null;
        }

        #endregion
    }
}
