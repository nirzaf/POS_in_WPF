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
using PosControls;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for LicenseSetupControl.xaml
    /// </summary>
    public partial class LicenseSetupControl : UserControl
    {
        public LicenseSetupControl()
        {
            InitializeComponent();
            InitializeListbox();
        }

        private void InitializeListbox()
        {
            License[] licenses = License.GetAll();
            listBox.Items.Clear();
            foreach (License license in licenses)
            {
                AddLicense(license);
            }
        }

        private FormattedListBoxItem AddLicense(License license)
        {
            Customer customer = Customer.Get(license.CustomerId);
            FormattedListBoxItem item =
                new FormattedListBoxItem(license, customer.ContactsName +
                    Environment.NewLine + customer.BusinessName +
                    Environment.NewLine + customer.City + ", " + customer.State, true);
            listBox.Items.Add(item);
            return item;
        }

        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetLicenseDetails();
        }
        
        [Obfuscation(Exclude = true)]
        private void editorControl_LicenseChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void SetEditMode(bool inEditMode)
        {
            listBox.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonCancel.IsEnabled = inEditMode;
            buttonUpdate.IsEnabled = inEditMode;
        }

        private void SetLicenseDetails()
        {
            FormattedListBoxItem item = listBox.SelectedItem as FormattedListBoxItem;
            if (item != null)
            {
                License license = item.ReferenceObject as License;                
                editorControl.SelectedCustomer = Customer.Get(license.CustomerId);
                editorControl.SelectedLicense = license;
                editorControl.IsEnabled = true;
            }
            else
            {
                editorControl.SelectedCustomer = null;
                editorControl.SelectedLicense = null;
                editorControl.IsEnabled = false;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            listBox.SelectedItem = null;
            editorControl.SelectedLicense = null;
            editorControl.IsEnabled = true;
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool isNew = (editorControl.SelectedLicense == null);
            editorControl.Update();
            if (isNew)
                listBox.SelectedItem = AddLicense(editorControl.SelectedLicense);
            SetEditMode(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            SetLicenseDetails();
            SetEditMode(false);
        }
    }
}
