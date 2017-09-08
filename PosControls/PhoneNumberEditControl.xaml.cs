using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for PhoneNumberEditControl.xaml
    /// </summary>
    public partial class PhoneNumberEditControl : UserControl
    {
        #region Licensed Access Only
        static PhoneNumberEditControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PhoneNumberEditControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public event EventHandler NumberChanged = null;
        [Obfuscation(Exclude = true)]
        public event EventHandler ContentsInvalid = null;

        /*
        public bool CurrentPhoneNumberExists
        {
            get
            {
                PhoneNumber lookup = PhoneNumber.Find(PhoneNumber.GetAll(), CurrentPhoneNumber);
                return (lookup != null);
            }
        }
        */

        public string CurrentPhoneNumber
        {
            get
            {
                return "" + textBoxAreaCode.Text + "-" + textBoxPrefix.Text + "-" + textBoxNumber.Text;
            }
        }

        public PhoneNumberEditControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender == textBoxAreaCode) && (textBoxAreaCode.Text.Length == 3))
                textBoxPrefix.Focus();
            if ((sender == textBoxPrefix) && (textBoxPrefix.Text.Length == 3))
                textBoxNumber.Focus();

            if ((textBoxAreaCode.Text.Length == 3) && (textBoxPrefix.Text.Length == 3) &&
                (textBoxNumber.Text.Length == 4))
            {
                try
                {
                    int areaCode = Convert.ToInt32(textBoxAreaCode.Text);
                    int prefix = Convert.ToInt32(textBoxPrefix.Text);
                    int number = Convert.ToInt32(textBoxNumber.Text);
                    if (NumberChanged != null)
                        NumberChanged.Invoke(this, new EventArgs());
                    return;
                }
                catch
                {
                }
            }
            if (ContentsInvalid != null)
                ContentsInvalid.Invoke(this, new EventArgs());
        }

    }
}
