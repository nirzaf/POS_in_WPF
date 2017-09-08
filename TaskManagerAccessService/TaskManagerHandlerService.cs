using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using Microsoft.Win32;

namespace TaskManagerAccessService
{
    public partial class TaskManagerHandlerService : ServiceBase
    {
        private PipeServer _pipeServer = new PipeServer("TaskManagerAccessService");

        public TaskManagerHandlerService()
        {
            InitializeComponent();
            _pipeServer.MessageReceived += _pipeServer_MessageReceived;
        }

        private void SelfUninstall()
        {
            // Do OnStop()
            OnStop();

            // Uninstall
            string assemblyPath = Assembly.GetExecutingAssembly().Location;            
            ManagedInstallerClass.InstallHelper(new[] { "/u", assemblyPath });

            // Exit
            Process.GetCurrentProcess().Kill();
        }

        void _pipeServer_MessageReceived(object sender, TextEventArgs args)
        {
            if (args.Text == null) return;
            if (args.Text.ToLower().StartsWith("uninstall"))
                SelfUninstall();
            else
                SetTaskManagerEnabled(!args.Text.ToLower().StartsWith("false"));
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _pipeServer.Start();
        }

        protected override void OnStop()
        {
            _pipeServer.Stop();
            SetTaskManagerEnabled(true);
            base.OnStop();
        }

        /// <summary>
        /// For this to work, The Local Service princpal needs permission to modify the key
        /// </summary>
        /// <param name="isEnabled"></param>
        public static void SetTaskManagerEnabled(bool isEnabled)
        {
            // HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System\DisableTaskMgr
            // DWORD: 0 = Enabled, 1 = Disabled
            foreach (string subKeyName in Registry.Users.GetSubKeyNames())
            {
                try
                {
                    SetRegistry(subKeyName + @"\Software\Microsoft\Windows\CurrentVersion\Policies\System",
                                "DisableTaskMgr", (isEnabled ? 0 : 1));
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public static void SetRegistry(string subKey, string keyName, int value)
        {
            RegistryKey rkey1 = Registry.Users.OpenSubKey(subKey, true);
            if (rkey1 == null) return;
            rkey1.SetValue(keyName, value);
            rkey1.Close();
        }

    }
}
