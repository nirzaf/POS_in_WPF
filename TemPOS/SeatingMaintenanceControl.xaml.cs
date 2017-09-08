using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosModels.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingMaintenanceControl.xaml
    /// </summary>
    public partial class SeatingMaintenanceControl : UserControl
    {
        private enum SeatingViewMode
        {
            Rooms,
            Seating
        }

        private SeatingViewMode _viewMode = SeatingViewMode.Rooms;
        private Room _selectedRoom = null;
        private Seating _selectedSeating = null;

        private SeatingViewMode ViewMode
        {
            get
            {
                return _viewMode;
            }
            set
            {
                _viewMode = value;
                PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;
                if (parent != null)
                {
                    if (_viewMode == SeatingViewMode.Rooms)
                    {
                        groupBoxRoom.Visibility = Visibility.Visible;
                        groupBoxSeating.Visibility = Visibility.Collapsed;
                        groupBoxList.Header = Strings.Rooms;
                        buttonAdd.Text = Strings.AddRoom;
                        buttonDelete.Text = Strings.DeleteRoom;
                        buttonUpdate.Text = Strings.UpdateRoom;
                        buttonEditToggle.Text = Strings.EditSeatings;
                        parent.Title = Strings.RoomSetup;
                    }
                    else if (_viewMode == SeatingViewMode.Seating)
                    {
                        groupBoxRoom.Visibility = Visibility.Collapsed;
                        groupBoxSeating.Visibility = Visibility.Visible;
                        groupBoxList.Header = Strings.Seatings;
                        buttonAdd.Text = Strings.AddSeating;
                        buttonDelete.Text = Strings.DeleteSeating;
                        buttonUpdate.Text = Strings.UpdateSeating;
                        buttonEditToggle.Text = Strings.EditRooms;
                        parent.Title = Strings.SeatingSetupRoom + SelectedRoom.Description + "]";
                    }
                }
                InitializeListBox();
            }
        }

        public Room SelectedRoom
        {
            get
            {
                return _selectedRoom;
            }
            private set
            {
                _selectedRoom = value;
                roomEditorControl.SelectedRoom = value;
                seatingEditorControl.SelectedRoom = value;
            }
        }

        public Seating SelectedSeating
        {
            get
            {
                return _selectedSeating;
            }
            private set
            {
                _selectedSeating = value;
                seatingEditorControl.SelectedSeating = value;
            }
        }

        public SeatingMaintenanceControl()
        {
            InitializeComponent();
            InitializeListBox();
        }

        private void InitializeListBox()
        {
            listBox1.SelectedItem = null;
            listBox1.Items.Clear();
            if (ViewMode == SeatingViewMode.Rooms)
            {
                roomEditorControl.SelectedRoom = null;
                FormattedListBoxItem selected = null;
                Room[] rooms = SeatingManager.GetAllRoom();
                foreach (Room room in rooms)
                {
                    FormattedListBoxItem item =
                        new FormattedListBoxItem(room, room.Description, true);
                    if ((SelectedRoom != null) && (SelectedRoom.Id == room.Id))
                        selected = item;
                    listBox1.Items.Add(item);
                }
                listBox1.SelectedItem = selected;
                if (selected != null)
                    roomEditorControl.SelectedRoom =
                        seatingEditorControl.SelectedRoom =
                        selected.ReferenceObject as Room;
                SetEditMode(false);
            }
            else
            {
                if (SelectedRoom != null)
                {
                    seatingEditorControl.SelectedSeating = null;
                    FormattedListBoxItem selected = null;
                    IEnumerable<Seating> seatings = SeatingManager.GetAllSeating(SelectedRoom.Id);
                    foreach (Seating seating in seatings)
                    {
                        FormattedListBoxItem item =
                            new FormattedListBoxItem(seating, seating.Description, true);
                        if ((SelectedSeating != null) && (SelectedSeating.Id == seating.Id))
                            selected = item;
                        listBox1.Items.Add(item);
                    }
                    listBox1.SelectedItem = selected;
                    if (selected != null)
                        seatingEditorControl.SelectedSeating =
                            selected.ReferenceObject as Seating;
                    SetEditMode(false);
                }
            }
        }

        private void ToggleViewMode()
        {
            ViewMode = ViewMode == SeatingViewMode.Rooms ?
                SeatingViewMode.Seating : SeatingViewMode.Rooms;
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                if (listBox1.SelectedItem == null)
                    buttonDelete.IsEnabled = false;
                return;
            }
            if (ViewMode == SeatingViewMode.Rooms)
            {
                FormattedListBoxItem item = listBox1.SelectedItem as FormattedListBoxItem;
                if (item != null) SelectedRoom = item.ReferenceObject as Room;
                roomEditorControl.IsEnabled = true;
            }
            else if (ViewMode == SeatingViewMode.Seating)
            {
                FormattedListBoxItem item = listBox1.SelectedItem as FormattedListBoxItem;
                if (item != null) SelectedSeating = item.ReferenceObject as Seating;
                seatingEditorControl.IsEnabled = true;
            }
            buttonDelete.IsEnabled = true;
            SetEditMode(false);
        }

        private void Add()
        {
            listBox1.SelectedItem = null;
            if (ViewMode == SeatingViewMode.Rooms)
            {
                SelectedRoom = null;
            }
            else if (ViewMode == SeatingViewMode.Seating)
            {
                SelectedSeating = null;
            }
            SetEditMode(true);
        }

        private void Delete()
        {
            bool confirmed = false;
            if (listBox1.SelectedItem == null)
                return;
            FormattedListBoxItem selectedItem =
                listBox1.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;

            if (ViewMode == SeatingViewMode.Rooms)
            {
                string message = (buttonEditToggle.IsEnabled ?
                    Strings.AreYouSureYouWantToDeleteTheSelectedRoomAndAllItsSeatings :
                    Strings.AreYouSureYouWantToDeleteTheSelectedRoom);
                if (PosDialogWindow.ShowDialog(
                    message, Strings.ConfirmDeletion, DialogButtons.OkCancel) == DialogButton.Ok)
                {
                    Room room = selectedItem.ReferenceObject as Room;
                    if (room != null)
                    {
                        SeatingManager.DeleteRoom(room.Id);
                        SeatingManager.DeleteAllSeating(room.Id);
                    }
                    buttonEditToggle.IsEnabled = false;
                    roomEditorControl.IsEnabled = false;
                    SelectedRoom = null;
                    SelectedSeating = null;
                    confirmed = true;
                }
                
            }
            else if (ViewMode == SeatingViewMode.Seating)
            {
                if (PosDialogWindow.ShowDialog(
                    Strings.AreYouSureYouWantToDeleteTheSelectedSeating,
                    Strings.ConfirmDeletion, DialogButtons.OkCancel) == DialogButton.Ok)
                {
                    Seating seating = selectedItem.ReferenceObject as Seating;
                    if (seating != null) SeatingManager.DeleteSeating(seating.Id);
                    seatingEditorControl.IsEnabled = false;
                    SelectedSeating = null;
                    confirmed = true;
                }
            }
            if (confirmed)
            {
                listBox1.Items.Remove(selectedItem);
                listBox1.SelectedItem = null;
                buttonDelete.IsEnabled = false;
            }
        }

        private void UpdateChanges()
        {
            bool updated = false;
            if (ViewMode == SeatingViewMode.Rooms)
            {
                bool isNew = (roomEditorControl.SelectedRoom == null);
                Room oldRoom = (roomEditorControl.SelectedRoom != null ?
                    SeatingManager.GetRoom(roomEditorControl.SelectedRoom.Id) : null);
                if (roomEditorControl.UpdateRoom())
                {
                    if (isNew)
                    {
                        FormattedListBoxItem item =
                            new FormattedListBoxItem(roomEditorControl.SelectedRoom,
                            roomEditorControl.SelectedRoom.Description, true);
                        listBox1.Items.Add(item);
                        SelectedRoom = roomEditorControl.SelectedRoom;
                        listBox1.SelectedItem = item;
                        SetDefaultSeating(SelectedRoom);
                        updated = true;
                    }
                    else if (listBox1.SelectedItem != null)
                    {
                        FormattedListBoxItem selected = listBox1.SelectedItem as FormattedListBoxItem;
                        if (selected != null)
                        {
                            Room room = selected.ReferenceObject as Room;
                            if (room != null)
                            {
                                selected.Set(room, room.Description);
                                if (oldRoom != null && oldRoom.TicketingType != room.TicketingType)
                                    SetDefaultSeating(room);
                            }
                            updated = true;
                        }
                    }
                }
            }
            else if (ViewMode == SeatingViewMode.Seating)
            {
                bool isNew = (seatingEditorControl.SelectedSeating == null);
                if (seatingEditorControl.UpdateSeating())
                {
                    if (isNew)
                    {
                        FormattedListBoxItem item =
                            new FormattedListBoxItem(seatingEditorControl.SelectedSeating,
                            seatingEditorControl.SelectedSeating.Description, true);
                        listBox1.Items.Add(item);
                        SelectedSeating = seatingEditorControl.SelectedSeating;
                        listBox1.SelectedItem = item;
                        updated = true;
                    }
                    else if (listBox1.SelectedItem != null)
                    {
                        FormattedListBoxItem selected = listBox1.SelectedItem as FormattedListBoxItem;
                        if (selected != null)
                        {
                            Seating seating = selected.ReferenceObject as Seating;
                            if (seating != null) selected.Set(seating, seating.Description);
                        }
                        updated = true;
                    }
                }
            }
            if (updated)
                SetEditMode(false);
        }

        // ToDo: These are needed per-say, but what if someone create
        // Multiple Rooms with TicketingTypes of the same value, then this
        // could be useful, otherwise should prevent adding multiple rooms
        // of these types
        private void SetDefaultSeating(Room room)
        {
            SeatingManager.DeleteAllSeating(room.Id);
            if (room.TicketingType == PosModels.Types.TicketType.DriveThru)
                SeatingManager.AddSeating(room.Id, Strings.DriveThru, 0);
            if (room.TicketingType == PosModels.Types.TicketType.Delivery)
                SeatingManager.AddSeating(room.Id, Strings.Delivery, 0);
            if (room.TicketingType == PosModels.Types.TicketType.Pickup)
                SeatingManager.AddSeating(room.Id, Strings.Carryout, 0);
            if (room.TicketingType == PosModels.Types.TicketType.Catering)
                SeatingManager.AddSeating(room.Id, Strings.Catering, 0);
        }

        private void CancelChanges()
        {
            if (ViewMode == SeatingViewMode.Rooms)
            {
                roomEditorControl.SelectedRoom = roomEditorControl.SelectedRoom;
            }
            else if (ViewMode == SeatingViewMode.Seating)
            {
                seatingEditorControl.SelectedSeating = seatingEditorControl.SelectedSeating;
            }
            SetEditMode(false);
        }

        private void SetEditMode(bool inEdit)
        {            
            buttonAdd.IsEnabled =
                buttonEditToggle.IsEnabled =
                listBox1.IsEnabled = !inEdit;
            if ((ViewMode == SeatingViewMode.Rooms) && (listBox1.SelectedItem == null))
                buttonEditToggle.IsEnabled = false;
            else if ((ViewMode == SeatingViewMode.Rooms) &&
                (SelectedRoom.TicketingType != PosModels.Types.TicketType.DineIn))
                buttonEditToggle.IsEnabled = false;

            buttonDelete.IsEnabled = ((listBox1.SelectedItem != null) && !inEdit);
            roomEditorControl.IsEnabled =
                seatingEditorControl.IsEnabled =
                (inEdit || listBox1.SelectedItem != null);
            buttonCancel.IsEnabled =
                buttonUpdate.IsEnabled = inEdit;

            PosDialogWindow parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null)
                parentWindow.SetButtonsEnabled(!inEdit);
        }
        
        [Obfuscation(Exclude = true)]
        private void roomEditorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void seatingEditorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closing +=
                SeatingMaintenanceControl_Closing;
        }

        [Obfuscation(Exclude = true)]
        void SeatingMaintenanceControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ConfigurationManager.UseSeating && (listBox1.Items.Count > 0) &&
                (PosDialogWindow.ShowDialog(
                Strings.OccasionSelectionIsCurrentlyDisabledDoYouWantToEnableIt,
                Strings.EnableOccasionSelection, DialogButtons.YesNo) == DialogButton.Yes))
            {
                ConfigurationManager.SetUseSeating(true);
            }
            else if (ConfigurationManager.UseSeating && (listBox1.Items.Count == 0))
            {
                ConfigurationManager.SetUseSeating(false);
                PosDialogWindow.ShowDialog(
                    Strings.OccasionSelectionWasDisabledBecauseNoRoomsExist,
                    Strings.OccasionSelectionDisabled);
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEditToggle_Click(object sender, RoutedEventArgs e)
        {
            ToggleViewMode();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateChanges();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

    }
}
