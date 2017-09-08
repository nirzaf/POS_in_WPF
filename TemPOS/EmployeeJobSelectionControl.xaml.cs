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
    /// Interaction logic for EmployeeJobSelectionControl.xaml
    /// </summary>
    public partial class EmployeeJobSelectionControl : UserControl
    {
        public Employee SelectedEmployee
        {
            get;
            set;
        }

        public EmployeeJobSelectionControl(Employee selectedEmployee)
        {
            SelectedEmployee = selectedEmployee;
            InitializeComponent();
            InitializeJobsList();
        }

        private void InitializeJobsList()
        {
            List<EmployeeJob> employeeJobs = new List<EmployeeJob>(SelectedEmployee.GetJobs());
            List<EmployeeJob> filterList = (
                from job in EmployeeJob.GetAll() 
                let found = employeeJobs.Any(employeeJob => employeeJob.Id == job.Id) 
                where !found select job).ToList();
            foreach (FormattedListBoxItem item in filterList
                .Select(job => new FormattedListBoxItem(job, job.Description, true)))
            {
                listBoxAllJobs.Items.Add(item);
            }
            foreach (FormattedListBoxItem item in
                from employeeJob in employeeJobs
                let pay = EmployeePayRate.GetEmployeePayRateForJob(SelectedEmployee.Id, employeeJob.Id)
                select GetJobListBoxItem(employeeJob, pay))
            {
                listBoxSelectedJobs.Items.Add(item);
            }
        }

        private FormattedListBoxItem GetJobListBoxItem(EmployeeJob employeeJob, EmployeePayRate pay)
        {
            return new FormattedListBoxItem(employeeJob,
                        employeeJob.Description + Environment.NewLine +
                        pay.Wage.ToString("C2"), true);
        }

        private void RemoveJob()
        {
            if (listBoxSelectedJobs.SelectedItem == null)
                return;
            FormattedListBoxItem item = listBoxSelectedJobs.SelectedItem as FormattedListBoxItem;
            if (item != null)
            {
                EmployeeJob job = item.ReferenceObject as EmployeeJob;
                if (job != null)
                {
                    EmployeePayRate.Delete(SelectedEmployee.Id, job.Id);
                    listBoxSelectedJobs.Items.Remove(listBoxSelectedJobs.SelectedItem);
                    item = new FormattedListBoxItem(job, job.Description, true);
                    listBoxAllJobs.Items.Add(item);
                    listBoxAllJobs.SelectedItem = item;
                    listBoxAllJobs.ScrollToEnd();
                }
            }
            buttonRemove.IsEnabled = false;
            buttonAdd.IsEnabled = true;
        }

        private void AddJob()
        {
            if (listBoxAllJobs.SelectedItem == null)
                return;
            double? payRate = PosDialogWindow.PromptCurrency(Strings.EmployeeJobEditorPayRate, 0.0);
            if (payRate == null) return;
            FormattedListBoxItem item = listBoxAllJobs.SelectedItem as FormattedListBoxItem;
            if (item != null)
            {
                EmployeeJob job = item.ReferenceObject as EmployeeJob;
                if (job != null)
                {
                    EmployeePayRate rate =
                        EmployeePayRate.Add(SelectedEmployee.Id, job.Id, payRate.Value, 0);
                    listBoxAllJobs.Items.Remove(listBoxAllJobs.SelectedItem);
                    item = GetJobListBoxItem(job, rate);
                    listBoxSelectedJobs.Items.Add(item);
                    listBoxSelectedJobs.SelectedItem = item;
                    listBoxSelectedJobs.ScrollToEnd();
                }
            }
            buttonAdd.IsEnabled = false;
            buttonRemove.IsEnabled = true;
        }

        private void EditEmployeePayRate()
        {
            double? payRate = PosDialogWindow.PromptCurrency(Strings.EmployeeJobEditorPayRate, 0.0);
            if (payRate != null)
            {
                FormattedListBoxItem item = listBoxSelectedJobs.SelectedItem as FormattedListBoxItem;
                if (item != null)
                {
                    EmployeeJob job = item.ReferenceObject as EmployeeJob;
                    EmployeePayRate rate =
                        EmployeePayRate.GetEmployeePayRateForJob(SelectedEmployee.Id, job.Id);
                    rate.SetWage(payRate.Value);
                    rate.Update();
                    item.Set(job, job.Description + Environment.NewLine +
                                  rate.Wage.ToString("C2"));
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBoxAllJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxSelectedJobs.SelectedItem = null;
            buttonAdd.IsEnabled = true;
            buttonEditPayRate.IsEnabled =
                buttonRemove.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxSelectedJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxAllJobs.SelectedItem = null;
            buttonAdd.IsEnabled = false;
            buttonEditPayRate.IsEnabled =
                buttonRemove.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddJob();
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveJob();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEditPayRate_Click(object sender, RoutedEventArgs e)
        {
            EditEmployeePayRate();
        }

        public static PosDialogWindow CreateInDefaultWindow(Employee employee)
        {
            Person person = PersonManager.GetPerson(employee.PersonId);
            EmployeeJobSelectionControl control = new EmployeeJobSelectionControl(employee);
            return new PosDialogWindow(control, Strings.EmployeeJobEditorSelectJobsFor + " \'" +
                person.LastName + ", " + person.FirstName + "\'", 450, 550);
        }
    }
}
