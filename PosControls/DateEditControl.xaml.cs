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
using PosControls.Helpers;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateEditControl.xaml
    /// </summary>
    public partial class DateEditControl : UserControl
    {
        #region Licensed Access Only
        static DateEditControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEditControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool haltEvents = false;
        private DateTime currentDateTime = DateTime.Now;

        [Obfuscation(Exclude = true)]
        public event EventHandler CurrentDateTimeChanged;
        public bool IsOutOfRange
        {
            get;
            private set;
        }

        public int Month
        {
            get
            {
                return CurrentDateTime.Month;
            }
            set
            {
                DateTime originalCurrentDateTime = CurrentDateTime;
                bool originalIsOutOfRange = IsOutOfRange;
                try
                {
                    CurrentDateTime = new DateTime(CurrentDateTime.Year, value, CurrentDateTime.Day);
                    IsOutOfRange = false;
                }
                catch (Exception)
                {
                    IsOutOfRange = true;
                }
                DoCurrentDateTimeChangedEvent(originalCurrentDateTime, originalIsOutOfRange);
            }
        }

        public int Day
        {
            get
            {
                return CurrentDateTime.Day;
            }
            set
            {
                DateTime originalCurrentDateTime = CurrentDateTime;
                bool originalIsOutOfRange = IsOutOfRange;
                try
                {
                    CurrentDateTime = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, value);
                    IsOutOfRange = false;
                }
                catch (Exception)
                {
                    IsOutOfRange = true;
                }
                DoCurrentDateTimeChangedEvent(originalCurrentDateTime, originalIsOutOfRange);
            }

        }

        public int Year
        {
            get
            {
                return CurrentDateTime.Year;
            }
            set
            {
                DateTime originalCurrentDateTime = CurrentDateTime;
                bool originalIsOutOfRange = IsOutOfRange;
                try
                {
                    CurrentDateTime = new DateTime(value, CurrentDateTime.Month, CurrentDateTime.Day);
                    IsOutOfRange = false;
                }
                catch (Exception)
                {
                    IsOutOfRange = true;
                }
                DoCurrentDateTimeChangedEvent(originalCurrentDateTime, originalIsOutOfRange);
            }
        }

        private void DoCurrentDateTimeChangedEvent(DateTime originalCurrentDateTime, bool originalIsOutOfRange)
        {
            if (IsInitialized && (CurrentDateTimeChanged != null) &&
                (!originalCurrentDateTime.Equals(CurrentDateTime) || (originalIsOutOfRange != IsOutOfRange)))
                CurrentDateTimeChanged.Invoke(this, new EventArgs());
        }

        public DateTime CurrentDateTime
        {
            get { return currentDateTime; }
            set
            {
                currentDateTime = value;
                haltEvents = true;
                textBoxMonth.Text = "" + Month;
                textBoxDay.Text = "" + Day;
                comboBoxYear.SelectedIndex = Year - DateTime.Now.Year;
                UpdateOnScreenEditLabel();
                haltEvents = false;
            }
        }

        public DateEditControl()
        {
            IsOutOfRange = false;
            InitializeComponent();
            if (!ConfigurationManager.UseOnScreenTextEntry)
            {
                InitializeTextBoxes();
                InitializeYears();
            }
            else
            {
                gridPanel.Visibility = Visibility.Hidden;
                dockPanel.Visibility = Visibility.Visible;
                UpdateOnScreenEditLabel();
            }
        }

        private void UpdateOnScreenEditLabel()
        {
            labelDate.Content = CurrentDateTime.ToString("MM-dd-yyyy");
        }

        private void InitializeTextBoxes()
        {
            textBoxDay.Text = "" + CurrentDateTime.Day;
            textBoxMonth.Text = "" + CurrentDateTime.Month;
        }

        private void InitializeYears()
        {
            int year = DateTime.Now.Year;
            comboBoxYear.Items.Clear();
            for (int i = year; i < (year + 5); i++)
            {
                comboBoxYear.Items.Add("" + i);
            }
            comboBoxYear.SelectedIndex = 0;
        }

        private void PromptForDate()
        {
            Window window = Window.GetWindow(this);
            if (window is PosDialogWindow)
            {
                PosDialogWindow dialogWindow = (PosDialogWindow)window;
                dialogWindow.ShowShadingOverlay = true;
                DateTime? date = PosDialogWindow.PromptForDay("Enter Date", CurrentDateTime);
                if (date.HasValue)
                {
                    Year = date.Value.Year;
                    Month = date.Value.Month;
                    Day = date.Value.Day;
                }
                dialogWindow.ShowShadingOverlay = false;
            }
            else
            {
                DateTime? date = PosDialogWindow.PromptForDay("Enter Date", CurrentDateTime);
                if (date.HasValue)
                {
                    Year = date.Value.Year;
                    Month = date.Value.Month;
                    Day = date.Value.Day;
                }
            }
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void textBoxMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!haltEvents)
            {
                try
                {
                    Month = Convert.ToInt32(textBoxMonth.Text);
                }
                catch { }
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxDay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!haltEvents)
            {
                try
                {
                    Day = Convert.ToInt32(textBoxDay.Text);
                }
                catch { }
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxDay_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !e.Key.IsNumber();
        }

        [Obfuscation(Exclude = true)]
        private void stackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PromptForDate();
            comboBoxYear.Focus();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!haltEvents)
            {
                string text = comboBoxYear.SelectedItem;
                Year = Convert.ToInt32(text);
            }
        }

    }
}
