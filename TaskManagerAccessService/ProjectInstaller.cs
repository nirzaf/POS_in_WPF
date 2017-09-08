using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;


namespace TaskManagerAccessService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        [Flags]
        public enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }
        
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, int access);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr databaseHandle, string serviceName, int access);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DeleteService(IntPtr serviceHandle);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CloseServiceHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName,
           MoveFileFlags dwFlags);

        public static void RemoveServiceFilesOnReboot()
        {
            foreach (string fileName in Directory.GetFiles(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                Path.DirectorySeparatorChar + "TemPOS"))
            {
                MoveFileEx(fileName, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
            }
        }

        public static void WritePendingFileRenameOperations()
        {
            RegistryKey rkey1 = Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Control\Session Manager");
            if (rkey1 == null) return;
            var values = rkey1.GetValue("PendingFileRenameOperations") as string[];
            rkey1.Close();
            List<string> regStrings = (values != null) ?
                new List<string>(values) : new List<string>();

            foreach (string fileName in Directory.GetFiles(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                Path.DirectorySeparatorChar + "TemPOS"))
            {
                regStrings.Add(@"\\??\\" + fileName.Replace(@"\\", @"\").Replace(@"\", @"\\"));
                regStrings.Add("");
            }

            rkey1 = Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Control\Session Manager", true);
            if (rkey1 == null) return;
            rkey1.SetValue("PendingFileRenameOperations", regStrings.ToArray());
            rkey1.Close();
        }

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

        /// <summary>
        /// This works executed as a user, but not as a service.
        /// </summary>
        /// <param name="serviceName"></param>
        private static void Uninstall(string serviceName)
        {
            IntPtr num1 = OpenSCManager(null, null, 983103);
            if (num1 == IntPtr.Zero)
                throw new Win32Exception();
            IntPtr num2 = IntPtr.Zero;
            try
            {
                num2 = OpenService(num1, serviceName, 65536);
                if (num2 == IntPtr.Zero)
                    throw new Win32Exception();
                DeleteService(num2);
            }
            finally
            {
                if (num2 != IntPtr.Zero)
                    CloseServiceHandle(num2);
                CloseServiceHandle(num1);
            }
            try
            {
                using (var serviceController = new ServiceController(serviceName))
                {
                    if (serviceController.Status != ServiceControllerStatus.Stopped)
                    {
                        serviceController.Stop();
                        int num3 = 10;
                        serviceController.Refresh();
                        while (serviceController.Status != ServiceControllerStatus.Stopped)
                        {
                            if (num3 <= 0)
                                break;
                            Thread.Sleep(1000);
                            serviceController.Refresh();
                            --num3;
                        }
                    }
                }
            }
            catch
            {
            }
            Thread.Sleep(5000);
        }

        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
