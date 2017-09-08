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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateEntryDayControl.xaml
    /// </summary>
    public partial class DateEntryDayControl : UserControl
    {
        #region Licensed Access Only
        static DateEntryDayControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEntryDayControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private DateTime dateTimeStart = DateTime.Now;
        private DateTime? selectedDay;

        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedDayChanged;

        public DateTime? SelectedDay
        {
            get
            {
                return selectedDay;
            }
            set
            {
                selectedDay = value;
                UpdateSelectedDay();
                if (SelectedDayChanged != null)
                    SelectedDayChanged.Invoke(this, new EventArgs());
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                return (selectedDay != null ? selectedDay.Value : DateTime.Now);
            }
        }

        public ToggleButton[] Buttons
        {
            get;
            private set;
        }

        public DateEntryDayControl()
        {
            InitializeComponent();
            Buttons = new ToggleButton[] {
                button10, button11, button12, button13, button14, button15, button16,
                button20, button21, button22, button23, button24, button25, button26,
                button30, button31, button32, button33, button34, button35, button36,
                button40, button41, button42, button43, button44, button45, button46,
                button50, button51, button52, button53, button54, button55, button56,
                button60, button61, button62, button63, button64, button65, button66,
            };
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            int month = CurrentDateTime.Month;
            int year = CurrentDateTime.Year;
            int day = 1;
            DateTime firstOfMonth = new DateTime(year, month, 1);
            int startingIndex = (int)firstOfMonth.DayOfWeek;
            for (int index = 0; index < startingIndex; index++)
            {
                Buttons[index].IsEnabled = false;
                Buttons[index].Content = "";
            }
            for (int index = startingIndex; index < Buttons.Length; index++)
            {
                try
                {
                    DateTime dayDateTime = new DateTime(year, month, day);
                }
                catch
                {
                    startingIndex = index;
                    break;
                }
                Buttons[index].IsEnabled = true;
                Buttons[index].Content = "" + day++;
            }
            for (int index = startingIndex; index < Buttons.Length; index++)
            {
                Buttons[index].IsEnabled = false;
                Buttons[index].Content = "";
            }
        }

        private void UpdateSelectedDay()
        {
            bool selectedDayIsVisible = ((selectedDay != null) && (selectedDay.HasValue) &&
                    (CurrentDateTime.Month == SelectedDay.Value.Month) &&
                    (CurrentDateTime.Year == SelectedDay.Value.Year));
            foreach (ToggleButton button in Buttons)
            {
                if (String.IsNullOrEmpty(button.Content as string))
                    continue;
                if (selectedDayIsVisible)
                {
                    int day = Convert.ToInt32(button.Content as string);
                    button.IsChecked = (day == selectedDay.Value.Day);
                }
                else
                {
                    button.IsChecked = false;
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton pushToggleButtonSender = (ToggleButton)sender;
            if (String.IsNullOrEmpty(pushToggleButtonSender.Content as string))
                return;
            int day = Convert.ToInt32(pushToggleButtonSender.Content as string);
            DateTime newSelectedDay = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, day);
            if (newSelectedDay == SelectedDay)
                SelectedDay = null;
            else
                SelectedDay = newSelectedDay;
        }

    }
}
