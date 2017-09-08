using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosModels;
using PosModels.Managers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ChangePasswordControl.xaml
    /// </summary>
    public partial class ChangePasswordControl : UserControl
    {
        public char PasswordChar
        {
            get;
            private set;
        }

        public ChangePasswordControl()
        {
            InitializeComponent();
            PasswordChar = new PasswordBox().PasswordChar;
            textOldPassword.TextMaskCharacter = PasswordChar;
            textNewPassword1.TextMaskCharacter = PasswordChar;
            textNewPassword2.TextMaskCharacter = PasswordChar;
        }

        [Obfuscation(Exclude = true)]
        void ChangePasswordControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            if (parent == null) return;
            parent.Closed += ChangePasswordControl_Closed;
            parent.Loaded += parent_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void parent_Loaded(object sender, RoutedEventArgs e)
        {
            StoreSetting setting =
                SettingManager.GetStoreSetting("EnableAutoLogoffInPasswordChange");
            if ((setting.IntValue == null) || (setting.IntValue.Value == 0))
                LoginControl.StopAutoLogoutTimer();
        }

        [Obfuscation(Exclude = true)]
        void ChangePasswordControl_Closed(object sender, EventArgs e)
        {
            LoginControl.StartAutoLogoutTimer();
        }

        [Obfuscation(Exclude = true)]
        private void TextBlockButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionManager.ActiveEmployee.CheckPassword(textOldPassword.Text))
            {
                PosDialogWindow.ShowDialog(Strings.PasswordIncorrect, Strings.Error);
                return;
            }
            if (textNewPassword1.Text != textNewPassword2.Text)
            {
                PosDialogWindow.ShowDialog(Strings.NewPasswordsMismatch, Strings.Error);
                return;
            }
            if (textNewPassword1.Text.Length < 5)
            {
                PosDialogWindow.ShowDialog(Strings.NewPasswordToShort, Strings.Error);
                return;
            }
                
            SessionManager.ActiveEmployee.SetScanCodeData(textNewPassword1.Text);
            SessionManager.ActiveEmployee.Update();
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textOldPassword.Text) ||
                string.IsNullOrEmpty(textNewPassword1.Text) ||
                string.IsNullOrEmpty(textNewPassword2.Text))
            {
                buttonControl.IsEnabled = false;
                return;
            }
            if (!buttonControl.IsEnabled)
                buttonControl.IsEnabled = true;
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            ChangePasswordControl control = new ChangePasswordControl();
            return new PosDialogWindow(control, Strings.ChangePasswordWindowTitle, 350, 270);
        }

    }
}
