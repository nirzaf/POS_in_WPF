using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DayOfOperationRangeSelectionControl.xaml
    /// </summary>
    public partial class DayOfOperationRangeSelectionControl : UserControl
    {
        public DateTime StartRange
        {
            get;
            private set;
        }

        public DateTime EndRange
        {
            get;
            private set;
        }

        public DayOfOperationRangeSelectionControl()
        {
            InitializeComponent();
            InitializeDayOfOperations();
        }

        private void InitializeDayOfOperations()
        {
            DayOfOperation mostRecent =
                DayOfOperation.GetLatestInYear(DayOfOperation.CurrentYear);
            listBoxStartRange.Items.Add(new FormattedListBoxItem(mostRecent, FormatDay(mostRecent), true));
            for (int id = mostRecent.Id - 1; id >= 1; id--)
            {
                DayOfOperation day = DayOfOperation.Get(id);
                listBoxStartRange.Items.Add(new FormattedListBoxItem(day, FormatDay(day), true));
            }
        }

        private string FormatDay(DayOfOperation day)
        {
            return Strings.StartOfDay + ": " + day.StartOfDay.DayOfWeek + ", " + day.StartOfDay +
                ((day.EndOfDay != null) ?
                Environment.NewLine + Strings.EndOfDay + ": " + day.EndOfDay.Value.DayOfWeek + ", " +
                day.EndOfDay.Value : "");
        }

        public DayOfOperationRangeSelectionControl(DateTime startRange, DateTime endRange)
        {
            InitializeComponent();
            StartRange = startRange;
            EndRange = endRange;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxStartRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormattedListBoxItem item = listBoxStartRange.SelectedItem as FormattedListBoxItem;
            if (item == null) return;
            DayOfOperation day = item.ReferenceObject as DayOfOperation;
            if (day == null) return;
            StartRange = day.StartOfDay;
            PopulateEndRange(day);
        }

        private void PopulateEndRange(DayOfOperation selectedDay)
        {
            listBoxEndRange.SelectedItem = null;
            listBoxEndRange.Items.Clear();
            DayOfOperation lastDay = DayOfOperation.Get(DayOfOperation.Count());
            for (int id = selectedDay.Id; id <= lastDay.Id; id++)
            {
                DayOfOperation day = DayOfOperation.Get(id);
                listBoxEndRange.Items.Insert(0, 
                    new FormattedListBoxItem(day, FormatDay(day), true));
            }
            listBoxEndRange.ScrollToEnd();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxEndRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {
                FormattedListBoxItem item = listBoxEndRange.SelectedItem as FormattedListBoxItem;
                if (item != null)
                {
                    DayOfOperation day = item.ReferenceObject as DayOfOperation;
                    buttonSelectSpecified.IsEnabled = true;
                    if (day != null && day.EndOfDay == null)
                        EndRange = DateTime.Now;
                    else if (day != null)
                        EndRange = day.EndOfDay.Value;
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonSelectSpecified_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllThisYear_Click(object sender, RoutedEventArgs e)
        {
            DayOfOperation first = DayOfOperation.GetEarliestInYear(DayOfOperation.CurrentYear);
            DayOfOperation last = DayOfOperation.GetLatestInYear(DayOfOperation.CurrentYear);

            StartRange = first.StartOfDay;
            EndRange = last.EndOfDay == null ? DateTime.Now : last.EndOfDay.Value;

            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonAll_Click(object sender, RoutedEventArgs e)
        {
            DayOfOperation first = DayOfOperation.Get(1);
            DayOfOperation last = DayOfOperation.Get(DayOfOperation.Count());

            StartRange = first.StartOfDay;
            EndRange = last.EndOfDay == null ? DateTime.Now : last.EndOfDay.Value;

            Window.GetWindow(this).Close();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            DayOfOperationRangeSelectionControl control = new DayOfOperationRangeSelectionControl();
            return new PosDialogWindow(control, Strings.SelectedRange, 710, 530);
        }

    }
}
