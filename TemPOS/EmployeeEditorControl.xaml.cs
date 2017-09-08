using System;
using System.Collections.Generic;
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
    /// Interaction logic for EmployeeEditorControl.xaml
    /// </summary>
    public partial class EmployeeEditorControl
    {
        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                employeeDetailsControl.SelectedEmployee = value;
                buttonTerminate.IsEnabled =
                    groupBoxProperties.IsEnabled =
                    (value != null);
                buttonModifyJobs.IsEnabled =
                    ((value != null) && !value.IsTerminated);
                buttonTerminate.Text =
                    (buttonModifyJobs.IsEnabled ?
                    Strings.EmployeeEditorTerminateEmployee : 
                    Strings.EmployeeEditorRehireEmployee);
                buttonRemove.IsEnabled = ((value != null) && value.IsTerminated);
            }

        }

        public EmployeeEditorControl()
        {
            InitializeComponent();
            InitializeEmployeeListBox();
            Loaded += EmployeeEditorControl_Loaded;
            
        }

        [Obfuscation(Exclude = true)]
        void EmployeeEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null) window.Closed += EmployeeEditorControl_Closed;
        }

        [Obfuscation(Exclude = true)]
        void EmployeeEditorControl_Closed(object sender, EventArgs e)
        {
            // Update the active employee so permission changes to an ActiveEmployee
            // are reflected
            SessionManager.ActiveEmployee =
                Employee.Get(SessionManager.ActiveEmployee.Id);
        }

        private void InitializeEmployeeListBox()
        {
            foreach (Employee employee in EmployeeManager.GetAllEmployees())
            {
                AddListEntry(employee, PersonManager.GetPerson(employee.PersonId));
            }
        }

        private FormattedListBoxItem AddListEntry(Employee employee, Person person)
        {
            var item = new FormattedListBoxItem(employee, 
                    person.LastName + ", " + person.FirstName, true);
            listBoxEmployees.Items.Add(item);
            return item;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set the SelectedEmployee
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            SelectedEmployee = ((Employee)((FormattedListBoxItem)e.AddedItems[0]).ReferenceObject);
        }

        [Obfuscation(Exclude = true)]
        private void employeeDetailsControl_DetailsChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void RehireEmployee()
        {
            if (PosDialogWindow.ShowDialog(
                Strings.EmployeeEditorConfirmRehire,
                Strings.Confirmation, DialogButtons.YesNo) != DialogButton.Yes) return;
            
            if (SelectedEmployee.Rehire(DateTime.Now))
            {
                SelectedEmployee = SelectedEmployee;
            }
        }

        private void SetEditMode(bool inEditMode)
        {
            if (inEditMode || (listBoxEmployees.SelectedItem == null))
                groupBoxProperties.IsEnabled = inEditMode;
            groupBoxList.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonModifyJobs.IsEnabled = !inEditMode;
            buttonTerminate.IsEnabled = !inEditMode;
            buttonUpdate.IsEnabled = inEditMode;
            buttonCancelChanges.IsEnabled = inEditMode;
            
            var parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null) parentWindow.SetButtonsEnabled(!inEditMode);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            listBoxEmployees.SelectedItem = null;
            SetEditMode(true);
            employeeDetailsControl.SelectedEmployee = null;
            employeeDetailsControl.textBoxFirstName.Focus();
        }

        [Obfuscation(Exclude = true)]
        private void buttonModifyJobs_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee == null)
            {
                buttonModifyJobs.IsEnabled = false;
                return;
            }
            PosDialogWindow window = EmployeeJobSelectionControl.CreateInDefaultWindow(SelectedEmployee);
            window.ShowDialog(Window.GetWindow(this));
        }

        [Obfuscation(Exclude = true)]
        private void buttonTerminate_Click(object sender, RoutedEventArgs e)
        {
            if ((SelectedEmployee == null) || (SelectedEmployee.IsTerminated))
            {
                RehireEmployee();
                return;
            }
            if (SelectedEmployee.Id == SessionManager.ActiveEmployee.Id)
            {
                PosDialogWindow.ShowDialog(
                Strings.EmployeeEditorCantTerminateSelf, Strings.Error);
                return;
            }
            if (PosDialogWindow.ShowDialog(
                Strings.EmployeeEditorConfirmTerminate,
                Strings.Confirmation, DialogButtons.YesNo) == DialogButton.Yes)
            {
                if (SelectedEmployee.Terminate(DateTime.Now))
                {
                    SelectedEmployee = SelectedEmployee;
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee == null)
            {
                buttonRemove.IsEnabled = false;
                return;
            }
            if (!SelectedEmployee.IsTerminated)
            {
                PosDialogWindow.ShowDialog(
                Strings.EmployeeEditorTerminateFirst, Strings.Error);
                return;
            }
            if (PosDialogWindow.ShowDialog(
                Strings.EmployeeEditorConfirmRemove,
                Strings.Confirmation, DialogButtons.YesNo) == DialogButton.Yes)
            {
                if (EmployeeManager.RemoveEmployee(SelectedEmployee))
                {
                    listBoxEmployees.Items.Remove(listBoxEmployees.SelectedItem);
                    listBoxEmployees.SelectedItem = null;
                    SelectedEmployee = null;
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (employeeDetailsControl.SelectedEmployee == null)
            {
                Employee employee = employeeDetailsControl.Add();
                if (employee == null)
                    return;
                SelectedEmployee = employee;
                FormattedListBoxItem listItem =
                    AddListEntry(SelectedEmployee, employeeDetailsControl.SelectedPerson);
                listBoxEmployees.SelectedItem = listItem;
                listBoxEmployees.UpdateLayout();
                if (listItem != null)
                {
                    listBoxEmployees.SelectedItem = listItem;
                    listBoxEmployees.ScrollToEnd();
                }
            }
            else
            {
                if (!employeeDetailsControl.Update())
                    return;
                var formattedListBoxItem = listBoxEmployees.SelectedItem as FormattedListBoxItem;
                if ((listBoxEmployees.SelectedItem != null) && (formattedListBoxItem != null))
                {
                    formattedListBoxItem.Text =
                        employeeDetailsControl.SelectedPerson.LastName + ", " +
                        employeeDetailsControl.SelectedPerson.FirstName;
                }
            }
            SetEditMode(false);

            if (SelectedEmployee.ScanCodeData == null)
            {
                PosDialogWindow.ShowDialog(
                    Strings.EmployeeEditorPasswordWarning,
                    Strings.Warning);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            SelectedEmployee = _selectedEmployee;
            SetEditMode(false);
        }

    }
}
