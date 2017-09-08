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
    /// Interaction logic for TimeEntryTimeControl.xaml
    /// </summary>
    public partial class TimeEntryTimeControl : UserControl
    {
        #region Licensed Access Only
        static TimeEntryTimeControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TimeEntryTimeControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public event EventHandler TimeChanged;

        public TimeSpan TimeOfDay
        {
            get { return timeEntryClockControl.Time; }
            set {
                timeEntryClockControl.Time = value;
                if (value.Hours >= 12)
                    buttonAmPm.Content = "PM";
                else
                    buttonAmPm.Content = "AM";
            }
        }

        public bool UseMilitaryFormat
        {
            get { return timeEntryClockControl.UseMilitaryFormat; }
            set
            {
                timeEntryClockControl.UseMilitaryFormat = value;
                buttonAmPm.Visibility = labelAmPmTime.Visibility = 
                    (value ? Visibility.Hidden : Visibility.Visible);
                labelMilitaryTime.Visibility = 
                    (!value ? Visibility.Hidden : Visibility.Visible);
            }
        }

        public TimeEntryTimeControl()
        {
            InitializeComponent();
        }

        private void OnTimeChanged()
        {
            if (TimeChanged != null)
                TimeChanged.Invoke(this, new EventArgs());
        }
        
        [Obfuscation(Exclude = true)]
        private void timeEntryClockControl_TimeChanged(object sender, EventArgs e)
        {
            int hour = timeEntryClockControl.Hour;
            if (hour == 0)
                hour = 12;
            if (hour > 12)
                hour -= 12;
            labelAmPmTime.Content = hour.ToString() + ":" +
                timeEntryClockControl.Minute.ToString("D2");
            labelMilitaryTime.Content = timeEntryClockControl.Hour.ToString() + ":" +
                timeEntryClockControl.Minute.ToString("D2");
            OnTimeChanged();
        }

        [Obfuscation(Exclude = true)]
        private void buttonAmPm_Click(object sender, RoutedEventArgs e)
        {
            Button actualSender = (Button)sender;
            if (((string)actualSender.Content).Equals("AM"))
            {
                actualSender.Content = "PM";
                timeEntryClockControl.IsPM = true;
            }
            else
            {
                actualSender.Content = "AM";
                timeEntryClockControl.IsPM = false;
            }
            OnTimeChanged();
        }

    }
}
