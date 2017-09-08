using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PosModels;
using PosModels.Types;
using TemPOS.Helpers;

namespace TemPOS.Managers
{
    public static class SessionManager
    {
        #region Licensed Access Only
        static SessionManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(SessionManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        #region Fields
        private static Employee activeEmployee = null;
        private static Party activeParty = null;
        private static Ticket activeTicket = null;
        private static Category activeCategory = null;
        private static Item activeItem = null;
        private static TicketItem activeTicketItem = null;
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveEmployeeChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActivePartyChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveTicketChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveCategoryChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveItemChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveTicketItemChanged = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler SessionReset = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler SessionResetItem = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler TicketCleared = null;
        [Obfuscation(Exclude = true)]
        public static event EventHandler TicketClosed = null;
        #endregion

        #region Properties
        public static Employee ActiveEmployee
        {
            get { return activeEmployee; }
            set
            {
                activeEmployee = value;
                if (ActiveEmployeeChanged != null)
                    ActiveEmployeeChanged.Invoke(value, new EventArgs());
            }
        }

        /// <summary>
        /// If a some other employee's permissions were used, this is that employee.
        /// </summary>
        public static Employee PseudoEmployee
        {
            get;
            set;
        }

        public static int? PseudoEmployeeId
        {
            get
            {
                if (PseudoEmployee == null)
                    return null;
                return PseudoEmployee.Id;
            }
        }

        public static Party ActiveParty
        {
            get { return activeParty; }
            set
            {
                activeParty = value;
                if (ActivePartyChanged != null)
                    ActivePartyChanged.Invoke(value, new EventArgs());
            }
        }

        public static Ticket ActiveTicket
        {
            get { return activeTicket; }
            set
            {
                activeTicket = value;
                if (ActiveTicketChanged != null)
                    ActiveTicketChanged.Invoke(value, new EventArgs());
            }
        }

        public static Category ActiveCategory
        {
            get { return activeCategory; }
            set
            {
                activeCategory = value;
                if (ActiveCategoryChanged != null)
                    ActiveCategoryChanged.Invoke(value, new EventArgs());
            }

        }

        public static Item ActiveItem
        {
            get { return activeItem; }
            set
            {
                activeItem = value;
                if (ActiveItemChanged != null)
                    ActiveItemChanged.Invoke(value, new EventArgs());
            }
        }

        public static TicketItem ActiveTicketItem
        {
            get { return activeTicketItem; }
            set
            {
                activeTicketItem = value;
                if (ActiveTicketItemChanged != null)
                    ActiveTicketItemChanged.Invoke(value, new EventArgs());
            }
        }

        public static bool IsActiveTicketLocked
        {
            get
            {
                if (ActiveTicket == null)
                    return false;
                return PosHelper.IsLocked(ActiveTicket);
            }
            set
            {
                if (ActiveTicket != null) 
                {
                    bool isLocked = IsActiveTicketLocked;
                    if (value && !isLocked)
                        PosHelper.Lock(ActiveTicket, ActiveEmployee);
                    else if (!value && isLocked)
                        PosHelper.Unlock(ActiveTicket);
                }
            }
        }
        #endregion

        #region Functions
        public static void Reset()
        {
            ActiveCategory = null;
            ActiveItem = null;
            ActiveParty = null;
            ActiveTicket = null;
            ActiveTicketItem = null;
            ActiveEmployee = null;
            PseudoEmployee = null;
            if (SessionReset != null)
                SessionReset.Invoke(null, new EventArgs());
        }

        public static void ResetItemEntry()
        {
            ActiveCategory = null;
            ActiveItem = null;
            ActiveTicketItem = null;
            if (SessionResetItem != null)
                SessionResetItem.Invoke(null, new EventArgs());
        }

        public static void CloseTicket()
        {
            CloseTicket(ActiveTicket);
            if (TicketClosed != null)
                TicketClosed.Invoke(null, new EventArgs());
        }

        public static void CloseTicket(Ticket ticket)
        {
            if (!ActiveTicket.IsFutureTimed ||
                (ActiveTicket.StartTime < DateTime.Now))
            {
                Lock.Delete(TableName.Ticket, ActiveTicket.PrimaryKey.Id);
                ActiveTicket.SetPrepareTime(DateTime.Now);
                ActiveTicket.Update();
            }
        }

        public static void CloseTickets(IEnumerable<Ticket> tickets)
        {
            foreach (Ticket ticket in tickets)
            {
                if (!ticket.IsFutureTimed ||
                    (ticket.StartTime < DateTime.Now))
                {
                    Lock.Delete(TableName.Ticket, ActiveTicket.PrimaryKey.Id);
                    ticket.SetPrepareTime(DateTime.Now);
                    ticket.Update();
                }
            }
            if (TicketClosed != null)
                TicketClosed.Invoke(null, new EventArgs());
        }

        public static void ClearTicket()
        {
            LoginControl.StartAutoLogoutTimer();
            PosHelper.Unlock(ActiveTicket);
            IsActiveTicketLocked = false;
            ActiveTicket = null;
            ActiveItem = null;
            ActiveCategory = null;
            ActiveTicketItem = null;
            PseudoEmployee = null;
            if (TicketCleared != null)
                TicketCleared.Invoke(null, new EventArgs());
        }
        #endregion

    }
}
