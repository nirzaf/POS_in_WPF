using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosModels.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EmployeeClockInControl.xaml
    /// </summary>
    public partial class EmployeeClockInControl : UserControl
    {
        private Employee ActiveEmployee { get; set; }
        public EmployeeJob ActiveJob { get; set; }

        public EmployeeClockInControl(Employee activeEmployee)
        {
            ActiveEmployee = activeEmployee;
            InitializeComponent();
            InitializeJobList();
            InitializeFields();
        }

        private void InitializeFields()
        {
            Person person = PersonManager.GetPersonByEmployeeId(
                ActiveEmployee.Id);
            timeEntryTimeControl.TimeOfDay = DateTime.Now.TimeOfDay;
            labelName.Content = person.FirstName + ", " + person.LastName;
        }

        private void InitializeJobList()
        {
            List<EmployeeJob> jobs = new List<EmployeeJob>(ActiveEmployee.GetJobs());
            if (jobs.Count > 0)
            {
                foreach (FormattedListBoxItem item in jobs
                    .Select(job => new FormattedListBoxItem(job, job.Description, true)))
                {
                    listBox1.Items.Add(item);
                }
                FormattedListBoxItem firstItem = listBox1.Items[0] as FormattedListBoxItem;
                listBox1.SelectedItem = firstItem;
                if (firstItem != null) ActiveJob = firstItem.ReferenceObject as EmployeeJob;
            }
            else
                ActiveJob = null;
        }

        private void ClockIn()
        {
            EmployeeTimesheet.Add(ActiveEmployee.Id, ActiveJob.Id);
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                return;
            }
            FormattedListBoxItem selectedItem = listBox1.SelectedItem as FormattedListBoxItem;
            if (selectedItem != null) ActiveJob = selectedItem.ReferenceObject as EmployeeJob;
        }

        [Obfuscation(Exclude = true)]
        private void buttonClockIn_Click(object sender, RoutedEventArgs e)
        {
            ClockIn();
        }

        public static PosDialogWindow CreateInDefaultWindow(Employee employee)
        {
            EmployeeClockInControl control = new EmployeeClockInControl(employee);
            return new PosDialogWindow(control, Strings.ClockIn, 545, 420);
        }
    }
}
