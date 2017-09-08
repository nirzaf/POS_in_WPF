using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum TicketType : short
    {
        DineIn = 0,             // Customer is being served
        DriveThru = 1,          // Customer will pick it up with their vehicle
        Pickup = 2,             // Customer will pick it up, and take it to-go
        Delivery = 3,           // Order will be delivered to customer
        Retail = 4,             // No seating, we don't care who the customer is
        Catering = 5,           // Delivery with more than one tickets
        GasStation = 6,         // Extenstion of Retail with support for authorizing and pre-paying gas pumps
        Reshipping = 7,         // Buy and re-sell, products are shipped and received, via cargo.
        
        EndOfDayDummyTicket = 100,
    }

    public static class TicketTypeHelper
    {
        public static TicketType GetTicketType(this short ticketTypeId)
        {
            try
            {
                return (TicketType)ticketTypeId;
            }
            catch
            {
                return TicketType.EndOfDayDummyTicket;
            }
        }

        public static string GetFriendlyName(this TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.DineIn:
                    return "Dine-In";
                case TicketType.DriveThru:
                    return "Drive-Thru";
                case TicketType.Pickup:
                    return "Carryout";
                default:
                    return ticketType.ToString();
            }
        }
    }
}
