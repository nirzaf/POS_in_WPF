using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosControls.Interfaces;
using PosModels.Managers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EmployeeClockOutControl.xaml
    /// </summary>
    public partial class EmployeeClockOutControl : UserControl
    {
        private double? _tipsDeclared;
        private readonly EmployeeTimesheet _clockIn =
            EmployeeTimesheet.GetOpenEntryForEmployee(
            SessionManager.ActiveEmployee.Id);
        private DateTime _clockOutTime = DateTime.Now;

        public EmployeeClockOutControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            Person person = PersonManager.GetPersonByEmployeeId(
                SessionManager.ActiveEmployee.Id);
            timeEntryTimeControl.TimeOfDay = _clockOutTime.TimeOfDay;
            labelName.Content = person.FirstName + ", " + person.LastName;
            EmployeeJob job = EmployeeJob.Get(_clockIn.JobId);
            if (!job.HasTips)
                buttonDeclareTips.Visibility = Visibility.Collapsed;
        }

        private void DeclareTips()
        {
            IShadeable parentWindow = Window.GetWindow(this) as IShadeable;
            if (parentWindow != null) parentWindow.ShowShadingOverlay = true;
            _tipsDeclared = PosDialogWindow.PromptCurrency(
                Strings.ClockOutDeclareTips, _tipsDeclared);
            if (parentWindow != null) parentWindow.ShowShadingOverlay = false;
            if (_tipsDeclared == null)
                buttonDeclareTips.Text = Strings.ClockOutDeclareTips;
            else
                buttonDeclareTips.Text = Strings.ClockOutTips + _tipsDeclared.Value.ToString("C2");
        }

        private void ClockOut()
        {
            _clockIn.SetEndTime(_clockOutTime);
            _clockIn.SetDeclaredTipAmount(_tipsDeclared);
            _clockIn.Update();
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDeclareTips_Click(object sender, RoutedEventArgs e)
        {
            DeclareTips();
        }

        [Obfuscation(Exclude = true)]
        private void buttonClockOut_Click(object sender, RoutedEventArgs e)
        {
            ClockOut();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            EmployeeClockOutControl control = new EmployeeClockOutControl();
            return new PosDialogWindow(control, Strings.ClockOut, 310, 425);
        }

    }
}
