using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PosControls;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Helpers;
using TemPOS.Managers;
using TemPOS.Networking;
using TemposLibrary;
using System.Reflection;
using TemPOS.Types;
using PosModels.Helpers;
using System.IO;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        #region Fields
        private ActionNotificationControl _notification = null;
        private bool _didOnce = false;
        private MainWindow _mainWindow;
        private static string _ensureDatabaseConnectionStatus;
        #endregion

        #region Properties
        public static StartupWindow Singleton
        {
            get;
            private set;
        }

        public bool PastRestartPoint
        {
            get;
            set;
        }

#if !DONT_USE_ELLIPSE_STARTUP
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            Point ellipsePoint = new Point(layoutSlotSize.Width / 2, layoutSlotSize.Height / 2);
            Geometry borderClip = new EllipseGeometry(ellipsePoint,
                layoutSlotSize.Width / 2.1, layoutSlotSize.Height / 2.1);
            borderClip.Freeze();

            Geometry baseClip = base.GetLayoutClip(layoutSlotSize);
            if (baseClip == null)
                return borderClip;

            CombinedGeometry mergedClip = new CombinedGeometry(
                GeometryCombineMode.Intersect, baseClip, borderClip);

            mergedClip.Freeze();
            return mergedClip;
        }
#endif
        #endregion

        public StartupWindow()
        {
            InitializeComponent();
            Singleton = this;
            PastRestartPoint = false;

            // Setup strings
            StringsCore.SetDefaultLanguage();
            StringsCore.LanguageChanged += StringsCore_LanguageChanged;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                labelVersion.Content = Types.Strings.Version + version.Major + "." + version.Minor;
                labelBuild.Content = Types.Strings.Build + " " + version.Build +
                    (version.Revision > 0 ? Types.Strings.Revision + version.Revision : "");
            }
            else
            {
                Version version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                labelVersion.Content = "Version 1.0";
                labelBuild.Content = Types.Strings.Build + " " + version.Build;
            }
#if DEMO
            labelBuild.Content += " (DEMO)";
#endif
            labelCopyright.Content = GetCopyright();

            Logger.WriteLog(Types.Strings.StartingApplication);

            //this.Width = SystemParameters.PrimaryScreenWidth;
            //this.Height = SystemParameters.PrimaryScreenHeight;
            
            Application.Current.SessionEnding += Current_SessionEnding;
            Application.Current.DispatcherUnhandledException +=
                Current_DispatcherUnhandledException;
        }

        private static void StringsCore_LanguageChanged(object sender, EventArgs e)
        {
            WpfHelper.UpdateStringBindings();
        }

        private string GetCopyright()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            AssemblyCopyrightAttribute attribute = null;
            if (attributes.Length > 0)
            {
                attribute = attributes[0] as AssemblyCopyrightAttribute;
            }
            return (attribute != null) ? attribute.Copyright : null;
        }

        /*
        private void WaitForProcess(Process process)
        {
            Process found = null;
            process.WaitForExit();
            Process[] processesOnDesktop = desktop.GetProcesses();
            do
            {
                found = null;
                foreach (Process desktopProcess in processesOnDesktop)
                {
                    try
                    {
                        if ((desktopProcess == null) ||
                            (desktopProcess.MainModule == null) ||
                            (desktopProcess.MainModule.FileName == null) ||
                            (desktopProcess == process) ||
                            (desktopProcess.HasExited) ||
                            (desktopProcess.HandleCount == 0) ||
                            (!desktopProcess.Responding) ||
                            (desktopProcess.Threads.Count == 0))
                            continue;
                        if (desktopProcess.MainModule.FileName.Equals(Application.ResourceAssembly.Location))
                        {
                            found = desktopProcess;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }
            } while (found != null);
        }
        */

        [Obfuscation(Exclude = true)]
        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            // Stop the auto-logoof timer
            LoginControl.StopAutoLogoutTimer();

            // Close all database connections
            DataModelBase.CloseAll();

#if !DEMO
            // Send the exception to the upgrade server
            Updater.StartCrashReport(e.Exception);
#endif
            // Save the exception, serialized to the AppData folder
            try
            {
                DateTime now = DateTime.Now;
                string rootDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    Path.DirectorySeparatorChar + "TemPOS";
                if (!Directory.Exists(rootDirectory))
                    Directory.CreateDirectory(rootDirectory);
                string path = rootDirectory + Path.DirectorySeparatorChar + "crashdata-" +
                    now.Year + now.Month.ToString("D2") + now.Day.ToString("D2") + "-" +
                    now.Hour.ToString("D2") + now.Minute.ToString("D2") + now.Second.ToString("D2") +
                    ".bin";
                using (FileStream fileStream = new FileStream(path, FileMode.CreateNew))
                {
                    byte[] serialData = e.Exception.SerializeObject();
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        writer.Write(serialData);
                    }
                }
            }
            catch { }

            // Display the exception message
            DisplayExceptionDialog(e.Exception);

            // Remove employee table lock (that prevents simultaneous login)
            if (SessionManager.ActiveEmployee != null)
                PosHelper.Unlock(TableName.Employee, SessionManager.ActiveEmployee.Id);

            // Close this app
            if (_mainWindow != null)
            {
                _mainWindow.AllowClose = true;
                _mainWindow.Close();
            }

            // Disable user control crap
            if (ConfigurationManager.UseKeyboardHook)
                UserControlManager.Disable();

            UserControlManager.ShowTaskbar(true);
#if !DEBUG
            if (PastRestartPoint)
            {
                // Restart application
                Process.Start(Application.ResourceAssembly.Location, "/RESTART");
                App.SwitchToDefaultDesktopOnClose = false;
            }
            else
            {
                App.SwitchToDefaultDesktopOnClose = true;
            }
#else
            App.SwitchToDefaultDesktopOnClose = true;
#endif
            // Shutdown current application
            Application.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
        }

        private void DisplayExceptionDialog(Exception exception)
        {
            while (exception.InnerException != null)
                exception = exception.InnerException;
            string remainingStackTrace = exception.StackTrace;
            string shortStackTrace = "";
            for (int i = 0; ; i++)
            {
                int index = remainingStackTrace.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (index == -1)
                {
                    if (shortStackTrace.Equals(""))
                        shortStackTrace = exception.StackTrace;
                    break;
                }
                shortStackTrace += remainingStackTrace.Substring(0, index + 1);
                remainingStackTrace = remainingStackTrace.Substring(index + 1);
            }
            Logger.WriteLog(exception.Message + Environment.NewLine + Environment.NewLine +
                shortStackTrace);
            Logger.WriteLog(Types.Strings.ExitingApplicationDueToAnUnhandledException);
            Logger.CloseLog();
            MessageBox.Show(Types.Strings.UnhandledException + exception.Message);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!_didOnce)
            {
                _didOnce = true;
                UpdateLayout();
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 0, 0, 200)
                };
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        [Obfuscation(Exclude = true)]
        void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            if (timer != null) timer.Stop();
            Startup();
        }

        private static bool EnsureDatabaseConnection()
        {
            int count = 0;
            if (LocalSetting.FileExists)
            {
                while (count < 2)
                {
                    string statusMessage;
                    if (DataModelBase.TestConnection(out statusMessage))
                        return true;
                    _ensureDatabaseConnectionStatus = statusMessage;
                    count++;
                }
            }
            _ensureDatabaseConnectionStatus = "Local settings does not exist yet.";
            return false;
        }

        public static bool PromptForConnectionString(bool isPersonalInfoHidden)
        {
            //Hide();
            PosDialogWindow window = SystemSettingsEditorControl.CreateInDefaultWindow(isPersonalInfoHidden);
            SystemSettingsEditorControl control = window.DockedControl as SystemSettingsEditorControl;
            control.InitializeFields(isPersonalInfoHidden);
            window.Topmost = true;
            window.IsClosable = false;
            window.ShowDialog();
            return !control.IsCanceled;
        }

        private void Startup()
        {
#if !DEMO
            // Handle an invalid license
            if ((LocalSetting.Values.String["IsAuthorized"] == null) ||
                !LocalSetting.Values.String["IsAuthorized"].Equals("Yes"))
            {
                // Display the Strings.LocalsettingEditor
                if (PromptForConnectionString(false))
                {
                    if (!string.IsNullOrEmpty(LocalSetting.CompanyName) &&
                        !string.IsNullOrEmpty(LocalSetting.ApplicationSerialNumber))
                    {
                        // Check for access to the update server
                        var client = new Srp6ClientSocket(
                            LocalSetting.CompanyName, LocalSetting.ApplicationSerialNumber);
                        client.ConnectFailed += client_ConnectFailed;
                        client.Disconnected += client_Disconnected;
                        client.Authenticated += client_Authenticated;
                        client.Start();
                        return;
                    }
                }
                BadLicenseShutdown();
            }
            else
#endif
            {
#if !DEMO
                if ((App.StartupArgs.Length == 1) && App.StartupArgs[0].ToLower().Equals(@"/update"))
                {
                    PosDialogWindow window = GeneralSettingsUpdateControl.CreateInDefaultWindow();
                    GeneralSettingsUpdateControl control = window.DockedControl as GeneralSettingsUpdateControl;
                    Hide();
                    window.ShowDialog();
                    UserControlManager.ShowTaskbar(true);
                    Application.Current.Shutdown();
                    return;
                }            
#endif
                PosDialogWindow.SetStartupWindow(this);
                BeginStartup();
            }
        }

#if !DEMO
        private void BadLicenseShutdown(bool showDialog = true)
        {
            if (showDialog)
                PosDialogWindow.ShowDialog(
                    Strings.YouHaveNotEnteredAValidCompanyNameAndorSerialNumber,
                    Strings.Error);
            UserControlManager.ShowTaskbar(true);
            Application.Current.Shutdown();
        }

        [Obfuscation(Exclude = true)]
        void client_Authenticated(object sender, EventArgs e)
        {
            var client = sender as Srp6ClientSocket;
            if (client == null) return;
            client.Disconnected -= client_Disconnected;
            client.Socket.Disconnect();
            LocalSetting.Values.String["IsAuthorized"] = "Yes";
            LocalSetting.Update();
            BeginStartup();
        }

        [Obfuscation(Exclude = true)]
        void client_Disconnected(object sender, EventArgs e)
        {
            BadLicenseShutdown();
        }

        [Obfuscation(Exclude = true)]
        void client_ConnectFailed(object sender, EventArgs e)
        {
            PosDialogWindow.ShowDialog(
                Strings.CanNotConnectToTheTemposUpdateServerToVerifyYourSerialNumberPleaseCheckYourInternetConnectionIfYouAreConnectedToTheInternetPleaseTryAgainLaterTheUpdateServerIsDownForMaintenance,
                Strings.ConnectionFailed);
            BadLicenseShutdown(false);
        }

        [Obfuscation(Exclude = true)]
        void client_Connected(object sender, EventArgs e)
        {
            
        }
#endif

        private void BeginStartup()
        {
            if ((ServiceHelper.IsSqlServiceLocal && !ServiceHelper.IsSqlServiceRunningLocally) ||
                (ServiceHelper.IsSqlBrowserServiceLocal && !ServiceHelper.IsSqlBrowserServiceRunningLocally &&
                !string.IsNullOrEmpty(LocalSetting.DatabaseServerLoginName)))
            {
                string message;
                if (!ServiceHelper.IsSqlServiceRunningLocally &&
                    !ServiceHelper.IsSqlBrowserServiceLocal)
                    message = Types.Strings.TheSQLServiceAndSQLBrowserServiceAreNotRunningWouldYouLikeToStartThem;
                else if (!ServiceHelper.IsSqlServiceRunningLocally)
                    message = Types.Strings.TheSQLServiceIsNotRunningWouldYouLikeToStartIt;
                else
                    message = Types.Strings.TheSQLBrowserServiceIsNotRunningWouldYouLikeToStartIt;
                if (PosDialogWindow.ShowDialog(message,
                    Types.Strings.Warning, DialogButtons.YesNo) == DialogButton.Yes)
                {
                    _notification = ActionNotificationControl.Create(null,
                        Types.Strings.StartingSQLServices, Types.Strings.Notification);
                    _notification.Show();
                    Thread thread = new Thread(ServiceStartThreadStart);
                    thread.Start(thread);
                    return;
                }
            }
            ContinueStartUp();
        }

        private void ServiceStartThreadStart(object threadObject)
        {
            Thread thread = threadObject as Thread;
            if (thread == null) return;
            VistaSecurity.RestartElevated("/STARTSQL", true);
            Dispatcher.Invoke((Action)(() =>
            {                
                for (int maxDelay = 200; (!ServiceHelper.IsSqlServiceRunningLocally && (maxDelay > 0)); maxDelay--)
                {
                    Thread.Sleep(100);
                }
                if (ServiceHelper.IsSqlServiceRunningLocally)
                {
                    ContinueStartUp();
                }
                else
                {
                    if (PosDialogWindow.ShowDialog(
                        Types.Strings.CouldNotStartTheSQLServiceWouldYouLikeToContinue,
                        Types.Strings.Warning, DialogButtons.YesNo) == DialogButton.Yes)
                    {
                        ContinueStartUp();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }));
            thread.Abort();
        }

        private void ContinueStartUp()
        {
            if (_notification != null)
                _notification.Close();
            new Thread(CheckDatabaseConnection).Start();
        }

        private void CheckDatabaseConnection()
        {
            if (!EnsureDatabaseConnection())
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    if (PromptForChanges())
                    {
                        new Thread(CheckDatabaseConnection).Start();
                    }
                    else
                    {
                        UserControlManager.ShowTaskbar(true);
                        Application.Current.Shutdown();
                    }
                }));
            }
            else
            {
                Dispatcher.Invoke((Action)(DoFinishInstall));
            }
        }

        private bool PromptForChanges()
        {

            PosDialogWindow.ShowDialog(_ensureDatabaseConnectionStatus, Types.Strings.DatabaseConnectionTimeout, false);
            return PromptForConnectionString(true);
        }

        private void DoFinishInstall()
        {
            bool exists = false;
            try
            {
                exists = DataModelBase.DatabaseExists("TemPOS"); 
            }
            catch {  }

            if (!exists)
            {
                _notification = ActionNotificationControl.Create(null,
                    Types.Strings.InstallingSQLDatabase, Types.Strings.Notification);
                _notification.Show();

                new Thread(DoFinishInstallThreadStart).Start();
            }
            else
            {
                _notification = null;
                FinalStartUp();
            }
        }

        private void DoFinishInstallThreadStart()
        {
            SqlServerSetup.FinishInstall();
            Dispatcher.Invoke((Action)(FinalStartUp));
        }


        /// <summary>
        /// Called when all startup conditions have been meet. The database is now
        /// accessible as well.
        /// </summary>
        private void FinalStartUp()
        {
            if ((_notification != null) && _notification.IsLoaded && _notification.IsVisible)
                _notification.Close();

            // Disable AlwaysUseDefaults which was being used prior to this...
            ConfigurationManager.AlwaysUseDefaults = false;

            // Task Manager Protection
           // SettingManager.SetStoreSetting("DontDisableTaskManager", 0);
            int? isDisabled = SettingManager.GetInt32("DontDisableTaskManager");
            if (isDisabled.HasValue && (isDisabled.Value == 1))
            {
                BeginStartup();
                return;
            }
            if (!TaskManagerServiceHelper.IsInstalled)
            {
                if (PosDialogWindow.ShowDialog(Types.Strings.DoYouWantToInstallTheTaskManagerAccessService,
                    Types.Strings.InstallService, DialogButtons.YesNo, false) == DialogButton.Yes)
                    new Thread(TaskManagerServiceHelper.InstallThread).Start();
            }
            else if (!TaskManagerServiceHelper.IsStarted)
            {
                if (PosDialogWindow.ShowDialog(Types.Strings.DoYouWantToStartTheTaskManagerAccessService,
                    Types.Strings.StartService, DialogButtons.YesNo, false) == DialogButton.Yes)
                    new Thread(TaskManagerServiceHelper.StartThread).Start();
            }
            else
            {
                TaskManagerServiceHelper.IsTaskManagerDisabled = true;
            }


            // Install the SQL Assembly (if one is pending installation)
            if (Updater.InstallSQLAssembly())
            {
#if DEBUG
                PosDialogWindow.ShowDialog("New SQL Assembly Installed", Types.Strings.Information);
#endif
            }

            // Apply any required database patches at runtime
            SqlServerSetup.ApplyDatabasePatches();

            // Check to make sure the model classes and the tables match
            if (!DataModelBase.ValidateDatabase())
            {
#if DEBUG
                string results = DataModelBase.InvalidDatabaseReport();
                PosDialogWindow.ShowDialog(
                    results,
                    "Invalid Database Report");
#else
                PosDialogWindow.ShowDialog(
                    Strings.TheDatabaseDesignCurrentlyBeingUsedIsIncorrectForThisVersionOfTempos,
                    Strings.StartupError);
#endif
                UserControlManager.ShowTaskbar(true);
                Application.Current.Shutdown();
                return;
            }

            // Enable user control
            UserControlManager.Enable(ConfigurationManager.UseKeyboardHook);
            UserControlManager.ShowTaskbar(false);

            // Start-up MainWindow
            Show();
            _mainWindow = new MainWindow();
            _mainWindow.Show();            
        }

        [Obfuscation(Exclude = true)]
        void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            //Application.Current.Shutdown();
        }

        public static void HideWindow()
        {
            Singleton.Topmost = false;
            Singleton.Hide();
            //MessageBox.Show("Window Hidden");
        }

    }
}
