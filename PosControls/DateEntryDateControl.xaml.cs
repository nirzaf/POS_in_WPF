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
    /// Interaction logic for DateEntryDateControl.xaml
    /// </summary>
    public partial class DateEntryDateControl : UserControl
    {
        #region Licensed Access Only
        static DateEntryDateControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEntryDateControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool haltEvents = false;
        private DateTime? selectedDay = null;
        
        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedDayChanged;

        public DateTime DisplayedDateTime
        {
            get;
            private set;
        }

        public DateTime? SelectedDay
        {
            get { return selectedDay; }
            set
            {
                selectedDay = value;
                haltEvents = true;
                if (value != null)
                {
                    DisplayedDateTime = selectedDay.Value;
                    dateEntryYearControl.SelectedYear = selectedDay.Value.Year;
                    dateEntryDayControl.SelectedDay = selectedDay.Value;
                    dateEntryMonthControl.SelectedMonth = (Month)selectedDay.Value.Month;
                }
                else
                {
                    DisplayedDateTime = DateTime.Now;
                    dateEntryYearControl.SelectedYear = DisplayedDateTime.Year;
                    dateEntryDayControl.SelectedDay = DisplayedDateTime;
                    dateEntryMonthControl.SelectedMonth = (Month)DisplayedDateTime.Month;
                }
                //labelDateTitle.Content = dateEntryDayControl.DateText;

                if ((dateEntryDayControl.SelectedDay != null) &&
                    (dateEntryDayControl.SelectedDay.HasValue))
                {
                    DateTime dt = dateEntryDayControl.SelectedDay.Value;
                    buttonChangeMonth.Content = dt.ToString("MMMM dd");
                    buttonChangeYear.Content = dt.ToString("yyyy");
                }
                else
                {
                    DateTime dt = dateEntryDayControl.CurrentDateTime; //dateEntryDayControl.SelectedDay
                    buttonChangeMonth.Content = dt.ToString("MMMM");
                    buttonChangeYear.Content = dt.ToString("yyyy");
                }
                if (SelectedDayChanged != null)
                    SelectedDayChanged.Invoke(this, new EventArgs());
                haltEvents = false;
            }
        }

        public DateEntryDateControl()
        {
            InitializeComponent();
            buttonChangeMonth.Content = DateTime.Now.ToString("MMMM");
            buttonChangeYear.Content = DateTime.Now.ToString("yyyy");
        }

        private void ChangeYear()
        {
            //buttonChangeYear.IsChecked = !buttonChangeYear.IsChecked;
            if (buttonChangeYear.IsChecked == true)
            {
                dateEntryMonthControl.Visibility = Visibility.Hidden;
                dateEntryDayControl.Visibility = Visibility.Hidden;
                dateEntryYearControl.Visibility = Visibility.Visible;
                buttonChangeMonth.IsChecked = false;
            }
            else
            {
                dateEntryYearControl.Visibility = Visibility.Hidden;
                dateEntryMonthControl.Visibility = Visibility.Hidden;
                dateEntryDayControl.Visibility = Visibility.Visible;
            }
        }

        private void ChangeMonth()
        {
            //buttonChangeMonth.IsChecked = !buttonChangeMonth.IsChecked;
            if (buttonChangeMonth.IsChecked == true)
            {
                dateEntryMonthControl.Visibility = Visibility.Visible;
                dateEntryYearControl.Visibility = Visibility.Hidden;
                dateEntryDayControl.Visibility = Visibility.Hidden;
                buttonChangeYear.IsChecked = false;
            }
            else
            {
                dateEntryYearControl.Visibility = Visibility.Hidden;
                dateEntryMonthControl.Visibility = Visibility.Hidden;
                dateEntryDayControl.Visibility = Visibility.Visible;
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void dateEntryControl_SelectedDayChanged(object sender, EventArgs e)
        {
            if (haltEvents)
                return;
            SelectedDay = dateEntryDayControl.SelectedDay;
        }

        [Obfuscation(Exclude = true)]
        private void dateEntryMonthControl_SelectedMonthChanged(object sender, EventArgs e)
        {
            if (haltEvents)
                return;
            int day = DisplayedDateTime.Day;
            while (true)
            {
                try
                {
                    SelectedDay = new DateTime(
                        DisplayedDateTime.Year,
                        (int)dateEntryMonthControl.SelectedMonth,
                        day);

                    break;
                }
                catch (Exception ex)
                {
                    day--;
                    if (day < 0)
                        throw ex;
                }
            }
            dateEntryMonthControl.Visibility = Visibility.Hidden;
            dateEntryDayControl.Visibility = Visibility.Visible;
            buttonChangeMonth.IsChecked = false;
        }

        [Obfuscation(Exclude = true)]
        private void dateEntryYearControl_SelectedYearChanged(object sender, EventArgs e)
        {
            if (haltEvents)
                return;
            try
            {
                SelectedDay = new DateTime(
                    (int)dateEntryYearControl.SelectedYear,
                    DisplayedDateTime.Month,
                    DisplayedDateTime.Day);
            }
            catch
            {
                // Year change from year with Feb29 to a year without, change to Feb28
                SelectedDay = new DateTime(
                    (int)dateEntryYearControl.SelectedYear,
                    DisplayedDateTime.Month,
                    DisplayedDateTime.Day - 1);
            }
            dateEntryYearControl.Visibility = Visibility.Hidden;
            dateEntryDayControl.Visibility = Visibility.Visible;
            buttonChangeYear.IsChecked = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonChangeMonth_Click(object sender, RoutedEventArgs e)
        {
            ChangeMonth();
        }

        [Obfuscation(Exclude = true)]
        private void buttonChangeYear_Click(object sender, RoutedEventArgs e)
        {
            ChangeYear();
        }

    }
}
