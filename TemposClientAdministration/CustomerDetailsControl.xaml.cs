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
using TemposUpdateServiceModels;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for CustomerDetailsControl.xaml
    /// </summary>
    public partial class CustomerDetailsControl : UserControl
    {
        private bool haltEvents = false;
        private Customer selectedCustomer = null;

        [Obfuscation(Exclude = true)]
        public event EventHandler CustomerChanged;

        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                selectedCustomer = value;
                haltEvents = true;
                if (value != null)
                {
                    textBoxBusinessName.Text = value.BusinessName;
                    textBoxContactsName.Text = value.ContactsName;
                    textBoxAddress.Text = value.Address;
                    textBoxCity.Text = value.City;
                    textBoxState.Text = value.State;
                    textBoxZip.Text = value.ZipCode;
                    textBoxPhone1.Text = value.Phone1;
                    textBoxPhone2.Text = value.Phone2;
                }
                else
                {
                    textBoxBusinessName.Text =
                        textBoxContactsName.Text =
                        textBoxAddress.Text =
                        textBoxCity.Text =
                        textBoxState.Text =
                        textBoxZip.Text =
                        textBoxPhone1.Text =
                        textBoxPhone2.Text = null;
                }
                haltEvents = false;
            }
        }

        public CustomerDetailsControl()
        {
            InitializeComponent();
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxBusinessName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxContactsName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAddress_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxCity_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxState_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxZip_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPhone1_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPhone2_TextChanged(object sender, RoutedEventArgs e)
        {
            DoCustomerChanged();
        }

        [Obfuscation(Exclude = true)]
        private void DoCustomerChanged()
        {
            if (!haltEvents && (CustomerChanged != null))
                CustomerChanged.Invoke(this, new EventArgs());
        }

        public void Update()
        {
            if (SelectedCustomer == null)
            {
                SelectedCustomer = Customer.Add(textBoxBusinessName.Text, textBoxContactsName.Text,
                    textBoxAddress.Text, textBoxCity.Text, textBoxState.Text,
                    textBoxZip.Text, textBoxPhone1.Text, textBoxPhone2.Text);
            }
            else
            {
                SelectedCustomer.SetBusinessName(textBoxBusinessName.Text);
                SelectedCustomer.SetContactsName(textBoxContactsName.Text);
                SelectedCustomer.SetAddress(textBoxAddress.Text);
                SelectedCustomer.SetCity(textBoxCity.Text);
                SelectedCustomer.SetState(textBoxState.Text);
                SelectedCustomer.SetZipCode(textBoxZip.Text);
                SelectedCustomer.SetPhone1(textBoxPhone1.Text);
                SelectedCustomer.SetPhone2(textBoxPhone2.Text);
                SelectedCustomer.Update();
            }
        }
    }
}
