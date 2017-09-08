using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using PosModels;
using PosControls;
using PosControls.Interfaces;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TimesheetEditorControl.xaml
    /// </summary>
    public partial class TimesheetEditorControl : UserControl
    {
        #region Fields
        private bool _haltUpdate;
        private TimesheetMaintenanceControl.TimesheetMaintenanceDataModel _dataModel;
        private double? _driverComp;
        private double? _tipsDeclared;
        #endregion

        #region Properties
        public bool IsModelUpdated
        {
            get;
            set;
        }

        public bool IsModelDeleted
        {
            get;
            set;
        }

        public bool AreBothTimesValid
        {
            get
            {
                if ((dateTimeEditStartTime.SelectedDateTime == null) ||
                    (dateTimeEditEndTime.SelectedDateTime == null))
                    return false;
                return (dateTimeEditStartTime.SelectedDateTime.Value <=
                    dateTimeEditEndTime.SelectedDateTime.Value);
            }
        }

        public TimesheetMaintenanceControl.TimesheetMaintenanceDataModel DataModel
        {
            get { return _dataModel; }
            set
            {
                _dataModel = value;
                if (value == null)
                    return;
                dateTimeEditStartTime.SelectedDateTime = _dataModel.StartTime;
                dateTimeEditEndTime.SelectedDateTime = _dataModel.EndTime;
                _tipsDeclared = _dataModel.DeclaredTipAmount;
                _driverComp = _dataModel.DriverCompensation;
                buttonTips.Text = _dataModel.DeclaredTipAmount == null ?
                    Strings.None : _dataModel.DeclaredTipAmount.Value.ToString("C2");
                buttonDriverComp.Text = _dataModel.DriverCompensation == null ?
                    Strings.None : _dataModel.DriverCompensation.Value.ToString("C2");
                foreach (FormattedListBoxItem item in Employee.GetJobs(_dataModel.GetEmployeeId())
                    .Select(job => new FormattedListBoxItem(job, job.Description, true)))
                {
                    listBoxJobs.Items.Add(item);
                }
                foreach (FormattedListBoxItem item in
                    from FormattedListBoxItem item in listBoxJobs.Items
                    let job = item.ReferenceObject as EmployeeJob
                    where job != null && job.Id == _dataModel.GetJobId()
                    select item)
                {
                    listBoxJobs.SelectedItem = item;
                    break;
                }
            }
        }
        #endregion

        public TimesheetEditorControl()
        {
            IsModelUpdated = false;
            IsModelDeleted = false;
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            buttonSave.IsEnabled = false;
        }
        
        [Obfuscation(Exclude = true)]
        private void dateTimeEditStartTime_SelectedDateTimeChanged(object sender, EventArgs e)
        {
            if (_haltUpdate)
                return;
            if (AreBothTimesValid)
            {
                buttonSave.IsEnabled = true;
            }
            else
            {
                _haltUpdate = true;
                dateTimeEditStartTime.SelectedDateTime = _dataModel.StartTime;
                _haltUpdate = false;
                PosDialogWindow.ShowDialog(
                    Strings.YouCanNotSetAStartTimeThatOccursAfterTheEndTime,
                    Strings.ValidationError);
            }
        }

        [Obfuscation(Exclude = true)]
        private void dateTimeEditEndTime_SelectedDateTimeChanged(object sender, EventArgs e)
        {
            if (_haltUpdate)
                return;
            if (AreBothTimesValid)
            {
                buttonSave.IsEnabled = true;
            }
            else
            {
                _haltUpdate = true;
                dateTimeEditEndTime.SelectedDateTime = _dataModel.EndTime;
                _haltUpdate = false;
                PosDialogWindow.ShowDialog(
                    Strings.YouCanNotSetAnEndTimeThatOccursBeforeTheStartTime,
                    Strings.ValidationError);
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBoxJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonTips_Click(object sender, RoutedEventArgs e)
        {
            double? initialValue = _dataModel.DeclaredTipAmount;
            IShadeable parentWindow = Window.GetWindow(this) as IShadeable;
            parentWindow.ShowShadingOverlay = true;
            _tipsDeclared = PosDialogWindow.PromptCurrency(
                Strings.DeclareTips, _tipsDeclared);
            parentWindow.ShowShadingOverlay = false;
            if (_tipsDeclared == null)
                buttonTips.Text = Strings.None;
            else
                buttonTips.Text = _tipsDeclared.Value.ToString("C2");
            if (initialValue != _tipsDeclared)
                buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonDriverComp_Click(object sender, RoutedEventArgs e)
        {
            double? initialValue = _dataModel.DriverCompensation;
            _driverComp = PosDialogWindow.PromptCurrency(Strings.DriverCompensation, _driverComp);
            buttonDriverComp.Text = _driverComp == null ? Strings.None : _driverComp.Value.ToString("C2");
            if (initialValue != _driverComp)
                buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            // Confirm
            if (PosDialogWindow.ShowDialog(
                Strings.AreYouSureYouWantToDeleteThisRecordThisActionCanNotBeUndone, Strings.Confirm, DialogButtons.YesNo) !=
                DialogButton.Yes)
                return;
            IsModelDeleted = true;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (dateTimeEditEndTime.SelectedDateTime != null &&
                dateTimeEditStartTime.SelectedDateTime != null &&
                !EmployeeTimesheet.IsOverlapping(_dataModel.Id, _dataModel.GetEmployeeId(),
                    dateTimeEditStartTime.SelectedDateTime.Value, dateTimeEditEndTime.SelectedDateTime.Value))
            {
                FormattedListBoxItem item = listBoxJobs.SelectedItem as FormattedListBoxItem;
                if (item != null)
                {
                    EmployeeJob job = item.ReferenceObject as EmployeeJob;
                    if (job != null)
                    {
                        _dataModel.SetJobId(job.Id);
                        _dataModel.StartTime = dateTimeEditStartTime.SelectedDateTime.Value;
                        _dataModel.EndTime = dateTimeEditEndTime.SelectedDateTime.Value;
                        _dataModel.DeclaredTipAmount = _tipsDeclared;
                        _dataModel.DriverCompensation = _driverComp;
                    }
                }
                IsModelUpdated = true;
                Window.GetWindow(this).Close();
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Strings.TheTimesSpecifiedWouldOverlapAnExistingShift, Strings.ValidationError);
            }

        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            TimesheetEditorControl control = new TimesheetEditorControl();
            return new PosDialogWindow(control, Strings.EditEntry, 760, 430);
        }

    }
}
