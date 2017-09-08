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
using PosControls.Types;
using System.Reflection;
using PosModels.Types;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateEntryMonthControl.xaml
    /// </summary>
    public partial class DateEntryMonthControl : UserControl
    {
        #region Licensed Access Only
        static DateEntryMonthControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEntryMonthControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private Month? selectedMonth;

[Obfuscation(Exclude = true)]
        public event EventHandler SelectedMonthChanged;

        public Month? SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                selectedMonth = value;
                foreach (object ctrl in gridControl.Children)
                {
                    if (!(ctrl is TextBlockButton))
                        continue;
                    TextBlockButton control = (TextBlockButton)ctrl;
                    Month month = (Month)Convert.ToInt32(control.Tag);
                    control.IsChecked = (value == month);
                }
                if (SelectedMonthChanged != null)
                    SelectedMonthChanged.Invoke(this, new EventArgs());
            }
        }


        public DateEntryMonthControl()
        {
            InitializeComponent();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButtonSender = (TextBlockButton)sender;
            SelectedMonth = (Month)Convert.ToInt32(pushButtonSender.Tag);
        }

    }
}
