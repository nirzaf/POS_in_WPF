using System;
using System.Reflection;
using System.Text;
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
using TemPOS.EventHandlers;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        #region Fields
        private static DispatcherTimer _autoLogoutTimer;
        private Window _parentWindow;
        #endregion

        #region Properties
        public static LoginControl Singleton
        {
            get;
            private set;
        }


        public bool IsLoggedIn
        {
            get;
            private set;
        }

        public MainWindow ParentWindow
        {
            get
            {
                if (_parentWindow == null)
                    _parentWindow = Window.GetWindow(this);
                return (MainWindow)_parentWindow;
            }
        }
        
        public bool UsePasswordPrompt
        {
            get
            {
                int? intValue = SettingManager.GetStoreSetting("UsePasswordPrompt").IntValue;
                if (intValue == null)
                    return false;
                return !intValue.Equals(0);
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event UserLoginEventHandler Login;

        [Obfuscation(Exclude = true)]
        public static event EventHandler AutoLogout;
        #endregion

        public LoginControl()
        {
            if (Singleton != null)
                throw new Exception(Types.Strings.LoginSingletonException);
            Singleton = this;
            IsLoggedIn = false;
            InitializeComponent();
            InitializeVisability(UsePasswordPrompt);
            Loaded += LoginControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void LoginControl_Loaded(object sender, RoutedEventArgs e)
        {
#if !DEMO
            if (!ConfigurationManager.IsInDesignMode && DeviceManager.ActiveScanner != null)
            {
                DeviceManager.ActiveScanner.DataEvent += ActiveScanner_DataEvent;
                DeviceManager.ActiveScanner.ErrorEvent += ActiveScanner_ErrorEvent;
            }
#endif
            if (!Employee.NoEmployeeExists) return;
            Person person = Person.Add("Administrator", null, "Administrator",
                                       "", "", 0, 0, 0, 0, 0, 0, 0, "");
            Employee employee =
                EmployeeManager.AddEmployee(person.Id, DateTime.Now,
                                            new Permissions[] { }, "tempos", null);
            employee.GrantAllPermissions();
            employee.Update();
            PosDialogWindow.ShowDialog(
                Types.Strings.LoginAdminInfo1 + "Administrator" + Environment.NewLine +
                Types.Strings.LoginAdminInfo1 + "tempos", Types.Strings.LoginAdminUserCreated);
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void ActiveScanner_ErrorEvent(object sender, Microsoft.PointOfService.DeviceErrorEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        void ActiveScanner_DataEvent(object sender, Microsoft.PointOfService.DataEventArgs e)
        {
            if (!IsLoggedIn)
            {
                string data = Encoding.ASCII.GetString(DeviceManager.ActiveScanner.ScanData);
                //MessageBox.Show(data, "Data");
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(() => DoLogin(data)));
            }
        }
#endif

        private void InitializeVisability(bool usePasswordPrompt)
        {
            passwordBox.Visibility =
                label1.Visibility =
                (!usePasswordPrompt ? Visibility.Visible : Visibility.Hidden);
            buttonLogin.Visibility =
                (usePasswordPrompt ? Visibility.Visible : Visibility.Hidden);
        }

        public new void Focus()
        {
            passwordBox.Focus();
        }

        [Obfuscation(Exclude = true)]
        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DoLogin(passwordBox.Password);
        }

        private void DoLogin(string scanCode)
        {
            Employee employee = EmployeeManager.LookupByScanCode(scanCode);
            if (employee == null)
            {
                // The debug version will have an exception here if escape-exit is used
                try
                {
                    PosDialogWindow.ShowDialog(Types.Strings.LoginLoginIncorrect, Types.Strings.Error);
                    return;
                }
                catch
                {
                    return;
                }
                
            }

            // Check if logged-in somewhere else
            if (PosHelper.IsLocked(TableName.Employee, employee.Id))
            {
#if !DEMO
                BroadcastClientSocket.SendRemoteLogout(employee.Id);
#endif
                PosHelper.Unlock(TableName.Employee, employee.Id);
            }

            // Check if clock-in is required
            if (!employee.IsClockedIn())
                if (!DoClockIn(employee) && !employee.HasPermission(Permissions.SystemMaintenance))
                    return;

            // Proceed with login            
            IsLoggedIn = true;

            // Clear dead-locks
            Lock.DeleteAllEmployeeLocks(employee.Id);

            // Lock the employee to prevent simultaneous logins
            PosHelper.Lock(TableName.Employee, employee.Id, employee.Id);

#if !DEMO
            // Tell other clients, that this employee just logged in
            BroadcastClientSocket.SendMessage("LOGIN " + employee.Id);
#endif
            StartAutoLogoutTimer();
            if (Login != null)
                Login.Invoke(this, new UserLoginEventArgs(employee));
        }

        public static void StartAutoLogoutTimer()
        {
            StoreSetting setting = SettingManager.GetStoreSetting("DisableAutoLogoff");
            if ((setting.IntValue == null) || (setting.IntValue.Value == 0))
            {
                if (_autoLogoutTimer == null)
                {
                    _autoLogoutTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 5)};
                    _autoLogoutTimer.Tick += timer_Tick;
                }
                if (!_autoLogoutTimer.IsEnabled)
                    _autoLogoutTimer.Start();
            }
        }

        public static void StopAutoLogoutTimer()
        {
            if (_autoLogoutTimer != null)
            {
                if (_autoLogoutTimer.IsEnabled)
                    _autoLogoutTimer.Stop();
                _autoLogoutTimer = null;
            }
        }
        
        [Obfuscation(Exclude = true)]
        private static void timer_Tick(object sender, EventArgs e)
        {
            StoreSetting setting = SettingManager.GetStoreSetting("EnableAutoLogoffForDialogWindows");
            bool disableForDialogWindows =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            if (disableForDialogWindows && PosDialogWindow.HasPosDialogWindowsOpen)
            {
                return;
            }

            setting = SettingManager.GetStoreSetting("AutoLogoffTimeout");
            if (setting.IntValue != null)
                SettingManager.SetStoreSetting("AutoLogoffTimeout", setting.IntValue.Value.Clamp(5, 9999));

            if (UserControlManager.UserInactivity <= new TimeSpan(0, 0, 10)) return;
            if (AutoLogout != null)
                AutoLogout.Invoke(null, new EventArgs());
            CloseDialogWindows();
            StopAutoLogoutTimer();
            Singleton.Logout();
            MainWindow.Singleton.loginControl.Visibility = Visibility.Visible;
            MainWindow.Singleton.orderEntryControl.Visibility = Visibility.Hidden;
        }

        private static void CloseDialogWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is PosDialogWindow)
                    window.Close();
            }
        }

        private bool DoClockIn(Employee employee)
        {
            PosDialogWindow window = EmployeeClockInControl.CreateInDefaultWindow(employee);
            EmployeeClockInControl control = window.DockedControl as EmployeeClockInControl;
            if (control.ActiveJob == null)
            {
                PosDialogWindow.ShowDialog(Types.Strings.LoginWarningJobs, Types.Strings.LoginError);
                return false; // Block login when moved to LoginControl
            }

            window.ShowDialog(ParentWindow);
            return (!window.ClosedByUser);
        }

        [Obfuscation(Exclude = true)]
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        [Obfuscation(Exclude = true)]
        public void Logout()
        {
            StopAutoLogoutTimer();
            IsLoggedIn = false;
            passwordBox.Password = "";
        }
        
        [Obfuscation(Exclude = true)]
        private void passwordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            buttonLogin.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Hidden;
            passwordBox.Visibility = Visibility.Hidden;
            MainWindow.ShowWindowShadingOverlay = true;
            string temp = PosDialogWindow.PromptKeyboard(Types.Strings.LoginEnterLogin, "", true, ShiftMode.None);
            MainWindow.ShowWindowShadingOverlay = false;
            if (App.IsAppShuttingDown)
                return;
            if (temp != null)
            {
                passwordBox.Password = temp;
                DoLogin(passwordBox.Password);
            }
            else
            {
                passwordBox.Password = "";
            }
        }
    }
}
