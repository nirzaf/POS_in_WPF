using System;
using System.Reflection;
using System.Windows;
using PosModels;
using TemPOS.Commands;
using TemPOS.Helpers;
using TemPOS.Types;
using TemposLibrary.Win32;
using TemposLibrary;
using System.Threading;
using PosControls;
using PosControls.Interfaces;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ExitControl.xaml
    /// </summary>
    public partial class ExitControl
    {
        private ActionNotificationControl _notification;

        public ExitControl()
        {
            InitializeComponent();
        }

        private void ServiceStopThreadStart(object threadObject)
        {
            var thread = threadObject as Thread;
            if (thread == null) return;
            VistaSecurity.RestartElevated("/STOPSQL", true);
            Thread.Sleep(500);
            Dispatcher.Invoke((Action) (() =>
            {
                _notification.Close();
                if (!ServiceHelper.IsSqlBrowserServiceRunningLocally)
                {
                    App.SwitchToDefaultDesktopOnClose = true;
                    App.ShutdownApplication();
                }
                else
                {
                    Window window = Window.GetWindow(this);
                    if (window != null) window.Close();
                }
            }));
            thread.Abort();
        }

        [Obfuscation(Exclude = true)]
        private void buttonLockWorkstation_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            User32.LockWorkStation();
        }

        [Obfuscation(Exclude = true)]
        private void buttonLogoff_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            User32.ExitWindowsEx(0, 0);
            App.SwitchToDefaultDesktopOnClose = true;
            App.ShutdownApplication(false);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonShutdown_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            User32.ExitWindowsEx(1, 0);
            App.SwitchToDefaultDesktopOnClose = true;
            App.ShutdownApplication(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            User32.ExitWindowsEx(2, 0);
            App.SwitchToDefaultDesktopOnClose = true;
            App.ShutdownApplication(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonHibernate_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            Powrprof.SetSuspendState(true, true, false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonSuspend_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            Powrprof.SetSuspendState(false, true, false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRestartProgram_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            //Desktop temposDesktop = Desktop.OpenDesktop("TemposDesktop");
            if (App.IsUsingDesktop && !LocalSetting.Values.Boolean["KioskMode"])
            {
                Desktop.Default.CreateProcess(Application.ResourceAssembly.Location, "/RESTART");
                App.SwitchToDefaultDesktopOnClose = true;
            }
            else
            {
                VistaSecurity.Restart("/RESTART");
                App.SwitchToDefaultDesktopOnClose = false;
            }
            App.ShutdownApplication();
        }

        [Obfuscation(Exclude = true)]
        private void buttonExitAll_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            var ownerWindow = (Window.GetWindow(this) as IShadeable);
            _notification = ActionNotificationControl.Create(ownerWindow,
                Strings.ExitStoppingSql, Strings.Notification);
            _notification.Show();
            var thread = new Thread(ServiceStopThreadStart);
            thread.Start(thread);
        }

        [Obfuscation(Exclude = true)]
        private void buttonExitProgram_Click(object sender, RoutedEventArgs e)
        {
            OrderEntryCommands.ExecuteLogoutCommand();
            App.SwitchToDefaultDesktopOnClose = true; 
            App.ShutdownApplication();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            var control = new ExitControl();
            return new PosDialogWindow(control, Strings.ExitExitTemPos, 910, 125);
        }
    }
}
