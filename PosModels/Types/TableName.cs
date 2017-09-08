using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum TableName : short
    {
        None = 0,
        Category = 1,
        Coupon = 2,
        CouponCategory = 3,
        CouponItem = 4,
        Customer = 5,
        DeliveryArea = 6,
        DeliveryRestriction = 7,
        Discount = 8,
        Employee = 9,
        EmployeeJob = 10,
        EmployeePayRate = 11,
        EmployeeSchedule = 12,
        EmployeeSetting = 14,
        EmployeeTimesheet = 15,
        GiftCard = 16,
        Ingredient = 17,
        Item = 18,
        ItemIngredient = 19,
        ItemOption = 20,
        ItemOptionSet = 21,
        ItemPricing = 22,
        ItemPricingChangeLog = 23,
        ItemWaste = 24,
        Lock = 25,
        Party = 26,
        PartyInvite = 27,
        PhoneNumber = 28,
        PhoneNumberSet = 29,
        Printer = 30,
        PrintOption = 31,
        PrintOptionSet = 32,
        Register = 33,
        RegisterDrawer = 34,
        RegisterDrop = 35,
        RegisterNoSale = 36,
        Room = 37,
        Seating = 38,
        SeatingReservation = 39,
        StoreSetting = 40,
        Tax = 41,
        Ticket = 42,
        TicketCoupon = 43,
        TicketDelivery = 44,
        TicketDiscount = 45,
        TicketItem = 46,
        TicketItemOption = 47,
        TicketPayment = 48,
        Vendor = 49,
        VendorItem = 50,
        VendorOrder = 51,
        VendorOrderItem = 52,
        ZipCode = 53,
        ZipCodeState = 54,
    }

    public static class TableNameExtensions
    {
        public static TableName GetTableName(this short tableNameId)
        {
            try
            {
                return (TableName)tableNameId;
            }
            catch
            {
                return TableName.None;
            }
        }
    }
}
