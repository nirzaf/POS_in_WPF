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
using System.Data.SqlTypes;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for StartDateEndDateControl.xaml
    /// </summary>
    public partial class StartDateEndDateControl : UserControl
    {
        #region Licensed Access Only
        static StartDateEndDateControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(StartDateEndDateControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public DateTime StartDate
        {
            get
            {
                if ((startDateEntryDateControl.SelectedDay == null) ||
                    !startDateEntryDateControl.SelectedDay.HasValue)
                    return SqlDateTime.MinValue.Value;
                DateTime date = startDateEntryDateControl.SelectedDay.Value;
                return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            }
        }

        public DateTime EndDate
        {
            get
            {
                if ((endDateEntryDateControl.SelectedDay == null) ||
                    !endDateEntryDateControl.SelectedDay.HasValue)
                    return SqlDateTime.MaxValue.Value;
                DateTime date = endDateEntryDateControl.SelectedDay.Value;
                return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            }
        }

        public StartDateEndDateControl()
        {
            InitializeComponent();
            startDateEntryDateControl.SelectedDay =
                endDateEntryDateControl.SelectedDay = DateTime.Now;
        }

        [Obfuscation(Exclude = true)]
        private void startDateEntryDateControl_SelectedDayChanged(object sender, EventArgs e)
        {
            if ((!startDateEntryDateControl.SelectedDay.HasValue) ||
                (!endDateEntryDateControl.SelectedDay.HasValue))
                return;
            DateTime start = startDateEntryDateControl.SelectedDay.Value;
            DateTime end = endDateEntryDateControl.SelectedDay.Value;
            if (end < start)
                endDateEntryDateControl.SelectedDay = start;
        }
        
        [Obfuscation(Exclude = true)]
        private void endDateEntryDateControl_SelectedDayChanged(object sender, EventArgs e)
        {
            if ((!startDateEntryDateControl.SelectedDay.HasValue) ||
                (!endDateEntryDateControl.SelectedDay.HasValue))
                return;
            DateTime start = startDateEntryDateControl.SelectedDay.Value;
            DateTime end = endDateEntryDateControl.SelectedDay.Value;
            if (end < start)
                startDateEntryDateControl.SelectedDay = end;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllDates_Click(object sender, RoutedEventArgs e)
        {
            startDateEntryDateControl.SelectedDay = null;
            endDateEntryDateControl.SelectedDay = null;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSelectRange_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate > EndDate)
                PosDialogWindow.ShowDialog(
                    "Your start date is later than your end date", "Invalid Range",
                    DialogButtons.Ok);
            else
                Window.GetWindow(this).Close();
        }
    }
}
