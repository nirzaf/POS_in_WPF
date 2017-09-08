using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using PosControls;
using TemPOS.Types;
using PosControls.Helpers;
using PosModels.Managers;
using TemPOS.Exceptions;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for PartyManagementControl.xaml
    /// </summary>
    public partial class PartyManagementControl : UserControl
    {
        #region Fields
        private Window _parentWindow;
        #endregion

        #region Properties
        public Ticket ParentTicket
        {
            get;
            private set;
        }

        public Ticket OriginalTicket
        {
            get;
            private set;
        }

        public Ticket CurrentTicket
        {
            get;
            private set;
        }

        public PosDialogWindow ParentWindow
        {
            get
            {
                if (_parentWindow == null)
                    _parentWindow = Window.GetWindow(this);
                return (PosDialogWindow)_parentWindow;
            }
        }

        private bool IsSelectedSourceTicketLocked
        {
            get
            {
                object item = listboxSourceTicket.SelectedItem;
                if (item == null)
                    return false;
                FormattedListBoxItem listItem = item as FormattedListBoxItem;
                if (listItem == null) return false;
                return !listItem.ReferenceObject.Equals(OriginalTicket.PrimaryKey) &&
                    PosHelper.IsLocked(TableName.Ticket, listItem.Id);
            }
        }

        private bool IsSelectedDestinationTicketLocked
        {
            get
            {
                object item = listBoxDestinationTicket.SelectedItem;
                if (item == null)
                    return false;
                FormattedListBoxItem listItem = item as FormattedListBoxItem;
                if (listItem == null) return false;
                return !listItem.ReferenceObject.Equals(OriginalTicket.PrimaryKey) &&
                    PosHelper.IsLocked(TableName.Ticket, listItem.Id);
            }
        }
        #endregion

        #region Initialize
        public PartyManagementControl()
        {
            OriginalTicket = null;
            CurrentTicket = null;
            InitializeComponent();
        }

        public void Initialize(Ticket ticket)
        {
            OriginalTicket = ticket;
            CurrentTicket = ticket;
            // If this ticket is not part of a party, it will need
            // to have Party entry created for it
            if (OriginalTicket.PartyId == 0)
            {
                Party party = Party.Add(null, 0, null);
                OriginalTicket.SetPartyId(party.Id);
                OriginalTicket.Update();
            }
            InitializeSourceTickets();
            SetupButtons();
        }

        private void InitializeSourceTickets()
        {
            listboxSourceTicket.Items.Clear();

            FormattedListBoxItem selectedTicket = null;
            List<Ticket> partyTickets = new List<Ticket>(TicketManager.GetPartyTickets(OriginalTicket.PartyId));
            if (partyTickets.Count <= 0) return;
            ParentTicket = null;
            foreach (Ticket ticket in partyTickets.Where(ticket => !ticket.IsClosed))
            {
                if (ParentTicket == null)
                    ParentTicket = ticket;
                FormattedListBoxItem item =
                    new FormattedListBoxItem(ticket.PrimaryKey, Types.Strings.Ticket + ticket.PrimaryKey.Id, true);
                if (OriginalTicket.PrimaryKey.Equals(ticket.PrimaryKey))
                    selectedTicket = item;
                listboxSourceTicket.Items.Add(item);
            }
            if (ParentTicket == null)
                ParentTicket = OriginalTicket;

            // Select the active ticket
            if (selectedTicket == null) return;
            if (CurrentTicket == null)
                throw new WtfException();
            listboxSourceTicket.SelectedItem = selectedTicket;
            InitializeReceiptTape(CurrentTicket.PrimaryKey);
        }
        #endregion

        private void ChangeSeating()
        {
            if (!LockCheckPass(true, false, Types.Strings.ThisTicketIsOpenSomewhereElseItCanNotBeModified))
                return;

            // Create the seating selection control
            PosDialogWindow window = SeatingSelectionControl.CreateInDefaultWindow();
            SeatingSelectionControl control = window.DockedControl as SeatingSelectionControl;
            control.InitializeFromTicket(CurrentTicket);

            // Show the dialog
            ParentWindow.ShowShadingOverlay = true;
            window.ShowDialog(ParentWindow);

            if (!window.ClosedByUser)
            {
                // Process Results
                if (CurrentTicket != null)
                {
                    if (control.SelectedCustomer != null)
                        CurrentTicket.SetCustomerId(control.SelectedCustomer.Id);
                    CurrentTicket.SetType(control.TicketType);
                    CurrentTicket.SetSeatingId(control.SelectedSeatingId);
                    CurrentTicket.Update();
                }
            }
            ParentWindow.ShowShadingOverlay = false;
        }

        private bool LockCheckPass(bool checkSource, bool checkDestination, string failMessage)
        {
            bool sourceLock = IsSelectedSourceTicketLocked;
            bool destLock = IsSelectedDestinationTicketLocked;
            if ((checkSource && sourceLock) || (checkDestination && destLock))
            {
                PosDialogWindow.ShowDialog(
                    failMessage, Types.Strings.TicketLocked);
                return false;
            }
            return true;
        }

        private void EditPartyInfo()
        {
            if (PosHelper.IsLocked(TableName.Party, CurrentTicket.PartyId))
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.ThePartyInformationForThisTicketIsCurrentlyBeingModifiedSomewhereElse,
                    Types.Strings.PartyInformationLocked);
                return;
            }            
            PosHelper.Lock(TableName.Party, CurrentTicket.PartyId, SessionManager.ActiveEmployee.Id);

            PosDialogWindow window = PartyEditControl.CreateInDefaultWindow();
            PartyEditControl control = window.DockedControl as PartyEditControl;
            control.Initialize(ParentTicket.PartyId);
            window.ShowDialog(ParentWindow);

            control.ActiveParty.Update();
            PosHelper.Unlock(TableName.Party, CurrentTicket.PartyId);
        }

        private void SelectAllReceiptTapeItems()
        {
            foreach (TicketItemTemplate item in receiptTape.Items)
            {
                item.IsSelected = true;
            }
            PopulateDestinationList();
        }

        private void UnselectAllReceiptTapeItems()
        {
            foreach (TicketItemTemplate item in receiptTape.Items)
            {
                item.IsSelected = false;
            }
            listBoxDestinationTicket.Items.Clear();
        }

        private void AddTicket()
        {
#if DEMO
            IEnumerable<Ticket> tickets = TicketManager.GetPartyTickets(ParentTicket.PartyId);
            int count = tickets.Count<Ticket>();
            if (count >= 2)
            {
                PosDialogWindow.ShowDialog(Window.GetWindow(this),
                    Types.Strings.YouCanOnlyHave2PartyTicketsInTheDemoVersion,
                    Types.Strings.DemoRestriction);
                return;
            }
#endif
            // Create a new ticket with the same data as the current ticket
            Ticket newTicket = TicketManager.Add(OriginalTicket.Type,
                OriginalTicket.PartyId, OriginalTicket.SeatingId,
                OriginalTicket.EmployeeId, OriginalTicket.CustomerId);

            newTicket.SetStartTime(OriginalTicket.StartTime);
            newTicket.SetPrepareTime(OriginalTicket.PrepareTime);
            newTicket.SetReadyTime(OriginalTicket.ReadyTime);
            newTicket.Update();

            string text = Types.Strings.Ticket + ": " + newTicket.PrimaryKey.Id;
            // Add the new ticket to the source list
            listboxSourceTicket.Items.Add(
                new FormattedListBoxItem(newTicket.PrimaryKey, text, true));

            if ((receiptTape.SelectedItems != null) && (receiptTape.SelectedItems.Count > 0))
                //listBoxDestinationTicket.Items.Add(new FormattedListBoxItem(newTicket.Id, text, 280));
                PopulateDestinationList();
        }

        private void RemoveTicket()
        {
            if (!LockCheckPass(true, false, Types.Strings.TheSelectedTicketCanNotBeRemovedBecauseItAlreadyOpenedSomewhereElse))
                return;

            FormattedListBoxItem selected =
                (FormattedListBoxItem)listboxSourceTicket.SelectedItem;
            if (selected == null)
                return;
            if (TicketItem.GetAll(selected.ReferenceObject as YearId).Any())
            {
                // Remove the ticket
                TicketManager.Delete(selected.ReferenceObject as YearId);
                listboxSourceTicket.Items.Remove(listboxSourceTicket.SelectedItem);
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouCanNotRemoveATicketThatHasItemsOnItFirstTransferThoseItemsToADifferentTicket, Types.Strings.TicketNotEmpty);
            }

        }

        private void ConfirmRestoreToSingleTicket()
        {
            // Confirm with user
            if (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToConvertThisPartyBackIntoASingleTicket,
                Types.Strings.ConfirmSingleTicket, DialogButtons.YesNo) == DialogButton.Yes)
            {
                RestoreToSingleTicket();
            }
        }

        private void RestoreToSingleTicket()
        {
            // Need to check for locks on any of the settings in the source list
            if (listboxSourceTicket.Items.Cast<FormattedListBoxItem>()
                .Where(item => item.Id != OriginalTicket.PrimaryKey.Id)
                .Any(item => PosHelper.IsLocked(TableName.Ticket, item.Id)))
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.OneOrMoreOfTheTicketsInThisPartyIsCurrentlyBeingModifiedSomewhereElseCanNotChangeToSingleTicket, Types.Strings.TicketLocked);
                return;
            }

            // Move TicketItems
            IEnumerable<Ticket> tickets = TicketManager.GetPartyTickets(ParentTicket.PartyId);
            foreach (Ticket ticket in tickets)
            {
                if (ticket.PrimaryKey.Equals(ParentTicket.PrimaryKey) ||
                    ticket.IsClosed || ticket.IsCanceled)
                    continue;
                // Move all ticket items that are not on the ParentTicket, back to
                // the ParentTicket
#if DEMO
                int ticketItemCount = ParentTicket.GetNumberOfTicketItems() +
                    ticket.GetNumberOfTicketItems();
                if (ticketItemCount > 3)
                {
                    PosDialogWindow.ShowDialog(Window.GetWindow(this),
                        Types.Strings.YouCanNotAddMoreThan3TicketItemsToASingleTicketInTheDemoVersionAdditionalTicketItemsWillBeRemoved,
                        Types.Strings.DemoRestriction);
                }

#endif
                IEnumerable<TicketItem> ticketItems = TicketItem.GetAll(ticket.PrimaryKey);
                foreach (TicketItem ticketItem in ticketItems)
                {
#if DEMO
                    if (ParentTicket.GetNumberOfTicketItems() >= 3)
                    {
                        ticketItem.Delete();
                        continue;
                    }
#endif
                    ticketItem.SetTicketId(ParentTicket.PrimaryKey.Id);
                    ticketItem.UpdateTicketId();
                }

                // Delete the child ticket
                TicketManager.Delete(ticket.PrimaryKey);
            }

            // Delete Party Invites
            IEnumerable<PartyInvite> invites = PartyInvite.GetAll(ParentTicket.PartyId);
            foreach (PartyInvite invite in invites)
            {
                PartyInvite.Delete(invite.Id);
            }

            // Delete the party
            Party.Delete(ParentTicket.PartyId);
            ParentTicket.SetPartyId(0);
            ParentTicket.Update();
            CurrentTicket = OriginalTicket = ParentTicket;

            // Done, Close the parent window
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void listboxSourceTicket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectionChange();           
        }

        private void ProcessSelectionChange()
        {
            listBoxDestinationTicket.Items.Clear();
            receiptTape.Items.Clear();
            if (!LockCheckPass(true, false, Types.Strings.ThisTicketIsOpenSomewhereElseItCanNotBeModified))
            {
                buttonChangeSeating.IsEnabled =
                    buttonRemoveTicket.IsEnabled =
                    buttonUnselect.IsEnabled =
                    buttonSelectAll.IsEnabled = false;
                return;
            }
            YearId primaryKeyTicket = InitializeReceiptTape(null);
            if (primaryKeyTicket == null)
            {
                return;
                //throw new WtfException();
            }

            SetupButtons();

            if ((CurrentTicket == null) || !CurrentTicket.PrimaryKey.Equals(primaryKeyTicket))
                CurrentTicket = TicketManager.GetTicket(primaryKeyTicket);
        }

        private void SetupButtons()
        {
            buttonChangeSeating.IsEnabled =
                (listboxSourceTicket.SelectedItem != null);
            buttonRemoveTicket.IsEnabled =
                (listboxSourceTicket.SelectedItem != null);
            buttonUnselect.IsEnabled =
                buttonSelectAll.IsEnabled =
                (receiptTape.Items.Count > 0);
        }


        private YearId InitializeReceiptTape(YearId sourceTicket)
        {
            receiptTape.Items.Clear();
            if (sourceTicket == null)
                sourceTicket = GetSelectedSourceTicketId();
            if (sourceTicket == null)
                return null;
            IEnumerable<TicketItem> items = TicketItem.GetAll(sourceTicket);
            foreach (TicketItem ticketItem in items)
            {
                if (ticketItem.IsCanceled)
                    continue;
                // Add items to the receipt tape
                TicketItemTemplate temp =
                    new TicketItemTemplate(ticketItem);
                receiptTape.Items.Insert(0, temp);
            }                
            return sourceTicket;
        }
        
        [Obfuscation(Exclude = true)]
        private void receiptTape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDestinationList();
        }

        private void PopulateDestinationList()
        {
            listBoxDestinationTicket.Items.Clear();

            foreach (FormattedListBoxItem item in listboxSourceTicket.Items)
            {
                if (item.IsSelected)
                    continue;
                YearId primaryKey = (YearId)item.ReferenceObject;
                listBoxDestinationTicket.Items.Add(
                    new FormattedListBoxItem(primaryKey, item.Text));
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBoxDestinationTicket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
                MoveTicketItem((FormattedListBoxItem)e.AddedItems[0]);
            listBoxDestinationTicket.SelectedItem = null;
        }

        private void MoveTicketItem(FormattedListBoxItem destinationItem)
        {
#if DEMO
            int skipCount = 0;
            int moveCount = 0;
#endif
            if (!LockCheckPass(false, true, Types.Strings.TheDestinationTicketIsOpenSomewhereElseItCanNotHaveItemsTransferedToIt))
                return;

            // Take all selected items from the receipt tape and transfer them
            // to the selected ticket
            if (receiptTape.SelectedItems.Count == 0)
                return;
            YearId destinationPrimaryKey = destinationItem.ReferenceObject as YearId;
            if (destinationPrimaryKey != null)
            {
#if DEMO
                Ticket destinationTicket = Ticket.Get(destinationPrimaryKey);
#endif
                foreach (TicketItemTemplate item in receiptTape.SelectedItems)
                {
#if DEMO
                    if (destinationTicket.GetNumberOfTicketItems() < 3)
                    {
#endif
                    item.TicketItem.SetTicketId(destinationPrimaryKey.Id);
                    item.TicketItem.UpdateTicketId();
#if DEMO
                        moveCount++;
                        continue;
                    }
                    skipCount++;
#endif
                }
            }

            // Clear the destination list (since receipt tape selection is empty)
            listBoxDestinationTicket.Items.Clear();

            // Reinitialize receipt tape
            InitializeReceiptTape(CurrentTicket.PrimaryKey);
            SetupButtons();
#if DEMO
            if (skipCount == 0)
                return;
            if (moveCount == 0)
                PosDialogWindow.ShowDialog(Window.GetWindow(this),
                    Types.Strings.YouCanNotMoveAnyMoreTicketItemsToTheDestinationTicketTheDemoVersionIsLimitedTo3TicketItemsPerTicket,
                    Types.Strings.DemoRestriction);
            else
                PosDialogWindow.ShowDialog(Window.GetWindow(this),
                    Types.Strings.NotAllTheTicketItemsWereMovedTheDemoVersionLimitedTo3TicketItemsPerTicket,
                    Types.Strings.DemoRestriction);
#endif
        }

        private YearId GetSelectedSourceTicketId()
        {
            if (listboxSourceTicket.SelectedItem == null)
                return null;
            return ((YearId)((FormattedListBoxItem)listboxSourceTicket.SelectedItem).ReferenceObject);
        }

        private YearId GetSelectedDestinationTicketId()
        {
            if (listBoxDestinationTicket.SelectedItem == null)
                return null;
            return ((YearId)((FormattedListBoxItem)listBoxDestinationTicket.SelectedItem).ReferenceObject);
        }

        #region Button Click Events
        [Obfuscation(Exclude = true)]
        private void buttonAddTicket_Click(object sender, RoutedEventArgs e)
        {
            AddTicket();
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemoveTicket_Click(object sender, RoutedEventArgs e)
        {
            RemoveTicket();
        }

        [Obfuscation(Exclude = true)]
        private void buttonChangeSeating_Click(object sender, RoutedEventArgs e)
        {
            ChangeSeating();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEditParty_Click(object sender, RoutedEventArgs e)
        {
            EditPartyInfo();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAllReceiptTapeItems();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUnselect_Click(object sender, RoutedEventArgs e)
        {
            UnselectAllReceiptTapeItems();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSingleTicket_Click(object sender, RoutedEventArgs e)
        {
            ConfirmRestoreToSingleTicket();
        }
        #endregion

        public static PosDialogWindow CreateInDefaultWindow()
        {
            PartyManagementControl control = new PartyManagementControl();
            return new PosDialogWindow(control, Types.Strings.ManageParty, 755, 600);
        }
    }
}
