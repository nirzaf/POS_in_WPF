using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PosModels.Types;
using PosModels;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketTypeFilterControl.xaml
    /// </summary>
    public partial class TicketTypeFilterControl : UserControl
    {
        private static TicketType? _currentFilter;

        [Obfuscation(Exclude = true)]
        public static event EventHandler CurrentFilterChanged;

        public static TicketType? CurrentFilter
        {
            get
            {
                return _currentFilter;
            }
            set
            {
                if (_currentFilter != value)
                {
                    _currentFilter = value;
                    if (CurrentFilterChanged != null)
                        CurrentFilterChanged.Invoke(null, new EventArgs());
                }
            }
        }

        public ContextMenu TicketTypeFilterContextMenu
        {
            get
            {
                DependencyObject depObject = VisualTreeHelper.GetParent(this);
                while (depObject != null)
                {
                    if (depObject is ContextMenu)
                        return depObject as ContextMenu;
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
                return null;
            }
        }

        public TicketTypeFilterControl()
        {
            InitializeComponent();
            InitializeButtons();
            CurrentFilterChanged += TicketTypeFilterControl_CurrentFilterChanged;
            Loaded += TicketTypeFilterControl_Loaded;
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        void TicketTypeFilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginControl.AutoLogout += LoginControl_AutoLogout;
            bool hasDineIn = false;
            bool hasCarryout = false;
            bool hasDelivery = false;
            bool hasCatering = false;
            bool hasDriveThru = false;
            foreach (Room room in Room.GetAll())
            {
                if (room.IsUnused)
                    continue;
                if (room.TicketingType == TicketType.Catering)
                    hasCatering = true;
                else if (room.TicketingType == TicketType.Delivery)
                    hasDelivery = true;
                else if (room.TicketingType == TicketType.DineIn)
                    hasDineIn = true;
                else if (room.TicketingType == TicketType.DriveThru)
                    hasDriveThru = true;
                else if (room.TicketingType == TicketType.Pickup)
                    hasCarryout = true;
            }
            if (!hasDineIn)
                buttonDineInToggle.Visibility = Visibility.Collapsed;
            if (!hasDelivery)
                buttonDeliveryToggle.Visibility = Visibility.Collapsed;
            if (!hasDriveThru)
                buttonDriveThruToggle.Visibility = Visibility.Collapsed;
            if (!hasCarryout)
                buttonCarryOutToggle.Visibility = Visibility.Collapsed;
            if (!hasCatering)
                buttonCateringToggle.Visibility = Visibility.Collapsed;
        }

        [Obfuscation(Exclude = true)]
        void LoginControl_AutoLogout(object sender, EventArgs e)
        {
            TicketTypeFilterContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        void TicketTypeFilterControl_CurrentFilterChanged(object sender, EventArgs e)
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            buttonNoneToggle.IsSelected = (CurrentFilter == null);
            buttonCarryOutToggle.IsSelected = (CurrentFilter == TicketType.Pickup);
            buttonCateringToggle.IsSelected = (CurrentFilter == TicketType.Catering);
            buttonDeliveryToggle.IsSelected = (CurrentFilter == TicketType.Delivery);
            buttonDineInToggle.IsSelected = (CurrentFilter == TicketType.DineIn);
            buttonDriveThruToggle.IsSelected = (CurrentFilter == TicketType.DriveThru);
        }

        private void SetRadioChecked(object sender)
        {
            buttonCarryOutToggle.IsSelected = Equals(sender, buttonCarryOutToggle);
            buttonCateringToggle.IsSelected = Equals(sender, buttonCateringToggle);
            buttonDeliveryToggle.IsSelected = Equals(sender, buttonDeliveryToggle);
            buttonDineInToggle.IsSelected = Equals(sender, buttonDineInToggle);
            buttonDriveThruToggle.IsSelected = Equals(sender, buttonDriveThruToggle);
            buttonNoneToggle.IsSelected = Equals(sender, buttonNoneToggle);
            TicketTypeFilterContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonNoneToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = null;
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonDineInToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketType.DineIn;
        }

        [Obfuscation(Exclude = true)]
        private void buttonCarryOutToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketType.Pickup;
        }

        [Obfuscation(Exclude = true)]
        private void buttonDeliveryToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketType.Delivery;
        }

        [Obfuscation(Exclude = true)]
        private void buttonDriveThruToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketType.DriveThru;
        }

        [Obfuscation(Exclude = true)]
        private void buttonCateringToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketType.Catering;
        }

    }
}
