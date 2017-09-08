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
    /// Interaction logic for CustomerSetupControl.xaml
    /// </summary>
    public partial class CustomerSetupControl : UserControl
    {
        public CustomerSetupControl()
        {
            InitializeComponent();
            InitializeListbox();
        }

        private void InitializeListbox()
        {
            Customer[] customers = Customer.GetAll();
            listBox.Items.Clear();
            foreach (Customer customer in customers)
            {
                listBox.Items.Add(
                    new FormattedListBoxItem(customer, customer.ContactsName +
                        Environment.NewLine + customer.BusinessName, true));
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCustomerDetails();
        }
        
        [Obfuscation(Exclude = true)]
        private void editorControl_CustomerChanged(object sender, EventArgs e)
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

        private void SetCustomerDetails()
        {
            FormattedListBoxItem item = listBox.SelectedItem as FormattedListBoxItem;
            if (item != null)
            {
                editorControl.IsEnabled = true;
                editorControl.SelectedCustomer = item.ReferenceObject as Customer;
            }
            else
            {
                editorControl.IsEnabled = false;
                editorControl.SelectedCustomer = null;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            listBox.SelectedItem = null;
            editorControl.SelectedCustomer = null;
            editorControl.IsEnabled = true;
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool isNew = (editorControl.SelectedCustomer == null);
            editorControl.Update();
            if (isNew)
            {
                FormattedListBoxItem item = new FormattedListBoxItem(editorControl.SelectedCustomer,
                    editorControl.SelectedCustomer.ContactsName + Environment.NewLine +
                    editorControl.SelectedCustomer.BusinessName, true);
                listBox.Items.Add(item);
                listBox.SelectedItem = item;
            }
            SetEditMode(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            SetCustomerDetails();
            SetEditMode(false);
        }
    }
}
