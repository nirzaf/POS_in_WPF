using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using TemPOS.Networking;
using TemPOS.Types;
using TemposLibrary;
using PosControls;
using System.Reflection;
using System.Windows.Threading;

namespace TemPOS.Helpers
{
    public static class TaskManagerServiceHelper
    {
        private static readonly string InstallFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            Path.DirectorySeparatorChar + "TemPOS" +
            Path.DirectorySeparatorChar + "TaskManagerAccessService.exe";

        private static readonly string InstallLocalFilePath =
            Path.GetDirectoryName(Application.ResourceAssembly.Location) +
            Path.DirectorySeparatorChar + "DataFiles" +
            Path.DirectorySeparatorChar + "TaskManagerAccessService.exe";

        private static readonly string UninstallFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.Programs) +
            Path.DirectorySeparatorChar + "Startup" +
            Path.DirectorySeparatorChar + "TaskManagerAccessServiceUninstaller.exe";

        private static readonly string UninstallLocalFilePath =
            Path.GetDirectoryName(Application.ResourceAssembly.Location) +
            Path.DirectorySeparatorChar + "DataFiles" +
            Path.DirectorySeparatorChar + "TaskManagerAccessServiceUninstaller.exe";

        public static event EventHandler Installed;
        public static event EventHandler InstallFailed;
        public static event EventHandler Uninstalled;
        public static event EventHandler UninstallFailed;
        public static event EventHandler Started;
        public static event EventHandler StartFailed;
        public static event EventHandler Stopped;
        public static event EventHandler StopFailed;

        public static event EventHandler ThreadCompleted;
        public static event EventHandler ThreadFailed;

        static TaskManagerServiceHelper()
        {
            IsInstalled = (File.Exists(InstallFilePath) &&
                           ServiceHelper.IsTaskManagerAccessServiceInstalled);
            if (IsInstalled)
                IsStarted = ServiceHelper.IsTaskManagerAccessServiceRunning;
            string dirPath = Path.GetDirectoryName(InstallFilePath);
            if ((dirPath != null) && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public static bool IsInstalled
        {
            get;
            private set;
        }

        public static bool IsStarted
        {
            get;
            private set;
        }

        public static int FailCode
        {
            get; private set;
        }

        public static bool IsTaskManagerDisabled
        {
            get
            {
                RegistryKey rkey1 = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                int? results = null;
                if (rkey1 != null)
                {
                    results = rkey1.GetValue("DisableTaskMgr") as int?;
                    rkey1.Close();
                }
                return (results.HasValue && (results.Value == 1));
            }
            set
            {
                PipeClient pipeClient = new PipeClient("TaskManagerAccessService", ".");

                try
                {
                    pipeClient.SendMessage(value ? "false" : "true");
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void Install()
        {
            if (IsInstalled) return;
            new Thread(InstallThreadStart).Start();
        }

        public static void Uninstall()
        {
            if (!IsInstalled) return;
            new Thread(UninstallThreadStart).Start();
        }

        public static void Start()
        {
            if (!IsInstalled) return;
            new Thread(StartThreadStart).Start();
        }

        public static bool UnelevatedInstall()
        {
            if (!VistaSecurity.RestartElevated("/INSTALLTMA", true))
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
                {
                    PosDialogWindow.ShowDialog(
                        Strings.FailedToInstallTheTaskManagerAccessService + Assembly.GetExecutingAssembly().Location,
                        Strings.ServiceInstallationError);
                }));
                return false;
            }
            return true;
        }

        public static bool UnelevatedStart()
        {
            if (!VistaSecurity.RestartElevated("/STARTTMA", true))
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
                {
                    PosDialogWindow.ShowDialog(
                        Strings.FailedToStartTheTaskManagerAccessService,
                        Strings.ServiceStartError);
                }));
                return false;
            }
            return true;
        }

        public static void InstallThread()
        {
            if (TaskManagerServiceHelper.UnelevatedInstall())
            {
                TaskManagerServiceHelper.IsTaskManagerDisabled = true;
                TaskManagerServiceHelper.PersistentEnable();
                OnThreadCompleted();
            }
            else
            {
                OnThreadFailed();
            }
        }

        public static void StartThread()
        {
            if (TaskManagerServiceHelper.UnelevatedStart())
            {
                TaskManagerServiceHelper.IsTaskManagerDisabled = true;
                TaskManagerServiceHelper.PersistentEnable();
                OnThreadCompleted();
            }
            else
            {
                OnThreadFailed();
            }
        }

        private static void OnThreadCompleted()
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
            {
                if (ThreadCompleted != null)
                    ThreadCompleted.Invoke(null, new EventArgs());
            }));
        }

        private static void OnThreadFailed()
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
            {
                if (ThreadFailed != null)
                    ThreadFailed.Invoke(null, new EventArgs());
            }));
        }

        public static void PersistentEnable()
        {
            Thread.Sleep(500);
            int retriesRemaining = 20;
            while ((retriesRemaining > 0) && !TaskManagerServiceHelper.IsTaskManagerDisabled)
            {
                retriesRemaining--;
                TaskManagerServiceHelper.IsTaskManagerDisabled = true;
                Thread.Sleep(500);
            }
        }

        public static void Stop()
        {
            if (!IsInstalled) return;
            new Thread(StopThreadStart).Start();
        }

        private static void StartThreadStart()
        {
            if (ServiceHelper.StartTaskManagerAccessService(10000))
            {
                if (Started != null)
                    Started.Invoke(null, new EventArgs());
            }
            else
            {
                if (StartFailed != null)
                    StartFailed.Invoke(null, new EventArgs());
            }
        }

        private static void StopThreadStart()
        {
            if (ServiceHelper.StopTaskManagerAccessService(10000))
            {
                if (Stopped != null)
                    Stopped.Invoke(null, new EventArgs());
            }
            else
            {
                if (StopFailed != null)
                    StopFailed.Invoke(null, new EventArgs());
            }
        }

        private static void InstallThreadStart()
        {
            try
            {
                string dirPath = Path.GetDirectoryName(InstallFilePath);
                if (dirPath == null)
                {
                    FailCode = 1;
                    if (InstallFailed != null)
                        InstallFailed.Invoke(null, new EventArgs());
                    return;
                }
                FailCode = 2;
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                FailCode = 3;
                if (File.Exists(InstallFilePath))
                    File.Delete(InstallFilePath);
                FailCode = 4;
                File.Copy(InstallLocalFilePath, InstallFilePath);
                FailCode = 5;
                if (File.Exists(UninstallFilePath))
                    File.Delete(UninstallFilePath);
                FailCode = 6;
                File.Copy(UninstallLocalFilePath, UninstallFilePath);
                FailCode = 7;
            }
            catch (Exception)
            {
                if (InstallFailed != null)
                    InstallFailed.Invoke(null, new EventArgs());
                return;
            }
            IsInstalled = InstallUtil(true);
            FailCode = 8;
            if (IsInstalled && (Installed != null))
                Installed.Invoke(null, new EventArgs());
            else if (InstallFailed != null)
                InstallFailed.Invoke(null, new EventArgs());
        }

        private static void UninstallThreadStart()
        {
            IsInstalled = !InstallUtil(false);
            if (!IsInstalled)
            {
                try
                {
                    string dirPath = Path.GetDirectoryName(InstallFilePath);
                    if (dirPath != null)
                    {
                        foreach (string file in Directory.GetFiles(dirPath))
                        {
                            File.Delete(file);
                        }
                        Directory.Delete(dirPath);
                    }
                }
                catch (Exception)
                {
                    if (UninstallFailed != null)
                        UninstallFailed.Invoke(null, new EventArgs());
                    return;
                }
                try
                {
                    if (File.Exists(UninstallFilePath))
                        File.Delete(UninstallFilePath);
                }
                catch (Exception)
                {
                    if (UninstallFailed != null)
                        UninstallFailed.Invoke(null, new EventArgs());
                    return;
                }
                if (Uninstalled != null)
                    Uninstalled.Invoke(null, new EventArgs());
            }
            else if (UninstallFailed != null)
                UninstallFailed.Invoke(null, new EventArgs());
        }

        private static bool InstallUtil(bool install)
        {
            string installUtilPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
            return VistaSecurity.StartElevated(installUtilPath, (install ? "/i" : "/u") +
                " \"" + InstallFilePath + "\"", true, true);
        }
    
    }
}
