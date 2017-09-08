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
using TemposLibrary;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for LicenseDetailsControl.xaml
    /// </summary>
    public partial class LicenseDetailsControl : UserControl
    {
        private bool haltEvents = false;
        private License selectedLicense = null;
        Customer[] customers = null;

        [Obfuscation(Exclude = true)]
        public event EventHandler LicenseChanged;

        public Customer SelectedCustomer
        {
            get;
            set;
        }

        public License SelectedLicense
        {
            get { return selectedLicense; }
            set
            {
                selectedLicense = value;
                haltEvents = true;
                if (value != null)
                {
                    InitializeCustomers();
                    textBoxSerialNumber.Text = value.SerialNumber;
                    radioButtonIsNotValid.IsSelected = !value.IsValid;
                    radioButtonIsValid.IsSelected = value.IsValid;
                }
                else
                {
                    comboBoxCustomer.Items.Clear();
                    textBoxSerialNumber.Text = null;
                    radioButtonIsNotValid.IsSelected = false;
                    radioButtonIsValid.IsSelected = true;
                }
                haltEvents = false;
            }
        }

        public LicenseDetailsControl()
        {
            InitializeComponent();
            InitializeCustomers();
        }

        private void InitializeCustomers()
        {
            customers = Customer.GetAll();
            comboBoxCustomer.Items.Clear();
            int selectedIndex = -1;
            for(int i = 0; i < customers.Length; i++)
            {
                Customer customer = customers[i];
                if ((SelectedCustomer != null) && (SelectedCustomer.Id == customer.Id))
                    selectedIndex = comboBoxCustomer.Items.Count;
                comboBoxCustomer.Items.Add(customer.BusinessName);
            }
            if (selectedIndex >= 0)
                comboBoxCustomer.SelectedIndex = selectedIndex;
        }
        
        [Obfuscation(Exclude = true)]
        private void comboBoxCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCustomer = customers[comboBoxCustomer.SelectedIndex];
            DoLicenseChanged();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxSerialNumber_TextChanged(object sender, RoutedEventArgs e)
        {
            DoLicenseChanged();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsValid_SelectionGained(object sender, EventArgs e)
        {
            DoLicenseChanged();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsNotValid_SelectionGained(object sender, EventArgs e)
        {
            DoLicenseChanged();
        }

        private void DoLicenseChanged()
        {
            if (!haltEvents && (LicenseChanged != null))
                LicenseChanged.Invoke(this, new EventArgs());
        }

        public void Update()
        {
            if (SelectedLicense == null)
            {
                SelectedLicense = License.Add(GetCustomerId(), textBoxSerialNumber.Text,
                    GetIdentityHash());
            }
            else
            {
                SelectedLicense.SetCustomerId(GetCustomerId());
                SelectedLicense.SetSerialNumber(textBoxSerialNumber.Text);
                SelectedLicense.SetIdentityHash(GetIdentityHash());
                SelectedLicense.SetIsValid(radioButtonIsValid.IsSelected);
                SelectedLicense.Update();
            }
        }

        private int GetCustomerId()
        {
            if (SelectedCustomer == null)
                return 0;
            return SelectedCustomer.Id;
        }

        private byte[] GetIdentityHash()
        {
            if (SelectedCustomer == null)
                return new byte[20];
            return SRP6.GenerateIdentityHash(SelectedCustomer.BusinessName,
                textBoxSerialNumber.Text);
        }

    }
}
