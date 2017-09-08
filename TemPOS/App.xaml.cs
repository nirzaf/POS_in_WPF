using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using PosControls;
using PosModels;
using TemPOS.Helpers;
using TemPOS.Managers;
using TemposLibrary;
using TemPOS.Networking;

using TemPOS.Types;
#if SPEECH_COMMANDS
using System.Speech.Recognition;
#endif

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static bool _newMutexCreated;

        public static bool IsAppShuttingDown
        {
            get;
            private set;
        }

        public static bool WasLocalSettingReset
        {
            get;
            private set;
        }

        public static Desktop TemposDesktop
        {
            get;
            private set;
        }

        public static string[] StartupArgs
        {
            get;
            private set;
        }

        public static bool IsUsingDesktop
        {
            get;
            private set;
        }

        public static bool SwitchToDefaultDesktopOnClose
        {
            get;
            set;
        }

        public static int ExitCode
        {
            get; set;
        }

        [Obfuscation(Exclude = true)]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ExitCode = 0;
            StartupArgs = e.Args;
            WasLocalSettingReset = false;
            TemposDesktop = null;
            IsAppShuttingDown = false;
            IsUsingDesktop = false;
            Current.SessionEnding += Current_SessionEnding;

            
            // Set the current directory to the application path
            // So we don't start-up in the \windows\System32\ folder at windows login
            try
            {
                //System.IO.Directory.SetCurrentDirectory(
                //    System.IO.Path.GetDirectoryName(VistaSecurity.GetExecutablePath())
                //);
                System.IO.Directory.SetCurrentDirectory(
                    System.IO.Path.GetDirectoryName(ResourceAssembly.Location)
                );
            }
            catch (Exception ex)
            {
                // Ignore, since the original app was in the currect folder, so should
                // the app on the alternate desktop.
                //MessageBox.Show(ex.Message);
            }

#if DEBUG
            if (SpecialStart.IsEnabled)
            {
                LocalSetting.Initialize();
                SpecialStart.Start();
                Process.GetCurrentProcess().Kill();
                return;
            }
#endif
            if ((e.Args.Length == 1) && e.Args[0].Equals("/INSTALLTMA"))
            {
                bool isDone = false;
                LocalSetting.Initialize();
                TaskManagerServiceHelper.InstallFailed += (o, args) =>
                {
                    ExitCode = -1 * TaskManagerServiceHelper.FailCode;
                    isDone = true;
                };
                TaskManagerServiceHelper.Installed += (o, args) =>
                {
                    TaskManagerServiceHelper.Start();
                };
                TaskManagerServiceHelper.StartFailed += (o, args) =>
                {
                    ExitCode = -2;
                    isDone = true;
                };
                TaskManagerServiceHelper.Started += (o, args) =>
                {
                    isDone = true;
                };
                TaskManagerServiceHelper.Install();
                while (!isDone)
                    Thread.Sleep(300);
                Current.Shutdown();
                return;
            }

            if ((e.Args.Length == 1) && e.Args[0].Equals("/UNINSTALLTMA"))
            {
                bool isDone = false;
                LocalSetting.Initialize();
                TaskManagerServiceHelper.UninstallFailed += (o, args) =>
                {
                    ExitCode = -1;
                    isDone = true;
                };
                TaskManagerServiceHelper.Uninstalled += (o, args) =>
                {
                    isDone = true;
                };
                TaskManagerServiceHelper.Uninstall();
                while (!isDone)
                    Thread.Sleep(300);
                Current.Shutdown();
                return;
            }

            if ((e.Args.Length == 1) && e.Args[0].Equals("/STARTTMA"))
            {
                bool isDone = false;
                LocalSetting.Initialize();
                TaskManagerServiceHelper.StartFailed += (o, args) =>
                {
                    ExitCode = -1;
                    isDone = true;
                };
                TaskManagerServiceHelper.Started += (o, args) =>
                {
                    isDone = true;
                };
                TaskManagerServiceHelper.Start();
                while (!isDone)
                    Thread.Sleep(300);
                Current.Shutdown();
                return;
            }

            if ((e.Args.Length == 1) && e.Args[0].Equals("/STOPTMA"))
            {
                bool isDone = false;
                LocalSetting.Initialize();
                TaskManagerServiceHelper.StopFailed += (o, args) =>
                {
                    ExitCode = -1;
                    isDone = true;
                };
                TaskManagerServiceHelper.Stopped += (o, args) =>
                {
                    isDone = true;
                };
                TaskManagerServiceHelper.Stop();
                while (!isDone)
                    Thread.Sleep(300);
                Current.Shutdown();
                return;
            }

            if ((e.Args.Length == 1) && e.Args[0].Equals("/STOPSQL"))
            {
                LocalSetting.Initialize();
                if (ServiceHelper.IsSqlServiceRunningLocally)
                    ServiceHelper.StopSqlService(20000);
                if (ServiceHelper.IsSqlBrowserServiceRunningLocally)
                    ServiceHelper.StopSqlBrowserService(20000);
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
                return;
            }

            if ((e.Args.Length == 1) && e.Args[0].Equals("/STARTSQL"))
            {
                LocalSetting.Initialize();
                if (!ServiceHelper.IsSqlBrowserServiceRunningLocally &&
                    !string.IsNullOrEmpty(LocalSetting.DatabaseServerLoginName))
                    ServiceHelper.StartSqlBrowserService(20000);
                if (!ServiceHelper.IsSqlServiceRunningLocally)
                    ServiceHelper.StartSqlService(20000);
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
                return;
            }

            // Initialize Local Settings
            WasLocalSettingReset = LocalSetting.Initialize();

            // Some old LocalSettings file doesn't have LocalSetting.Values.Boolean in it, old
            if (LocalSetting.Values.Boolean == null)
            {
                LocalSetting.Reset();
                LocalSetting.Update();
            }

            string desktopName = Desktop.GetDesktopName();
            if (desktopName != null)
            {
                if (LocalSetting.Values.Boolean["KioskMode"] &&
                    desktopName.Contains("Default"))
                {
                    // Relaunch application on the TemposDesktop
                    {
                        TemposDesktop = Desktop.CreateDesktop("TemposDesktop");
                        try
                        {
                            Process protectedProcess =
                                TemposDesktop.CreateProcess(ResourceAssembly.Location);
                            TemposDesktop.Show();
                            protectedProcess.WaitForExit();
                            //TemposDesktop.Close(false);
                        }
                        catch (Exception ex)
                        {
                            if (TemposDesktop != null)
                                TemposDesktop.Close();
                            MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                        Current.Shutdown();
                        return;
                    }
                }

                if (desktopName.Contains("TemposDesktop"))
                {
                    IsUsingDesktop = true;
                    TemposDesktop = Desktop.GetCurrent();
                }
            }
            if (((desktopName == null) || desktopName.Contains("Default")) &&
                ((e.Args.Length != 1) || !e.Args[0].ToUpper().Equals("/RESTART")))
            {
                if (!IsOnlyProcess())
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }

#if !DEMO
            Updater.RemoveUpdateDirectory();
#else
            // Just show the Strings.SystemSettings dialog and exit
            if (Keyboard.GetKeyStates(Key.LeftCtrl) != KeyStates.Down) return;
            StartupWindow.PromptForConnectionString(true);
            Process.GetCurrentProcess().Kill();
#endif
        }

        private bool IsOnlyProcess()
        {
            // This works, where Muxtex didn't with XP
            if (GetDuplicateProcesses() != null)
                return false;

            // Mutux way to check for multiple instances
            if (!IsFirstInstance())
                return false;

            return true;
        }

        [Obfuscation(Exclude = true)]
        protected override void OnExit(ExitEventArgs e)
        {
            e.ApplicationExitCode = ExitCode;
            base.OnExit(e);
            Environment.Exit(ExitCode);
        }

        [Obfuscation(Exclude = true)]
        void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            IsAppShuttingDown = true;
        }

        public static void ShutdownApplication(bool showTaskbar = true)
        {
            IsAppShuttingDown = true;
            LocalSetting.Update();
#if !DEMO
            if (LocalSetting.Values.String["StartBroadcastServer"] != null)
                BroadcastServerSocket.Stop();
            BroadcastClientSocket.Stop();
#endif
            if (TaskManagerServiceHelper.IsStarted)
                TaskManagerServiceHelper.IsTaskManagerDisabled = false;

            if (ConfigurationManager.UseKeyboardHook)
                UserControlManager.Disable();
            if (showTaskbar)
                UserControlManager.ShowTaskbar(true);
            Logger.WriteLog("Exiting application");
            Logger.CloseLog();
            TemPOS.MainWindow.Singleton.AllowClose = true;
            TemPOS.MainWindow.Singleton.Dispatcher.Invoke((Action)(() =>
            {
                TemPOS.MainWindow.Singleton.Closed += SingletonClosed;
                TemPOS.MainWindow.Singleton.Close();
            }));
        }

        [Obfuscation(Exclude = true)]
        static void SingletonClosed(object sender, EventArgs e)
        {
            if (TemposDesktop != null)
                TemposDesktop.Close();
            if (SwitchToDefaultDesktopOnClose)
                Desktop.Default.Show();
        }

#if TEST
        private void WaitForListeningPort()
        {
            // Key: HKEY_LOCAL_MACHINE\
            // Name: TcpPort
            // Value: REG_SZ (string)
            string ipAddress = RegistryHelper.GetRegistry(
                @"SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQLServer\SuperSocketNetLib\Tcp\IP1",
                "IpAddress");
            string portString = RegistryHelper.GetRegistry(
                @"SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQLServer\SuperSocketNetLib\Tcp\IP1",
                "TcpPort");
            int port = Convert.ToInt32(portString);
            bool isConnected = false;
            while (!isConnected && !string.IsNullOrEmpty(LocalSetting.ConnectionString))
            {
                using (SqlConnection cn = new SqlConnection(LocalSetting.ConnectionString + ";Connection Timeout=1"))
                {
                    try
                    {
                        
                        cn.Open();
                        cn.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }
#endif

        private static Process GetDuplicateProcesses()
        {
            Process[] processes = Process.GetProcesses();
            //string currentProcessName = Process.GetCurrentProcess().MainModule.FileName;
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            foreach (Process process in processes)
            {
                try
                {
                    //if (process.MainModule.FileName.Equals(currentProcessName))
                    if (process.ProcessName.Equals(currentProcessName))
                    {
                        if (process.Id == Process.GetCurrentProcess().Id)
                            continue;
                        return process;
                    }
                }
                catch (Win32Exception)
                {

                }
            }
            return null;
        }

        private static bool IsFirstInstance()
        {
            // The name of the mutex is to be prefixed with Local\ to make
            // sure that its is created in the per-session namespace,
            // not in the global namespace.
            string mutexName = "Local\\" +
              System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            Mutex mutex = null;
            try
            {
                // Create a new mutex object with a unique name
                _newMutexCreated = false;
                mutex = new Mutex(false, mutexName, out _newMutexCreated);
            }
            catch (Exception)
            {
                return false;
            }

            // When the mutex is created for the first time
            // we run the program since it is the first instance.
            if (_newMutexCreated)
            {
                // Set task priority to realtime
                Process thisProc = Process.GetCurrentProcess();
                thisProc.PriorityClass = ProcessPriorityClass.AboveNormal;
                return true;
            }

            return false;
        }

#if SPEECH_COMMANDS
        private static SpeechRecognizer rec = new SpeechRecognizer();
        public static SpeechRecognizer SpeechRecognizer
        {
            get { return rec; }
        }
#endif

#if SPEECH_COMMANDS
        public static void DisableSpeechRecognizer()
        {
            App.SpeechRecognizer.Enabled = false;
        }

        public static void InitializeSpeechRecognizerChoices(Choices c)
        {
            var gb = new GrammarBuilder(c);
            var g = new Grammar(gb);
            App.SpeechRecognizer.LoadGrammar(g);
            App.SpeechRecognizer.Enabled = true;
        }
#endif
    }
}
