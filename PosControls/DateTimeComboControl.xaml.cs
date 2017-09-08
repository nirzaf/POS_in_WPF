using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateTimeComboControl.xaml
    /// </summary>
    public partial class DateTimeComboControl : UserControl
    {
        #region Licensed Access Only
        static DateTimeComboControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateTimeComboControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool isOK = false;

        public bool IsOK
        {
            get { return isOK; }
            private set { isOK = value; }
        }

        public TextBlockButton ButtonOK
        {
            get { return buttonOk; }
        }

        public TextBlockButton ButtonCancel
        {
            get { return buttonCancel; }
        }

        public StackPanel StackPanelButtons
        {
            get { return stackPanelButtons; }
        }

        public DateTime? SelectedDateTime
        {
            get
            {
                if ((dateEntryDateControl.SelectedDay == null) ||
                    !dateEntryDateControl.SelectedDay.HasValue)
                    return null;
                DateTime value = dateEntryDateControl.SelectedDay.Value;
                TimeSpan timeValue = timeEntryTimeControl.TimeOfDay;
                DateTime result = new DateTime(value.Year, value.Month, value.Day,
                    timeValue.Hours, timeValue.Minutes, 0);
                return result;
            }
            set
            {
                dateEntryDateControl.SelectedDay = value;
                if ((value != null) && value.HasValue)
                    timeEntryTimeControl.TimeOfDay = value.Value.TimeOfDay;
            }
        }

        public DateTimeComboControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        public void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsOK = true;
            Window.GetWindow(this).Close();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        public void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
