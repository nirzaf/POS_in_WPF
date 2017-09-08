using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PosModels;
using PosControls;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryRegisterMenuControl.xaml
    /// </summary>
    public partial class OrderEntryRegisterMenuControl : UserControl
    {
        public ContextMenu RegisterContextMenu
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

        public OrderEntryRegisterMenuControl()
        {
            InitializeComponent();
            InitializeButtons();
            Loaded += OrderEntryRegisterMenuControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void OrderEntryRegisterMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginControl.AutoLogout += LoginControl_AutoLogout;
            RegisterContextMenu.Opened += RegisterContextMenu_Opened;
        }

        [Obfuscation(Exclude = true)]
        void LoginControl_AutoLogout(object sender, EventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        void RegisterContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
            UpdateLayout();            
        }

        private void InitializeButtons()
        {
            // For designer
            if (SessionManager.ActiveEmployee == null)
                return;

            buttonStartRegister.Visibility =
                (((RegisterManager.ActiveRegisterDrawer == null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterStart)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonCloseOut.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterClose)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonDrop.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterDrop)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonNoSale.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterNoSale)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonPayout.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterPayout)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonReport.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterReport)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonDeposit.Visibility =
                (((RegisterManager.ActiveRegisterDrawer != null) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterDeposit)) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonDock.Visibility = ((RegisterManager.ActiveRegisterDrawer == null) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonFloat.Visibility = ((RegisterManager.ActiveRegisterDrawer != null) ?
                Visibility.Visible : Visibility.Collapsed);
        }

        public void PrintRegisterReport(RegisterDrawer drawer)
        {
            ActionNotificationControl notification = 
                ActionNotificationControl.Create(MainWindow.Singleton,
                Types.Strings.RegisterMenuPreparingReport, Types.Strings.Notification);
            notification.Show();
            notification.ParentWindow.Closed += Notification_Closed;
            ReportManager.PrintRegisterReport(drawer, ParentWindow_Closed);
            notification.Close();
        }

        [Obfuscation(Exclude = true)]
        void ParentWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.Singleton.ShowShadingOverlay = false;
        }

        [Obfuscation(Exclude = true)]
        void Notification_Closed(object sender, EventArgs e)
        {
            MainWindow.Singleton.ShowShadingOverlay = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonStartRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            if (DayOfOperation.Today != null)
            {
                RegisterManager.StartRegister();
                OrderEntryCommands.SetupNoOrderCommands();
                OrderEntryCommands.UpdateTicketDetailCommands();
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.RegisterMenuRunStartOfDay,
                    Types.Strings.RegisterMenuUnableToProceed);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonNoSale_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            RegisterNoSale.Add(RegisterManager.ActiveRegisterDrawer.Id,
                SessionManager.ActiveEmployee.Id);
            RegisterManager.OpenCashDrawer();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDeposit_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            double? amount = PosDialogWindow.PromptCurrency(Types.Strings.RegisterMenuDepositAmount, null);
            if (amount.HasValue && (amount.Value > 0))
            {
                RegisterDeposit.Add(RegisterManager.ActiveRegisterDrawer.Id,
                    SessionManager.ActiveEmployee.Id, amount.Value);
                RegisterManager.ActiveRegisterDrawer.AddToCurrentAmount(amount.Value);
                RegisterManager.OpenCashDrawer();
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonDrop_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            double? amount = PosDialogWindow.PromptCurrency(Types.Strings.RegisterMenuDropAmount, null);
            if (amount.HasValue && (amount.Value > 0))
            {
                double total = RegisterManager.ActiveRegisterDrawer.CurrentAmount;
                total = RegisterDrop.GetAll(RegisterManager.ActiveRegisterDrawer.Id)
                    .Aggregate(total, (current, drop) => current - drop.Amount);
                if (amount.Value < total)
                {
                    RegisterDrop.Add(RegisterManager.ActiveRegisterDrawer.Id,
                        SessionManager.ActiveEmployee.Id, amount.Value, DateTime.Now);
                    RegisterManager.ActiveRegisterDrawer.RemoveFromCurrentAmount(amount.Value);
                    RegisterManager.OpenCashDrawer();
                }
                else
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.RegisterMenuCantDropThatMuch,
                        Types.Strings.RegisterMenuInvalidAmount);
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonPayout_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            string reason = PosDialogWindow.PromptKeyboard(Types.Strings.RegisterMenuPayoutReason, null);
            if (reason != null)
            {
                double? amount = PosDialogWindow.PromptCurrency(Types.Strings.RegisterMenuPayoutAmount, null);
                if (amount.HasValue && (amount.Value > 0))
                {
                    RegisterPayout.Add(RegisterManager.ActiveRegisterDrawer.Id,
                        SessionManager.ActiveEmployee.Id, amount.Value, reason, DateTime.Now);
                    RegisterManager.ActiveRegisterDrawer.RemoveFromCurrentAmount(amount.Value);
                    RegisterManager.OpenCashDrawer();
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonFloat_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.RegisterMenuConfirmFloat,
                Types.Strings.Confirmation, DialogButtons.YesNo) == DialogButton.Yes)
            {
                RegisterManager.OpenCashDrawer();
                RegisterManager.FloatActiveRegisterDrawer();
                OrderEntryCommands.SetupNoOrderCommands();
                PosDialogWindow.ShowDialog(
                    Types.Strings.RegisterMenuNotifyFloat, Types.Strings.Notification);
            }
            OrderEntryCommands.UpdateTicketDetailCommands();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDock_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            int registerSubId = 0;
            RegisterDrawer registerDrawer =
                RegisterDrawer.GetFloating(SessionManager.ActiveEmployee.Id);
            if (registerDrawer == null)
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.RegisterMenuNotFloating, Types.Strings.Error);
                return;
            }
            if (registerSubId == 0)
                DeviceManager.OpenCashDrawer1();
            else if (registerSubId == 1)
                DeviceManager.OpenCashDrawer2();
            RegisterManager.DockRegisterDrawer(registerDrawer, registerSubId);
            OrderEntryCommands.SetupNoOrderCommands();
            PosDialogWindow.ShowDialog(
                Types.Strings.RegisterMenuDrawerIsNowDocked, Types.Strings.Notification);
            OrderEntryCommands.UpdateTicketDetailCommands();
        }

        [Obfuscation(Exclude = true)]
        private void buttonReport_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            PrintRegisterReport(RegisterManager.ActiveRegisterDrawer);
        }

        [Obfuscation(Exclude = true)]
        private void buttonCloseOut_Click(object sender, RoutedEventArgs e)
        {
            RegisterContextMenu.IsOpen = false;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.RegisterMenuConfirmDrawerClose,
                Types.Strings.Confirmation, DialogButtons.YesNo) != DialogButton.Yes) return;
            RegisterManager.OpenCashDrawer();
            RegisterDrawer activeRegisterDrawer = RegisterManager.ActiveRegisterDrawer;
            RegisterManager.CloseActiveRegisterDrawer();
            OrderEntryCommands.UpdateTicketDetailCommands();
            PrintRegisterReport(activeRegisterDrawer);
        }

    }
}
