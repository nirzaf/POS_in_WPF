using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using TemPOS.Networking;
using PosControls;
using PosControls.Interfaces;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.EventHandlers;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IShadeable
    {
        private readonly DispatcherTimer _showMe = new DispatcherTimer();

        public static MainWindow Singleton
        {
            get;
            private set;
        }

        private bool _showingShadingOverlay;
        private ShadingAdorner _adorner;

        public bool AllowClose
        {
            get;
            set;
        }

        /// <summary>
        /// This is used to gray out the parent window when showing a dialog
        /// </summary>
        public static bool ShowWindowShadingOverlay
        {
            get
            {
                return Singleton.ShowShadingOverlay;
            }
            set
            {
                Singleton.ShowShadingOverlay = value;
            }
        }

        /// <summary>
        /// This is used to gray out the parent window when showing a dialog
        /// </summary>
        public bool ShowShadingOverlay
        {
            get
            {
                return _showingShadingOverlay;
            }
            set
            {
                if (_showingShadingOverlay == value)
                    return;
                _showingShadingOverlay = value;
                if (value)
                {
                    if (_adorner == null)
                        _adorner = new ShadingAdorner(mainPane);
                    AdornerLayer.GetAdornerLayer(mainPane).Add(_adorner);
                }
                else
                {
                    if (_adorner != null)
                        AdornerLayer.GetAdornerLayer(mainPane).Remove(_adorner);
                }
            }
        }

        #region Constructor
        public MainWindow()
        {
            if (Singleton != null)
                throw new Exception("MainWindow Singleton Exception");
            Singleton = this;
            AllowClose = false;
            InitializeComponent();
            InitializeSize();

            // Timed delayed Show()
            _showMe.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _showMe.Tick += showMe_Tick;
            _showMe.Start();

            // Not sure why I put this here, but it doesn't hurt anything
            LocalSetting.Update();
#if !DEMO
            // Start the client broadcast server if running locally
            if (BroadcastServerSocket.IsEnabled)
                BroadcastServerSocket.Start();

            // Start the client broadcast client
            BroadcastClientSocket.Connected += MessageSocket_Connected;
            BroadcastClientSocket.ReceivedMessage += MessageSocket_ReceivedMessage;
            BroadcastClientSocket.Start();
#endif
            // Restore focus if lost
            LostFocus += MainWindowDialog_LostFocus;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartupWindow.Singleton.PastRestartPoint = true;
            PosDialogWindow.ForceTopMost = false;
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void MessageSocket_Connected(object sender, EventArgs e)
        {
            int? terminalNumber = NetworkTools.GetLastLanByte();
            if (terminalNumber != null)
                BroadcastClientSocket.SendMessage("AppStarted " + terminalNumber.Value);
            else
                BroadcastClientSocket.SendMessage("AppStarted");
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        void MessageSocket_ReceivedMessage(object sender, EventArgs e)
        {
            var message = sender as string;
            if (message == null) return;
            if (message.Equals("ServerPortMayHaveChanged"))
            {
                // Restart the client
                BroadcastClientSocket.Stop();
                BroadcastClientSocket.Start();
            }
            else if (message.StartsWith("UpdateServerInfo "))
            {
                string[] tokens = message.Split(' ');
                if (tokens.Length != 3)
                    return;
                LocalSetting.Values.String["UpdateServerPort"] = tokens[2];
                LocalSetting.Values.String["UpdateServer"] = tokens[1];
                LocalSetting.Update();
            }
        }
#endif

        [Obfuscation(Exclude = true)]
        void showMe_Tick(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            if (timer != null) timer.Stop();
            Show();
            StartupWindow.HideWindow();
            Activate();
            BringIntoView();
        }

        [Obfuscation(Exclude = true)]
        void MainWindowDialog_LostFocus(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        private void InitializeSize()
        {
            Top = 0;
            Left = 0;
            Width = ConfigurationManager.ProgramWidth;
            Height = ConfigurationManager.ProgramHeight;
            WindowState = ConfigurationManager.WindowState;
        }

        #endregion

        [Obfuscation(Exclude = true)]
        private void Grid_Initialized(object sender, EventArgs e)
        {
            loginControl.Login += loginControl_Login;
            OrderEntryCommands.Logout.Executed += orderEntrycommands_Logout;
        }

        [Obfuscation(Exclude = true)]
        void loginControl_Login(object sender, UserLoginEventArgs args)
        {
            Employee employee = args.Employee;
            if (employee != null)
            {
                orderEntryControl.Login(employee);
                loginControl.Visibility = Visibility.Hidden;
                orderEntryControl.Visibility = Visibility.Visible;
            }
        }

        [Obfuscation(Exclude = true)]
        void orderEntrycommands_Logout(object sender, EventArgs e)
        {
            loginControl.Logout();
            loginControl.Visibility = Visibility.Visible;
            orderEntryControl.Visibility = Visibility.Hidden;
        }

        [Obfuscation(Exclude = true)]
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!AllowClose)
                e.Cancel = true;
            else
            {
                DeviceManager.ClosePosDevices();
                DataModelBase.CloseAll();
            }
        }

        [Obfuscation(Exclude = true)]
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            //while (true) ;
        }

    }
}
