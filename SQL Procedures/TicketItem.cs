using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

/// <summary>
/// Server-side procudures for the TicketItem model
/// </summary>
public class TicketItem
{
    #region AddTicketItem
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void AddTicketItem(SqlInt16 year, SqlInt32 ticketId, SqlInt32 itemId, SqlInt16 quantity,
        SqlDouble price, SqlDateTime orderTime, SqlDateTime preparedTime, SqlInt32 parentTicketItemId, out SqlInt32 id)
    {
        string query = "INSERT INTO TicketItem (TicketItemYear, TicketItemTicketId, TicketItemItemId, TicketItemQuantity, TicketItemPendingQuantity, TicketItemPrice, TicketItemOrderTime, TicketItemPreparedTime, TicketItemWhenCanceled, TicketItemIsWasted, TicketItemParentTicketItemId) " +
            "VALUES (@TicketItemYear, @TicketItemTicketId, @TicketItemItemId, @TicketItemQuantity, NULL, @TicketItemPrice, @TicketItemOrderTime, @TicketItemPreparedTime, NULL, 0, @TicketItemParentTicketItemId);" +
            "SELECT CAST(scope_identity() AS int)";
	    using (SqlConnection conn = new SqlConnection("context connection=true"))
	    {
		    using (SqlCommand cmd = new SqlCommand(query, conn))
		    {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemTicketId", SqlDbType.Int, ticketId);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemItemId", SqlDbType.Int, itemId);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemQuantity", SqlDbType.SmallInt, quantity);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemPrice", SqlDbType.Float, price);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemOrderTime", SqlDbType.DateTime, orderTime);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemPreparedTime", SqlDbType.DateTime, preparedTime);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemParentTicketItemId", SqlDbType.Int, parentTicketItemId);
                conn.Open();
			    id = new SqlInt32((Int32)cmd.ExecuteScalar());
		    }
            conn.Close();
	    }
    }
    #endregion

    #region GetTicketItemCost
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void GetTicketItemCost(SqlInt16 year, SqlInt32 ticketItemId, SqlInt16 refundedItemQuantity, out SqlDouble itemCost)
    {
        itemCost = GetItemCost(ticketItemId, year);
        SqlInt32 itemId = GetItemId(ticketItemId, year);
        SqlInt16 pendingQuantity = GetPendingQuantity(ticketItemId, year);
        SqlInt16 quantity = refundedItemQuantity;
        if (!pendingQuantity.IsNull)
            quantity = pendingQuantity;
        else if (refundedItemQuantity == 0)
            quantity = GetQuantity(ticketItemId, year);

        bool inExcess = false;
        SqlByte count = 0;
        SqlByte freeAllowed = 0;
        SqlInt32 currentItemOptionSetId = 0;
        SqlDouble additionalCost = 0;
        ItemOptionTempTableModel[] results = CreateTicketItemCostTempTable(ticketItemId, year);
        foreach (ItemOptionTempTableModel result in results)
        {
            if (currentItemOptionSetId != result.SetId)
            {
                inExcess = false;
                count = 0;
                currentItemOptionSetId = result.SetId;
                freeAllowed = GetFreeAllowed(result.SetId);
                additionalCost = GetAdditionalCost(result.SetId);
            }
            count = count + 1;
            if (count > freeAllowed)
                inExcess = true;

            if (inExcess)
            {
                if (!result.CostForExtra.IsNull)
                {
                    if (result.Type == 1)
                        itemCost += result.CostForExtra;
                    else if ((result.Type == 3) || (result.Type == 4))
                        itemCost += (result.CostForExtra / 2);
                }
                else
                {
                    if (result.Type == 1)
                        itemCost += additionalCost;
                    else if ((result.Type == 3) || (result.Type == 4))
                        itemCost += (additionalCost / 2);
                }
            }
        }
        itemCost *= quantity;
    }

    private static SqlDouble GetAdditionalCost(SqlInt32 itemSetId)
    {
        SqlDouble cost = 0;
        string query = "SELECT CAST((SELECT ItemOptionSetExcessUnitCost FROM ItemOptionSet WHERE ItemOptionSetId=@ItemOptionSetId) AS float)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@ItemOptionSetId", SqlDbType.Int, itemSetId);
                conn.Open();
                cost = new SqlDouble((Double)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return cost;
    }

    private static SqlByte GetFreeAllowed(SqlInt32 itemSetId)
    {
        SqlByte freeAllowed = 0;
        string query = "SELECT CAST((SELECT ItemOptionSetNumFree FROM ItemOptionSet WHERE ItemOptionSetId=@ItemOptionSetId) AS tinyint)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@ItemOptionSetId", SqlDbType.Int, itemSetId);
                conn.Open();
                freeAllowed = new SqlByte((Byte)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return freeAllowed;
    }

    private static ItemOptionTempTableModel[] CreateTicketItemCostTempTable(SqlInt32 ticketItemId, SqlInt16 year)
    {
        List<ItemOptionTempTableModel> list = new List<ItemOptionTempTableModel>();
        string query =
            "SELECT TicketItemOptionYear,TicketItemOptionItemOptionId,ItemOption.ItemOptionItemOptionSetId,TicketItemOptionType,TicketItemOptionChangeCount,ItemOption.ItemOptionCostForExtra " +
            "FROM TicketItemOption " +
            "INNER JOIN ItemOption ON TicketItemOption.TicketItemOptionItemOptionId = ItemOption.ItemOptionId " +
            "WHERE (TicketItemOptionYear=@TicketItemOptionYear AND TicketItemOptionTicketItemId=@TicketItemId) " +
            "ORDER BY ItemOption.ItemOptionItemOptionSetId,ItemOption.ItemOptionCostForExtra;";

        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemOptionYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildItemOptionTempTableModel(rdr));
                }
            }
            conn.Close();
        }
        return list.ToArray();
    }

    private static ItemOptionTempTableModel BuildItemOptionTempTableModel(SqlDataReader rdr)
    {
        SqlInt16 year = new SqlInt16(Convert.ToInt16(rdr[0].ToString()));
        SqlInt32 id = new SqlInt32(Convert.ToInt32(rdr[1].ToString()));
        SqlInt32 setId = new SqlInt32(Convert.ToInt32(rdr[2].ToString()));
        SqlByte type = new SqlByte(Convert.ToByte(rdr[3].ToString()));
        SqlByte changeCount = new SqlByte(Convert.ToByte(rdr[4].ToString()));
        SqlDouble costForExtra = SqlDouble.Null;
        if (!rdr.IsDBNull(5))
             costForExtra = new SqlDouble(Convert.ToDouble(rdr[5].ToString()));
        return new ItemOptionTempTableModel(year, id, setId, type, changeCount, costForExtra);
    }

    private class ItemOptionTempTableModel
    {
        public SqlInt16 Year;
        public SqlInt32 ItemOptionId;
        public SqlInt32 SetId;
        public SqlByte Type;
        public SqlByte ChangeCount;
        public SqlDouble CostForExtra;
        public ItemOptionTempTableModel(SqlInt16 year, SqlInt32 itemOptionId, SqlInt32 setId,
            SqlByte type, SqlByte changeCount, SqlDouble costForExtra)
        {
            Year = year;
            ItemOptionId = itemOptionId;
            SetId = setId;
            Type = type;
            ChangeCount = changeCount;
            CostForExtra = costForExtra;
        }
    }

    private static SqlDouble GetItemCost(SqlInt32 ticketItemId, SqlInt16 year)
    {
        SqlDouble cost = 0;
        string query = "SELECT CAST((SELECT TicketItemPrice FROM TicketItem " +
            "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)) AS float)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                cost = new SqlDouble((Double)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return cost;
    }

    private static SqlInt32 GetItemId(SqlInt32 ticketItemId, SqlInt16 year)
    {
        SqlInt32 itemId = 0;
        string query = "SELECT CAST((SELECT TicketItemPrice FROM TicketItem " +
            "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)) AS int)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                itemId = new SqlInt32((Int32)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return itemId;
    }

    private static SqlInt16 GetPendingQuantity(SqlInt32 ticketItemId, SqlInt16 year)
    {
        SqlInt16 itemId = 0;
        string query = "SELECT CAST((SELECT TicketItemPendingQuantity FROM TicketItem " +
            "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)) AS smallint)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == DBNull.Value)
                    itemId = SqlInt16.Null;
                else
                    itemId = new SqlInt16((Int16)result);
            }
            conn.Close();
        }
        return itemId;
    }

    private static SqlInt16 GetQuantity(SqlInt32 ticketItemId, SqlInt16 year)
    {
        SqlInt16 itemId = 0;
        string query = "SELECT CAST((SELECT TicketItemQuantity FROM TicketItem " +
            "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)) AS smallint)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                itemId = new SqlInt16((Int16)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return itemId;
    }
    #endregion

    #region GetTicketItemCoupon
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void GetTicketItemCoupon(SqlInt16 year, SqlInt32 ticketItemId, SqlDouble itemPrice, out SqlDouble itemCoupon)
    {
        itemCoupon = 0;
        SqlInt32 ticketId = GetTicketId(ticketItemId, year);
        TicketItemCouponTempTableModel[] coupons =
            CreateTicketItemCouponTempTable(ticketId, year);
        foreach (TicketItemCouponTempTableModel coupon in coupons)
        {
            if ((coupon.TicketItemId.IsNull) || (coupon.TicketItemId == ticketItemId))
            {
                SqlDouble thisCouponAmount = 0;
                SqlDouble couponAmount = GetCouponAmount(coupon.CouponId);
                SqlBoolean amountIsPercentage = GetCouponAmountIsPercentage(coupon.CouponId);
                SqlDouble amountLimit = GetCouponAmountLimit(coupon.CouponId);
                SqlBoolean thirdPartyCompensation = GetCouponThirdPartyCompensation(coupon.CouponId);

                if (!coupon.TicketItemId.IsNull)
                {
                    if (amountIsPercentage)
                        thisCouponAmount = itemPrice * couponAmount;
                    else
                        thisCouponAmount = couponAmount;
                }
                else if (amountIsPercentage)
                {
                    thisCouponAmount = itemPrice * couponAmount;
                }
                if (!amountLimit.IsNull && (amountLimit < couponAmount))
                    thisCouponAmount = amountLimit;
                itemCoupon += thisCouponAmount;
            }
        }
    }

    private static SqlBoolean GetCouponThirdPartyCompensation(SqlInt32 couponId)
    {
        SqlBoolean is3rdParty = false;
        string query = "SELECT CAST((SELECT CouponAmountIsPercentage FROM Coupon WHERE CouponId=@CouponId) AS bit)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@CouponId", SqlDbType.Int, couponId);
                conn.Open();
                is3rdParty = new SqlBoolean((Boolean)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return is3rdParty;
    }

    private static SqlDouble GetCouponAmountLimit(SqlInt32 couponId)
    {
        SqlDouble amountLimit = 0;
        string query = "SELECT CAST((SELECT CouponAmountLimit FROM Coupon WHERE CouponId=@CouponId) AS float)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@CouponId", SqlDbType.Int, couponId);
                conn.Open();
                amountLimit = new SqlDouble((Double)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return amountLimit;
    }

    private static SqlBoolean GetCouponAmountIsPercentage(SqlInt32 couponId)
    {
        SqlBoolean amountIsPercentage = false;
        string query = "SELECT CAST((SELECT CouponAmountIsPercentage FROM Coupon WHERE CouponId=@CouponId) AS bit)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@CouponId", SqlDbType.Int, couponId);
                conn.Open();
                amountIsPercentage = new SqlBoolean((Boolean)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return amountIsPercentage;
    }

    private static SqlInt32 GetTicketId(SqlInt32 ticketItemId, SqlInt16 year)
    {
        SqlInt32 ticketId = 0;
        string query = "SELECT CAST((SELECT TicketItemTicketId FROM TicketItem " +
            "WHERE (TicketItemYear=@TicketItemYear AND TicketItemId=@TicketItemId)) AS int)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketItemId", SqlDbType.Int, ticketItemId);
                conn.Open();
                ticketId = new SqlInt32((Int32)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return ticketId;
    }

    private static SqlDouble GetCouponAmount(SqlInt32 couponId)
    {
        SqlDouble couponAmount = 0;
        string query = "SELECT CAST((SELECT CouponAmount FROM Coupon WHERE CouponId=@CouponId) AS float)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@CouponId", SqlDbType.Int, couponId);
                conn.Open();
                couponAmount = new SqlDouble((Double)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return couponAmount;
    }

    private static TicketItemCouponTempTableModel[] CreateTicketItemCouponTempTable(SqlInt32 ticketId, SqlInt16 year)
    {
        List<TicketItemCouponTempTableModel> list = new List<TicketItemCouponTempTableModel>();
        string query =
            "SELECT TicketCouponCouponId, TicketCouponYear, TicketCouponTicketItemId " +
            "FROM TicketCoupon " +
            "WHERE (TicketCouponYear=@TicketYear AND TicketCouponTicketId=@TicketId);";

        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketId", SqlDbType.Int, ticketId);
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildTicketItemCouponTempTableModel(rdr));
                }
            }
            conn.Close();
        }
        return list.ToArray();
    }

    private static TicketItemCouponTempTableModel BuildTicketItemCouponTempTableModel(SqlDataReader rdr)
    {
        SqlInt32 couponId = new SqlInt32(Convert.ToInt32(rdr[0].ToString()));
        SqlInt16 year = new SqlInt16(Convert.ToInt16(rdr[1].ToString()));
        SqlInt32 ticketItemId = SqlInt32.Null;
        if (!rdr.IsDBNull(2))
            ticketItemId = new SqlInt32(Convert.ToInt32(rdr[2].ToString()));
        return new TicketItemCouponTempTableModel(couponId, year, ticketItemId);
    }

    private class TicketItemCouponTempTableModel
    {
        public SqlInt32 CouponId;
        public SqlInt16 Year;
        public SqlInt32 TicketItemId;
        public TicketItemCouponTempTableModel(SqlInt32 couponId, SqlInt16 year, 
            SqlInt32 ticketItemId)
        {
            CouponId = couponId;
            Year = year;
            TicketItemId = ticketItemId;
        }
    }
    #endregion

    #region GetTicketItemDiscount
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void GetTicketItemDiscount(SqlInt16 year, SqlInt32 ticketItemId, SqlDouble ticketItemPrice, out SqlDouble itemDiscount)
    {
        SqlInt32 ticketId = GetTicketId(ticketItemId, year);
        TicketItemDiscountTempTableModel[] ticketDiscounts =
            CreateTicketItemDiscountTempTable(ticketId, year);
        itemDiscount = 0;
        foreach (TicketItemDiscountTempTableModel ticketDiscount in ticketDiscounts)
        {
            if ((!ticketDiscount.TicketItemId.IsNull) &&
                (ticketDiscount.TicketItemId != ticketItemId))
                continue;
            SqlDouble discountAmount = 0;
            SqlDouble discountUsed = 0;
            
            // Is the amount specified in the TicketDiscount table (or Discount table)?
            if (!ticketDiscount.DiscountAmount.IsNull)
                discountUsed = ticketDiscount.DiscountAmount;
            else
                discountUsed = GetDiscountAmount(ticketDiscount.DiscountId);
            
            // Process the discount
            SqlBoolean amountIsPercentage = GetDiscountAmountIsPercentage(ticketDiscount.DiscountId);
            if (discountUsed > 0 && amountIsPercentage)
                discountAmount = (ticketItemPrice * discountUsed);
            else if (discountUsed > 0)
                discountAmount = discountUsed;
            
            // Adjust totals
            ticketItemPrice -= discountAmount;
            itemDiscount += discountAmount;
        }
    }

    private static SqlDouble GetDiscountAmount(SqlInt32 discountId)
    {
        SqlDouble discountAmount = 0;
        string query = "SELECT CAST((SELECT DiscountAmount FROM Discount WHERE DiscountId=@DiscountId) AS float)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@DiscountId", SqlDbType.Int, discountId);
                conn.Open();
                discountAmount = new SqlDouble((Double)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return discountAmount;
    }

    private static SqlBoolean GetDiscountAmountIsPercentage(SqlInt32 discountId)
    {
        SqlBoolean amountIsPercentage = false;
        string query = "SELECT CAST((SELECT DiscountAmountIsPercentage FROM Discount WHERE DiscountId=@DiscountId) AS bit)";
        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@DiscountId", SqlDbType.Int, discountId);
                conn.Open();
                amountIsPercentage = new SqlBoolean((Boolean)cmd.ExecuteScalar());
            }
            conn.Close();
        }
        return amountIsPercentage;
    }

    private static TicketItemDiscountTempTableModel[] CreateTicketItemDiscountTempTable(SqlInt32 ticketId, SqlInt16 year)
    {
        List<TicketItemDiscountTempTableModel> list = new List<TicketItemDiscountTempTableModel>();
        string query =
            "SELECT TicketDiscountDiscountId, TicketDiscountYear, TicketDiscountTicketItemId, TicketDiscountAmount " +
            "FROM TicketDiscount " +
            "WHERE (TicketDiscountYear=@TicketYear AND TicketDiscountTicketId=@TicketId);";

        using (SqlConnection conn = new SqlConnection("context connection=true"))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketYear", SqlDbType.SmallInt, year);
                DatabaseHelper.BuildSqlParameter(cmd, "@TicketId", SqlDbType.Int, ticketId);
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(BuildTicketItemDiscountTempTableModel(rdr));
                }
            }
            conn.Close();
        }
        return list.ToArray();
    }

    private static TicketItemDiscountTempTableModel BuildTicketItemDiscountTempTableModel(SqlDataReader rdr)
    {
        SqlInt32 discountId = new SqlInt32(Convert.ToInt32(rdr[0].ToString()));
        SqlInt16 year = new SqlInt16(Convert.ToInt16(rdr[1].ToString()));
        SqlInt32 ticketItemId = SqlInt32.Null;
        if (!rdr.IsDBNull(2))
            ticketItemId = new SqlInt32(Convert.ToInt32(rdr[2].ToString()));
        SqlDouble discountAmount = SqlDouble.Null;
        if (!rdr.IsDBNull(3))
            discountAmount = new SqlInt32(Convert.ToInt32(rdr[3].ToString()));
        return new TicketItemDiscountTempTableModel(discountId, year, ticketItemId, discountAmount);
    }

    private class TicketItemDiscountTempTableModel
    {
        public SqlInt32 DiscountId;
        public SqlInt16 Year;
        public SqlInt32 TicketItemId;
        public SqlDouble DiscountAmount;
        public TicketItemDiscountTempTableModel(SqlInt32 discountId, SqlInt16 year, SqlInt32 ticketItemId, SqlDouble discountAmount)
        {
            DiscountId = discountId;
            Year = year;
            TicketItemId = ticketItemId;
            DiscountAmount = discountAmount;
        }
    }
    #endregion
}
