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
using PosModels;
using TemPOS.Managers;
using PosControls;
using PosModels.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for RegisterBankrollDriverControl.xaml
    /// </summary>
    public partial class RegisterBankrollDriverControl : UserControl
    {
        public RegisterBankrollDriverControl()
        {
            InitializeComponent();
        }

        // Todo: Use for bankrolling, not dispatching
        private void LoadsAvailableDriversRatherThanBankrolledDrivers()
        {
            List<EmployeeTimesheet> clockedIn = new List<EmployeeTimesheet>(EmployeeTimesheet.GetAllOpen());
            List<int> activeDeliveryDrivers = new List<int>(DeliveryDriver.GetAllActiveEmployeeIds());
            List<int> dispatchableJobIds = new List<int>(EmployeeJob.GetDispatchableJobIds());
            foreach (EmployeeTimesheet timesheet in clockedIn)
            {
                if (dispatchableJobIds.Contains(timesheet.JobId) &&
                    !activeDeliveryDrivers.Contains(timesheet.EmployeeId))
                {
                    Person person = PersonManager.GetPersonByEmployeeId(timesheet.EmployeeId);
                    //listBoxDrivers.Items.Add(new FormattedListBoxItem(0, person.FirstName + " " +
                    //    person.LastName, true));
                }
            }
        }

    }
}
