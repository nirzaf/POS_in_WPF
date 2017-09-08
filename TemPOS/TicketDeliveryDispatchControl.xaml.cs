using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using TemPOS.Types;
using PosControls;
using PosModels.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketDeliveryDispatchControl.xaml
    /// </summary>
    public partial class TicketDeliveryDispatchControl : UserControl
    {
        public TicketDeliveryDispatchControl()
        {
            InitializeComponent();
            InitializeDeliveryDrivers();
            InitializeDeliveryTickets();
        }

        private void InitializeDeliveryDrivers()
        {
            IEnumerable<DeliveryDriver> deliveryDrivers = DeliveryDriver.GetAllActive();
            int[] dispatchedEmployeeIds = GetDispatchedEmployeeIds();
            foreach (DeliveryDriver deliverDriver in deliveryDrivers)
            {
                if (!dispatchedEmployeeIds.Contains(deliverDriver.DriversEmployeeId))
                {
                    Person person = PersonManager.GetPersonByEmployeeId(deliverDriver.DriversEmployeeId);
                    listBoxDrivers.Items.Add(new FormattedListBoxItem(0, person.FirstName + " " +
                    person.LastName, true));
                }
            }
        }

        private int[] GetDispatchedEmployeeIds()
        {
            IEnumerable<TicketDelivery> deliveries = TicketDelivery.GetAllActive();
            return deliveries.Select(delivery => DeliveryDriver.Get(delivery.DeliveryDriverId))
                .Select(driver => driver.DriversEmployeeId).ToArray();
        }

        private void InitializeDeliveryTickets()
        {
            IEnumerable<Ticket> openTickets = TicketManager.GetOpenTickets();
            foreach (Ticket ticket in openTickets)
            {
                if (ticket.Type != TicketType.Delivery)
                    continue;
                Person customerPerson =
                    PersonManager.GetPersonByCustomerId(ticket.CustomerId);
                ZipCode zipCode = ZipCode.Get(customerPerson.ZipCodeId);
                ZipCodeCity zipCodeCity = ZipCodeCity.Get(zipCode.CityId);
                ZipCodeState zipCodeState = ZipCodeState.Get(zipCodeCity.StateId);
                listBoxDeliveries.Items.Add(
                    new FormattedListBoxItem(0, ((ticket.OrderId != null) ? 
                        Types.Strings.OrderNumber + ticket.OrderId.Value + ", " : "") +
                    Types.Strings.TicketNumber + ticket.PrimaryKey.Id + Environment.NewLine +
                    Environment.NewLine +
                    customerPerson.FirstName + " " + customerPerson.LastName + 
                    Environment.NewLine + customerPerson.AddressLine1 +
                    (string.IsNullOrEmpty(customerPerson.AddressLine2) ? "" :
                    Environment.NewLine + customerPerson.AddressLine2) +
                    Environment.NewLine + zipCodeCity.City + ", " +
                    zipCodeState.Abbreviation + " " + zipCode.PostalCode.ToString("D5"), true));
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBoxDrivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckDispatchButtonEnable();
        }
        
        [Obfuscation(Exclude = true)]
        private void listBoxDeliveries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckDispatchButtonEnable();
        }

        private void CheckDispatchButtonEnable()
        {
            buttonDispatch.IsEnabled =
                ((listBoxDrivers.SelectedItem != null) &&
                 (listBoxDeliveries.SelectedItems.Count > 0));
        }

        [Obfuscation(Exclude = true)]
        private void buttonDispatch_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            TicketDeliveryDispatchControl control = new TicketDeliveryDispatchControl();
            return new PosDialogWindow(control, Types.Strings.DeliveryDriverDispatch, 650, 500);
        }

    }
}
