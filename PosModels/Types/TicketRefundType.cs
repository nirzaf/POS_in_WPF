using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum TicketRefundType : byte
    {
        Unknown = 0,
        Reopened = 1,
        CancelUnmade = 2,
        CancelMade = 3,
        Void = 4
    }

    public static class TicketRefundTypeExtensions
    {
        public static TicketRefundType GetTicketRefundType(this byte ticketRefundType)
        {
            try
            {
                return (TicketRefundType)ticketRefundType;
            }
            catch
            {
                return TicketRefundType.Unknown;
            }
        }
    }

}
