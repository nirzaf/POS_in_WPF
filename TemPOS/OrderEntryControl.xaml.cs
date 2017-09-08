using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TemPOS.Networking;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

using TemPOS.Types;
#if SPEECH_COMMANDS
using System.Speech.Recognition;
#endif

/* These need to get added back in...
                <Button x:Name="buttonCustomers"  Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.Customers IsEnabled="False" />
  
                <Button x:Name="buttonGiftCards" Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.GiftCards IsEnabled="False" />
                <Button x:Name="buttonRegister"  Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.Register IsEnabled="False" />
  
                <Button x:Name="buttonVendor" Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.Vendors IsEnabled="False" />
                <Button x:Name="buttonVendorItems" Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.VendorItems IsEnabled="False" />
                <Button x:Name="buttonVendorOrders"  Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.VendorOrders IsEnabled="False"  />
  
                <Button x:Name="buttonMap"  Margin="5,0,5,0" FontWeight="ExtraBold" Width="70" Height="64" VerticalAlignment="Bottom" HorizontalAlignment="Left" Selected="button_Selected" Text=Strings.Map />
 */

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryControl.xaml
    /// </summary>
    public partial class OrderEntryControl : UserControl
    {
        #region Fields
        private DispatcherTimer _dateTimeUpdate;
        private DispatcherTimer _checkFutureOrders;
        private DispatcherTimer _weatherCheck;
        private DateTime _lastFutureOrdersUpdate = DateTime.Now;
        private Window _parentWindow;
        #endregion

        #region Properties
        public MainWindow ParentWindow
        {
            get
            {
                if (_parentWindow == null)
                    _parentWindow = Window.GetWindow(this);
                return (MainWindow)_parentWindow;
            }
        }

        public static UIElementCollection ReceiptTapeItems
        {
            get
            {
                if (Singleton == null)
                    return null;
                if (Singleton.receiptTape1 == null)
                    return null;
                return Singleton.receiptTape1.Items;
            }
        }

        private static OrderEntryControl Singleton
        {
            get;
            set;
        }
        #endregion

        #region Initialization and Login
        public OrderEntryControl()
        {
            if (Singleton != null)
                throw new Exception("OrderEntryControl Singleton Exception");
            Singleton = this;
            InitializeComponent();
            InitializeStatusBarUpdate();
            InitializeSessionManager();
            InitializeReceiptTape();
            InitializeCheckFutureOrdersTimer();
            InitializeSpeechCommands();
            InitializeBroadcastMessageEventHanlder();

            // Wired into XAML this causes an exception in Release build
            OrderEntryTicketSelectionControl.SelectedTicketChanged +=
                ticketSelectionControl_SelectedTicketChanged;

            StringsCore.LanguageChanged += StringsCore_LanguageChanged;
        }

        void StringsCore_LanguageChanged(object sender, EventArgs e)
        {
            QueryCurrentWeather();
            OrderEntryCommands.SetupNoOrderCommands();
            SetDisplayedTicketTypeToStatusBar();
        }

        private void InitializeBroadcastMessageEventHanlder()
        {
#if !DEMO
            BroadcastClientSocket.ReceivedMessage +=
                BroadcastClientSocket_ReceivedMessage;
#endif
        }

        private void InitializeSpeechCommands()
        {
#if SPEECH_COMMANDS
            App.SpeechRecognizer.SpeechRecognized +=
            [Obfuscation(Exclude = true)]
                new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizer_SpeechRecognized);
            InitializeGrammer();
#endif
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void BroadcastClientSocket_ReceivedMessage(object sender, EventArgs e)
        {
            string message = sender as string;
            if ((message == null) || (!message.StartsWith("RemoteLogout"))) return;
            try
            {
                string[] tokens = message.Split(' ');
                int employeeId = Convert.ToInt32(tokens[1]);
                if ((SessionManager.ActiveEmployee.Id == employeeId) &&
                    OrderEntryCommands.Logout.CanExecute(this))
                    OrderEntryCommands.Logout.Execute(this);
            }
            catch { }
        }
#endif

        private void InitializeSessionManager()
        {
            SessionManager.SessionReset += SessionManager_SessionReset;
            SessionManager.SessionResetItem += SessionManager_SessionResetItem;
            SessionManager.TicketCleared += SessionManager_TicketCleared;
            SessionManager.ActiveTicketItemChanged += SessionManager_ActiveTicketItemChanged;
            SessionManager.ActiveItemChanged += SessionManager_ActiveItemChanged;
        }

        private void InitializeReceiptTape()
        {
            receiptTape1.Items.Clear();
            this.receiptTape1.SelectionChanged +=
                listBoxTransactions_SelectionChanged;
        }

        [Obfuscation(Exclude = true)]
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SessionManager.Reset();
        }

        public void Login(Employee employee)
        {
            Person employeePerson = Person.Get(Employee.GetPersonId(employee.Id));
            textBlockUserName.Text = employeePerson.FirstName;
            SessionManager.ActiveEmployee = employee;
            SettingManager.SetEmployeeSetting(employee.Id, "LastLogin", DateTime.Now);

            // Initialize Employee Settings
            InitializeEmployeeSettings(employee);

            RegisterManager.SetActiveRegisterDrawer();
            OrderEntryCommands.SetupNoOrderCommands();
            InitializeTicketSelection(SessionManager.ActiveTicket);
            ShowTicketSelection(true);
            LoadSuspendedTicket();
        }

        private void InitializeEmployeeSettings(Employee employee)
        {
            EmployeeSetting employeeSetting = EmployeeSetting.Get(employee.Id, "TemperatureScale");
            if ((employeeSetting == null) || !employeeSetting.IntValue.HasValue)
                WeatherHelper.Scale = TemperatureScale.Fahrenheit;
            else
                WeatherHelper.Scale = (TemperatureScale)employeeSetting.IntValue.Value;

            employeeSetting = EmployeeSetting.Get(employee.Id, "Language");
            if ((employeeSetting == null) || !employeeSetting.IntValue.HasValue)
                StringsCore.Language = Languages.English;
            else
                StringsCore.Language = (Languages)employeeSetting.IntValue.Value;
        }


        private void LoadSuspendedTicket()
        {
            // Does the employee have any suspended tickets open
            Ticket ticket = Ticket.GetSuspendedTicket(SessionManager.ActiveEmployee.Id);
            if (ticket != null)
            {
                SetAndLoadActiveTicket(ticket);
                return;
            }

            // Also need to check for TicketItem's, on this employee's open Ticket's, that have
            // no TicketItemOrderTime set
            foreach (Ticket employeeTicket in
                Ticket.GetAllActive(SessionManager.ActiveEmployee.Id)
                .Where(employeeTicket => TicketItem.GetAll(employeeTicket.PrimaryKey, false, true).Any()))
            {
                // Skip future orders
                if ((employeeTicket.StartTime != null) && (employeeTicket.PrepareTime == null)) continue;
                // Load ticket
                SetAndLoadActiveTicket(employeeTicket);
                return;
            }
        }
        #endregion

        #region SessionManager event handlers
        [Obfuscation(Exclude = true)]
        void SessionManager_SessionReset(object sender, EventArgs e)
        {
            receiptTape1.Items.Clear();
            orderEntryItemSelection.Clear();
            buttonTouchInputCategory.Clear();
            orderEntryItemOptions.Clear();
            orderEntryPizzaItemOptions.Clear();
        }

        [Obfuscation(Exclude = true)]
        void SessionManager_SessionResetItem(object sender, EventArgs e)
        {
            // Reset interface
            buttonTouchInputCategory.Show();
            buttonTouchInputCategory.SelectCategoryButton(-1);
            orderEntryItemOptions.Clear();
            orderEntryItemSelection.Clear();
            orderEntryPizzaItemOptions.Clear();
        }

        [Obfuscation(Exclude = true)]
        void SessionManager_TicketCleared(object sender, EventArgs e)
        {            
            ShowCouponControl(false);
            ShowDiscountControl(false);
            couponEntryControl.Visibility = Visibility.Hidden;
            couponEntryControl.SelectedTicket = null;
            discountControl.Visibility = Visibility.Hidden;
            discountControl.SelectedTicket = null;
            buttonTouchInputCategory.Clear();
            orderEntryItemSelection.Clear();
            orderEntryItemOptions.Clear();
            orderEntryPizzaItemOptions.Clear();
            receiptTape1.Items.Clear();
            InitializeTicketSelection(SessionManager.ActiveTicket);
            ShowTicketSelection(true);
            OrderEntryCommands.SetupNoOrderCommands();
        }

        [Obfuscation(Exclude = true)]
        void SessionManager_ActiveItemChanged(object sender, EventArgs e)
        {
            //orderEntryItemSelection.ActiveItem = SessionManager.ActiveItem;
            //orderEntryItemSelection.SetActiveItem(SessionManager.ActiveItem);
        }

        [Obfuscation(Exclude = true)]
        void SessionManager_ActiveTicketItemChanged(object sender, EventArgs e)
        {
            TextBlockButton siButton = null, ciButton = null, viButton = null;
            Item item = SessionManager.ActiveItem;
            TicketItem ticketItem = SessionManager.ActiveTicketItem;
            if ((item == null) || (ticketItem == null))
            {
#if OLDCODE
                ciButton = actionBar.FindButton(Strings.OrderEntryCancelItem);
                if (ciButton != null)
                    ciButton.IsEnabled = false;
                viButton = actionBar.FindButton(Strings.OrderEntryVoidItem);
                if (viButton != null)
                    viButton.IsEnabled = false;                
                siButton = actionBar.FindButton(Strings.OrderEntrySpecialInstructions);
                if (siButton != null)
                    siButton.IsEnabled = false;
#endif
                return;
            }

            // Setup Category
            buttonTouchInputCategory.Show();
            SessionManager.ActiveCategory =
                buttonTouchInputCategory.ActiveCategory =
                Category.Get(item.CategoryId);
            buttonTouchInputCategory.SelectCategoryButton(item.CategoryId);

            // Setup item
            orderEntryItemSelection.Show(item.CategoryId);
            orderEntryItemSelection.SelectItemButton(item.Id);

            // Setup item options
            orderEntryItemOptions.Clear();
            orderEntryPizzaItemOptions.Clear();
            orderEntryItemOptions.SetItemOptions(item);
            orderEntryPizzaItemOptions.SetItemOptions(item);
            orderEntryItemOptions.SetupTicketItemOptions(ticketItem);
            orderEntryPizzaItemOptions.SetupTicketItemOptions(ticketItem);

            // Setup coupon entry control
            couponEntryControl.SelectedTicketItem = ticketItem;
#if OLDCODE
            ciButton = actionBar.FindButton(Strings.OrderEntryCancelItem);
            if (ciButton != null)
                ciButton.IsEnabled = true;
            viButton = actionBar.FindButton(Strings.OrderEntryVoidItem);
            if (viButton != null)
                viButton.IsEnabled = true;
            siButton = actionBar.FindButton(Strings.OrderEntrySpecialInstructions);
            if (siButton != null)
                siButton.IsEnabled = true;
#endif
        }
        #endregion

        #region Command Setup
        #region Speech Commands
#if SPEECH_COMMANDS
        public static void InitializeGrammer()
        {
            var c = new Choices();
            c.Add(Strings.HideButtons);
            c.Add(Strings.ShowButtons);

            // Ticket Filter Changes
            c.Add(Strings.ShowMyTickets);
            c.Add(Strings.ShowMyOpenTickets);
            c.Add(Strings.ShowAllTickets);
            c.Add(Strings.ShowAllOpenTickets);
            c.Add(Strings.ShowClosedTickets);
            c.Add(Strings.ShowCanceledTickets);
            c.Add(Strings.ShowTodaysTickets);

            c.Add(Strings.CreateTicket);
            c.Add(Strings.Reports);
            c.Add("Command Console");
            c.Add("Logout");
            c.Add("Exit");

            // Register Commands
            c.Add("No Sale");
            c.Add("Safe Drop");

            // Setup dialogs
            c.Add("Employee Setup");
            c.Add("Employee Job Setup");
            c.Add("Employee Jobs Setup");
            c.Add("Coupon Setup");
            c.Add("Discount Setup");
            c.Add("Hardware Setup");
            c.Add("Coupon Setup");
            c.Add("Tax Setup");
            c.Add("Taxes Setup");
            c.Add("Tax Setup");
            c.Add("Room Setup");
            c.Add("Seating Setup");
            c.Add("Occasion Setup");
            c.Add("Item Setup");
            c.Add("Category Setup");

            [Obfuscation(Exclude = true)]
            // System Functions
            [Obfuscation(Exclude = true)]
            c.Add("Start Of Day");
            c.Add("End Of Day");
            c.Add("End Of Year");
            c.Add("Edit Time Sheet");
            App.InitializeSpeechRecognizerChoices(c);
        }

        [Obfuscation(Exclude = true)]
        void SpeechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (SessionManager.ActiveEmployee == null)
                return;
            if (e.Result.Text.Equals(Strings.CreateTicket))
            {
            }
            else if (e.Result.Text.Equals("Employee Setup") &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.EmployeeMaintenance))
            {
                OrderEntrySetupControl.ShowEmployeeSetup(this);
            }
            else if (e.Result.Text.Equals("Exit") &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.ExitProgram))
            {
                ExitCommand();
            }
            else if (e.Result.Text.Equals(Strings.Reports) &&
                SessionManager.ActiveEmployee.HasPermission(Permissions.ReportsMenu))
            {
                ReportsMenuCommand();
            }
        }
#endif
        #endregion
        #endregion

        #region Command Handling
        public static void OpenSelectedTicket()
        {
            Singleton._OpenSelectedTicket();
        }

        private void _OpenSelectedTicket()
        {
            ParentWindow.ShowShadingOverlay = true;
            if (DoOpen == null)
                DoOpen += OrderEntryControl_DoOpen;
            DoOpen.BeginInvoke(null, new EventArgs(), BeginInvokeCallback, null);
        }

        private void BeginInvokeCallback(IAsyncResult result)
        {
            // Invokation required
            Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        //myCheckBox.IsChecked = true;
                        SetAndLoadActiveTicket(ticketSelectionControl.SelectedTicket);
                        ParentWindow.ShowShadingOverlay = false;
                    }
                ), DispatcherPriority.Normal);
            DoOpen.EndInvoke(result);
        }

        [Obfuscation(Exclude = true)]
        private event EventHandler DoOpen = null;

        [Obfuscation(Exclude = true)]
        void OrderEntryControl_DoOpen(object sender, EventArgs e)
        {
        }

        #region Party Management
        public static void EditPartyCommand(Ticket ticket)
        {
            Singleton._EditPartyCommand(ticket);
        }

        private void _EditPartyCommand(Ticket ticket)
        {
            PosDialogWindow window = PartyManagementControl.CreateInDefaultWindow();
            var control = window.DockedControl as PartyManagementControl;
            if (control == null) return;

            control.Initialize(ticket);
            PosDialogWindow.ShowPosDialogWindow( window);

            if (SessionManager.ActiveTicket != null)
            {
                if (control.CurrentTicket == null)
                    SetAndLoadActiveTicket(control.ParentTicket);
                else if (control.CurrentTicket.Exists())
                    SetAndLoadActiveTicket(control.CurrentTicket);
                else
                    SessionManager.ClearTicket();
            }
            else
            {
                if (control.CurrentTicket == null)
                    InitializeTicketSelection(control.ParentTicket);
                else if (control.CurrentTicket.Exists())
                    InitializeTicketSelection(control.CurrentTicket);
                else
                    InitializeTicketSelection(control.ParentTicket);
            }
        }
        #endregion

        #region Seating
        public static void SetSeating(string tableName)
        {
            Singleton._SetSeating(tableName, "");
        }

        public static void SetSeating(string tableName, string customerName)
        {
            Singleton._SetSeating(tableName, customerName);
        }

        private void _SetSeating(string tableName, string customerName)
        {
            textBlockTableName.Text = tableName;
            textBlockCustomerName.Text = customerName;
        }
        #endregion

        public static void SetAndLoadActiveTicket(Ticket ticket)
        {
            Singleton._SetAndLoadActiveTicket(ticket);
        }

        private void _SetAndLoadActiveTicket(Ticket ticket)
        {
            if (PosHelper.IsLocked(ticket))
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.OrderEntryErrorTicketAlreadyOpen,
                    Types.Strings.OrderEntryTicketLocked);
                return;
            }

            SessionManager.ActiveTicket = ticket;
            PosHelper.Lock(ticket, SessionManager.ActiveEmployee);

            // Stop auto-logoff timer, if it should be disabled
            StoreSetting setting =
                SettingManager.GetStoreSetting("EnableAutoLogoffWhileInOrderEntry");
            if ((setting.IntValue == null) || (setting.IntValue.Value == 0))
                LoginControl.StopAutoLogoutTimer();

            ShowTicketSelection(false);
            couponEntryControl.SelectedTicket =
                receiptTape1.SelectedTicket =
                discountControl.SelectedTicket = SessionManager.ActiveTicket;

            InitializeTicket();
        }

        public static void SetOrderAmountText(string amount)
        {
            Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntrySubTotal + amount;
        }

        public static void InitializeTicket()
        {
            Singleton._InitializeTicket();
        }

        private void _InitializeTicket()
        {
            textBlockOrderAmount.Text = Types.Strings.OrderEntrySubTotal + 
                receiptTape1.TicketSubTotal.ToString("C2");

            SessionManager.ActiveCategory = buttonTouchInputCategory.ActiveCategory = null;
            OrderEntryCommands.SetupInOrderCommands();
            buttonTouchInputCategory.IsEnabled =
                orderEntryItemSelection.IsEnabled =
                orderEntryItemOptions.IsEnabled = 
                orderEntryPizzaItemOptions.IsEnabled = 
                (!SessionManager.ActiveTicket.IsCanceled &&
                !SessionManager.ActiveTicket.IsClosed &&
                (DayOfOperation.Today != null));
            orderEntryItemOptions.Visibility = Visibility.Visible;

            ClearItemEntry();
            SessionManager.ActiveTicket.SetIsBeingModified();
        }
        #endregion

        public void SetOrderEntryCommands(bool isSet)
        {

            groupBoxNonOrderCommands.Visibility =
                ticketDetailsControl.Visibility =
                isSet ? Visibility.Collapsed : Visibility.Visible;
            groupBoxOrderCommands.Visibility =
                groupBoxCategories.Visibility =
                groupBoxItems.Visibility =
                groupBoxOptions.Visibility =
                isSet ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Future Orders Timer
        private void InitializeCheckFutureOrdersTimer()
        {
            _checkFutureOrders = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            _checkFutureOrders.Tick += checkFutureOrders_Tick;
            _checkFutureOrders.Start();
            OrderEntryCommands.UpdateFutureOrders();
        }

        [Obfuscation(Exclude = true)]
        void checkFutureOrders_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime lastUpdate = _lastFutureOrdersUpdate;
            _lastFutureOrdersUpdate = now;
            if (lastUpdate.Minute < now.Minute)
                OrderEntryCommands.UpdateFutureOrders();
        }

        #endregion

        #region StatusBar
        private void InitializeStatusBarUpdate()
        {
            _dateTimeUpdate = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
            _dateTimeUpdate.Tick += dateTimeUpdate_Tick;
            _dateTimeUpdate.Start();
            _weatherCheck = new DispatcherTimer {Interval = new TimeSpan(0, 2, 0)};
            _weatherCheck.Tick += weatherCheck_Tick;
            _weatherCheck.Start();
            WeatherHelper.QueryCompleted += WeatherHelper_QueryCompleted;
            QueryCurrentWeather();
        }

        public void QueryCurrentWeather()
        {
            StoreSetting enabledSetting = SettingManager.GetStoreSetting("ShowWeather");
            bool disableWeather =
                ((enabledSetting.IntValue == null) || (enabledSetting.IntValue.Value == 0));
            if (!disableWeather)
            {
                StoreSetting zipCodeSetting = SettingManager.GetStoreSetting("WeatherZipCode");
                int zipCode = (!zipCodeSetting.IntValue.HasValue ? 0 : zipCodeSetting.IntValue.Value);
                WeatherHelper.QueryCurrentWeather(zipCode);
            }
            else
            {
                textBlockWeather.Text = "";
            }
        }

        [Obfuscation(Exclude = true)]
        void WeatherHelper_QueryCompleted(object sender, EventArgs e)
        {
            textBlockWeather.Text = WeatherHelper.CurrentWeather;
        }

        [Obfuscation(Exclude = true)]
        void weatherCheck_Tick(object sender, EventArgs e)
        {
            WeatherHelper.QueryCurrentWeather(53704);
        }

        [Obfuscation(Exclude = true)]
        void dateTimeUpdate_Tick(object sender, EventArgs e)
        {
            textBlockTime.Text = DateTime.Now.ToShortTimeString();
            textBlockDate.Text = DateTime.Now.ToLongDateString();
        }

        public static void SetDisplayedTicketTypeToStatusBar()
        {
            switch (TicketFilterControl.CurrentFilter)
            {
                case TicketSelectionShow.All:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllTickets;
                    break;
                case TicketSelectionShow.AllDay:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllTicketsToday;
                    break;
                case TicketSelectionShow.AllOpen:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllOpenTickets;
                    break;
                case TicketSelectionShow.Canceled:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllCanceledTickets;
                    break;
                case TicketSelectionShow.Closed:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllClosedTickets;
                    break;
                case TicketSelectionShow.Dispatched:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingAllDispatchedTickets;
                    break;
                case TicketSelectionShow.MyOpen:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingYourOpenTickets;
                    break;
                case TicketSelectionShow.Future:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingFutureTickets;
                    break;
                case TicketSelectionShow.Range:
                    Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntryShowingTicketsFrom +
                        Singleton.ticketSelectionControl.RangeStart + Types.Strings.OrderEntryTo +
                        Singleton.ticketSelectionControl.RangeEnd;
                    break;
            }
            switch (TicketTypeFilterControl.CurrentFilter)
            {
                case TicketType.Catering:
                    Singleton.textBlockOrderAmount.Text += " ( " + Types.Strings.OrderEntryCatering + ")";
                    break;
                case TicketType.Delivery:
                    Singleton.textBlockOrderAmount.Text += " ( " + Types.Strings.OrderEntryDelivery + ")";
                    break;
                case TicketType.DineIn:
                    Singleton.textBlockOrderAmount.Text += " ( " + Types.Strings.OrderEntryDineIn + ")";
                    break;
                case TicketType.DriveThru:
                    Singleton.textBlockOrderAmount.Text += " ( " + Types.Strings.OrderEntryDriveThru + ")";
                    break;
                case TicketType.Pickup:
                    Singleton.textBlockOrderAmount.Text += " ( " + Types.Strings.OrderEntryCarryout + ")";
                    break;
            }
        }
        #endregion

        #region Ticket Item Selection
        public static int CurrentTicketItemCount
        {
            get
            {
                if ((Singleton == null) || (Singleton.receiptTape1 == null))
                    return 0;
                int count = Singleton.receiptTape1.Items.Cast<TicketItemTemplate>()
                    .Count(item => item.TicketItem.ParentTicketItemId == null);
                return count;
            }
        }

        // Unused
        public static int ActualTicketItemCount
        {
            get
            {
                if ((Singleton == null) || (Singleton.ticketSelectionControl == null) ||
                    (Singleton.ticketSelectionControl.SelectedTicket == null))
                    return 0;
                return Singleton.ticketSelectionControl.SelectedTicket.GetNumberOfTicketItems();
            }
        }

        public static void ClearReceiptTape()
        {
            Singleton.receiptTape1.ClearSelection();
        }

        [Obfuscation(Exclude = true)]
        void listBoxTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TicketItemTemplate obj in e.AddedItems.OfType<TicketItemTemplate>())
            {
                ProcessTicketItemSelection(obj);
            }
            OrderEntryCommands.UpdateInOrderCommands();
        }

        private void ProcessTicketItemSelection(TicketItemTemplate ticketItemTemplate)
        {
            TicketItem ticketItem = ticketItemTemplate.TicketItem;
            Item item = Item.Get(ticketItem.ItemId);

            // Setup session manager
            SessionManager.ActiveItem = item;
            SessionManager.ActiveTicketItem = ticketItem;
        }
        #endregion

        #region Ticket Selection
        private bool _recursionHalt;
        [Obfuscation(Exclude = true)]
        private void ticketSelectionControl_SelectedTicketChanged(object sender, EventArgs e)
        {
            if (!_recursionHalt)
            {
                _recursionHalt = true;
                UpdateSelectedTicketDetails();
                _recursionHalt = false;
            }
            if ((ticketSelectionControl.SelectedTicket == null) &&
                (ticketDetailsControl.SelectedTicket != null))
                ticketDetailsControl.SelectedTicket = null;
        }

        public static void InitializeTicketSelection(Ticket selectedTicket)
        {
            Singleton._InitializeTicketSelection(selectedTicket);
        }

        private void _InitializeTicketSelection(Ticket selectedTicket)
        {
            ticketSelectionControl.SelectedTicket = selectedTicket;
            ticketSelectionControl.UpdateTickets();
        }

        public static void UpdateSelectedTicketDetails()
        {
            Singleton._UpdateSelectedTicketDetails();
        }

        private void _UpdateSelectedTicketDetails()
        {
            ticketSelectionControl.UpdateSelectedTicket();
            ticketDetailsControl.SelectedTicket =
                ticketSelectionControl.SelectedTicket;
            couponEntryControl.SelectedTicket = ticketSelectionControl.SelectedTicket;
            discountControl.SelectedTicket = ticketSelectionControl.SelectedTicket;
        }

        public static void ShowTicketSelection(bool show)
        {
            Singleton._ShowTicketSelection(show);
        }

        private void _ShowTicketSelection(bool show)
        {

            ticketSelectionControl.Visibility =
                ticketDetailsControl.Visibility = (show ? Visibility.Visible : Visibility.Hidden);

            receiptTape1.Visibility =                
                orderEntryItemSelection.Visibility =
                buttonTouchInputCategory.Visibility = (show ? Visibility.Hidden : Visibility.Visible);

            if (show)
            {
                orderEntryItemOptions.Visibility = Visibility.Hidden;
                orderEntryPizzaItemOptions.Visibility = Visibility.Hidden;
                textBlockCustomerName.Text =
                    textBlockTableName.Text = "";
            }
            else
            {
                // ToDo: remove the (|| true) below without losing the items
                // options border
                orderEntryItemOptions.Visibility = 
                    ((orderEntryItemOptions.IsBeingUsed || true) ? Visibility.Visible : Visibility.Collapsed);

                orderEntryPizzaItemOptions.Visibility =
                    ((orderEntryPizzaItemOptions.IsBeingUsed) ? Visibility.Visible : Visibility.Collapsed);
            }
        }
        #endregion

        #region Coupon/Discount Control Handling
        public static void ShowCouponControl(bool show)
        {
            Singleton._ShowCouponControl(show);
        }

        private void _ShowCouponControl(bool show)
        {
            mainContentBorder.Visibility =
                couponEntryControl.Visibility =
                (show ? Visibility.Visible : Visibility.Hidden);

            if (show)
                discountControl.Visibility = Visibility.Hidden;
                
            orderEntryItemSelection.Visibility =
                buttonTouchInputCategory.Visibility =
                (show ? Visibility.Hidden : Visibility.Visible);

            if (show)
            {
                orderEntryItemOptions.Visibility = Visibility.Hidden;
                orderEntryPizzaItemOptions.Visibility = Visibility.Hidden;
            }
            else
            {
                // ToDo: Remove (|| true)
                orderEntryItemOptions.Visibility =
                    ((orderEntryItemOptions.IsBeingUsed || true) ? Visibility.Visible : Visibility.Collapsed);

                orderEntryPizzaItemOptions.Visibility =
                    ((orderEntryPizzaItemOptions.IsBeingUsed) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public static void ShowDiscountControl(bool show)
        {
            Singleton._ShowDiscountControl(show);
        }

        private void _ShowDiscountControl(bool show)
        {
            mainContentBorder.Visibility =
                discountControl.Visibility =
                (show ? Visibility.Visible : Visibility.Hidden);

            if (show)
                couponEntryControl.Visibility = Visibility.Hidden;

            orderEntryItemSelection.Visibility =
                buttonTouchInputCategory.Visibility =
                (show ? Visibility.Hidden : Visibility.Visible);

            if (show)
            {
                orderEntryItemOptions.Visibility = Visibility.Hidden;
                orderEntryPizzaItemOptions.Visibility = Visibility.Hidden;
            }
            else
            {
                // ToDo: Remove (|| true)
                orderEntryItemOptions.Visibility =
                    ((orderEntryItemOptions.IsBeingUsed || true) ? Visibility.Visible : Visibility.Collapsed);

                orderEntryPizzaItemOptions.Visibility =
                    ((orderEntryPizzaItemOptions.IsBeingUsed) ? Visibility.Visible : Visibility.Collapsed);
            }

        }
        #endregion

        #region Category, Item and Item Options
        private static void ClearItemEntry()
        {
            Singleton.orderEntryItemSelection.Clear();
            ClearItemOptions();
        }

        public static void ClearItemOptions()
        {
            Singleton.orderEntryItemOptions.Clear();
            Singleton.orderEntryPizzaItemOptions.Clear();
        }

        public static void SetItemOptions()
        {
            Singleton.orderEntryItemOptions.SetItemOptions(SessionManager.ActiveItem);
        }

        public static void SetCategory()
        {
            Singleton.orderEntryItemSelection.Show(SessionManager.ActiveCategory.Id);
        }

        public static void UpdateDisplayedOrderAmount()
        {
            Singleton.textBlockOrderAmount.Text = Types.Strings.OrderEntrySubTotal +
                Singleton.receiptTape1.TicketSubTotal.ToString("C2");
        }

        public static void RemoveSelectedTicketItem()
        {
            Singleton.receiptTape1.Items.Remove(Singleton.receiptTape1.SelectedItem);
            OrderEntryCommands.UpdateInOrderCommands();            
        }

        public static TicketItem AddItemToOrder(Item item)
        {
            TicketItem result = Singleton.receiptTape1.AddItemToOrder(item);
            OrderEntryCommands.UpdateInOrderCommands();
            return result;
        }

        public static void UpdateSelectedTicketItemTemplate()
        {
            TicketItemTemplate item = Singleton.receiptTape1.SelectedItem;
            if (item == null)
                return;
            item.Update();
            OrderEntryCommands.UpdateInOrderCommands();
        }
        #endregion
    }
}
