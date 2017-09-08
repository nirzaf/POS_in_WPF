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
    /// Interaction logic for DateEntryYearControl.xaml
    /// </summary>
    public partial class DateEntryYearControl : UserControl
    {
        #region Licensed Access Only
        static DateEntryYearControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEntryYearControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private int currentYear = DateTime.Now.Year;
        private int? selectedYear = null;

[Obfuscation(Exclude = true)]
        public event EventHandler SelectedYearChanged;

        public int? SelectedYear
        {
            get { return selectedYear; }
            set
            {
                selectedYear = value;
                InitializeButtons();
                if (SelectedYearChanged != null)
                    SelectedYearChanged.Invoke(this, new EventArgs());
            }
        }
        public TextBlockButton[] Buttons
        {
            get;
            private set;
        }

        public DateEntryYearControl()
        {
            InitializeComponent();
            Buttons = new TextBlockButton[] {
                button0, button1, button2, button3, button4, button5, button6,
                button7, button8, button9, button10, button11,
            };
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            int year = currentYear;
            for (int i = 1; i < (Buttons.Length - 1); i++)
            {
                Buttons[i].IsChecked =
                    (SelectedYear != null && SelectedYear.HasValue &&
                     SelectedYear.Value == year);
                Buttons[i].Text = "" + year++;
            }
            Buttons[0].Text = "Earlier";
            Buttons[11].Text = "Later";
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton pushButtonSender = (TextBlockButton)sender;
            if (pushButtonSender.Text.Equals("Earlier"))
            {
                currentYear = Convert.ToInt32(Buttons[1].Text) - 10;
                InitializeButtons();
            }
            else if (pushButtonSender.Text.Equals("Later"))
            {
                currentYear = Convert.ToInt32(Buttons[10].Text) + 1;
                InitializeButtons();
            }
            else
            {
                SelectedYear = Convert.ToInt32(pushButtonSender.Text);
            }
        }

    }
}
