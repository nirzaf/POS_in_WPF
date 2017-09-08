using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosModels;
using PosModels.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryChangeEmployeeControl.xaml
    /// </summary>
    public partial class OrderEntryChangeEmployeeControl
    {
        public Ticket SelectedTicket
        {
            get;
            private set;
        }

        public Employee SelectedEmployee
        {
            get;
            private set;
        }

        public OrderEntryChangeEmployeeControl()
        {
            InitializeComponent();
        }

        public void InitializeListBox(Ticket selectedTicket)
        {
            SelectedTicket = selectedTicket;
            foreach (Employee employee in EmployeeManager.GetAllEmployees())
            {
                if ((selectedTicket.EmployeeId != employee.Id) &&
                    !employee.IsTerminated)
                //(EmployeeTimesheet.GetOpenEntryForEmployee(employee.Id) != null))
                {
                    Person person = Person.Get(employee.PersonId);
                    var createdItem =
                        new FormattedListBoxItem(employee, person.FirstName + " " +
                            person.LastName, true);
                    listBox.Items.Add(createdItem);
                }
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            button.IsEnabled = true;
            var formattedListBoxItem = listBox.SelectedItem as FormattedListBoxItem;
            if (formattedListBoxItem != null)
            {
                var listEmployee = formattedListBoxItem.ReferenceObject as Employee;
                if (listEmployee != null)
                    SelectedEmployee = listEmployee;
            }
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedEmployee != null)
            {
                SelectedTicket.SetEmployeeId(SelectedEmployee.Id);
                SelectedTicket.Update();
            }
            Window window1 = Window.GetWindow(this);
            if (window1 != null) window1.Close();
        }
    }
}
