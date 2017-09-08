using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PosModels;
using TemPOS.Types;
using PosControls;
using PosModels.Types;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntrySetupControl.xaml
    /// </summary>
    public partial class OrderEntrySetupControl : UserControl
    {
        public ContextMenu SetupContextMenu
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

        public OrderEntrySetupControl()
        {
            InitializeComponent();
            InitializeButtons();
            Loaded += OrderEntrySetupControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void OrderEntrySetupControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginControl.AutoLogout += LoginControl_AutoLogout;
            SetupContextMenu.Opened += SetupContextMenu_Opened;
        }
        
        [Obfuscation(Exclude = true)]
        void LoginControl_AutoLogout(object sender, EventArgs e)
        {
            SetupContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        void SetupContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
            UpdateLayout();
        }

        private void InitializeButtons()
        {
            Employee employee = SessionManager.ActiveEmployee;
            
            // For the designer
            if (employee == null)
                return;

            buttonEmployees.Visibility =
                (employee.HasPermission(Permissions.EmployeeMaintenance) ?
                Visibility.Visible : Visibility.Collapsed);
#if DEMO
            buttonHardware.Visibility = Visibility.Collapsed;
#endif
            buttonGeneralSettings.Visibility =
                buttonCoupons.Visibility =
                buttonItems.Visibility =
                buttonSeating.Visibility =
                buttonTaxes.Visibility =
#if !DEMO
                buttonHardware.Visibility =
#endif
                (employee.HasPermission(Permissions.SystemMaintenance) ?
                Visibility.Visible : Visibility.Collapsed);
        }

        private static void ShowDialogWindow(Control parentControl, Control control, string title)
        {
            PosDialogWindow window = new PosDialogWindow(control, title, 1000, 620);
            PosDialogWindow.ShowPosDialogWindow(parentControl, window);
        }

        /*
        <my:CategoryMaintenanceControl x:Name="categoryMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:IngredientMaintenanceControl x:Name="ingredientMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:ItemMaintenanceControl x:Name="itemMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:ItemOptionSetMaintenanceControl x:Name="itemOptionMaintenanceControl" Margin="10" Visibility="Hidden" OptionSetsUpdated="itemOptionMaintenanceControl_OptionSetsUpdated" />
        <my:CouponMaintenanceControl x:Name="couponMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:DiscountMaintenanceControl x:Name="discountMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:DeliveryMaintenanceControl x:Name="deliveryMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:SeatingMaintenanceControl x:Name="seatingMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:TaxMaintenanceControl x:Name="taxMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:DeviceSelectionControl x:Name="deviceSelectionControl" Margin="10" Visibility="Hidden" />
        <my:GeneralSettingsControl x:Name="storeSettingsMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:EmployeeEditorControl x:Name="employeeEditorControl" Margin="10" />
        <my:EmployeeJobMaintenanceControl x:Name="employeeJobsMaintenanceControl" Margin="10" Visibility="Hidden" />
        <my:EmployeeScheduleMaintenanceControl x:Name="scheduleMaintenanceControl" Margin="10" Visibility="Hidden" />
        [Obfuscation(Exclude = true)]
        <my:TimesheetMaintenanceControl x:Name="timesheetMaintenanceControl" Margin="10" Visibility="Hidden" />
        */

        [Obfuscation(Exclude = true)]
        private void buttonGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            ShowDialogWindow(this, new GeneralSettingsControl(), Types.Strings.SetupGeneralSettings);
        }

        [Obfuscation(Exclude = true)]
        private void buttonItems_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            ShowDialogWindow(this, new ItemMaintenanceControl(), Types.Strings.SetupItemSetup);
        }

        [Obfuscation(Exclude = true)]
        private void buttonCoupons_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            string[] tabNames = new[] { Types.Strings.SetupCoupons, Types.Strings.SetupDiscounts };
            FrameworkElement[] controls = { new CouponMaintenanceControl(), new DiscountMaintenanceControl() };
            double[] tabWidths = new double[] { 100, 110 };
            PosDialogWindow window = new PosDialogWindow(Types.Strings.SetupCouponAndDiscountSetup,
                controls, tabNames, tabWidths, 1000, 620);
            PosDialogWindow.ShowPosDialogWindow(this, window);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonSeating_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            ShowDialogWindow(this, new SeatingMaintenanceControl(), Types.Strings.SetupRoomSetup);
        }

        [Obfuscation(Exclude = true)]
        private void buttonEmployees_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            string[] tabNames = new[] { Types.Strings.SetupEmployees, Types.Strings.SetupEmployeeJobs };
            FrameworkElement[] controls = { new EmployeeEditorControl(), new EmployeeJobMaintenanceControl() };
            double[] tabWidths = new double[] { 100, 130 };
            PosDialogWindow window = new PosDialogWindow(Types.Strings.SetupEmployeeSetup,
                controls, tabNames, tabWidths, 1000, 620);
            PosDialogWindow.ShowPosDialogWindow(this, window);
        }

        [Obfuscation(Exclude = true)]
        private void buttonTaxes_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            ShowDialogWindow(this, new TaxMaintenanceControl(), Types.Strings.SetupTaxSetup);
        }

        [Obfuscation(Exclude = true)]
        private void buttonHardware_Click(object sender, RoutedEventArgs e)
        {
            SetupContextMenu.IsOpen = false;
            ShowDialogWindow(this, new DeviceSelectionControl(), Types.Strings.SetupHardwareSetup);
        }
    }
}
