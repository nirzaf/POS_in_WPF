using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PosModels;
using PosModels.Types;
using PosControls;
using TemPOS.Types;
using PosControls.Interfaces;
using PosModels.Managers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryTicketSelectionControl.xaml
    /// </summary>
    public partial class OrderEntryTicketSelectionControl
    {
        #region Fields
        private Ticket _selectedTicket;
        private DispatcherTimer _updateTimer;
        private int _previousUpdateCount;
        #endregion

        [Obfuscation(Exclude = true)]
        public static event EventHandler SelectedTicketChanged;

        #region Properties
        public Ticket[] DisplayedTickets
        {
            get
            {
                return (
                    from FormattedListBoxItem item in listBox1.Items
                    select item.ReferenceObject as Ticket).ToArray();
            }
        }

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                if (value != null)
                {
                    bool found = false;
                    foreach (FormattedListBoxItem item in
                        from FormattedListBoxItem item in listBox1.Items
                        let ticket = item.ReferenceObject as Ticket
                        where (ticket != null) && ticket.PrimaryKey.Equals(value.PrimaryKey) select item)
                    {
                        listBox1.SelectedItem = item;
                        listBox1.ScrollIntoView(listBox1.SelectedItem);
                        found = true;
                        break;
                    }
                    if (!found)
                        _selectedTicket = null;
                }
                if (_selectedTicket == null)
                    listBox1.SelectedItem = null;
                if (SelectedTicketChanged != null)
                    SelectedTicketChanged.Invoke(this, new EventArgs());
            }

        }

        private FormattedListBoxItem SelectedItem
        {
            get {
                if (listBox1 == null)
                    return null;
                if (listBox1.SelectedItem is FormattedListBoxItem)
                    return listBox1.SelectedItem as FormattedListBoxItem;
                return null;
            }
        }

        public UIElementCollection Items
        {
            get
            {
                if (listBox1 == null)
                    return null;
                return listBox1.Items;
            }
        }

        public DateTime RangeStart
        {
            get;
            private set;
        }

        public DateTime RangeEnd
        {
            get;
            private set;
        }
        #endregion

        public OrderEntryTicketSelectionControl()
        {
            InitializeComponent();
            Loaded += OrderEntryTicketSelectionControl_Loaded;
        }
        
        [Obfuscation(Exclude = true)]
        void OrderEntryTicketSelectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            buttonFilter.ContextMenu = GetFilterContextMenu();
            buttonTicketTypeFilter.ContextMenu = GetTicketTypeFilterContextMenu();
            _updateTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 4)
            };
            _updateTimer.Tick += updateTimer_Tick;
            TicketFilterControl.CurrentFilterChanged +=
                TicketFilterControl_CurrentFilterChanged;
            TicketFilterControl.CurrentFilterReselected +=
                TicketFilterControl_CurrentFilterChanged;
            TicketTypeFilterControl.CurrentFilterChanged +=
                TicketTypeFilterControl_CurrentFilterChanged;
        }

        private ContextMenu GetFilterContextMenu()
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                var entry = (DictionaryEntry)e.Current;
                var name = entry.Key as string;
                if (name == "filtersContextMenu")
                {
                    contextMenu = entry.Value as ContextMenu;
                    break;
                }
            }
            return contextMenu;
        }

        private ContextMenu GetTicketTypeFilterContextMenu()
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                var entry = (DictionaryEntry)e.Current;
                var name = entry.Key as string;
                if (name == "ticketTypeFilterContextMenu")
                {
                    contextMenu = entry.Value as ContextMenu;
                    break;
                }
            }
            return contextMenu;
        }

        [Obfuscation(Exclude = true)]
        void TicketTypeFilterControl_CurrentFilterChanged(object sender, EventArgs e)
        {
            UpdateTickets();
        }

        [Obfuscation(Exclude = true)]
        void TicketFilterControl_CurrentFilterChanged(object sender, EventArgs e)
        {
            if (TicketFilterControl.CurrentFilter == TicketSelectionShow.Range)
            {
                DateTime now = DateTime.Now;
                DateTime startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                DateTime endDate = now;
                if (PosDialogWindow.PromptDateRange(Types.Strings.SelectDateRange,
                    ref startDate, ref endDate))
                {
                    RangeStart = startDate;
                    RangeEnd = endDate;
                }
            }
            UpdateTickets();
        }

        [Obfuscation(Exclude = true)]
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            int currentUpdateCount =
                Ticket.GetTotalNumberOfTickets(DayOfOperation.CurrentYear);
            if (_previousUpdateCount != currentUpdateCount)
            {
                _previousUpdateCount = currentUpdateCount;
                InitializeListBox(TicketFilterControl.CurrentFilter);
            }
        }

        private void ShowTicketTypeContextMenu()
        {
            if (buttonTicketTypeFilter.ContextMenu != null)
            {
                buttonTicketTypeFilter.ContextMenu.Placement =
                    System.Windows.Controls.Primitives.PlacementMode.Top;
                buttonTicketTypeFilter.ContextMenu.PlacementTarget = buttonTicketTypeFilter;
                buttonTicketTypeFilter.ContextMenu.IsOpen = true;
            }
        }

        private void ShowContentMenu()
        {
            if (buttonFilter.ContextMenu != null)
            {
                buttonFilter.ContextMenu.Placement =
                    System.Windows.Controls.Primitives.PlacementMode.Top;
                buttonFilter.ContextMenu.PlacementTarget = buttonFilter;
                buttonFilter.ContextMenu.IsOpen = true;
            }
        }

        private void CreateTicketCommand()
        {
            // Make sure that start-of-day has occured
            if (DayOfOperation.Today == null)
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouCanNotCreateTicketsUntilStartOfDayHasBeenCompleted,
                    Types.Strings.RequiresStartOfDay, DialogButtons.Ok);
                return;
            }

            // Create the seating selection control
            if (ConfigurationManager.UseSeating)
            {
                PosDialogWindow window = SeatingSelectionControl.CreateInDefaultWindow();
                var control = window.DockedControl as SeatingSelectionControl;
                if (control == null) return;
                control.InitializeFromTicket(null);

                // Show the dialog
                PosDialogWindow.ShowPosDialogWindow(window);

                if (!window.ClosedByUser)
                {
                    SessionManager.ActiveTicket =
                        TicketManager.Add(control.TicketType, Party.NoPartyId,
                        control.SelectedSeatingId, SessionManager.ActiveEmployee.Id,
                        ((control.SelectedCustomer != null) ?
                        control.SelectedCustomer.Id : Customer.NoCustomerId));
                    SelectedTicket = SessionManager.ActiveTicket;
                    OrderEntryControl.SetAndLoadActiveTicket(SessionManager.ActiveTicket);
                }
                MainWindow.ShowWindowShadingOverlay = false;
            }
            else
            {
                SessionManager.ActiveTicket = TicketManager.Add(TicketType.DineIn,
                    Party.NoPartyId, Seating.NoSeatingId, SessionManager.ActiveEmployee.Id,
                    Customer.NoCustomerId);
                SelectedTicket = SessionManager.ActiveTicket;
                OrderEntryControl.SetAndLoadActiveTicket(SessionManager.ActiveTicket);
            }

            // Stop auto-logoff timer, if it should be disabled
            StoreSetting setting =
                SettingManager.GetStoreSetting("EnableAutoLogoffWhileInOrderEntry");
            if ((setting.IntValue == null) || (setting.IntValue.Value == 0))
                LoginControl.StopAutoLogoutTimer();

            // Need to change displayed tickets to open tickets, otherwise it will cause
            //  an exception in the TicketCashoutControl when cashing-out from in-order
            if ((TicketFilterControl.CurrentFilter != TicketSelectionShow.MyOpen) &&
                (TicketFilterControl.CurrentFilter != TicketSelectionShow.AllOpen) &&
                (TicketFilterControl.CurrentFilter != TicketSelectionShow.AllDay) &&
                (TicketFilterControl.CurrentFilter != TicketSelectionShow.All))
                TicketFilterControl.CurrentFilter = TicketSelectionShow.MyOpen;
            UpdateTickets();
        }

        [Obfuscation(Exclude = true)]
        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            var item = listBox1.SelectedItem as FormattedListBoxItem;
            if (item != null)
                listBox1.ScrollIntoView(item);
            SelectedTicket = ((SelectedItem != null) ? (SelectedItem.ReferenceObject as Ticket) : null);
        }


        internal void UpdateSelectedTicket()
        {
            if ((SelectedTicket == null) && (SelectedItem != null))
            {
                SelectedTicket = SelectedItem.ReferenceObject as Ticket;
            }
            else if ((SelectedTicket == null) && (SelectedItem == null))
            {
                foreach (ISelectable item in Items)
                {
                    if (item.IsSelected)
                    {
                        listBox1.SelectedItem = item as UIElement;
                        break;
                    }
                }
            }
        }

        public void InitializeListBox(TicketSelectionShow show)
        {
            var tickets = new List<Ticket>();
            if (show == TicketSelectionShow.All)
                tickets.AddRange(TicketManager.GetAllTickets());
            else if (show == TicketSelectionShow.Range)
            {
                tickets.AddRange(TicketManager.GetRange(RangeStart, RangeEnd));
            }
            else if (show == TicketSelectionShow.AllDay)
            {
                if (DayOfOperation.Today != null)
                {
                    tickets.AddRange(TicketManager.GetRange(DayOfOperation.Today.StartOfDay, DateTime.Now));
                }
                else
                {
                    DayOfOperation lastDay =
                        DayOfOperation.GetLatestInYear(DayOfOperation.CurrentYear);
                    if (lastDay.EndOfDay != null)
                        tickets.AddRange(TicketManager.GetRange(lastDay.StartOfDay, lastDay.EndOfDay.Value));
                }
            }
            else if (show == TicketSelectionShow.AllOpen)
                tickets.AddRange(TicketManager.GetOpenTickets());
            else if (show == TicketSelectionShow.MyOpen)
                tickets.AddRange(TicketManager.GetOpenTickets(SessionManager.ActiveEmployee.Id));
            else if (show == TicketSelectionShow.Closed)
                tickets.AddRange(TicketManager.GetTodaysClosedTickets());
            else if (show == TicketSelectionShow.Canceled)
                tickets.AddRange(TicketManager.GetTodaysCanceledTickets());
            else if (show == TicketSelectionShow.Dispatched)
                tickets.AddRange(TicketManager.GetDispatchedTickets());
            else if (show == TicketSelectionShow.Future)
                tickets.AddRange(TicketManager.GetFutureOrderTickets());

            Items.Clear();
            foreach (Ticket ticket in tickets
                .Where(ticket => (TicketTypeFilterControl.CurrentFilter == null) ||
                    (TicketTypeFilterControl.CurrentFilter.Value == ticket.Type)))
            {
                string text;
                Seating seat = SeatingManager.GetSeating(ticket.SeatingId);
                if (seat != null)
                {

                    text = (ticket.OrderId != null ? Types.Strings.Order + ": " + ticket.OrderId.Value + ", " : "") +
                           Types.Strings.Ticket + ": " + ticket.PrimaryKey.Id + (ticket.PartyId != 0 ?
                            Types.Strings.Party + ticket.PartyId : "") + ", " + seat.Description +
                           Environment.NewLine + Types.Strings.CreateTime + ticket.CreateTime;
                    if (!String.IsNullOrEmpty(ticket.ManagerNote))
                        text += Environment.NewLine + Types.Strings.Comment +
                                ticket.ManagerNote;
                }
                else
                {
                    text = (ticket.OrderId != null ?                        
                        Types.Strings.Order + ": " + ticket.OrderId.Value +
                        ", " : "") + Types.Strings.Ticket + ": " + ticket.PrimaryKey.Id + (ticket.PartyId != 0 ?
                        Types.Strings.Party + ticket.PartyId : "") + ", " + ticket.Type.GetFriendlyName() +
                        Environment.NewLine + Types.Strings.CreateTime + ticket.CreateTime;
                    if (!String.IsNullOrEmpty(ticket.ManagerNote))
                        text += Environment.NewLine + Types.Strings.Comment +
                                ticket.ManagerNote;
                }
                AddItem(ticket, text);
            }
            OrderEntryControl.SetDisplayedTicketTypeToStatusBar();
        }

        public void UpdateTickets()
        {
            var item = listBox1.SelectedItem as FormattedListBoxItem;
            InitializeListBox(TicketFilterControl.CurrentFilter);
            if (item == null) return;
            bool found = false;
            foreach (FormattedListBoxItem listItem in listBox1.Items.Cast<FormattedListBoxItem>()
                .Where(listItem => listItem.ReferenceObject == item.ReferenceObject))
            {
                found = true;
                listBox1.SelectedItem = listItem;
                break;
            }
            if (!found)
            {
                MainWindow.Singleton.orderEntryControl.
                    ticketDetailsControl.SelectedTicket = null;
            }
        }

        private void AddItem(Ticket ticket, string text)
        {
            var item = new FormattedListBoxItem(ticket, text, true);
            Items.Add(item);
            if ((SelectedTicket != null) && ticket.PrimaryKey.Equals(SelectedTicket.PrimaryKey))
            {
                item.IsSelected = true;
                MainWindow.Singleton.
                    orderEntryControl.ticketDetailsControl.SelectedTicket = SelectedTicket;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonCreate_Click(object sender, RoutedEventArgs e)
        {
            listBox1.SelectedItem = null;
            SelectedTicket = null;
            CreateTicketCommand();
        }

        [Obfuscation(Exclude = true)]
        private void buttonFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowContentMenu();
        }

        [Obfuscation(Exclude = true)]
        private void buttonTicketTypeFilter_Click(object sender, RoutedEventArgs e)
        {
            ShowTicketTypeContextMenu();
        }

    }
}
