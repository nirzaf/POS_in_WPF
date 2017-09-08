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
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingPersonInformationControl.xaml
    /// </summary>
    public partial class SeatingPersonInformationControl : UserControl
    {
        private ControlMode _controlMode = ControlMode.DineIn;
        private Customer _selectedCustomer;

        private enum ControlMode
        {
            DineIn, // Future-Time Dine-In's
            Carryout,
            Delivery,
            Catering
        }

        public int SelectedRoomId
        {
            get;
            set;
        }

        public int SelectedSeatingId
        {
            get;
            private set;
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                if (value != null)
                {
                    Person person = Person.Get(_selectedCustomer.PersonId);
                    textBoxCustomerName.Text = person.FirstName;
                    if (person.MiddleInitial != null)
                        textBoxCustomerName.Text += " " + person.MiddleInitial;
                    if (!string.IsNullOrEmpty(person.LastName))
                        textBoxCustomerName.Text += " " + person.LastName;
                    PhoneNumber phoneNumber = PhoneNumber.Get(person.PhoneNumberId1);
                    phoneNumberEditControl.Text = phoneNumber.Number;
                    ZipCode zipCode = ZipCode.Get(person.ZipCodeId);
                    textBoxZip.Text =
                        person.ZipCodeId > 0 ? person.ZipCodeId.ToString("D5") : "";
                    textBoxStreetAddress.Text = person.AddressLine1;
                    textBoxStreetAddress2.Text = person.AddressLine2;
                    if (zipCode != null)
                    {
                        ZipCodeCity city = ZipCodeCity.Get(zipCode.CityId);
                        ZipCodeState state = ZipCodeState.Get(city.StateId);
                        textBoxCityState.Text = city.City + ", " + state.Abbreviation;
                    }
                    else
                    {
                        textBoxCityState.Text = "";
                    }
                }
                else
                {
                    textBoxCustomerName.Text = "";
                    //phoneNumberEditControl.Text = "";
                    textBoxZip.Text = "";
                    textBoxStreetAddress.Text = "";
                    textBoxStreetAddress2.Text = "";
                    textBoxCityState.Text = "";
                }
            }
        }

        public Ticket ActiveTicket { get; set; }

        public SeatingPersonInformationControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void phoneNumberEditControl_TextChanged(object sender, RoutedEventArgs e)
        {
            if ((phoneNumberEditControl.Text != null) &&
                (phoneNumberEditControl.Text.Length == 10))
            {
                textBoxCustomerName.IsEnabled = true;
                textBoxStreetAddress.IsEnabled = true;
                textBoxStreetAddress2.IsEnabled = true;
                textBoxZip.IsEnabled = true;
                Customer customer = Customer.GetByPhoneNumber(phoneNumberEditControl.Text);
                SelectedCustomer = customer;
            }
            else
            {
                ResetTextBox(textBoxCustomerName);
                ResetTextBox(textBoxCityState);
                ResetTextBox(textBoxZip);
                ResetTextBox(textBoxStreetAddress);
                ResetTextBox(textBoxStreetAddress2);
            }
            SetStartTicketButton();
        }

        private int ParseName(string str, out string firstName, out char? middleInitial, out string lastName)
        {
            middleInitial = null;
            firstName = "";
            lastName = "";
            if (str == null)
                return 0;
            string[] tokens = str.Split(' ');
            if (tokens.Length > 0)
                firstName = tokens[0];
            if (tokens.Length == 2)
                lastName = tokens[1];
            else if (tokens.Length == 3)
            {
                middleInitial = tokens[1][0];
                lastName = tokens[2];
            }
            return tokens.Length;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxCustomerName_TextChanged(object sender, RoutedEventArgs e)
        {
            SetStartTicketButton();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxStreetAddress_TextChanged(object sender, RoutedEventArgs e)
        {
            SetStartTicketButton();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxStreetAddress2_TextChanged(object sender, RoutedEventArgs e)
        {
            SetStartTicketButton();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxZip_TextChanged(object sender, RoutedEventArgs e)
        {
            SetStartTicketButton();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxCityState_TextChanged(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetStartTicketButton()
        {
            if ((_controlMode == ControlMode.Carryout) ||
                (_controlMode == ControlMode.DineIn))
            {
                buttonStartTicket.IsEnabled = ((phoneNumberEditControl.Text != null) &&
                    (phoneNumberEditControl.Text.Length == 10) &&
                    !String.IsNullOrEmpty(textBoxCustomerName.Text));
            }
            else
            {
                ZipCode zipCode = null;
                if ((textBoxZip.Text == null) ||
                    (textBoxZip.Text.Length != 5) ||
                    ((zipCode = ZipCode.Get(GetZipCode())) == null))
                {
                    textBoxCityState.Text = null;
                }
                else
                {
                    ZipCodeCity zipCodeCity = ZipCodeCity.Get(zipCode.CityId);
                    ZipCodeState zipCodeSate = ZipCodeState.Get(zipCodeCity.StateId);
                    textBoxCityState.Text = zipCodeCity.City + ", " + zipCodeSate.Abbreviation;
                }
                buttonStartTicket.IsEnabled = (
                    !string.IsNullOrEmpty(phoneNumberEditControl.Text) && 
                    !string.IsNullOrEmpty(textBoxCustomerName.Text) && 
                    !string.IsNullOrEmpty(textBoxStreetAddress.Text) &&
                    (zipCode != null));
            }
        }

        private void SetControlMode(ControlMode value)
        {
            _controlMode = value;
            labelCityState.Visibility =
                labelStreet.Visibility =
                labelZip.Visibility = 
                labelStreet2.Visibility =
                textBoxCityState.Visibility =
                textBoxStreetAddress.Visibility =
                textBoxStreetAddress2.Visibility =
                textBoxZip.Visibility =
                    ((value == ControlMode.Catering) || (value == ControlMode.Delivery)) ?
                    Visibility.Visible : Visibility.Collapsed;
            labelSelectSeating.Content =
                value == ControlMode.DineIn ? Strings.DineIn : value.ToString();
            SetStartTicketButton();
        }

        private void ResetTextBox(CustomTextBox textBox)
        {
            textBox.IsEnabled = false;
            textBox.Text = null;
        }

        private int GetZipCode()
        {
            try
            {
                return Convert.ToInt32(textBoxZip.Text);
            }
            catch
            {
                return 0;
            }
        }

        public void SetCarryoutMode()
        {
            SetControlMode(ControlMode.Carryout);
        }

        public void SetDeliveryMode()
        {
            SetControlMode(ControlMode.Delivery);
        }

        public void SetCateringMode()
        {
            SetControlMode(ControlMode.Catering);
        }

        public void SetDineInMode()
        {
            SetControlMode(ControlMode.DineIn);
        }

        [Obfuscation(Exclude = true)]
        private void buttonStartTicket_Click(object sender, RoutedEventArgs e)
        {
            char? middleInitial;
            string firstName, lastName;
            ParseName(textBoxCustomerName.Text,
                out firstName, out middleInitial, out lastName);
            if (SelectedCustomer == null)
            {
                PhoneNumber phoneNumber = PhoneNumber.Add(phoneNumberEditControl.Text, null);
                Person person = Person.Add(firstName, middleInitial, lastName,
                    textBoxStreetAddress.Text, textBoxStreetAddress2.Text, GetZipCode(),
                    phoneNumber.Id, 0, 0, 0, 0, 0, null);
                SelectedCustomer = Customer.Add(person.Id, null);
            }
            else
            {
                Person person = Person.Get(SelectedCustomer.PersonId);
                person.SetAddressLine1(textBoxStreetAddress.Text);
                person.SetAddressLine2(textBoxStreetAddress2.Text);
                person.SetFirstName(firstName);
                person.SetLastName(lastName);
                person.SetMiddleInitial(middleInitial);
                person.SetZipCodeId(GetZipCode());
                person.Update();
            }
            if (ActiveTicket != null)
            {
                ActiveTicket.SetCustomerId(SelectedCustomer.Id);
                ActiveTicket.Update();
            }
            // Closes the dialog window, so the carryout ticket can be created
            Window.GetWindow(this).Close();
        }
    }
}
