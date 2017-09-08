using System;
using System.Collections.Generic;
using System.Linq;
using PosModels.Types;

namespace PosModels.Managers
{
    public static class TicketManager
    {
        private static Dictionary<YearId, Ticket> Tickets = new Dictionary<YearId, Ticket>();

        static TicketManager()
        {
            IEnumerable<Ticket> tickets = Ticket.GetAll();
            foreach (Ticket ticket in tickets)
            {
                Tickets.Add(ticket.PrimaryKey, ticket);
            }
        }

        /// <summary>
        /// Add a new entry to the Ticket table
        /// </summary>
        public static Ticket Add(TicketType type, int partyId, int seatingId,
            int employeeId, int customerId)
        {
            Ticket newTicket = Ticket.Add(type, partyId, seatingId, employeeId, customerId);
            Tickets.Add(newTicket.PrimaryKey, newTicket);
            return newTicket;
        }

        /// <summary>
        /// Delete a Ticket table entry
        /// </summary>
        public static bool Delete(YearId primaryKey)
        {
            // Scan existing
            if (Tickets.Keys.Contains(primaryKey))
            {
                Tickets.Remove(primaryKey);
            }
            return Ticket.Delete(primaryKey);
        }

        /// <summary>
        /// Get a single ticket from the Ticket table
        /// </summary>
        public static Ticket GetTicket(YearId primaryKey)
        {
            // Scan existing
            if (Tickets.Keys.Contains(primaryKey))
            {
                Ticket.Refresh(Tickets[primaryKey]);
                return Tickets[primaryKey];
            }

            // Not found, let's check the database
            Ticket newTicket = Ticket.Get(primaryKey);
            if (newTicket != null)
            {
                Tickets.Add(newTicket.PrimaryKey, newTicket);
                return newTicket;
            }
            return null;
        }

        /// <summary>
        /// Get all tickets in the Ticket table
        /// </summary>
        public static IEnumerable<Ticket> GetAllTickets()
        {
            IEnumerable<Ticket> currentTickets = Ticket.GetAll();
            foreach (Ticket ticket in currentTickets)
            {
                yield return GetUpdatedManagedTicket(ticket);
            }
        }

        /// <summary>
        /// Get all the tickets in a party, for the specified party
        /// </summary>
        public static IEnumerable<Ticket> GetPartyTickets(int partyId)
        {
            IEnumerable<Ticket> currentPartyTickets = Ticket.GetAll(partyId);
            foreach (Ticket ticket in currentPartyTickets)
            {
                yield return GetUpdatedManagedTicket(ticket);
            }
        }

        /// <summary>
        /// Get all open tickets
        /// </summary>
        public static IEnumerable<Ticket> GetOpenTickets(int? employeeId = null)
        {
            foreach (Ticket ticket in Ticket.GetAllActive())
            {
                if ((employeeId != null) &&
                    (ticket.StartTime != null) &&
                    (ticket.PrepareTime == null))
                    continue;
                if ((employeeId == null) || (ticket.EmployeeId == employeeId.Value))
                    yield return GetUpdatedManagedTicket(ticket);
            }
        }

        /// <summary>
        /// Get all closed tickets for today
        /// </summary>
        public static IEnumerable<Ticket> GetTodaysClosedTickets()
        {
            if (DayOfOperation.Today != null)
            {
                IEnumerable<Ticket> currentTickets =
                    Ticket.GetAllStartingAt(GetOrderIdOffset() + 1);
                currentTickets = FilterByClosed(currentTickets, true);
                foreach (Ticket ticket in currentTickets)
                {
                    yield return GetUpdatedManagedTicket(ticket);
                }
            }
        }

        /// <summary>
        /// Get all voided tickets for today
        /// </summary>
        public static IEnumerable<Ticket> GetTodaysCanceledTickets()
        {
            if (DayOfOperation.Today != null)
            {
                IEnumerable<Ticket> currentTickets =
                    Ticket.GetAllStartingAt(GetOrderIdOffset() + 1);
                currentTickets = FilterByCanceled(currentTickets, true);
                foreach (Ticket ticket in currentTickets)
                {
                    yield return GetUpdatedManagedTicket(ticket);
                }
            }
        }

        public static IEnumerable<Ticket> GetTickets(IEnumerable<YearId> ticketPrimaryKeys)
        {
            foreach (YearId ticketPrimaryKey in ticketPrimaryKeys)
            {
                yield return TicketManager.GetTicket(ticketPrimaryKey);
            }
        }

        public static double GetSumOfCoupons(IEnumerable<Ticket> tickets)
        {
            double couponTotal = 0;
            foreach (Ticket ticket in tickets)
            {
                couponTotal += ticket.GetCouponTotal();
            }
            return couponTotal;
        }

        public static double GetSumOfDiscounts(IEnumerable<Ticket> tickets)
        {
            double discountTotal = 0;
            foreach (Ticket ticket in tickets)
            {
                discountTotal += ticket.GetDiscountTotal();
            }
            return discountTotal;
        }

        public static double GetSumOfTaxes(IEnumerable<Ticket> tickets)
        {
            double taxes = 0;
            foreach (Ticket ticket in tickets)
            {
                taxes += ticket.GetTax();
            }
            return taxes;
        }

        public static double[] GetSumOfTickets(IEnumerable<Ticket> tickets)
        {
            double totalAmount = 0;
            double totalTaxExempt = 0;
            foreach (Ticket ticket in tickets)
            {
                double ticketTotal = ticket.GetSubTotal() + ticket.GetTax() -
                    ticket.GetDiscountTotal() - ticket.GetCouponTotal();
                totalAmount += ticketTotal;
                if (ticket.TaxExemptId != null)
                    totalTaxExempt += ticketTotal;
            }
            return new double[] { totalAmount, totalTaxExempt };
        }

        public static IEnumerable<Ticket> GetDispatchedTickets()
        {
            foreach (TicketDelivery ticketDelivery in TicketDelivery.GetAllActive())
            {
                yield return TicketManager.GetTicket(
                    new YearId(ticketDelivery.PrimaryKey.Year, ticketDelivery.TicketId));
            }
        }

        public static IEnumerable<Ticket> GetFutureOrderTickets()
        {
            foreach (Ticket ticket in Ticket.GetAllActive())
            {
                if ((ticket.PrepareTime == null) &&
                    (ticket.StartTime != null) &&
                    (ticket.StartTime > DateTime.Now))
                {
                    yield return ticket;
                }
            }
        }

        public static IEnumerable<Ticket> GetRange(DateTime startDate, DateTime endDate)
        {
            foreach (Ticket ticket in Ticket.GetAll(startDate, endDate))
            {
                yield return GetUpdatedManagedTicket(ticket);
            }
        }

        public static IEnumerable<Ticket> FilterByDate(IEnumerable<Ticket> tickets, DateTime date)
        {
            foreach (Ticket ticket in tickets)
            {
                if (ticket.CreateTime.Date.Equals(date.Date))
                    yield return ticket;
            }
        }

        public static IEnumerable<Ticket> FilterByCanceled(IEnumerable<Ticket> tickets, bool isCanceled)
        {
            foreach (Ticket ticket in tickets)
            {
                if (ticket.IsCanceled == isCanceled)
                    yield return ticket;
            }
        }

        public static IEnumerable<Ticket> FilterByClosed(IEnumerable<Ticket> tickets, bool isClosed)
        {
            foreach (Ticket ticket in tickets)
            {
                if (!ticket.IsCanceled &&
                    (((ticket.CloseTime == null) && !isClosed) ||
                     ((ticket.CloseTime != null) && isClosed)))
                    yield return ticket;
            }
        }

        public static int GetOrderIdOffset()
        {
            StoreSetting dailyIdOffset =
                SettingManager.GetStoreSetting("DailyIdOffset");
            if ((dailyIdOffset == null) || !dailyIdOffset.IntValue.HasValue)
                return 0;
            return dailyIdOffset.IntValue.Value;
        }

        /// <summary>
        /// This is the last call in the end-of-day report
        /// </summary>
        public static void SetOrderIdOffset()
        {
            Ticket ticket = Ticket.Add(TicketType.EndOfDayDummyTicket, 0, 0, 0, 0);
            int offsetId = Ticket.GetHighestTicketId(ticket.PrimaryKey.Year);
            Ticket.Delete(ticket.PrimaryKey);
            SettingManager.SetStoreSetting("DailyIdOffset", offsetId);
        }

        /// <summary>
        /// Gets the managed Ticket, or creates a managed Ticket if
        /// one doesn't exist.
        /// </summary>
        private static Ticket GetUpdatedManagedTicket(Ticket ticket)
        {
            if (!Tickets.Keys.Contains(ticket.PrimaryKey))
            {
                // Ticket is not a managed instance yet
                Tickets.Add(ticket.PrimaryKey, ticket);
            }
            else
            {
                // Refresh the managed Ticket with the current ticket instance
                Ticket.Refresh(Tickets[ticket.PrimaryKey], ticket);
            }
            return Tickets[ticket.PrimaryKey];
        }

    }
}
