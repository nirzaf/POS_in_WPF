using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Globalization;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for RegisterDrawerStartControl.xaml
    /// </summary>
    public partial class RegisterDrawerStartControl : UserControl
    {
        public double? StartingAmount
        {
            get;
            private set;
        }

        public RegisterDrawerStartControl()
        {
            StartingAmount = null;
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxStartingAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = textBoxStartingAmount.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "");
                StartingAmount = Convert.ToDouble(text);
            }
            catch
            {
                StartingAmount = null;
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void TextBlockButton_Click(object sender, RoutedEventArgs e)
        {
            if ((StartingAmount == null) || (StartingAmount.Value < 0.01))
            {
                PosDialogWindow.ShowDialog(
                    Strings.YouHaveNotSpecifiedAStartingAmount, Strings.Error);
            }
            else
            {
                Window.GetWindow(this).Close();
            }
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            RegisterDrawerStartControl control = new RegisterDrawerStartControl();
            return new PosDialogWindow(control, Strings.StartUpRegister, 355, 165);
        }
    }
}
