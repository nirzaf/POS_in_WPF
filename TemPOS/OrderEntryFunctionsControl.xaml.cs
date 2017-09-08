using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PosModels;
using PosControls;
using PosControls.Helpers;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.Helpers;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryFunctionsControl.xaml
    /// </summary>
    public partial class OrderEntryFunctionsControl : UserControl
    {
        public ContextMenu FunctionsContextMenu
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

        public OrderEntryFunctionsControl()
        {
            InitializeComponent();
            InitializeButtons();
            this.Loaded += OrderEntryFunctionsControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void OrderEntryFunctionsControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginControl.AutoLogout += LoginControl_AutoLogout;
            FunctionsContextMenu.Opened += FunctionsContextMenu_Opened;
        }
        
        [Obfuscation(Exclude = true)]
        void LoginControl_AutoLogout(object sender, EventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        void FunctionsContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
            UpdateLayout();
        }

        private void InitializeButtons()
        {
            // For the designer
            if (SessionManager.ActiveEmployee == null)
                return;
            bool waitingForEndOfYear = DayOfOperation.WaitingForEndOfYear;
            bool allowStartOfDay = (!waitingForEndOfYear && (DayOfOperation.Today == null));
            bool allowEndOfDay = (DayOfOperation.Today != null);
            buttonStartOfDay.Visibility = ((allowStartOfDay &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.StartOfDay)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonEndOfDay.Visibility = ((allowEndOfDay &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.EndOfDay)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonEndOfYear.Visibility = ((waitingForEndOfYear && !allowEndOfDay &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.EndOfDay)) ? Visibility.Visible : Visibility.Collapsed);
            buttonDispatchDriver.Visibility = (allowEndOfDay &&
                (SessionManager.ActiveEmployee.HasPermission(Permissions.DriverDispatch)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonEditInventory.Visibility =
                (SessionManager.ActiveEmployee.HasPermission(Permissions.InventoryAdjustments) ?
                Visibility.Visible : Visibility.Collapsed);
        }

        [Obfuscation(Exclude = true)]
        private void buttonDispatchDriver_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            PosDialogWindow window = TicketDeliveryDispatchControl.CreateInDefaultWindow();
            PosDialogWindow.ShowPosDialogWindow(this, window);
        }

        [Obfuscation(Exclude = true)]
        private void buttonStartOfDay_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            PosHelper.StartOfDay();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEndOfDay_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            PosHelper.EndOfDay();
            OrderEntryCommands.UpdateTicketDetailCommands();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonEndOfYear_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            PosHelper.EndOfYear();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEditTimesheet_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            PosDialogWindow window = TimesheetMaintenanceControl.CreateInDefaultWindow();
            PosDialogWindow.ShowPosDialogWindow(this, window);

        }

        [Obfuscation(Exclude = true)]
        private void buttonEditInventory_Click(object sender, RoutedEventArgs e)
        {
            FunctionsContextMenu.IsOpen = false;
            InventoryEditorControl.CreateInDefaultWindow().ShowDialog(MainWindow.Singleton);
        }
    }
}
