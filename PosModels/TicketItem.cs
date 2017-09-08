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
    [ModeledDataClass]
    public class TicketItem : DataModelBase
    {
        #region Licensed Access Only
        static TicketItem()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TicketItem).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData("Year", "Id")]
        [ModeledDataType("SMALLINT", "INT")]
        public YearId PrimaryKey
        {
            get;
            private set;
        }

        [ModeledData]
        public int ItemId
        {
            get;
            private set;
        }

        [ModeledData]
        public int TicketId
        {
            get;
            private set;
        }

        [ModeledData]
        [ModeledDataType("SMALLINT")]
        public int Quantity
        {
            get;
            private set;
        }

        [ModeledData("PendingQuantity")]
        [ModeledDataType("SMALLINT")]
        public int? QuantityPending
        {
            get;
            private set;
        }

        [ModeledData]
        public double Price
        {
            get;
            private set;
        }

        [ModeledData]
        public DateTime? OrderTime
        {
            get;
            private set;
        }

        [ModeledData]
        public DateTime? PreparedTime
        {
            get;
            private set;
        }

        [ModeledData]
        public DateTime? WhenCanceled
        {
            get;
            private set;
        }

        [ModeledData]
        public int? CancelingEmployeeId
        {
            get;
            private set;
        }

        [ModeledData]
        [ModeledDataType("TINYINT")]
        public CancelType? CancelType
        {
            get;
            private set;
        }

        [ModeledData("SpecialInstruction")]
        [ModeledDataNullable]
        public string SpecialInstructions
        {
            get;
            private set;
        }

        [ModeledData]
        public bool IsWasted
        {
            get;
            private set;
        }

        [ModeledData]
        public int? ParentTicketItemId
        {
            get;
            private set;
        }

        [ModeledData]
        public DateTime? FireTime
        {
            get;
            private set;
        }

        private TicketItem(YearId primaryKey, int ticketId, int itemId, int quantity, int? pendingQuantity,
            double price, DateTime? orderTime, DateTime? preparedTime, DateTime? whenCanceled,
            int? cancelingEmployeeId, CancelType? cancelType, string specialInstructions, bool isWasted,
            int? parentTicketItemId, DateTime? fireTime)
        {
            PrimaryKey = primaryKey;
            ItemId = itemId;
            TicketId = ticketId;
            Quantity = quantity;
            QuantityPending = pendingQuantity;
            Price = price;
            OrderTime = orderTime;
            PreparedTime = preparedTime;
            WhenCanceled = whenCanceled;
            CancelingEmployeeId = cancelingEmployeeId;
            CancelType = cancelType;
            SpecialInstructions = specialInstructions;
            IsWasted = isWasted;
            ParentTicketItemId = parentTicketItemId;
            FireTime = fireTime;
            // Non-model
            IsChanged = false;
            IsQuantityChanged = false;
            IsTicketItemOptionsChanged = false;
            IsPendingReturn = false;
            QuantityPendingReturn = 0;
        }

        public bool IsChanged
        {
            get;
            private set;
        }

        public bool IsQuantityChanged
        {
            get;
            private set;
        }

        public bool IsTicketItemOptionsChanged
        {
            get;
            private set;
        }

        public bool IsPendingReturn
        {
            get;
            private set;
        }

        public int QuantityPendingReturn
        {
            get;
            private set;
        }

        public bool IsCanceled
        {
            get
            {
                return (WhenCanceled != null);
            }
        }

        public bool Delete(bool adjustInventory = true)
        {
            IsPendingReturn = false;
            QuantityPendingReturn = 0;
            Instances.Remove(PrimaryKey);
            TicketItemOption.DeleteAll(this, adjustInventory);
            return Delete(PrimaryKey, adjustInventory);
        }

        public void Fire()
        {
            FireTime = DateTime.Now;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText =
                    "UPDATE TicketItem SET TicketItemFireTime=@TicketItemFireTime " +
                    "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)";

                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemFireTime", SqlDbType.DateTime, FireTime);

                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Pulls the immediate quantity for this TicketItem
        /// </summary>
        /// <returns></returns>
        public int GetCurrentQuantity()
        {
            int result = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TicketItemQuantity FROM TicketItem WHERE (TicketItemYear=" +
                PrimaryKey.Year + " AND TicketItemId=" + PrimaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = rdr.GetInt16(0);
                    }
                }
            }
            FinishedWithConnection(cn);

            return result;
        }

        /// <summary>
        /// Gets the cost of this TicketItem, plus it's TicketItemOption costs
        /// </summary>
        public double GetTotalCost(int returnItemQuantity = 0)
        {
            double result = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketItemCost", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@ReturnedItemQuantity", SqlDbType.SmallInt, returnItemQuantity);
                BuildSqlParameter(sqlCmd, "@ItemPrice", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@ItemPrice"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@ItemPrice"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Gets the discount value for this TicketItem
        /// </summary>
        public double GetTotalDiscount(int returnItemQuantity = 0)
        {
            double itemCost = GetTotalCost(returnItemQuantity);
            return GetTotalDiscountInternal(itemCost);
        }

        /// <summary>
        /// Gets the discount value for this TicketItem
        /// </summary>
        public double GetTotalDiscount(double itemCost)
        {
            return GetTotalDiscountInternal(itemCost);
        }

        private double GetTotalDiscountInternal(double itemCost)
        {
            double result = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketItemDiscount", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemPrice", SqlDbType.Float, itemCost);
                BuildSqlParameter(sqlCmd, "@ItemDiscount", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@ItemDiscount"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@ItemDiscount"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Gets the coupon value for this TicketItem
        /// </summary>
        public double GetTotalCoupon(int returnItemQuantity = 0)
        {
            double itemCost = GetTotalCost(returnItemQuantity);
            return GetTotalCouponInternal(itemCost);
        }

        /// <summary>
        /// Gets the coupon value for this TicketItem
        /// </summary>
        public double GetTotalCoupon(double itemCost)
        {
            return GetTotalCouponInternal(itemCost);
        }

        private double GetTotalCouponInternal(double itemCost)
        {
            double result = 0;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("GetTicketItemCoupon", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemPrice", SqlDbType.Float, itemCost);
                BuildSqlParameter(sqlCmd, "@ItemCoupon", SqlDbType.Float, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                if (sqlCmd.Parameters["@ItemCoupon"].Value != DBNull.Value)
                    result = Convert.ToDouble(sqlCmd.Parameters["@ItemCoupon"].Value);
            }
            FinishedWithConnection(cn);
            return result;
        }

        /// <summary>
        /// Used for returns
        /// </summary>
        public double GetTax(double itemCost)
        {
            Item item = Item.Get(ItemId);
            Tax tax = Tax.Get(item.TaxId);
            Ticket ticket = Ticket.Get(YearId.Create(PrimaryKey.Year, TicketId));
            if (item.IsTaxExemptable && (ticket.TaxExemptId != null))
                return 0;
            return (itemCost * tax.Percentage);
        }

        public bool PrintsTo(PrintDestination destination)
        {
            Item item = Item.Get(ItemId);
            return ((item.PrintDestinations & destination) != 0);
        }

        public void SetIsWasted(bool isWasted)
        {
            IsWasted = isWasted;
        }

        public void SetQuantityPendingReturn(int quantity)
        {
            quantity = quantity.Clamp(1, Quantity);
            QuantityPendingReturn = quantity;
        }

        public void SetTicketId(int ticketId)
        {
            IsChanged = true;
            TicketId = ticketId;
        }

        public void SetItemId(int itemId)
        {
            IsChanged = true;
            ItemId = itemId;
        }

        public void SetQuantity(int quantity)
        {
            quantity = quantity.Clamp(1, short.MaxValue);
            int difference = quantity - Quantity;
            if (difference > 0)
                AdjustInventory(this, false, difference);
            else if (difference < 0)
                AdjustInventory(this, true, (difference * -1));
            Quantity = quantity;
            IsChanged = true;
            IsQuantityChanged = true;
            QuantityPending = quantity;
            UpdatePendingQuantity();
        }

        public void SetPrice(double price)
        {
            if (price < 0)
                price = 0;
            Price = price;
            IsChanged = true;
        }

        public void SetOrderTime(DateTime? time)
        {
            OrderTime = time;
            IsChanged = true;
        }

        public void SetPreparedTime(DateTime? time)
        {
            PreparedTime = time;
            IsChanged = true;
        }

        public void SetSpecialInstructions(string specialInstructions)
        {
            SpecialInstructions = specialInstructions;
            IsChanged = true;
        }

        public void SetTicketItemOptionsChanged()
        {
            IsTicketItemOptionsChanged = true;
            IsChanged = true;
        }

        public void SetIsPendingReturn(bool isPendingReturn)
        {
            IsPendingReturn = isPendingReturn;
        }

        public bool Update()
        {
            if (QuantityPending != null)
                Quantity = QuantityPending.Value;
            IsChanged = false;
            IsQuantityChanged = false;
            IsTicketItemOptionsChanged = false;
            IsPendingReturn = false;
            QuantityPendingReturn = 0;
            QuantityPending = null;
            return Update(this);
        }

        public bool UpdatePendingQuantity()
        {
            Int32 rowsAffected;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketItem SET TicketItemPendingQuantity=@TicketItemPendingQuantity WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)";

                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemPendingQuantity", SqlDbType.SmallInt, QuantityPending);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        public bool UpdateTicketId()
        {
            Int32 rowsAffected;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketItem SET TicketItemTicketId=@TicketItemTicketId WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)";

                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemTicketId", SqlDbType.Int, TicketId);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            return (rowsAffected != 0);
        }

        /// <summary>
        /// Cancel this ticket item
        /// </summary>
        public void Cancel(CancelType cancelType, int employeeId, bool isWasted)
        {
            CancelType = cancelType;
            IsWasted = isWasted;
            if (isWasted && (PreparedTime == null))
                SetPreparedTime(DateTime.Now);
            else if (!isWasted)
                SetPreparedTime(null);
            else
                IsChanged = true;
            CancelingEmployeeId = employeeId;
            WhenCanceled = DateTime.Now;

            if (!isWasted)
                AdjustInventory(this, true);

            Update();
        }

        /// <summary>
        /// UnCancels this ticket item
        /// </summary>
        public void UnCancel()
        {
            bool wasWasted = IsWasted;
            CancelType = null;
            CancelingEmployeeId = null;
            WhenCanceled = null;
            IsChanged = true;
            IsWasted = false;
            if (!wasWasted)
                AdjustInventory(this, false);
            
            Update();
        }

        #region Inventory Adjustments
        public static void AdjustInventory(IEnumerable<TicketItem> ticketItems,
            bool increase)
        {
            foreach (TicketItem ticketItem in ticketItems)
            {
                AdjustInventory(ticketItem, increase);
            }
        }

        public static void AdjustInventory(TicketItem ticketItem, bool increase,
            int? difference = null)
        {
            int quantity =
                (difference != null ? difference.Value :
                (ticketItem.QuantityPending != null ? ticketItem.QuantityPending.Value :
                ticketItem.Quantity));
            foreach (ItemIngredient itemIngredient in
                ItemIngredient.GetAll(ticketItem.ItemId))
            {
                PosModelHelper.AdjustInventoryByIngredient(itemIngredient.IngredientId,
                    increase, itemIngredient.Amount * quantity,
                    itemIngredient.MeasurementUnit);
            }

            foreach (TicketItemOption ticketItemOption in
                TicketItemOption.GetAll(ticketItem.PrimaryKey))
            {
                TicketItemOption.AdjustInventory(ticketItemOption, quantity,
                    increase);
            }
        }
        #endregion


        #region static
        #region Instance Management
        // This class manages it's instances so there is only
        // one instance for a given id.
        private static readonly Dictionary<YearId, TicketItem> Instances =
            new Dictionary<YearId, TicketItem>();

        private readonly List<TicketItemOption> _cachedItemOptions =
            new List<TicketItemOption>();

        public static TicketItem GetInstance(YearId primaryKey)
        {
            return Instances.Keys.Contains(primaryKey) ? Instances[primaryKey] : null;
        }

        public static void DumpCachedChanges()
        {
            foreach (TicketItem ticketItem in Instances.Values)
            {
                if (ticketItem.IsPendingReturn)
                {
                    ticketItem.QuantityPendingReturn = ticketItem.Quantity;
                    ticketItem.IsPendingReturn = false;
                    Get(ticketItem.PrimaryKey);
                }
                else if (ticketItem.OrderTime == null)
                {
                    Delete(ticketItem.PrimaryKey, true);
                }
                else if (ticketItem.IsChanged)
                {
                    if (ticketItem.IsTicketItemOptionsChanged)
                        RestorePreviousTicketItemOptions(ticketItem);
                    ticketItem.IsTicketItemOptionsChanged = false;
                    ticketItem.SetQuantity(ticketItem.GetCurrentQuantity());
                    ticketItem.Update();
                    Get(ticketItem.PrimaryKey);
                }
            }
        }

        private static void RestorePreviousTicketItemOptions(TicketItem ticketItem)
        {
            TicketItemOption.DeleteAll(ticketItem, true);
            foreach (TicketItemOption option in ticketItem._cachedItemOptions)
            {
                TicketItemOption.Add(option.TicketItemId, option.ItemOptionId,
                    option.Type, option.ChangeCount);
            }
        }

        private static TicketItem CreateOrUpdate(YearId primaryKey,
            int ticketId, int itemId, int quantity, int? pendingQuantity, double price,
            DateTime? orderTime, DateTime? preparedTime, DateTime? whenCanceled,
            int? cancelingEmployeeId, CancelType? cancelType, string specialInstructions,
            bool isWasted, int? parentTicketItemId, DateTime? fireTime)
        {
            TicketItem result;
            if (Instances.Keys.Contains(primaryKey))
            {
                result = Instances[primaryKey];
                // If changes are present, the old information in the
                // database will erase the changes, so skip updating
                // instance variables.
                if (!result.IsChanged)
                {
                    result.TicketId = ticketId;
                    result.ItemId = itemId;
                    result.Quantity = quantity;
                    result.Price = price;
                    result.OrderTime = orderTime;
                    result.PreparedTime = preparedTime;
                    result.WhenCanceled = whenCanceled;
                    result.CancelingEmployeeId = cancelingEmployeeId;
                    result.CancelType = cancelType;
                    result.SpecialInstructions = specialInstructions;
                    result.QuantityPending = pendingQuantity;
                    result.IsWasted = isWasted;
                    result.ParentTicketItemId = parentTicketItemId;
                    result.FireTime = fireTime;
                }
            }
            else
            {
                result = new TicketItem(primaryKey, ticketId, itemId, quantity, pendingQuantity, price, orderTime,
                    preparedTime, whenCanceled, cancelingEmployeeId, cancelType, specialInstructions,
                    isWasted, parentTicketItemId, fireTime);
                Instances.Add(primaryKey, result);
            }
            return result;
        }

        public static void CacheTicketItemOptions(TicketItem ticketItem)
        {
            IEnumerable<TicketItemOption> options = TicketItemOption.GetAll(ticketItem.PrimaryKey);
            ticketItem._cachedItemOptions.Clear();
            foreach (TicketItemOption option in options)
            {
                ticketItem._cachedItemOptions.Add(option);
            }
        }
        #endregion

        /// <summary>
        /// Adds an entry for the TicketItem table
        /// </summary>
        public static TicketItem Add(YearId ticketPrimaryKey, int itemId, int quantity,
            double price, DateTime? orderTime, DateTime? preparedTime, int? parentTicketItemId = null)
        {
            TicketItem result;
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = new SqlCommand("AddTicketItem", cn))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, ticketPrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemTicketId", SqlDbType.Int, ticketPrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemItemId", SqlDbType.Int, itemId);
                BuildSqlParameter(sqlCmd, "@TicketItemQuantity", SqlDbType.SmallInt, quantity);
                BuildSqlParameter(sqlCmd, "@TicketItemPrice", SqlDbType.Float, price);
                BuildSqlParameter(sqlCmd, "@TicketItemOrderTime", SqlDbType.DateTime, orderTime);
                BuildSqlParameter(sqlCmd, "@TicketItemPreparedTime", SqlDbType.DateTime, preparedTime);
                BuildSqlParameter(sqlCmd, "@TicketItemParentTicketItemId", SqlDbType.Int, parentTicketItemId);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, ParameterDirection.Output);
                sqlCmd.ExecuteNonQuery();
                result = CreateOrUpdate(YearId.Create(ticketPrimaryKey.Year, Convert.ToInt32(sqlCmd.Parameters["@TicketItemId"].Value)),
                    ticketPrimaryKey.Id, itemId, quantity, null, price, orderTime,
                    preparedTime, null, null, null, null, false, parentTicketItemId, null);
                if (result.PrimaryKey.Id == 0)
                    throw new Exception("No id returned");
            }
            FinishedWithConnection(cn);

            // Reduce the inventory of for the ticketitem
            AdjustInventory(result, false);

            return result;
        }

        public static void ResetAutoIdentity()
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd =
                new SqlCommand("DBCC CHECKIDENT (TicketItem,RESEED,0)", cn))
            {
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
        }

        public static bool YearHasTicketItems(short year)
        {
            bool foundTicket = false;
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TicketItemYear FROM TicketItem WHERE TicketItemYear=" + year, cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            foundTicket = true;
                    }
                }
            }
            FinishedWithConnection(cn);
            return foundTicket;
        }

        /// <summary>
        /// Delete an entry from the TicketItem table
        /// </summary>
        private static bool Delete(YearId primaryKey, bool adjustInventory)
        {
            Int32 rowsAffected = 0;
            TicketItem ticketItem = Get(primaryKey);
            if (ticketItem != null)
            {
                SqlConnection cn = GetConnection();
                using (SqlCommand sqlCmd = cn.CreateCommand())
                {
                    sqlCmd.CommandText = "DELETE FROM TicketItem WHERE (" +
                        "TicketItemYear=" + primaryKey.Year + " AND " +
                        "TicketItemId=" + primaryKey.Id + ")";
                    rowsAffected = sqlCmd.ExecuteNonQuery();
                }
                FinishedWithConnection(cn);

                // Increase the inventory of for the ticketitem
                if (adjustInventory)
                    AdjustInventory(ticketItem, true);
            }

            return (rowsAffected != 0);
        }

        public static IEnumerable<TicketItem> GetAllUnfired(YearId ticketPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (" +
                "TicketItemYear=" + ticketPrimaryKey.Year + " AND " +
                "TicketItemTicketId=" + ticketPrimaryKey.Id + " AND " +
                "(TicketItemWhenCanceled IS NULL) AND " +
                "(TicketItemFireTime IS NULL))", cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        TicketItem ticketItem = BuildTicketItem(rdr);
                        if (Item.GetIsFired(ticketItem.ItemId))
                            yield return ticketItem;
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                FinishedWithConnection(cn);
            }
        }

        public static IEnumerable<TicketItem> GetAllChildTicketItems(YearId ticketItemPrimaryKey)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {

                cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (TicketItemParentTicketItemId=@TicketItemParentTicketItemId AND TicketItemYear=@TicketItemYear)", cn);
                BuildSqlParameter(cmd, "@TicketItemParentTicketItemId", SqlDbType.Int, ticketItemPrimaryKey.Id);
                BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, ticketItemPrimaryKey.Year);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                FinishedWithConnection(cn);
            }
        }

        /// <summary>
        /// Gets all entries for the specified ticket key
        /// </summary>
        /// <param name="ticketPrimaryKey"></param>
        /// <param name="getCanceled"></param>
        /// <param name="getOnlyUnmade"></param>
        /// <returns></returns>
        public static IEnumerable<TicketItem> GetAll(YearId ticketPrimaryKey,
            bool getCanceled = true, bool getOnlyUnmade = false)
        {
            SqlConnection cn = GetConnection();
            SqlCommand cmd = null;
            try
            {
                if (getOnlyUnmade)
                {
                    if (getCanceled)
                        cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (" +
                        "TicketItemYear=" + ticketPrimaryKey.Year + " AND " +
                        "TicketItemTicketId=" + ticketPrimaryKey.Id + " AND " +
                        "(TicketItemOrderTime IS NULL))", cn);
                    else
                        cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (" +
                        "TicketItemYear=" + ticketPrimaryKey.Year + " AND " +
                        "TicketItemTicketId=" + ticketPrimaryKey.Id + " AND " +
                        "(TicketItemWhenCanceled IS NULL) AND " +
                        "(TicketItemOrderTime IS NULL))", cn);
                }
                else if (getCanceled)
                    cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (" +
                    "TicketItemYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketItemTicketId=" + ticketPrimaryKey.Id + ")", cn);
                else
                    cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (" +
                    "TicketItemYear=" + ticketPrimaryKey.Year + " AND " +
                    "TicketItemTicketId=" + ticketPrimaryKey.Id + " AND " +
                    "(TicketItemWhenCanceled IS NULL))", cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                FinishedWithConnection(cn);
            }
        }

        /// <summary>
        /// Get all ticket items in the specified DateTime range
        /// </summary>
        public static IEnumerable<TicketItem> GetAll(DateTime startDate, DateTime endDate,
            bool getCanceled = false)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItem " +
                "WHERE (TicketItemOrderTime >= @TicketItemSearchStartTime AND TicketItemOrderTime <= @TicketItemSearchEndTime AND " +
                (getCanceled ? "TicketItemWhenCanceled IS NOT NULL)" : "TicketItemWhenCanceled IS NULL)"), cn))
            {
                BuildSqlParameter(cmd, "@TicketItemSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all wasted ticket items in the specified DateTime range
        /// </summary>
        public static IEnumerable<TicketItem> GetAllWasted(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT * FROM TicketItem " +
                "WHERE (TicketItemOrderTime >= @TicketItemSearchStartTime AND TicketItemOrderTime <= @TicketItemSearchEndTime AND TicketItemIsWasted=1)", cn))
            {
                BuildSqlParameter(cmd, "@TicketItemSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all TicketItem usage the specified DateTime range
        /// </summary>
        public static IEnumerable<TicketItem> GetAllUsage(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT * FROM TicketItem " +
                "WHERE (TicketItemOrderTime >= @TicketItemSearchStartTime AND TicketItemOrderTime <= @TicketItemSearchEndTime AND " +
                "(TicketItemIsWasted=1 OR (TicketItemWhenCanceled IS NULL)))", cn))
            {
                BuildSqlParameter(cmd, "@TicketItemSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all TicketItem waste the specified DateTime range
        /// </summary>
        public static IEnumerable<TicketItem> GetAllWaste(DateTime startDate, DateTime endDate)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT * FROM TicketItem " +
                "WHERE (TicketItemOrderTime >= @TicketItemSearchStartTime AND TicketItemOrderTime <= @TicketItemSearchEndTime AND " +
                "TicketItemIsWasted=1)", cn))
            {
                BuildSqlParameter(cmd, "@TicketItemSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemSearchEndTime", SqlDbType.DateTime, endDate);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get all ticket items in the specified DateTime range, for a specific employee
        /// </summary>
        public static IEnumerable<TicketItem> GetAll(DateTime startDate, DateTime endDate, int employeeId, bool getCanceled = false)
        {
            SqlConnection cn = GetConnection();
            using (SqlCommand cmd = new SqlCommand(
                    "SELECT TicketItem.*,Ticket.TicketEmployeeId " +
                    "FROM TicketItem " +
                    "INNER JOIN Ticket ON TicketItem.TicketItemTicketId=Ticket.TicketId " +
                    "WHERE (Ticket.TicketEmployeeId=@TicketEmployeeId AND TicketItemOrderTime >= @TicketItemSearchStartTime AND TicketItemOrderTime <= @TicketItemSearchEndTime AND " +
                        (getCanceled ? "TicketItemWhenCanceled IS NOT NULL)" : "TicketItemWhenCanceled IS NULL)"), cn))
            {
                BuildSqlParameter(cmd, "@TicketItemSearchStartTime", SqlDbType.DateTime, startDate);
                BuildSqlParameter(cmd, "@TicketItemSearchEndTime", SqlDbType.DateTime, endDate);
                BuildSqlParameter(cmd, "@TicketEmployeeId", SqlDbType.Int, employeeId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return BuildTicketItem(rdr);
                    }
                }
            }
            FinishedWithConnection(cn);
        }

        /// <summary>
        /// Get an entry from the TicketItem table
        /// </summary>
        public static TicketItem Get(YearId primaryKey)
        {
            SqlConnection cn = GetConnection();
            TicketItem result = Get(cn, primaryKey);
            FinishedWithConnection(cn);
            return result;
        }

        private static TicketItem Get(SqlConnection cn, YearId primaryKey)
        {
            TicketItem result = null;
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM TicketItem WHERE (TicketItemYear=" +
                primaryKey.Year + " AND TicketItemId=" + primaryKey.Id + ")", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = BuildTicketItem(rdr);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update an entry in the TicketItem table
        /// </summary>
        private static bool Update(TicketItem ticketItem)
        {
            SqlConnection cn = GetConnection();
            bool result = Update(cn, ticketItem);
            FinishedWithConnection(cn);
            return result;
        }

        private static bool Update(SqlConnection cn, TicketItem ticketItem)
        {
            Int32 rowsAffected;
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "UPDATE TicketItem SET TicketItemTicketId=@TicketItemTicketId,TicketItemItemId=@TicketItemItemId,TicketItemQuantity=@TicketItemQuantity,TicketItemPendingQuantity=@TicketItemPendingQuantity,TicketItemPrice=@TicketItemPrice,TicketItemOrderTime=@TicketItemOrderTime,TicketItemPreparedTime=@TicketItemPreparedTime,TicketItemWhenCanceled=@TicketItemWhenCanceled,TicketItemCancelingEmployeeId=@TicketItemCancelingEmployeeId,TicketItemCancelType=@TicketItemCancelType,TicketItemSpecialInstruction=@TicketItemSpecialInstruction,TicketItemIsWasted=@TicketItemIsWasted,TicketItemParentTicketItemId=@TicketItemParentTicketItemId,TicketItemFireTime=@TicketItemFireTime WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)";

                BuildSqlParameter(sqlCmd, "@TicketItemYear", SqlDbType.SmallInt, ticketItem.PrimaryKey.Year);
                BuildSqlParameter(sqlCmd, "@TicketItemId", SqlDbType.Int, ticketItem.PrimaryKey.Id);
                BuildSqlParameter(sqlCmd, "@TicketItemTicketId", SqlDbType.Int, ticketItem.TicketId);
                BuildSqlParameter(sqlCmd, "@TicketItemItemId", SqlDbType.Int, ticketItem.ItemId);
                BuildSqlParameter(sqlCmd, "@TicketItemQuantity", SqlDbType.SmallInt, ticketItem.Quantity);
                BuildSqlParameter(sqlCmd, "@TicketItemPendingQuantity", SqlDbType.SmallInt, ticketItem.QuantityPending);
                BuildSqlParameter(sqlCmd, "@TicketItemPrice", SqlDbType.Float, ticketItem.Price);
                BuildSqlParameter(sqlCmd, "@TicketItemOrderTime", SqlDbType.DateTime, ticketItem.OrderTime);
                BuildSqlParameter(sqlCmd, "@TicketItemPreparedTime", SqlDbType.DateTime, ticketItem.PreparedTime);
                BuildSqlParameter(sqlCmd, "@TicketItemWhenCanceled", SqlDbType.DateTime, ticketItem.WhenCanceled);
                BuildSqlParameter(sqlCmd, "@TicketItemCancelingEmployeeId", SqlDbType.Int, ticketItem.CancelingEmployeeId);
                BuildSqlParameter(sqlCmd, "@TicketItemCancelType", SqlDbType.TinyInt, ticketItem.CancelType);
                BuildSqlParameter(sqlCmd, "@TicketItemSpecialInstruction", SqlDbType.Text, ticketItem.SpecialInstructions);
                BuildSqlParameter(sqlCmd, "@TicketItemIsWasted", SqlDbType.Bit, ticketItem.IsWasted);
                BuildSqlParameter(sqlCmd, "@TicketItemParentTicketItemId", SqlDbType.Int, ticketItem.ParentTicketItemId);
                BuildSqlParameter(sqlCmd, "@TicketItemFireTime", SqlDbType.DateTime, ticketItem.FireTime);

                rowsAffected = sqlCmd.ExecuteNonQuery();
            }
            return (rowsAffected != 0);
        }

        public static void Reset(Type type)
        {
            if (type != typeof(TicketItem))
                throw new Exception("Bad use of reset");
            SqlConnection cn = GetConnection();
            using (SqlCommand sqlCmd = cn.CreateCommand())
            {
                sqlCmd.CommandText = "DELETE FROM TicketItem WHERE TicketItemId>0";
                sqlCmd.ExecuteNonQuery();
            }
            FinishedWithConnection(cn);
            ResetAutoIdentity();
        }

        /// <summary>
        /// Build a TicketItem object from a SqlDataReader object
        /// </summary>
        private static TicketItem BuildTicketItem(SqlDataReader rdr)
        {
            return CreateOrUpdate(
                GetPrimaryKey(rdr),
                GetTicketId(rdr),
                GetItemId(rdr),
                GetQuantity(rdr),
                GetPendingQuantity(rdr),
                GetPrice(rdr),
                GetOrderTime(rdr),
                GetPreparedTime(rdr),
                GetWhenCanceled(rdr),
                GetCancelingEmployeeId(rdr),
                GetCancelType(rdr),
                GetSpecialInstructions(rdr),
                GetIsWasted(rdr),
                GetParentTicketItemId(rdr),
                GetFireTime(rdr));
        }

        private static YearId GetPrimaryKey(SqlDataReader rdr)
        {
            return YearId.Create(
                rdr.GetInt16(0),
                rdr.GetInt32(1));
        }

        private static int GetTicketId(SqlDataReader rdr)
        {
            return rdr.GetInt32(2);
        }

        private static int GetItemId(SqlDataReader rdr)
        {
            return rdr.GetInt32(3);
        }

        private static int GetQuantity(SqlDataReader rdr)
        {
            return rdr.GetInt16(4);
        }

        private static int? GetPendingQuantity(SqlDataReader rdr)
        {
            int? pendingQuantity = null;
            if (!rdr.IsDBNull(5))
                pendingQuantity = rdr.GetInt16(5);
            return pendingQuantity;
        }

        private static double GetPrice(SqlDataReader rdr)
        {
            return rdr.GetDouble(6);
        }

        private static DateTime? GetOrderTime(SqlDataReader rdr)
        {
            DateTime? orderTime = null;
            if (!rdr.IsDBNull(7))
                orderTime = rdr.GetDateTime(7);
            return orderTime;
        }

        private static DateTime? GetPreparedTime(SqlDataReader rdr)
        {
            DateTime? preparedTime = null;
            if (!rdr.IsDBNull(8))
                preparedTime = rdr.GetDateTime(8);
            return preparedTime;
        }

        private static DateTime? GetWhenCanceled(SqlDataReader rdr)
        {
            DateTime? whenCanceled = null;
            if (!rdr.IsDBNull(9))
                whenCanceled = rdr.GetDateTime(9);
            return whenCanceled;
        }

        private static int? GetCancelingEmployeeId(SqlDataReader rdr)
        {
            int? cancelingEmployeeId = null;
            if (!rdr.IsDBNull(10))
                cancelingEmployeeId = rdr.GetInt32(10);
            return cancelingEmployeeId;
        }

        private static CancelType? GetCancelType(SqlDataReader rdr)
        {
            CancelType? result = null;
            if (!rdr.IsDBNull(11))
                result = (CancelType)rdr.GetByte(11);
            return result;
        }

        private static string GetSpecialInstructions(SqlDataReader rdr)
        {
            string specialInstructions = null;
            if (!rdr.IsDBNull(12))
            {
                string value = rdr.GetString(12);
                if (!value.Equals(""))
                    specialInstructions = value;
            }
            return specialInstructions;
        }

        private static bool GetIsWasted(SqlDataReader rdr)
        {
            return rdr.GetBoolean(13);
        }
        
        private static int? GetParentTicketItemId(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(14))
                return null;
            return rdr.GetInt32(14);
        }

        private static DateTime? GetFireTime(SqlDataReader rdr)
        {
            if (rdr.IsDBNull(15))
                return null;
            return rdr.GetDateTime(15);
        }
        #endregion

    }
}
