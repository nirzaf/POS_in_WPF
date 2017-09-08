using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TaskManagerAccessServiceUninstaller
{
    static class Program
    {
        internal static bool IsApplicationInstalled
        {
            get
            {
                string rootPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey rkey1 = Registry.CurrentUser.OpenSubKey(rootPath);
                bool results = false;
                if (rkey1 != null)
                {
                    foreach (string subKeyName in rkey1.GetSubKeyNames())
                    {
                        string path = rootPath + @"\" + subKeyName;
                        RegistryKey rkey2 = Registry.CurrentUser.OpenSubKey(path);
                        if (rkey2 == null) continue;
                        var displayName = rkey2.GetValue("DisplayName") as string;
                        rkey2.Close();
                        if ((displayName == null) || !displayName.Equals("TemPOS")) continue;
                        results = true;
                        break;
                    }
                    rkey1.Close();
                }
                return results;
            }
        }

        internal static bool IsServiceInstalled
        {
            get
            {
                string rootPath = @"SYSTEM\CurrentControlSet\Services\Task Manager Access";
                RegistryKey rkey1 = Registry.LocalMachine.OpenSubKey(rootPath);
                bool results = false;
                if (rkey1 != null)
                {
                    results = true;
                    rkey1.Close();
                }
                return results;
            }
        }

        private static readonly string UninstallFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.Programs) +
            Path.DirectorySeparatorChar + "Startup" +
            Path.DirectorySeparatorChar + "TaskManagerAccessServiceUninstaller.exe";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if ((args != null) && (args.Length == 1) && args[0].ToUpper().Equals("/UNINSTALL"))
            {
                // Delete Original
                if (File.Exists(UninstallFilePath))
                    File.Delete(UninstallFilePath);

                // Self Destruct!
                VistaSecurity.StartProgram(
                    Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) +
                    Path.DirectorySeparatorChar + "cmd.exe",
                    "/C choice /C Y /N /D Y /T 3 & Del /Q " +
                    Assembly.GetExecutingAssembly().Location,
                    false, false, true);                
            }
            else if (IsServiceInstalled)
            //else if (IsServiceInstalled && !IsApplicationInstalled)
            {
                // This is the start now, need to send an "uninstall" pipe message to the 
                // Task Manager Access service. It will uninstall itself.
                int waitRetries = 30;
                var pipeClient = new PipeClient("TaskManagerAccessService", ".");
                try
                {
                    pipeClient.SendMessage("uninstall");
                    while (IsServiceInstalled && waitRetries > 0)
                    {
                        waitRetries--;
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Task Manager Acccess Service Uninstaller");
                }
                if (!IsServiceInstalled)
                    RemoveFiles();
            }
            else if (!IsServiceInstalled && !IsApplicationInstalled)
            {
                RemoveFiles();
            }
            Application.Exit();
        }

        private static void RemoveFiles()
        {
            UninstallTaskManagerAccessService();
            string tempFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                  Path.DirectorySeparatorChar + "TaskManagerAccessServiceUninstaller.exe";
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
            File.Copy(Assembly.GetExecutingAssembly().Location, tempFilePath);
            VistaSecurity.StartProgram(tempFilePath, "/UNINSTALL", false, false);
        }

        private static void UninstallTaskManagerAccessService()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                               Path.DirectorySeparatorChar + "TemPOS";
            if (!Directory.Exists(directory)) return;
            foreach (string fileName in Directory.GetFiles(directory))
            {
                File.Delete(fileName);
            }
            Directory.Delete(directory);
        }
    }
}
