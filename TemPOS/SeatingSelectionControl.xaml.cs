using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using TemPOS.Types;
using PosControls;
using PosModels.Managers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingSelectionControl.xaml
    /// </summary>
    public partial class SeatingSelectionControl
    {
        private Customer _selectedCustomer;
        private Room _selectedRoomId;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                personInformationControl.SelectedCustomer = value;
                _selectedCustomer = value;
            }
        }

        public Ticket ActiveTicket
        {
            get;
            private set;
        }

        public Room SelectedRoom
        {
            get { return _selectedRoomId; }
            private set
            {
                _selectedRoomId = value;
                personInformationControl.SelectedRoomId = value != null ? value.Id : 0;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return dineInControl.SelectedIndex;
            }
        }

        public string SelectedTableName
        {
            get
            {
                return dineInControl.SelectedTableName;
            }
        }

        public int SelectedSeatingId
        {
            get
            {
                return dineInControl.SelectedSeatingId;
            }
        }

        public TicketType TicketType
        {
            get;
            private set;
        }

        private SeatingSelectionControl()
        {
            ActiveTicket = null;
            InitializeComponent();
            InitializeRooms();
            Loaded += SeatingSelectionControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void SeatingSelectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null) window.Closing += SeatingSelectionControl_Closing;
        }

        [Obfuscation(Exclude = true)]
        void SeatingSelectionControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SelectedCustomer = personInformationControl.SelectedCustomer;
        }

        private void InitializeRooms()
        {
            Room[] rooms = SeatingManager.GetAllRoom();
            FormattedListBoxItem selectedItem = null;
            SelectedRoom = GetUsersPreviouslyUsedRoom();
            if (rooms == null)
                return; // Failed, not setup
            if ((SelectedRoom == null) && (rooms.Length > 0))
                SelectedRoom = rooms[0];
            foreach (Room room in rooms)
            {
                var listItem = new FormattedListBoxItem(room.Id, room.Description, true);
                if (SelectedRoom != null && room.Id == SelectedRoom.Id)
                {
                    selectedItem = listItem;
                    listItem.IsSelected = true;
                }
                listBoxRooms.Items.Add(listItem);
            }
            listBoxRooms.SelectedItem = selectedItem;
        }

        private Room GetUsersPreviouslyUsedRoom()
        {
            EmployeeSetting setting =
                SettingManager.GetEmployeeSetting(SessionManager.ActiveEmployee.Id,
                "LastRoomId");
            if (setting.IntValue != null)
                return SeatingManager.GetRoom(setting.IntValue.Value);
            return null;
        }

        private void SetupSeatingButtons()
        {
            if (SelectedRoom == null)
                return;
            foreach (FormattedListBoxItem item in listBoxRooms.Items)
            {
                if (item.Id == SelectedRoom.Id)
                {
                    listBoxRooms.SelectedItem = item;
                    break;
                }
            }
            switch (SelectedRoom.TicketingType)
            {
                case TicketType.DineIn:
                    TicketType = TicketType.DineIn;
                    personInformationControl.SetDineInMode();
                    personInformationControl.Visibility = Visibility.Hidden;
                    dineInControl.Visibility = Visibility.Visible;
                    dineInControl.SetupSeatingButtons(SelectedRoom, ActiveTicket);
                    break;
                case TicketType.Pickup:
                    TicketType = TicketType.Pickup;
                    dineInControl.Visibility = Visibility.Hidden;
                    personInformationControl.SetCarryoutMode();
                    personInformationControl.Visibility = Visibility.Visible;
                    break;
                case TicketType.Delivery:
                    TicketType = TicketType.Delivery;
                    dineInControl.Visibility = Visibility.Hidden;
                    personInformationControl.SetDeliveryMode();
                    personInformationControl.Visibility = Visibility.Visible;
                    break;
                case TicketType.Catering:
                    TicketType = TicketType.Catering;
                    dineInControl.Visibility = Visibility.Hidden;
                    personInformationControl.SetCateringMode();
                    personInformationControl.Visibility = Visibility.Visible;
                    break;
            }

        }

        [Obfuscation(Exclude = true)]
        private void listBoxRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            SelectedRoom =
                SeatingManager.GetRoom(((FormattedListBoxItem)listBoxRooms.SelectedItem).Id);
            SetupSeatingButtons();
        }

        public void InitializeFromTicket(Ticket activeTicket)
        {
            ActiveTicket = activeTicket;
            personInformationControl.buttonStartTicket.Text =
                ((activeTicket == null) ? Types.Strings.CreateTicket : Types.Strings.ChangeOccasion);
            personInformationControl.ActiveTicket = activeTicket;
            if (ActiveTicket == null)
            {
                TicketType = TicketType.DineIn;
                SetupSeatingButtons();
                return;
            }
            TicketType = ActiveTicket.Type;
            if (ActiveTicket.CustomerId > 0)
            {
                SelectedCustomer = Customer.Get(ActiveTicket.CustomerId);
                personInformationControl.SelectedCustomer = SelectedCustomer;
            }
            if (ActiveTicket.SeatingId > 0)
            {
                Seating seat = SeatingManager.GetSeating(ActiveTicket.SeatingId);
                SelectedRoom = SeatingManager.GetRoom(seat.RoomId);
            }
            else
            {
                Room[] rooms = SeatingManager.GetAllRoom();
                foreach (Room room in rooms)
                {
                    if (room.TicketingType == TicketType)
                    {
                        SelectedRoom = room;
                        break;
                    }
                }
            }
            SetupSeatingButtons();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            // Create the seating selection control
            var control = new SeatingSelectionControl();
            return new PosDialogWindow(control, Types.Strings.OccasionSelection, 675, 478);
        }
    }
}
