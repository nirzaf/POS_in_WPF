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
using TemPOS.Types;
using PosModels.Types;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketFilterControl.xaml
    /// </summary>
    public partial class TicketFilterControl : UserControl
    {
        #region Fields
        private static TicketSelectionShow _currentFilter = TicketSelectionShow.MyOpen;
        #endregion

        #region Properties
        public static TicketSelectionShow CurrentFilter
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

        public ContextMenu TicketFilterContextMenu
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
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public static event EventHandler CurrentFilterChanged;
        [Obfuscation(Exclude = true)]
        public static event EventHandler CurrentFilterReselected;
        #endregion

        public TicketFilterControl()
        {
            InitializeComponent();
            InitializeButtons();
            UpdatePermissionRequiredButtons();
            CurrentFilterChanged += TicketFilterControl_CurrentFilterChanged;
            buttonRangeToggle.Reselected += buttonRangeToggle_Reselected;
            Loaded += TicketFilterControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void TicketFilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginControl.AutoLogout += LoginControl_AutoLogout;
        }

        [Obfuscation(Exclude = true)]
        void LoginControl_AutoLogout(object sender, EventArgs e)
        {
            TicketFilterContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        void buttonRangeToggle_Reselected(object sender, EventArgs e)
        {
            if (CurrentFilterReselected != null)
                CurrentFilterReselected.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        void TicketFilterControl_CurrentFilterChanged(object sender, EventArgs e)
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            buttonFutureToggle.IsSelected = (CurrentFilter == TicketSelectionShow.Future);
            buttonAllToggle.IsSelected = (CurrentFilter == TicketSelectionShow.All);
            buttonCancelToggle.IsSelected = (CurrentFilter == TicketSelectionShow.Canceled);
            buttonClosedToggle.IsSelected = (CurrentFilter == TicketSelectionShow.Closed);
            buttonDispatchedToggle.IsSelected = (CurrentFilter == TicketSelectionShow.Dispatched);
            buttonAllOpenToggle.IsSelected = (CurrentFilter == TicketSelectionShow.AllOpen);
            buttonMyOpenToggle.IsSelected = (CurrentFilter == TicketSelectionShow.MyOpen);
            buttonRangeToggle.IsSelected = (CurrentFilter == TicketSelectionShow.Range);
            buttonAllDayToggle.IsSelected = (CurrentFilter == TicketSelectionShow.AllDay);
        }

        public bool UpdatePermissionRequiredButtons()
        {
            // For the designer
            if (SessionManager.ActiveEmployee == null)
                return false;
            buttonClosedToggle.Visibility =
                (SessionManager.ActiveEmployee.HasPermission(Permissions.Cashout) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonCancelToggle.Visibility =
                (SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonRangeToggle.Visibility =
                ((SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterRefund) ||
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterReturn) ||
                SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel) ||
                SessionManager.ActiveEmployee.HasPermission(Permissions.Void)) ?
                Visibility.Visible : Visibility.Collapsed);
#if DELIVERY_SUPPORT
            buttonDispatchedToggle.Visibility =
                [Obfuscation(Exclude = true)]
                (SessionManager.ActiveEmployee.HasPermission(Permissions.DriverDispatch) ?
                Visibility.Visible : Visibility.Collapsed);
#endif
            return true;
        }

        private void SetRadioChecked(object sender)
        {
            buttonFutureToggle.IsSelected = Equals(sender, buttonFutureToggle);
            buttonDispatchedToggle.IsSelected = Equals(sender, buttonDispatchedToggle);
            buttonClosedToggle.IsSelected = Equals(sender, buttonClosedToggle);
            buttonCancelToggle.IsSelected = Equals(sender, buttonCancelToggle);
            buttonAllToggle.IsSelected = Equals(sender, buttonAllToggle);
            buttonAllOpenToggle.IsSelected = Equals(sender, buttonAllOpenToggle);
            buttonMyOpenToggle.IsSelected = Equals(sender, buttonMyOpenToggle);
            buttonRangeToggle.IsSelected = Equals(sender, buttonRangeToggle);
            buttonAllDayToggle.IsSelected = Equals(sender, buttonAllDayToggle);
            TicketFilterContextMenu.IsOpen = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonClosedToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.Closed;
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancelToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.Canceled;
        }

        [Obfuscation(Exclude = true)]
        private void buttonDispatchedToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.Dispatched;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.All;
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonMyOpenToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.MyOpen;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllOpenToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.AllOpen;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllDay_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.AllDay;
        }

[Obfuscation(Exclude = true)]
        private void buttonRange_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.Range;
        }

        [Obfuscation(Exclude = true)]
        private void buttonFutureToggle_SelectionGained(object sender, EventArgs e)
        {
            SetRadioChecked(sender);
            CurrentFilter = TicketSelectionShow.Future;
        }

    }
}
