using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Managers;
using PosControls.Helpers;
using TemPOS.Networking;
using PosControls;
using PosModels.Managers;
using PosModels;
using System.Threading;
using TemPOS.Helpers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for GeneralSettingsGeneralPreferencesControl.xaml
    /// </summary>
    public partial class GeneralSettingsGeneralPreferencesControl : UserControl
    {
        public GeneralSettingsGeneralPreferencesControl()
        {
            InitializeComponent();
            InitializeCurrentValues();
#if DEMO
            groupBoxBroadcastServer.Visibility = Visibility.Collapsed;            
            radioButtonIsEnabled.IsEnabled = false;
            radioButtonIsNotEnabled.IsEnabled = false;
            radioButtonIsNotEnabled.IsSelected = true;
            textBoxPort.IsEnabled = false;
            buttonTest.IsEnabled = false;
#else
            BroadcastServerSocket.StartedListening += BroadcastServerSocket_StartedListening;
#endif
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void BroadcastServerSocket_StartedListening(object sender, EventArgs e)
        {
            if (BroadcastClientSocket.IsConnected)
                BroadcastClientSocket.Stop();
            BroadcastClientSocket.Start();
        }
#endif

        private void InitializeCurrentValues()
        {
            radioButtonIsEnabled.SelectionGained -= radioButtonIsEnabled_SelectionGained;
            radioButtonIsNotEnabled.SelectionGained -= radioButtonIsNotEnabled_SelectionGained;
            textBoxPort.Text = "";
#if !DEMO
            int? port = BroadcastServerSocket.Port;
            if (port != null)
              textBoxPort.Text += BroadcastServerSocket.Port;
            radioButtonIsNotEnabled.IsSelected =
                !(radioButtonIsEnabled.IsSelected =
                BroadcastServerSocket.IsEnabled);
            buttonTest.IsEnabled = radioButtonIsEnabled.IsSelected &&
                !string.IsNullOrEmpty(textBoxPort.Text);
            textBoxPort.TextChanged += textBoxPort_TextChanged;
            radioButtonIsEnabled.SelectionGained += radioButtonIsEnabled_SelectionGained;
            radioButtonIsNotEnabled.SelectionGained += radioButtonIsNotEnabled_SelectionGained;
#endif
            radioButtonIsUsingSeating.IsSelected = ConfigurationManager.UseSeating;
            radioButtonIsNotUsingSeating.IsSelected = !ConfigurationManager.UseSeating;
            radioButtonIsForceVoidWaste.IsSelected = ConfigurationManager.ForceWasteOnVoid;
            radioButtonIsNotForceVoidWaste.IsSelected = !ConfigurationManager.ForceWasteOnVoid;

            radioButtonIsLogoutOnPlaceOrder.IsSelected = ConfigurationManager.LogoutOnPlaceOrder;
            radioButtonIsNotLogoutOnPlaceOrder.IsSelected = !ConfigurationManager.LogoutOnPlaceOrder;

            radioButtonIsNotKeyboardRestricted.IsSelected = !ConfigurationManager.UseKeyboardHook;
            radioButtonIsKeyboardRestricted.IsSelected = ConfigurationManager.UseKeyboardHook;

            radioButtonIsUsingKioskMode.IsSelected = LocalSetting.Values.Boolean["KioskMode"];
            radioButtonIsNotUsingKioskMode.IsSelected = !LocalSetting.Values.Boolean["KioskMode"];

            StoreSetting setting = SettingManager.GetStoreSetting("DisableAutoLogoff");
            bool autoLogoutIsEnabled =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonYesAutoLogoutEnabled.IsSelected = autoLogoutIsEnabled;
            radioButtonNoAutoLogoutEnabled.IsSelected = !autoLogoutIsEnabled;

            setting = SettingManager.GetStoreSetting("EnableAutoLogoffWhileInOrderEntry");
            bool disableWhileInOrderEntry =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonYesDisableAutoLogoutInOrderEntry.IsSelected = disableWhileInOrderEntry;
            radioButtonNoDisableAutoLogoutInOrderEntry.IsSelected = !disableWhileInOrderEntry;

            setting = SettingManager.GetStoreSetting("EnableAutoLogoffInPasswordChange");
            bool disableInPasswordChange =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonYesDisableAutoLogoutForPasswordChanges.IsSelected = disableInPasswordChange;
            radioButtonNoDisableAutoLogoutForPasswordChanges.IsSelected = !disableInPasswordChange;

            setting = SettingManager.GetStoreSetting("EnableAutoLogoffForDialogWindows");
            bool disableForDialogWindows =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonYesDisableAutoLogoutForDialogWindows.IsSelected = disableForDialogWindows;
            radioButtonNoDisableAutoLogoutForDialogWindows.IsSelected = !disableForDialogWindows;

            setting = SettingManager.GetStoreSetting("AutoLogoffTimeout");
            if ((setting.IntValue == null) || (setting.IntValue.Value < 5))
                textBoxAutoLogoutTimeout.Text = "" + 20; // default is 20
            else
                textBoxAutoLogoutTimeout.Text = "" + setting.IntValue.Value.Clamp(5, 9999);

            setting = SettingManager.GetStoreSetting("ShowWeather");
            bool disableWeather =
                ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonWeatherIsEnabled.IsSelected = !disableWeather;
            radioButtonWeatherIsNotEnabled.IsSelected = disableWeather;

            setting = SettingManager.GetStoreSetting("WeatherZipCode");
            if ((setting.IntValue == null) || (setting.IntValue.Value == 0))
                textBoxWeatherZipCode.Text = "";
            else
                textBoxWeatherZipCode.Text = "" + setting.IntValue.Value;

            setting = SettingManager.GetStoreSetting("DontDisableTaskManager");
            bool dontDisableTaskManager = ((setting.IntValue == null) || (setting.IntValue.Value == 0));
            radioButtonIsBlockTaskManager.IsEnabled = !dontDisableTaskManager;
            radioButtonIsNotBlockTaskManager.IsEnabled = dontDisableTaskManager;

            SetAutoLogoutEnabledControls();
            SetWeatherControls();
        }

        private int? GetPort()
        {
            try
            {
                int port = Convert.ToInt32(textBoxPort.Text).Clamp(1, 65323);
                return port;
            }
            catch
            {
                return null;
            }
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        private void textBoxPort_TextChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
#endif

        [Obfuscation(Exclude = true)]
        private void radioButtonIsEnabled_SelectionGained(object sender, EventArgs e)
        {
#if !DEMO
            radioButtonIsNotEnabled.IsSelected = false;
            BroadcastServerSocket.IsEnabled = true;
            if (BroadcastServerSocket.Port != null)
                BroadcastServerSocket.Start();
#endif
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotEnabled_SelectionGained(object sender, EventArgs e)
        {
#if !DEMO
            radioButtonIsEnabled.IsSelected = false;
            BroadcastServerSocket.IsEnabled = false;
            if (BroadcastServerSocket.IsRunning)
                BroadcastServerSocket.Stop();
#endif
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void BroadcastClientSocket_ReceivedMessage(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                string message = (string)sender;
                if (message.Equals(Strings.Test))
                {
                    BroadcastClientSocket.ReceivedMessage -=
                        BroadcastClientSocket_ReceivedMessage;
                    PosDialogWindow.ShowDialog(
                        Strings.TestCompletedSuccessfully, Strings.TestPassed);
                }
            }));
        }
#endif

        [Obfuscation(Exclude = true)]
        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
#if !DEMO
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (BroadcastClientSocket.IsConnected &&
                    BroadcastServerSocket.IsRunning)
                {
                    BroadcastClientSocket.ReceivedMessage +=
                        BroadcastClientSocket_ReceivedMessage;
                    BroadcastServerSocket.BroadcastMessage(Strings.Test);
                }
                else if (!BroadcastServerSocket.IsRunning)
                {
                    PosDialogWindow.ShowDialog(
                        Strings.ServerIsNotRunning, Strings.TestFailed);
                }
                else // if (!BroadcastClientSocket.IsConnected)
                {
                    PosDialogWindow.ShowDialog(
                        Strings.ClientIsNotRunning, Strings.TestFailed);
                }
            }));
#endif
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsUsingSeating_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetUseSeating(true);
            radioButtonIsNotUsingSeating.IsSelected = false;
        }
        
        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotUsingSeating_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetUseSeating(false);
            radioButtonIsUsingSeating.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsForceVoidWaste_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetForceWasteOnVoid(true);
            radioButtonIsNotForceVoidWaste.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotForceVoidWaste_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetForceWasteOnVoid(false);
            radioButtonIsForceVoidWaste.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsKeyboardRestricted_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetUseKeyboardHook(true);
#if !DEBUG
            UserControlManager.Disable();
            UserControlManager.Enable(true);
#endif
            radioButtonIsNotKeyboardRestricted.IsSelected = false;
        }
        
        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotKeyboardRestricted_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetUseKeyboardHook(false);
#if !DEBUG
            UserControlManager.Disable();
            UserControlManager.Enable(false);
#endif
            radioButtonIsKeyboardRestricted.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonYesAutoLogoutEnabled_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("DisableAutoLogoff", 0);
            LoginControl.StartAutoLogoutTimer();
            radioButtonNoAutoLogoutEnabled.IsSelected = false;
            SetAutoLogoutEnabledControls();
        }

        private void SetAutoLogoutEnabledControls()
        {
            textBoxAutoLogoutTimeout.IsEnabled =
                radioButtonYesDisableAutoLogoutInOrderEntry.IsEnabled =
                radioButtonNoDisableAutoLogoutInOrderEntry.IsEnabled =
                radioButtonYesDisableAutoLogoutForDialogWindows.IsEnabled =
                radioButtonNoDisableAutoLogoutForDialogWindows.IsEnabled =
                radioButtonYesDisableAutoLogoutForPasswordChanges.IsEnabled =
                radioButtonNoDisableAutoLogoutForPasswordChanges.IsEnabled =
                labelAutoLogoutTimeOut.IsEnabled =
                labelAutoLogoutDisable.IsEnabled =
                labelAutoLogoutDisableOrder.IsEnabled =
                labelAutoLogoutDisableDialog.IsEnabled =
                labelAutoLogoutDisablePassword.IsEnabled =
                radioButtonYesAutoLogoutEnabled.IsSelected;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonNoAutoLogoutEnabled_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("DisableAutoLogoff", 1);
            LoginControl.StopAutoLogoutTimer();
            radioButtonYesAutoLogoutEnabled.IsSelected = false;
            SetAutoLogoutEnabledControls();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonYesDisableAutoLogoutInOrderEntry_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffWhileInOrderEntry", 0);
            radioButtonNoDisableAutoLogoutInOrderEntry.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonNoDisableAutoLogoutinOrderEntry_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffWhileInOrderEntry", 1);
            radioButtonYesDisableAutoLogoutInOrderEntry.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonYesDisableAutoLogoutForPasswordChanges_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffInPasswordChange", 0);
            radioButtonNoDisableAutoLogoutForPasswordChanges.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonNoDisableAutoLogoutForPasswordChanges_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffInPasswordChange", 1);
            radioButtonYesDisableAutoLogoutForPasswordChanges.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonYesDisableAutoLogoutForDialogWindows_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffForDialogWindows", 0);
            radioButtonNoDisableAutoLogoutForDialogWindows.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonNoDisableAutoLogoutForDialogWindows_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("EnableAutoLogoffForDialogWindows", 1);
            radioButtonYesDisableAutoLogoutForDialogWindows.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonWeatherIsEnabled_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("ShowWeather", 1);
            radioButtonWeatherIsNotEnabled.IsSelected = false;
            SetWeatherControls();
        }

        private void SetWeatherControls()
        {
            labelWeatherPostal.IsEnabled =
                textBoxWeatherZipCode.IsEnabled =
                radioButtonWeatherIsEnabled.IsSelected;
        }
        
        [Obfuscation(Exclude = true)]
        private void radioButtonWeatherIsNotEnabled_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("ShowWeather", 0);
            radioButtonWeatherIsEnabled.IsSelected = false;
            SetWeatherControls();
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxAutoLogoutTimeout_CommitEdit(object sender, EventArgs e)
        {
            // Autologout
            try
            {
                int timeout = Convert.ToInt32(textBoxAutoLogoutTimeout.Text).Clamp(5, 9999);
                string timeoutString = "" + timeout;
                if (!timeoutString.Equals(textBoxAutoLogoutTimeout.Text))
                    textBoxAutoLogoutTimeout.Text = timeoutString;
                SettingManager.SetStoreSetting("AutoLogoffTimeout", timeout);
            }
            catch
            {
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxWeatherZipCode_CommitEdit(object sender, EventArgs e)
        {
            // Weather
            try
            {
                int zipCode = Convert.ToInt32(textBoxWeatherZipCode.Text);
                SettingManager.SetStoreSetting("WeatherZipCode", zipCode);
            }
            catch
            {
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPort_CommitEdit(object sender, EventArgs e)
        {
            // Broadcast Server
#if !DEMO
            BroadcastServerSocket.Port = GetPort();
            if ((BroadcastServerSocket.IsEnabled) &&
                (BroadcastServerSocket.Port != null))
            {
                if (BroadcastServerSocket.IsRunning)
                {
                    BroadcastServerSocket.BroadcastMessage("ServerPortMayHaveChanged");
                    BroadcastServerSocket.Stop();
                }
                BroadcastServerSocket.Start();
            }
#endif
        }

        [Obfuscation(Exclude = true)]
        private void mainPane_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closed +=
                GeneralSettingsGeneralPreferencesControl_Closed;
        }

        [Obfuscation(Exclude = true)]
        void GeneralSettingsGeneralPreferencesControl_Closed(object sender, EventArgs e)
        {
            MainWindow.Singleton.orderEntryControl.QueryCurrentWeather();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsUsingKioskMode_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            radioButtonIsNotUsingKioskMode.IsSelected = false;
            LocalSetting.Values.Boolean["KioskMode"] = true;
            LocalSetting.Update();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotUsingKioskMode_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            radioButtonIsUsingKioskMode.IsSelected = false;
            LocalSetting.Values.Boolean["KioskMode"] = false;
            LocalSetting.Update();
        }

        private void radioButtonIsLogoutOnPlaceOrder_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetLogoutOnPlaceOrder(true);
            radioButtonIsNotLogoutOnPlaceOrder.IsSelected = false;
        }

        private void radioButtonIsNotLogoutOnPlaceOrder_SelectionGained(object sender, EventArgs e)
        {
            if (!IsInitialized) return;
            ConfigurationManager.SetLogoutOnPlaceOrder(false);
            radioButtonIsLogoutOnPlaceOrder.IsSelected = false;
        }

        private void radioButtonIsBlockTaskManager_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("DontDisableTaskManager", 0);
            radioButtonIsNotBlockTaskManager.IsEnabled = false;
            if (!TaskManagerServiceHelper.IsInstalled)
            {
                TaskManagerServiceHelper.ThreadFailed += TaskManagerServiceHelper_ThreadFailed;
                if (PosDialogWindow.ShowDialog(Strings.DoYouWantToInstallTheTaskManagerAccessService,
                    Strings.InstallService, DialogButtons.YesNo) == DialogButton.Yes)
                    new Thread(TaskManagerServiceHelper.InstallThread).Start();
                else
                {
                    radioButtonIsNotBlockTaskManager.IsEnabled = true;
                    radioButtonIsBlockTaskManager.IsEnabled = false;
                }
            }
            else if (!TaskManagerServiceHelper.IsStarted)
            {
                TaskManagerServiceHelper.ThreadFailed += TaskManagerServiceHelper_ThreadFailed;
                if (PosDialogWindow.ShowDialog(Strings.DoYouWantToStartTheTaskManagerAccessService,
                    Strings.StartService, DialogButtons.YesNo) == DialogButton.Yes)
                    new Thread(TaskManagerServiceHelper.StartThread).Start();
                else
                {
                    radioButtonIsNotBlockTaskManager.IsEnabled = true;
                    radioButtonIsBlockTaskManager.IsEnabled = false;
                }
            }
            else
            {
                TaskManagerServiceHelper.IsTaskManagerDisabled = true;
            }
        }

        void TaskManagerServiceHelper_ThreadFailed(object sender, EventArgs e)
        {
            TaskManagerServiceHelper.ThreadFailed -= TaskManagerServiceHelper_ThreadFailed;
            radioButtonIsNotBlockTaskManager.IsEnabled = true;
            radioButtonIsBlockTaskManager.IsEnabled = false;
        }

        private void radioButtonIsNotBlockTaskManager_SelectionGained(object sender, EventArgs e)
        {
            SettingManager.SetStoreSetting("DontDisableTaskManager", 1);
            radioButtonIsBlockTaskManager.IsEnabled = false;
            if (!TaskManagerServiceHelper.IsInstalled || !TaskManagerServiceHelper.IsStarted) return;
            TaskManagerServiceHelper.IsTaskManagerDisabled = false;
        }
    }
}
