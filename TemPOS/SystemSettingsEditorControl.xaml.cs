using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosModels;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EditConnectionStringControl.xaml
    /// </summary>
    public partial class SystemSettingsEditorControl : UserControl
    {
        public bool IsPersonalInfoHidden
        {
            get;
            private set;
        }

        public bool IsCanceled
        {
            get;
            private set;
        }

        public SystemSettingsEditorControl()
        {
            IsCanceled = false;
            IsPersonalInfoHidden = false;
            InitializeComponent();
        }

        public void InitializeFields(bool hidePersonalInfo = false)
        {
            if (hidePersonalInfo)
            {
                row1.Height = new GridLength(0);
                row2.Height = new GridLength(0);
                IsPersonalInfoHidden = true;
            }
            else
            {
                textBoxCompanyName.Text = LocalSetting.CompanyName;
                textBoxSerialNumber.Text = LocalSetting.ApplicationSerialNumber;
            }
            textBoxDBServer.Text = LocalSetting.DatabaseServerName;
            textBoxDBLogin.Text = LocalSetting.DatabaseServerLoginName;
            textBoxDBPassword.Text = LocalSetting.DatabaseServerPassword;
            textBoxDBInitialCatalog.Text = LocalSetting.DatabaseServerDatabaseName;
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPersonalInfoHidden)
            {
                LocalSetting.CompanyName = textBoxCompanyName.Text;
                LocalSetting.ApplicationSerialNumber = textBoxSerialNumber.Text;
            }
            LocalSetting.DatabaseServerName = textBoxDBServer.Text;
            LocalSetting.DatabaseServerDatabaseName = textBoxDBInitialCatalog.Text;
            LocalSetting.DatabaseServerLoginName = textBoxDBLogin.Text;
            LocalSetting.DatabaseServerPassword = textBoxDBPassword.Text;
            LocalSetting.Update();
            Window.GetWindow(this).Close();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            Window.GetWindow(this).Close();
        }

        public static PosDialogWindow CreateInDefaultWindow(bool hidePersonalInfo = false)
        {
            SystemSettingsEditorControl control = new SystemSettingsEditorControl();
            return new PosDialogWindow(control,
                Strings.SystemSettings, 400, (!hidePersonalInfo ? 330 : 250));
        }
    }
}
