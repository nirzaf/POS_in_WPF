using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TemPOS.Networking;
using TemPOS.Types;
using TemposLibrary;
using PosModels;
using PosControls;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for GeneralSettingsUpdateControl.xaml
    /// </summary>
    public partial class GeneralSettingsUpdateControl : UserControl
    {
        public GeneralSettingsUpdateControl()
        {
            InitializeComponent();
#if !DEMO
            Updater.Connected += Client_Connected;
            Updater.ConnectFailed += Client_ConnectFailed;
            Updater.Disconnected += Client_Disconnected;
            Updater.Authenticated += Client_Authenticated;
            Updater.UpdateReceived += Client_UpdateReceived;
            Updater.ReceivedFileBlock += Client_ReceivedFileBlock;
            Updater.ReceivedVersion += Updater_ReceivedVersion;
            Updater.ZipExtractionFailed += Updater_ZipExtractionFailed;
#if DEBUG
            //Updater.Debug += new TextEventHandler(Updater_Debug);
#endif
            flowDocumentScroll.IsEnabled = false;
            flowDocumentScroll.Document = new FlowDocument();

            int? value = StoreSetting.Get("AutoUpdate").IntValue;
            bool isAutoUpdateEnabled = (value != null) && (value.Value != 0);
            radioButtonAutoUpdateIsEnabled.IsSelected = isAutoUpdateEnabled;
            radioButtonAutoUpdateIsNotEnabled.IsSelected = !isAutoUpdateEnabled;
            textBoxServer.Text = LocalSetting.Values.String["UpdateServer"];
            textBoxPort.Text = LocalSetting.Values.String["UpdateServerPort"];
#endif
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void Updater_ZipExtractionFailed(object sender, TextEventArgs args)
        {
            PrintLine(Strings.UpdateDownloadError);
            SetIsEnabledSafe(true);
        }

        [Obfuscation(Exclude = true)]
        void Updater_Debug(object sender, TextEventArgs args)
        {
            PrintLine(args.Text);
        }

        [Obfuscation(Exclude = true)]
        void Client_Connected(object sender, EventArgs e)
        {
            PrintLine(Strings.UpdateConnected);
        }

        [Obfuscation(Exclude = true)]
        void Client_ConnectFailed(object sender, EventArgs e)
        {
            PrintLine(Strings.UpdateFailedToConnect);
            SetIsEnabledSafe(true);
        }

        [Obfuscation(Exclude = true)]
        void Client_Disconnected(object sender, EventArgs e)
        {
            Srp6ClientSocket client = sender as Srp6ClientSocket;
            PrintLine(Strings.UpdateDisconnected);
            SetIsEnabledSafe(true);

            /*
            if (updateReceived)
            {
                // We received the update (disconnect has already occured)
                MessageBox.Show("Update Received");
            }
            else if (!client.IsAuthenticated)
            {
                // Disconnect occured without authenticating
                MessageBox.Show("Disconnected, without authenticating");
            }
            else if (!client.IsUpdateReceived)
            {
                // Disconnect occured without getting the update file
                MessageBox.Show("Disconnected, without receiving update");
            }
            */
        }

        [Obfuscation(Exclude = true)]
        void Client_Authenticated(object sender, EventArgs e)
        {
            // We have authenticated 
            PrintLine(Strings.UpdateAuthenticated);
        }

        [Obfuscation(Exclude = true)]
        void Updater_ReceivedVersion(object sender, EventArgs e)
        {
            PrintLine(Strings.UpdateNewestVersion + " \"" + Srp6ClientSocket.NewestUpdateVersion + "\"");
        }

        [Obfuscation(Exclude = true)]
        void Client_ReceivedFileBlock(object sender, EventArgs e)
        {
            Srp6ClientSocket client = sender as Srp6ClientSocket;
            // We are receiving the update
            if (client == null) return;
            Dispatcher.Invoke((Action)(() =>
            {
                progressBar.Value = client.BytesReceived / (double)client.UpdateFileSize;
            }));
       }

        [Obfuscation(Exclude = true)]
        void Client_UpdateReceived(object sender, EventArgs e)
        {
            Srp6ClientSocket client = sender as Srp6ClientSocket;
            PrintLine(Strings.UpdateReceived);
        }

        private void SetIsEnabledSafe(bool isEnabled)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                buttonVersionCheck.IsEnabled = isEnabled;
                buttonUpdate.IsEnabled = isEnabled;
                radioButtonAutoUpdateIsNotEnabled.IsEnabled = isEnabled;
                radioButtonAutoUpdateIsEnabled.IsEnabled = isEnabled;
                textBoxPort.IsEnabled = isEnabled;
                textBoxServer.IsEnabled = isEnabled;
                if (!isEnabled)
                {
                    buttonSave.Tag = (buttonSave.IsEnabled ? "True" : "False");
                    buttonSave.IsEnabled = false;
                }
                else
                {
                    buttonSave.IsEnabled =                    
                        ((buttonSave.Tag != null) &&
                           (buttonSave.Tag != null) && buttonSave.Tag.Equals("True"));
                }
            }));
        }

        private void PrintLine(string text)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (flowDocumentScroll.Document.Blocks.Count > 0)
                {
                    Paragraph p = flowDocumentScroll.Document.Blocks.FirstBlock as Paragraph;
                    if (p != null)
                    {
                        Run r = p.Inlines.FirstInline as Run;
                        if (r != null) text = r.Text + Environment.NewLine + text;
                    }
                    flowDocumentScroll.Document.Blocks.Clear();
                }
                
                flowDocumentScroll.Document.Blocks.Add(new Paragraph(new Run(text)));
                flowDocumentScroll.UpdateLayout();
                dragScrollViewer.ScrollToEnd();
            }));
        }
#endif
        private void AdjustSizeOfTabControl()
        {
            if (flowDocumentScroll.ActualWidth > 5)
                flowDocumentScroll.Document.PageWidth = flowDocumentScroll.ActualWidth - 5;
            if (flowDocumentScroll.ActualHeight > 5)
                flowDocumentScroll.Document.PageHeight = flowDocumentScroll.ActualHeight - 5;
        }

        [Obfuscation(Exclude = true)]
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustSizeOfTabControl();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxServer_TextChanged(object sender, RoutedEventArgs e)
        {
            EnableSaveButton();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPort_TextChanged(object sender, RoutedEventArgs e)
        {
            EnableSaveButton();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonAutoUpdateIsEnabled_SelectionGained(object sender, EventArgs e)
        {
            EnableSaveButton();
            radioButtonAutoUpdateIsNotEnabled.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonAutoUpdateIsNotEnabled_SelectionGained(object sender, EventArgs e)
        {
            EnableSaveButton();
            radioButtonAutoUpdateIsEnabled.IsSelected = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.IsEnabled = false;
            SaveOptions();
        }

        private void EnableSaveButton()
        {
            if (IsLoaded)
                buttonSave.IsEnabled = true;
        }

        private void SaveOptions()
        {
#if !DEMO
            StoreSetting.Set("AutoUpdate",
                (radioButtonAutoUpdateIsEnabled.IsSelected ? 1 : 0));
            LocalSetting.Values.String["UpdateServerPort"] = textBoxPort.Text;
            LocalSetting.Values.String["UpdateServer"] = textBoxServer.Text;
            LocalSetting.Update();

            // Need to notify other clients of this information so they can update
            // their LocalSetting values
            BroadcastClientSocket.SendMessage("UpdateServerInfo " +
                LocalSetting.Values.String["UpdateServer"] + " " +
                LocalSetting.Values.String["UpdateServerPort"]);
#endif
        }

        [Obfuscation(Exclude = true)]
        private void buttonVersionCheck_Click(object sender, RoutedEventArgs e)
        {
#if !DEMO
            progressBar.Value = 0;
            SetIsEnabledSafe(false);
            Updater.StartVersionCheck();
#endif
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
#if !DEMO
            progressBar.Value = 0;
            SetIsEnabledSafe(false);
            Updater.StartUpdate();
#endif
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            GeneralSettingsUpdateControl control = new GeneralSettingsUpdateControl();
            return new PosDialogWindow(control, Strings.UpdateTemposUpdater, 700, 500);
        }
    }
}
