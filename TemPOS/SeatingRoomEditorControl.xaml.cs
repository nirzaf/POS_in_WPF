using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using TemPOS.Types;
using PosModels.Types;
using PosModels.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingRoomEditorControl.xaml
    /// </summary>
    public partial class SeatingRoomEditorControl : UserControl
    {
        #region Fields
        private bool _haltEvent;
        private Room _selectedRoom;
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if (!_haltEvent && (ValueChanged != null))
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        #region Properies
        public Room SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                _haltEvent = true;
                // Initialize Fields
                if (value != null)
                {
                    textBoxName.Text = _selectedRoom.Description;
                    SetTicketType(_selectedRoom.TicketingType);
                    textBoxName.IsEnabled = (comboBoxTicketType.SelectedIndex == 0);
                }
                else
                {
                    textBoxName.Text = "";
                    textBoxName.IsEnabled = true;
                    SetTicketType(TicketType.DineIn);
                }
                _haltEvent = false;
            }
        }
        #endregion

        public SeatingRoomEditorControl()
        {
            InitializeComponent();
            InitializeTicketTypeComboBox();
        }

        private void InitializeTicketTypeComboBox()
        {
            comboBoxTicketType.Items.Add(Types.Strings.DineIn);
            comboBoxTicketType.Items.Add(Types.Strings.Carryout);
            comboBoxTicketType.Items.Add(Types.Strings.Delivery);
            comboBoxTicketType.Items.Add(Types.Strings.DriveThru);
            comboBoxTicketType.Items.Add(Types.Strings.Catering);
        }

        public bool UpdateRoom()
        {
            // Add Room
            if (SelectedRoom == null)
            {
                SelectedRoom =
                    SeatingManager.AddRoom(textBoxName.Text, GetTicketType());
                return (SelectedRoom != null);
            }
            
            // Update Room
            SelectedRoom.SetDescription(textBoxName.Text);
            SelectedRoom.SetTicketingType(GetTicketType());
            return SelectedRoom.Update();
        }

        private TicketType GetTicketType()
        {
            if (comboBoxTicketType.SelectedIndex == 0)
                return TicketType.DineIn;
            if (comboBoxTicketType.SelectedIndex == 1)
                return TicketType.Pickup;
            if (comboBoxTicketType.SelectedIndex == 2)
                return TicketType.Delivery;
            if (comboBoxTicketType.SelectedIndex == 3)
                return TicketType.DriveThru;
            if (comboBoxTicketType.SelectedIndex == 4)
                return TicketType.Catering;
            throw new Exception(Types.Strings.UnsupportedTickettype);
        }

        private void SetTicketType(TicketType ticketType)
        {
            if (ticketType == TicketType.DineIn)
                comboBoxTicketType.SelectedIndex = 0;
            else if (ticketType == TicketType.Pickup)
                comboBoxTicketType.SelectedIndex = 1;
            else if (ticketType == TicketType.Delivery)
                comboBoxTicketType.SelectedIndex = 2;
            else if (ticketType == TicketType.DriveThru)
                comboBoxTicketType.SelectedIndex = 3;
            else if (ticketType == TicketType.Catering)
                comboBoxTicketType.SelectedIndex = 4;
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxTicketType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTicketType.SelectedIndex > 0)
                textBoxName.Text = GetTicketType().GetFriendlyName();
            textBoxName.IsEnabled = (comboBoxTicketType.SelectedIndex == 0);
            DoChangedValueEvent();
        }

    }
}
