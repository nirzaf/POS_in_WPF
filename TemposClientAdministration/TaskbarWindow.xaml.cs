using System.Reflection;
using System.Windows;
using System.ComponentModel;
using System;
using PosControls;
using System.Windows.Controls;
using System.Collections;
using TemposClientAdministration.Helpers;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for TaskbarWindow.xaml
    /// </summary>
    public partial class TaskbarWindow : Window
    {
        private PosDialogWindow mainWindow;

        public static TaskbarWindow Singleton
        {
            get;
            set;
        }

        public bool AllowClose
        {
            get;
            set;
        }

        static TaskbarWindow()
        {
            Singleton = null;
        }

        public TaskbarWindow()
        {
            if (Singleton != null)
                throw new Exception("Singleton Exception");
            InitializeComponent();
            InitializeMainWindow();
            Singleton = this;
            AllowClose = false;
            notifyIcon.ContextMenu = GetContextMenu();
        }

        private void InitializeMainWindow()
        {
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double aspectRatio = Math.Round(width / height, 1);
            if (aspectRatio == Math.Round((double)4 / 3, 1))
            {
                width = width * 0.90;
                height = height * 0.78;
            }
            else
            {
                width = width * 0.90;
                height = height * 0.90;
            }
            CustomerSetupControl customerControl = new CustomerSetupControl();
            LicenseSetupControl licenseControl = new LicenseSetupControl();
            UpdateBuilderControl builderControl = new UpdateBuilderControl();
            UpdateServiceControl serviceControl = new UpdateServiceControl();
            CrashReportControl crashReportControl = new CrashReportControl();
            mainWindow = new PosDialogWindow("TemPOS Client Administration",
                new FrameworkElement[] { customerControl, licenseControl, serviceControl, builderControl, crashReportControl },
                new string[] { "Customers", "Licenses", "Update Service", "Update Builder", "Crash Reports" },
                new double[] { 80, 75, 110, 110, 110 },
                width, height);
            //mainWindow.Topmost = true;
            mainWindow.IsClosable = true;
            mainWindow.Closing += new CancelEventHandler(mainWindow_Closing);
        }

        private ContextMenu GetContextMenu()
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)e.Current;
                string name = entry.Key as string;
                if (name == "trayContextMenu")
                {
                    contextMenu = entry.Value as ContextMenu;
                    break;
                }
            }
            return contextMenu;
        }

        [Obfuscation(Exclude = true)]
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (AllowClose)
                return;
            e.Cancel = true;
            mainWindow.Visibility = Visibility.Hidden;
            mainWindow.Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            notifyIcon.Dispose();

            base.OnClosing(e);
        }
        
        [Obfuscation(Exclude = true)]
        private void MyNotifyIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }

        public static void Shutdown()
        {
            if (UpdateServer.IsRunning)
                UpdateServer.Stop();
            Singleton.AllowClose = true;
            Application.Current.Shutdown();
        }

        public static void ShowWindow()
        {
            Singleton.mainWindow.Visibility = Visibility.Visible;
            Singleton.mainWindow.Show();
            Singleton.mainWindow.Activate();
            Singleton.mainWindow.BringIntoView();
        }
    }
}
