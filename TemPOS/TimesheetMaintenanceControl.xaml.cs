using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosControls.Helpers;
using PosModels.Managers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TimesheetMaintenanceControl.xaml
    /// </summary>
    public partial class TimesheetMaintenanceControl : UserControl
    {
        public class TimesheetMaintenanceDataModel
        {
            private int EmployeeId = 0;
            private int JobId = 0;

            public int Id { get; set; }
            public string EmployeeName { get; set; }
            public string JobName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public double? DeclaredTipAmount { get; set; }
            public double? DriverCompensation { get; set; }
            public bool IsLocked { get; set; }
            
            public TimesheetMaintenanceDataModel(int id, int employeeId,
                string employeeName, int jobId, DateTime startTime, DateTime? endTime,
                double? declaredTipAmount, double? driverCompensation, bool isLocked)
            {
                Id = id;
                EmployeeId = employeeId;
                EmployeeName = employeeName;
                SetJobId(jobId);
                StartTime = startTime;
                EndTime = endTime;
                DeclaredTipAmount = declaredTipAmount;
                DriverCompensation = driverCompensation;
                IsLocked = isLocked;
            }

            public int GetEmployeeId()
            {
                return EmployeeId;
            }

            public int GetJobId()
            {
                return JobId;
            }

            public void SetJobId(int jobId)
            {
                EmployeeJob job = EmployeeJob.Get(jobId);
                JobId = jobId;
                JobName = job.Description;
            }
        }

        private class TimesheetMaintenanceDataModelCompare : IComparer<TimesheetMaintenanceDataModel>
        {
            private ListSortDirection Direction;
            public TimesheetMaintenanceDataModelCompare(ListSortDirection direction)
            {
                Direction = direction;
            }
            public int Compare(TimesheetMaintenanceDataModel x, TimesheetMaintenanceDataModel y)
            {
                if (Direction == ListSortDirection.Ascending)
                    return x.StartTime.CompareTo(y.StartTime);
                return y.StartTime.CompareTo(x.StartTime);
            }
        }

        private List<TimesheetMaintenanceDataModel> dataSet;

        public TimesheetMaintenanceControl()
        {
            InitializeComponent();
            dataSet = new List<TimesheetMaintenanceDataModel>();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dataSet.Clear();
            IEnumerable<EmployeeTimesheet> timesheetEntries = EmployeeTimesheet.GetAll();
            foreach (EmployeeTimesheet timesheetEntry in timesheetEntries)
            {
                if (timesheetEntry.IsDeleted)
                    continue;
                int personId = Employee.GetPersonId(timesheetEntry.EmployeeId);
                Person person = PersonManager.GetPerson(personId);                
                TimesheetMaintenanceDataModel model = new TimesheetMaintenanceDataModel(
                    timesheetEntry.Id,
                    timesheetEntry.EmployeeId,
                    person.LastName + ", " + person.FirstName,
                    timesheetEntry.JobId,
                    timesheetEntry.StartTime,
                    timesheetEntry.EndTime,
                    timesheetEntry.DeclaredTipAmount,
                    timesheetEntry.DriverCompensationAmount,
                    timesheetEntry.IsLocked);

                dataSet.Add(model);
            }
            dataSet.Sort(new TimesheetMaintenanceDataModelCompare(ListSortDirection.Descending));
            dataGrid.ItemsSource = dataSet;       
        }

        [Obfuscation(Exclude = true)]
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            bool employeeWasClockedOut = false;
            e.Cancel = true;
            if (dataSet[dataGrid.SelectedIndex].IsLocked)
            {
                PosDialogWindow.ShowDialog(
                    Strings.ThisRecordIsLockedAndCanNotBeChanged, Strings.EntryLocked);
                return;
            }
            if ((dataSet[dataGrid.SelectedIndex].GetEmployeeId() == 
                SessionManager.ActiveEmployee.Id) &&
                (dataSet[dataGrid.SelectedIndex].EndTime == null))
            {
                PosDialogWindow.ShowDialog(
                    Strings.ThisRecordIsYourCurrentClockInAndCanNotBeChangedUntilYouClockedOut, Strings.EmployeeNotClockedOut);
                return;
            }
            if (dataSet[dataGrid.SelectedIndex].EndTime == null)
            {
                if (PosDialogWindow.ShowDialog(
                    Strings.ThisEmployeeMustClockOutBeforeYouCanEditWouldYouLikeToClockOutThisEmployeeRightNow, Strings.EmployeeNotClockedOut, DialogButtons.YesNo) ==
                    DialogButton.Yes)
                {
                    DateTime endTime = DateTime.Now;
                    dataSet[dataGrid.SelectedIndex].EndTime = endTime;
                    dataSet[dataGrid.SelectedIndex].DeclaredTipAmount = 0;
                    //dataSet[dataGrid.SelectedIndex].DriverCompensation = 0;
                    employeeWasClockedOut = true;
                }
                else
                    return;
            }
            // Show the dialog for editing a grid row
            PosDialogWindow window = TimesheetEditorControl.CreateInDefaultWindow();
            TimesheetEditorControl control = window.DockedControl as TimesheetEditorControl;

            control.DataModel = dataSet[dataGrid.SelectedIndex];
            window.ShowDialog(Window.GetWindow(this));

            if (control.IsModelDeleted)
                DeleteModel(control.DataModel);
            else if (control.IsModelUpdated || employeeWasClockedOut)
                UpdateModel(control.DataModel);
        }

        private void UpdateModel(TimesheetMaintenanceDataModel model)
        {
            EmployeeTimesheet entry = EmployeeTimesheet.Get(model.Id);

            LogChange(model, entry);
            entry.SetDeclaredTipAmount(model.DeclaredTipAmount);
            entry.SetDriverCompensationAmount(model.DriverCompensation);
            entry.SetStartTime(model.StartTime);
            entry.SetEndTime(model.EndTime);
            entry.SetJobId(model.GetJobId());
            entry.Update();
            dataGrid.Items.Refresh();
        }

        private void DeleteModel(TimesheetMaintenanceDataModel model)
        {
            EmployeeTimesheet entry = EmployeeTimesheet.Get(model.Id);

            LogChange(entry);
            entry.SetIsDeleted(true);
            entry.Update();
            dataSet.Remove(model);
            dataGrid.Items.Refresh();
        }

        private void LogChange(EmployeeTimesheet entry)
        {
            // For deletions, values are not changed, so all nullable fields are null
            EmployeeTimesheetChangeLog.Add(entry.Id, SessionManager.ActiveEmployee.Id, null,
                null, null, null, null);
        }

        private void LogChange(TimesheetMaintenanceDataModel model, EmployeeTimesheet entry)
        {
            DateTime? oldStartTime = null;
            DateTime? oldEndTime = null;
            int? oldJob = null;
            double? oldTips = null;
            double? oldDriverCompensation = null;

            if (model.StartTime != entry.StartTime)
                oldStartTime = entry.StartTime;
            if (model.EndTime != entry.EndTime)
                oldEndTime = entry.EndTime;
            if (model.GetJobId() != entry.JobId)
                oldJob = entry.JobId;
            if (model.DeclaredTipAmount != entry.DeclaredTipAmount)
                oldTips = entry.DeclaredTipAmount;
            if (model.DriverCompensation != entry.DriverCompensationAmount)
                oldDriverCompensation = entry.DriverCompensationAmount;

            EmployeeTimesheetChangeLog.Add(entry.Id, SessionManager.ActiveEmployee.Id,
                oldStartTime, oldEndTime, oldJob, oldTips, oldDriverCompensation);
        }

        [Obfuscation(Exclude = true)]
        private void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid.Items.Refresh();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            TimesheetMaintenanceControl control = new TimesheetMaintenanceControl();
            return new PosDialogWindow(control, Strings.TimesheetMaintenance, 1000, 620);
        }

    }
}
