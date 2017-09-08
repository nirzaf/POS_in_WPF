using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using TemPOS.Commands;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for FutureTimeEditControl.xaml
    /// </summary>
    public partial class FutureTimeEditControl : UserControl
    {
        public DateTime? SelectedDateTime
        {
            get { return dateTimeComboControl.SelectedDateTime; }
            set { dateTimeComboControl.SelectedDateTime = value; }
        }

        public bool KeepChanges
        {
            get;
            private set;
        }

        public FutureTimeEditControl()
        {
            InitializeComponent();
            InsertMakeNowButton();
            ReInitializeOtherButtons();
            SelectedDateTime = DateTime.Now;
        }

        private void ReInitializeOtherButtons()
        {
            dateTimeComboControl.ButtonOK.Text = Strings.FutureTimeSetTime;
            dateTimeComboControl.ButtonOK.Click -=
                dateTimeComboControl.buttonOk_Click;
            dateTimeComboControl.ButtonOK.Click += buttonOk_Click;
            dateTimeComboControl.ButtonCancel.Click -=
                dateTimeComboControl.buttonCancel_Click;
            dateTimeComboControl.ButtonCancel.Click += buttonCancel_Click;
        }

        private void InsertMakeNowButton()
        {
            TextBlockButton button = new TextBlockButton();
            button.Text = Strings.FutureTimeMakeNow;
            button.Margin = new Thickness(5, 0, 5, 0);
            button.Click += buttonMakeNow_Click;
            button.Width = 70;
            button.Height = 64;
            dateTimeComboControl.StackPanelButtons.Children.Insert(0, button);
        }

        [Obfuscation(Exclude = true)]
        void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            KeepChanges = false;
            // Closes the dialog window
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // Yuck, but it works
            DateTime nowTime = DateTime.Now;
            int? intValue = OrderEntryCommands.GetPrepTime().IntValue;
            if (intValue != null)
            {
                DateTime earliestTime = nowTime +
                    new TimeSpan(0, intValue.Value - 1, 60 - nowTime.Second);
                if (SelectedDateTime != null &&
                    SelectedDateTime.Value < (earliestTime - new TimeSpan(0, 1, 0)))
                {
                    PosDialogWindow.ShowDialog(
                        Strings.FutureTimeTooEarly +
                        earliestTime.ToLongTimeString(), Strings.FutureTimeTooEarlyError);
                }
                else
                {
                    KeepChanges = true;
                    // Closes the dialog window
                    Window.GetWindow(this).Close();
                }
            }
        }

        [Obfuscation(Exclude = true)]
        void buttonMakeNow_Click(object sender, RoutedEventArgs e)
        {
            KeepChanges = true;
            SelectedDateTime = null;
            // Closes the dialog window
            Window.GetWindow(this).Close();            
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            FutureTimeEditControl control = new FutureTimeEditControl();
            return new PosDialogWindow(control, Strings.FutureTime, 570, 485);
        }
    }
}
