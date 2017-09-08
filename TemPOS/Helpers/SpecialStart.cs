#if DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using PosControls;
using PosControls.Types;
using PosModels.Helpers;
using PosModels;
using System.Data.SqlTypes;
using PosModels.Types;
using TemPOS.Networking;
using TemposLibrary;

namespace TemPOS.Helpers
{
    // OOP approach to circumventing normal application startup for debugging purposes
    public static class SpecialStart
    {
        /// <summary>
        /// True if SpecialStart is enabled
        /// </summary>
        public static bool IsEnabled
        {
            get;
            private set;
        }

        static SpecialStart()
        {
            IsEnabled = false;
            _window = new Window
            {
                Width = 500,
                Height = 300,
                ShowInTaskbar = false
            };
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

        public static void Uninstall(string serviceName)
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
                            if (num3 > 0)
                            {
                                Thread.Sleep(1000);
                                serviceController.Refresh();
                                --num3;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
            }
            Thread.Sleep(5000);
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

        //static Label label = new Label();
        //static CustomTextBox textBox = new CustomTextBox();
        public static void Start()
        {
            if (!VistaSecurity.IsAdmin)
            {
                //VistaSecurity.RestartElevated(null);
                //return;
            }

            //ShowWpfWindow();
            WriteConsole("App.IsInstalled = " + IsApplicationInstalled + Environment.NewLine);
            WriteConsole("Service.IsInstalled = " + TaskManagerServiceHelper.IsInstalled + Environment.NewLine);

            //WritePendingFileRenameOperations();
            TaskManagerServiceHelper.InstallFailed += (sender, args) =>
            {
                WriteConsole("Installation Failed!" + Environment.NewLine);
            };

            TaskManagerServiceHelper.Installed += (sender, args) =>
            {
                WriteConsole("Installed!" + Environment.NewLine);
            };

            TaskManagerServiceHelper.UninstallFailed += (sender, args) =>
            {
                WriteConsole("Uninstallation Failed!" + Environment.NewLine);
            };

            TaskManagerServiceHelper.Uninstalled += (sender, args) =>
            {
                WriteConsole("Uninstalled!" + Environment.NewLine);
            };

            TaskManagerServiceHelper.StartFailed += (sender, args) =>
            {
                WriteConsole("Start Failed!" + Environment.NewLine);
            };

            TaskManagerServiceHelper.Started += (sender, args) =>
            {
                WriteConsole("Started!" + Environment.NewLine);
            };

            //RemoveServiceFilesOnReboot();
            //Install();
            //Uninstall("Task Manager Access");
            UnInstall();
            //SendMessgage("true");


            //TaskManagerServiceHelper.Start();

            //ServiceController service = new ServiceController("Task Manager Access");
            //WriteConsole("Status = " + service.Status + Environment.NewLine);
            

            ShowConsole();

            //string text =
            //    DataModelBase.GetAssemblyHexString(
            //        @"D:\Viipe.com\PointOfSale\PointOfSale\bin\Release_Demo\TemposProcs.dll");
            //WriteConsole(text);

            //DataModelBase.ValidateDatabase();

            /*
            var modelEntries = DataModelBase.GetModelClasses().SelectMany(DataModelBase.GetModeledDataProperties).ToList();
            modelEntries.Sort();

            var databaseEntries = DataModelBase.GetTableColumnTypes().ToList();
            databaseEntries.Sort();

            foreach (string entry in modelEntries)
            {
                if (!databaseEntries.Contains(entry))
                    WriteConsole("[Missing Model Entry]: " + entry + Environment.NewLine);
            }
            WriteConsole(Environment.NewLine);
            foreach (string entry in databaseEntries)
            {
                if (!modelEntries.Contains(entry))
                    WriteConsole("[Missing Database Entry]: " + entry + Environment.NewLine);
            }
            ShowConsole();
            */

            //DoLanguageTranslations();


            /*
            StringsCore.LanguageChanged += (sender, args) =>
            {
                MessageBox.Show("Language Changed to " + StringsCore.Language.ToString());
            };
            StringsCore.SetDefaultLanguage();             
            //StringsCore.Language = PosModels.Types.Languages.Spanish;
            MessageBox.Show(PointOfSale.Types.Strings.Yes);
            MessageBox.Show(PosModels.Types.Strings.DayMonday);
             */
            //AllTicketItems();
            //CompareModelAndTables();
            //WindowsInstallerCustomAction.FinishInstall();
            //if (Updater.InstallSQLAssembly())
            //    MessageBox.Show("Updated");
            //else
            //    MessageBox.Show("Not Updated");
        }

        private static void Install()
        {
            if (!TaskManagerServiceHelper.IsInstalled)
            {
                WriteConsole("Installing..." + Environment.NewLine);
                TaskManagerServiceHelper.Install();
            }
        }

        private static void UnInstall()
        {
            if (TaskManagerServiceHelper.IsInstalled)
            {
                WriteConsole("Uninstalling..." + Environment.NewLine);
                TaskManagerServiceHelper.Uninstall();
            }
        }

        private static void SendMessgage(string message)
        {
            PipeClient pipeClient = new PipeClient("TaskManagerAccessService", ".");

            try
            {
                pipeClient.SendMessage(message);
            }
            catch (Exception ex)
            {
                WriteConsole("[Exception]: " + ex.Message +
                             Environment.NewLine + ex.StackTrace + Environment.NewLine);
            }
        }

        private static void ShowWpfWindow()
        {
            SpecialStartControl control = new SpecialStartControl();
            PosDialogWindow window = new PosDialogWindow(control, "Special Start", 835, 435);
            window.ShowDialog();
        }

        private static TextBox _textConsole;
        private static Window _window;
        public static void WriteConsole(string text)
        {
            if (!_window.Dispatcher.CheckAccess())
                _window.Dispatcher.Invoke((Action)(() => WriteConsole(text)));
            else
            {
                if (_textConsole == null) CreateTextConsole();
                _textConsole.AppendText(text);
            }
        }

        private static void ShowConsole()
        {
            if (_textConsole == null) CreateTextConsole();
            _window.Topmost = true;
            _window.ShowDialog();
        }

        private static void CreateTextConsole()
        {
            _textConsole = new TextBox();
            _textConsole.TextWrapping = TextWrapping.Wrap;
            _window.Content = _textConsole;
        }

        /*
        private static void AllTicketItems()
        {
            DragScrollListBox listBox = new DragScrollListBox();
            TicketItemTemplate item = null;
            foreach (TicketItem ticketItem in TicketItem.GetAll(
                SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, true))
            {
                item = new TicketItemTemplate(ticketItem, BranchType.None);
                listBox.Items.Add(item);
                item = new TicketItemTemplate(ticketItem, BranchType.Middle);
                listBox.Items.Add(item);
                item = new TicketItemTemplate(ticketItem, BranchType.Last);
                listBox.Items.Add(item);
                break;
            }
            PosDialogWindow window = new PosDialogWindow(listBox, "Test", 380, 650);
            window.ShowDialog();
        }

        private static void BrushSelectTest()
        {
            ConfigurationManager.AlwaysUseDefaults = false;

            BrushSelectBox control = new BrushSelectBox();
            control.Width = 120;
            control.SelectedBrush = ConfigurationManager.TextboxBackgroundBrush;

            GradientBrush test = control.SelectedBrush as GradientBrush;

            PosDialogWindow window = new PosDialogWindow(control, "Brush Select Test", 200, 140);
            window.ShowDialog();
        }

        public static void DebugCustomTextBox()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;

            textBox.UseContextMenuEditing = true;
            textBox.PromptType = CustomTextBoxType.Keyboard;
            textBox.SetNotifyCaretUpdate(new EventHandler(CheckIt));
            textBox.Text = "124533675950";

            label.Content = "Caret Index = " + textBox.CaretIndex;
            label.Foreground = Brushes.White;

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);

            PosDialogWindow window =
                new PosDialogWindow(stackPanel, "Special Start", 200, 115);
            window.ShowDialog();
        }

        private static void CheckIt(object sender, EventArgs args)
        {
            label.Content = "Caret Index = " + textBox.CaretIndex + ". " + DateTime.Now.Millisecond;
        }

        public static void CompareModelAndTables()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            List<string> modelEntries = new List<string>();
            foreach (Type t in DatabaseHelper.GetModelClasses())
            {
                foreach (string propName in DatabaseHelper.GetModeledDataProperties(t))
                {
                    modelEntries.Add(propName);
                }
            }
            modelEntries.Sort();

            List<string> databaseEntries = new List<string>();
            foreach (string columnName in DatabaseHelper.GetTableColumnTypes())
            {
                string[] tokens = columnName.Split('.');
                string value = columnName.Replace("." + tokens[0], ".").Trim();
                databaseEntries.Add(value);
            }
            databaseEntries.Sort();

            TextBlock textBlock1 = new TextBlock();
            textBlock1.Foreground = Brushes.White;
            textBlock1.Text = "";
            textBlock1.Margin = new Thickness(0, 0, 2, 0);
            foreach (string modelEntry in modelEntries)
            {
                if (!databaseEntries.Contains(modelEntry))
                    textBlock1.Text += modelEntry + Environment.NewLine;
            }

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Foreground = Brushes.White;
            textBlock2.Text = "";
            textBlock2.Margin = new Thickness(2, 0, 0, 0);
            foreach (string databaseEntry in databaseEntries)
            {
                if (!modelEntries.Contains(databaseEntry))
                    textBlock2.Text += databaseEntry + Environment.NewLine;
            }

            stackPanel.Children.Add(textBlock1);
            stackPanel.Children.Add(textBlock2);

            DragScrollViewer control = new DragScrollViewer();
            control.ScrollContent = stackPanel;

            PosDialogWindow window = new PosDialogWindow(control, "Debug", 700, 500);
            window.ShowDialog();
        }

        public static void OldStart1()
        {
            //LocalSetting.Initialize();
            //LocalSetting.Values["UpdateServer"] = "localhost";
            //LocalSetting.Values["UpdateServerPort"] = "43333";
            //LocalSetting.Update();
        }

        public static void OldStart2()
        {
            //string message = "This is going to be a really long dialog window control that you are about to see, or should I say, you are seeing right. Thanks and enjoy the rest of you day.";
            //string message = "This is going to be a short dialog window, but not too short A.";
            //MessageBox.Show(message, "Test");
            //PosDialogWindow.ShowDialog(null, message, "Test");
        }
        */
    }
}
#endif
