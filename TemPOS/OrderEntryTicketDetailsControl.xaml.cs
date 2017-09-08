using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Helpers;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Commands;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryTicketDetailsControl.xaml
    /// </summary>
    public partial class OrderEntryTicketDetailsControl
    {
        private Ticket _selectedTicket;

        [Obfuscation(Exclude = true)]
        public static event EventHandler SelectedTicketChanged;

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                gridControl.Visibility =
                    (value != null ? Visibility.Visible : Visibility.Hidden);
                if (value != null)
                {
                    string type = value.Type.GetFriendlyName();
                    labelTicketId.Content = value.PrimaryKey.Id + ((value.OrderId != null) ?
                        Types.Strings.OrderEntryOrderNumber + value.OrderId.Value : "") + " (" + type + ")";
                    Seating seating = SeatingManager.GetSeating(value.SeatingId);
                    if (seating != null)
                        labelSeating.Content = seating.Description + " (" +
                            SeatingManager.GetRoom(seating.RoomId).Description + ")";
                    else
                        labelSeating.Content = "";
                    Person person = PersonManager.GetPersonByEmployeeId(value.EmployeeId);
                    if (person != null)
                        labelEmployee.Content = person.FirstName + " " + person.LastName;
                    else
                        labelEmployee.Content = "";

                    labelCreatedDate.Content = value.CreateTime;
                    
                    bool isFuture =
                        ((value.StartTime != null) && (value.PrepareTime == null));
                    if (isFuture)
                    {
                        labelTime2Title.Content = Types.Strings.OrderEntryFutureTime;
                        labelFutureDate.Content = value.StartTime.Value;
                    }
                    else if (value.PrepareTime != null)
                    {
                        labelTime2Title.Content = Types.Strings.OrderEntryStartTime;
                        labelFutureDate.Content = value.PrepareTime.Value;
                    }
                    else
                    {
                        labelTime2Title.Content = "";
                        labelFutureDate.Content = "";
                    }

                    if (value.ReadyTime != null)
                        labelReadyDate.Content = value.ReadyTime.Value;
                    else
                        labelReadyDate.Content = "";

                    if (value.CloseTime != null)
                    {
                        labelClosedDate.Content = value.CloseTime.Value;
                        labelClosedText.Visibility = Visibility.Visible;
                        labelClosedDate.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        labelClosedDate.Content = "";
                        labelClosedText.Visibility = Visibility.Collapsed;
                        labelClosedDate.Visibility = Visibility.Collapsed;
                    }

                    textBoxManagerComments.Text = value.ManagerNote;

                    SetStatusLabel();
                }

                OrderEntryCommands.UpdateTicketDetailCommands();

                if (SelectedTicketChanged != null)
                    SelectedTicketChanged.Invoke(this, new EventArgs());
            }
        }

        public void SetStatusLabel()
        {
            bool isFuture =
                ((SelectedTicket.StartTime != null) && (SelectedTicket.PrepareTime == null));
            if ((SelectedTicket.PrepareTime == null) && (SelectedTicket.StartTime == null))
                labelStatus.Content = Types.Strings.OrderEntrySuspendedOrder;
            else if (SelectedTicket.IsCanceled)
                labelStatus.Content = Types.Strings.Canceled;
            else if (SelectedTicket.IsClosed)
                labelStatus.Content = Types.Strings.Closed;
            else if (isFuture)
                labelStatus.Content = Types.Strings.OrderEntryActiveFuture;
            else if (OrderEntryCommands.SelectedTicketHasUnfiredItems())
                labelStatus.Content = Types.Strings.OrderEntryActiveEntrees;
            else if (SelectedTicket.ReadyTime == null)
                labelStatus.Content = Types.Strings.OrderEntryActive;
            else
                labelStatus.Content = Types.Strings.OrderEntryServed;
        }

        public void UpdatePartyButton()
        {
            if (SelectedTicket == null)
                return;
            bool isReadOnly = (SelectedTicket.IsClosed || SelectedTicket.IsCanceled);
            buttonPartyEdit.Set(SelectedTicket.PartyId == 0 ?
                Types.Strings.OrderEntryConvertToParty : Types.Strings.OrderEntryManageParty, !isReadOnly);
        }

        public OrderEntryTicketDetailsControl()
        {
            InitializeComponent();
            gridControl.Visibility = Visibility.Hidden;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxManagerComments_CommitEdit(object sender, EventArgs e)
        {
            string text = textBoxManagerComments.Text;
            if ((text != null) && text.Equals(""))
                text = null;
            SelectedTicket.SetManagerNote(text);
            SelectedTicket.Update();

            OrderEntryControl.InitializeTicketSelection(SelectedTicket);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonMap_Click(object sender, RoutedEventArgs e)
        {
            // ToDo: Map
        }
    }
}
