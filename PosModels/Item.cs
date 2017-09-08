using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PosModels.Helpers;
using PosModels.Managers;
using PosModels.Types;
using System.Windows.Media;
using TemposLibrary;
using System.Data.SqlTypes;
using System.Data.Linq.Mapping;

namespace PosModels
{
    [ModeledDataClass()]
    public class Item : DataModelBase
    {
        #region Licensed Access Only
        static Item()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Item).Assembly.GetName().GetPublicKeyToken(),
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
        public int CategoryId
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
        public string ShortName
        {
            get;
            private set;
        }

        [ModeledData("DefaultPrice")]
        public double Price
        {
            get;
            private set;
        }

        [ModeledData("ItemOptionSetId1", "ItemOptionSetId2", "ItemOptionSetId3")]
        [ModeledDataType("INT", "INT", "INT")]
        public int[] ItemOptionSetIds
        {
            get;
            private set;
        }

        // TODO: Not using this or the PrintOptionSetId's, this just stores a
        //       PrintDestinations flagged enum value.
        [ModeledData("PrintOptionSetId")]
        [ModeledDataType("SMALLINT")]
        public PrintDestination PrintDestinations
        {
            get;
            private set;
        }

        [ModeledData("Active")]
        public bool IsActive
        {
            get;
            private set;
        }

        [ModeledData()]
        public int TaxId
        {
            get;
            private set;
        }

        /// <summary>
        /// Used to calculate delivery time
        /// </summary>
        [ModeledData()]
        public TimeSpan? PrepareTime
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsReturnable
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsTaxExemptable
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsFired
        {
            get;
            private set;
        }

        [ModeledData()]
        public bool IsOutOfStock
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

        [ModeledData()]
        public bool IsGrouping
        {
            get;
            private set;
        }

        [ModeledData("ButtonImage")]
        [ModeledDataType("VARBINARY")]
        [ModeledDataNullable()]
        public byte[] ButtonImage
        {
            get;
            private set;
        }

        private Item(int id, int categoryId, string fullName, string shortName,
            double price, int condimentOptions, int sideOptions, int preparationOptions,
            PrintDestination printDestinations, bool active, int taxId, TimeSpan? prepareTime,
            bool isReturnable, bool isTaxExemptable, bool isFired, bool isOutOfStock,
            bool isDiscontinued, bool isGrouping, byte[] buttonImage)
        {
            Id = id;
            CategoryId = categoryId;
            FullName = fullName;
            ShortName = shortName;
            Price = price;
            ItemOptionSetIds = new int[] { preparationOptions, sideOptions, condimentOptions };
            PrintDestinations = printDestinations;
            IsActive = active;
            TaxId = taxId;
            PrepareTime = prepareTime;
            IsReturnable = isReturnable;
            IsTaxExemptable = isTaxExemptable;
            IsFired = isFired;
            IsOutOfStock = isOutOfStock;
            IsDiscontinued = isDiscontinued;
            IsGrouping = isGrouping;
            ButtonImage = buttonImage;
        }

        /// <summary>
        /// Discontinue the item, making it no longer available for order entry or modification.
        /// The Item is effectively deleted with the exception of when creating reports.
        /// </summary>
        public void Discontinue()
        {
            IsActive = false;
            IsDiscontinued = true;
            Update(this);
        }

        public IEnumerable<Item> GetItemsInGroup()
        {
            if (IsGrouping)
            {
                foreach (ItemGroup itemGroup in ItemGroup.GetAll(Id))
                {
                    yield return Get(itemGroup.TargetItemId);
                }
            }
        }

        /// <summary>
        /// Gets the item's ingredient cost (for a single item)
        /// </summary>
        /// <returns></returns>
        public double GetCostOfIngredients()
        {
            double result = 0;
            foreach (ItemIngredient itemIngredient in ItemIngredient.GetAll(Id))
            {
                Ingredient ingredient = Ingredient.Get(itemIngredient.IngredientId);
                double amountInInventoryUnits = UnitConversion.Convert(itemIngredient.Amount,
                    itemIngredient.MeasurementUnit, ingredient.MeasurementUnit);
                result += (amountInInventoryUnits * ingredient.GetActualCostPerUnit());
            }
            return result;
        }

        /// <summary>
        /// Checks to see if there is enough Ingredients to make this Item
        /// </summary>
        /// <param name="quantity">The quantity of this item to be made</param>
        /// <returns></returns>
        public bool HaveIngredientsToMake(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException("Quantity must be a positive non-zero integer");
            foreach (ItemIngredient itemIngredient in ItemIngredient.GetAll(Id))
            {
                Ingredient ingredient = Ingredient.Get(itemIngredient.IngredientId);
                double amountInInventoryUnits = UnitConversion.Convert(itemIngredient.Amount,
                    itemIngredient.MeasurementUnit, ingredient.MeasurementUnit);
                if (ingredient.InventoryAmount < (amountInInventoryUnits * quantity))
                    return false;
            }
            return true;
        }

        public void SetCategory(int categoryId)
        {
            CategoryId = categoryId;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
        }

        public void SetIsFired(bool isFired)
        {
            IsFired = isFired;
        }

        public void SetIsReturnable(bool isRefundable)
        {
            IsReturnable = isRefundable;
        }

        public void SetIsGrouping(bool isGrouping)
        {
            IsGrouping = isGrouping;
        }

        public void SetIsTaxExemptable(bool isTaxExemptable)
        {
            IsTaxExemptable = isTaxExemptable;
        }

        public void SetShortName(string shortName)
        {
            ShortName = shortName;
        }

        public void SetPrice(double price)
        {
            Price = price;
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void SetOptionSets(int[] optionSetIds)
        {
            if ((optionSetIds == null) || (optionSetIds.Length != 3))
                throw new ArgumentException("Invalid item option set length");
            ItemOptionSetIds = optionSetIds;
        }

        public void SetOptionSet1(int optionSetId)
        {
            ItemOptionSetIds[0] = optionSetId;
        }

        public void SetOptionSet2(int optionSetId)
        {
            ItemOptionSetIds[1] = optionSetId;
        }

        public void SetOptionSet3(int optionSetId)
        {
            ItemOptionSetIds[2] = optionSetId;
        }

        public void SetPrintOptionSet(PrintDestination printDestinations)
        {
            PrintDestinations = printDestinations;
        }

        public void SetTaxId(int taxId)
        {
            TaxId = taxId;
        }

        public void SetPrepareTime(TimeSpan? prepareTime)
        {
            PrepareTime = prepareTime;
        }

        public void SetIsOutOfStock(bool isOutOfStock)
        {
            IsOutOfStock = isOutOfStock;
        }

        public void SetButtonImage(byte[] buttonImage)
        {
            ButtonImage = buttonImage;
        }

        public bool Update()
        {
            return Item.Update(this);
        }

        #region static
        /// <summary>
        /// Adds an entry to the Item table
        /// </summary>
        public static Item Add(int categoryId, string itemName, string shortName,
            double price, int opSet1, int opSet2, int opSet3,
            PrintDestination printDestinations, bool active, int taxId, TimeSpan? prepareTime,
            bool isReturnable, bool isTaxExemptable, bool isFired, bool isOutOfStock, bool isGrouping,
            byte[] buttonImage = null)
        {
            Item result = null;
            bool isDiscontinued = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddItem", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@ItemCategoryId", SqlDbType.Int, categoryId);
                BuildSqlParameter(sqlCmd, "@ItemName", SqlDbType.Text, itemName);
                BuildSqlParameter(sqlCmd, "@ItemShortName", SqlDbType.Text, shortName);
                BuildSqlParameter(sqlCmd, "@ItemDefaultPrice", SqlDbType.Float, price);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId1", SqlDbType.Int, opSet1);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId2", SqlDbType.Int, opSet2);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId3", SqlDbType.Int, opSet3);
                BuildSqlParameter(sqlCmd, "@ItemPrintOptionSetId", SqlDbType.SmallInt, printDestinations);
                BuildSqlParameter(sqlCmd, "@ItemActive", SqlDbType.Bit, active);
                BuildSqlParameter(sqlCmd, "@ItemTaxId", SqlDbType.Int, taxId);
                BuildSqlParameter(sqlCmd, "@ItemPrepareTime", SqlDbType.Time, prepareTime);
                BuildSqlParameter(sqlCmd, "@ItemIsReturnable", SqlDbType.Bit, isReturnable);
                BuildSqlParameter(sqlCmd, "@ItemIsTaxExemptable", SqlDbType.Bit, isTaxExemptable);
                BuildSqlParameter(sqlCmd, "@ItemIsFired", SqlDbType.Bit, isFired);
                BuildSqlParameter(sqlCmd, "@ItemIsOutOfStock", SqlDbType.Bit, isOutOfStock);
                BuildSqlParameter(sqlCmd, "@ItemIsDiscontinued", SqlDbType.Bit, isDiscontinued);
                BuildSqlParameter(sqlCmd, "@ItemIsGrouping", SqlDbType.Bit, isGrouping);
                BuildSqlParameter(sqlCmd, "@ItemButtonImage", SqlDbType.VarBinary, buttonImage);
                BuildSqlParameter(sqlCmd, "@ItemId", SqlDbType.Int, ParameterDirection.ReturnValue);
                if (sqlCmd.ExecuteNonQuery() > 0)
                {
                    result = new Item(Convert.ToInt32(sqlCmd.Parameters["@ItemId"].Value),
                        categoryId, itemName, shortName, price, opSet1, opSet2,
                        opSet3, printDestinations, active, taxId, prepareTime,
                        isReturnable, isTaxExemptable, isFired, isOutOfStock, isDiscontinued,
                        isGrouping, buttonImage);
                }
            }
            FinishedWithConnection(cn);
            return result;
        }

        public static IEnumerable<Item> GetAll()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Item WHERE ItemIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<Item> GetAllForCategory(int categoryId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Item WHERE ItemCategoryId=" + categoryId + " AND ItemIsDiscontinued=0", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static IEnumerable<Item> GetAllContainingItemOptionSet(int itemOptionSetId)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Item WHERE (ItemItemOptionSetId1=@ItemItemOptionSetId OR ItemItemOptionSetId2=@ItemItemOptionSetId OR ItemItemOptionSetId3=@ItemItemOptionSetId) AND ItemIsDiscontinued=0", cn))
            {
                BuildSqlParameter(cmd, "@ItemItemOptionSetId", SqlDbType.Int, itemOptionSetId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        public static Item Get(int categoryId, string shortName)
        {
            foreach (Item item in GetAllForCategory(categoryId))
            {
                if (item.ShortName == shortName)
                    return item;
            }
            return null;
        }

        public static int GetCategoryId(int itemId)
        {
            int result = 0;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT ItemCategoryId FROM Item WHERE ItemId=@ItemId", cn))
            {
                BuildSqlParameter(cmd, "@ItemId", SqlDbType.Int, itemId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = Convert.ToInt32(rdr[0].ToString());
                    }
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        public static Item Get(string itemName)
        {
            Item result = null;

            if (!String.IsNullOrEmpty(itemName))
            {
                SqlConnection cn = GetConnection();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Item WHERE ItemName LIKE @ItemName AND ItemIsDiscontinued=0", cn))
                {
                    BuildSqlParameter(cmd, "@ItemName", SqlDbType.Text, itemName);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            result = BuildItem(rdr);
                        }
                    }
                }
            }
            return result;
        }

        public static Item Get(int itemId)
        {
            Item result = null;

            SqlConnection cn = GetConnection();
            result = Get(cn, itemId);
            FinishedWithConnection(cn);
            return result;
        }

        private static Item Get(SqlConnection cn, int itemId)
        {
            Item result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Item WHERE ItemId=" + itemId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildItem(rdr);
                    }
                }
            }
            return result;
        }

        public static bool GetIsReturnable(int itemId)
        {
            bool isReturnable = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT ItemIsReturnable FROM Item WHERE ItemId=" + itemId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            isReturnable = Convert.ToBoolean(rdr[0].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            FinishedWithConnection(cn);
            return isReturnable;
        }

        public static bool GetIsFired(int itemId)
        {
            bool isFired = false;

            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT ItemIsFired FROM Item WHERE ItemId=" + itemId, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            isFired = Convert.ToBoolean(rdr[0].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            FinishedWithConnection(cn);
            return isFired;
        }

        public static void Refresh(Item item)
        {
            if (item == null) return;
            Item originalItem = Get(item.Id);
            if (originalItem == null) return;
            item.CategoryId = originalItem.CategoryId;
            item.FullName = originalItem.FullName;
            item.IsActive = originalItem.IsActive;
            item.IsDiscontinued = originalItem.IsDiscontinued;
            item.IsFired = originalItem.IsFired;
            item.IsGrouping = originalItem.IsGrouping;
            item.IsOutOfStock = originalItem.IsOutOfStock;
            item.IsReturnable = originalItem.IsReturnable;
            item.IsTaxExemptable = originalItem.IsTaxExemptable;
            item.ItemOptionSetIds = originalItem.ItemOptionSetIds;
            item.PrepareTime = originalItem.PrepareTime;
            item.Price = originalItem.Price;
            item.PrintDestinations = originalItem.PrintDestinations;
            item.ShortName = originalItem.ShortName;
            item.TaxId = originalItem.Id;
            item.ButtonImage = originalItem.ButtonImage;
        }

        /// <summary>
        /// Update an entry in the Item table
        /// </summary>
        public static bool Update(Item item)
        {
            bool result = false;

            SqlConnection cn = GetConnection();
            result = Update(cn, item);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, Item item)
        {
            Int32 rowsAffected = 0;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE Item SET ItemName=@ItemName,ItemShortName=@ItemShortName,ItemDefaultPrice=@ItemDefaultPrice,ItemItemOptionSetId1=@ItemItemOptionSetId1,ItemItemOptionSetId2=@ItemItemOptionSetId2,ItemItemOptionSetId3=@ItemItemOptionSetId3,ItemPrintOptionSetId=@ItemPrintOptionSetId,ItemActive=@ItemActive,ItemTaxId=@ItemTaxId,ItemIsReturnable=@ItemIsReturnable,ItemIsTaxExemptable=@ItemIsTaxExemptable,ItemIsFired=@ItemIsFired,ItemIsOutOfStock=@ItemIsOutOfStock,ItemIsDiscontinued=@ItemIsDiscontinued,ItemIsGrouping=@ItemIsGrouping WHERE ItemId=@ItemId";

                BuildSqlParameter(sqlCmd, "@ItemId", SqlDbType.Int, item.Id);
                BuildSqlParameter(sqlCmd, "@ItemName", SqlDbType.Text, item.FullName);
                BuildSqlParameter(sqlCmd, "@ItemShortName", SqlDbType.Text, item.ShortName);
                BuildSqlParameter(sqlCmd, "@ItemDefaultPrice", SqlDbType.Float, item.Price);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId1", SqlDbType.Int, item.ItemOptionSetIds[0]);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId2", SqlDbType.Int, item.ItemOptionSetIds[1]);
                BuildSqlParameter(sqlCmd, "@ItemItemOptionSetId3", SqlDbType.Int, item.ItemOptionSetIds[2]);
                BuildSqlParameter(sqlCmd, "@ItemPrintOptionSetId", SqlDbType.Int, item.PrintDestinations);
                BuildSqlParameter(sqlCmd, "@ItemActive", SqlDbType.Bit, item.IsActive);
                BuildSqlParameter(sqlCmd, "@ItemTaxId", SqlDbType.Int, item.TaxId);
                BuildSqlParameter(sqlCmd, "@ItemPrepareTime", SqlDbType.Time, item.PrepareTime);
                BuildSqlParameter(sqlCmd, "@ItemIsReturnable", SqlDbType.Bit, item.IsReturnable);
                BuildSqlParameter(sqlCmd, "@ItemIsTaxExemptable", SqlDbType.Bit, item.IsTaxExemptable);
                BuildSqlParameter(sqlCmd, "@ItemIsFired", SqlDbType.Bit, item.IsFired);
                BuildSqlParameter(sqlCmd, "@ItemIsOutOfStock", SqlDbType.Bit, item.IsOutOfStock);
                BuildSqlParameter(sqlCmd, "@ItemIsDiscontinued", SqlDbType.Bit, item.IsDiscontinued);
                BuildSqlParameter(sqlCmd, "@ItemIsGrouping", SqlDbType.Bit, item.IsGrouping);
                BuildSqlParameter(sqlCmd, "@ItemButtonImage", SqlDbType.VarBinary, item.ButtonImage.SerializeObject());
                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static Item FindById(IEnumerable<Item> collection, int id)
        {
            foreach (Item item in collection)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        private static Item BuildItem(SqlDataReader rdr)
        {
            return new Item(
                GetId(rdr),
                GetCategoryId(rdr),
                GetFullName(rdr),
                GetShortName(rdr),
                GetPrice(rdr),
                GetItemOptionSet1(rdr),
                GetItemOptionSet2(rdr),
                GetItemOptionSet3(rdr),
                GetPrintDestination(rdr),
                GetIsActive(rdr),
                GetTaxId(rdr),
                GetPrepareTime(rdr),
                GetIsReturnable(rdr),
                GetIsTaxExemptable(rdr),
                GetIsFired(rdr),
                GetIsOutOfStock(rdr),
                GetIsDiscontinued(rdr),
                GetIsGrouping(rdr),
                GetButtonImage(rdr));
        }

        private static int GetId(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }

        private static int GetCategoryId(SqlDataReader rdr)
        {
            return rdr.GetInt32(1);
        }

        private static string GetFullName(SqlDataReader rdr)
        {
            return rdr.GetString(2);
        }

        private static string GetShortName(SqlDataReader rdr)
        {
            return rdr.GetString(3);
        }

        private static double GetPrice(SqlDataReader rdr)
        {
            return rdr.GetDouble(4);
        }

        private static int GetItemOptionSet1(SqlDataReader rdr)
        {
            return rdr.GetInt32(5);
        }

        private static int GetItemOptionSet2(SqlDataReader rdr)
        {
            return rdr.GetInt32(6);
        }

        private static int GetItemOptionSet3(SqlDataReader rdr)
        {
            return rdr.GetInt32(7);
        }

        private static PrintDestination GetPrintDestination(SqlDataReader rdr)
        {
            return (PrintDestination)rdr.GetInt16(8);
        }

        private static bool GetIsActive(SqlDataReader rdr)
        {
            return rdr.GetBoolean(9);
        }

        private static int GetTaxId(SqlDataReader rdr)
        {
            return rdr.GetInt32(10);
        }

        private static TimeSpan? GetPrepareTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(11))
                return null;
            return rdr.GetTimeSpan(11);
        }

        private static bool GetIsReturnable(SqlDataReader rdr)
        {
            return rdr.GetBoolean(12);
        }

        private static bool GetIsTaxExemptable(SqlDataReader rdr)
        {
            return rdr.GetBoolean(13);
        }

        private static bool GetIsFired(SqlDataReader rdr)
        {
            return rdr.GetBoolean(14);
        }

        private static bool GetIsOutOfStock(SqlDataReader rdr)
        {
            return rdr.GetBoolean(15);
        }

        private static bool GetIsDiscontinued(SqlDataReader rdr)
        {
            return rdr.GetBoolean(16);
        }

        private static bool GetIsGrouping(SqlDataReader rdr)
        {
            return rdr.GetBoolean(17);
        }

        private static byte[] GetButtonImage(SqlDataReader rdr)
        {
            return (rdr.IsDBNull(18) ? null : (byte[]) rdr.GetSqlBytes(18).Buffer.Clone());
        }

        #endregion
    }
}
