using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TaxMaintenanceControl.xaml
    /// </summary>
    public partial class TaxMaintenanceControl : UserControl
    {
        public TaxMaintenanceControl()
        {
            InitializeComponent();
            InitializeTaxList();
            InitializeEventHandlers();
        }

        private void InitializeTaxList()
        {
            listBox1.Items.Clear();
            foreach (Tax tax in Tax.GetAll())
            {
                listBox1.Items.Add(new FormattedListBoxItem(tax.Id, tax.TaxName, true));
            }
        }

        [Obfuscation(Exclude = true)]
        private void InitializeEventHandlers()
        {
            editorControl.ValueChanged += editorControl_ValueChanged;
        }

        [Obfuscation(Exclude = true)]
        void editorControl_ValueChanged(object sender, EventHandlers.TaxValueChangedArgs args)
        {
            if (editorControl.ActiveTax != null)
            {
                groupBoxList.IsEnabled = false;
                buttonAdd.IsEnabled = false;
            }
            buttonCancel.IsEnabled = true;
            buttonUpdate.IsEnabled = true;
        }

        private void CancelChanges()
        {
            // Reinitialize
            editorControl.ActiveTax = editorControl.ActiveTax;
            groupBoxList.IsEnabled = true;
            buttonAdd.IsEnabled = true;
            buttonUpdate.IsEnabled = false;
            buttonCancel.IsEnabled = false;
            if (listBox1.SelectedItem == null)
            {
                editorControl.ActiveTax = null;
                groupBoxProperties.IsEnabled = false;
            }
        }

        private void UpdateTaxItem()
        {
            if (editorControl.UpdateTax())
            {
                groupBoxList.IsEnabled = true;
                if (listBox1.SelectedIndex >= 0)
                {
                    var item = (FormattedListBoxItem)listBox1.SelectedItem;
                    item.Set(editorControl.ActiveTax.Id, editorControl.ActiveTax.TaxName);
                }
                else
                {
                    var listItem = new FormattedListBoxItem(
                        editorControl.ActiveTax.Id,
                        editorControl.ActiveTax.TaxName, true);
                    listBox1.Items.Add(listItem);
                    listBox1.SelectedItem = listItem;
                    listBox1.ScrollIntoView(listBox1.Items[listBox1.Items.Count - 1]);
                }
                buttonAdd.IsEnabled = true;
                buttonUpdate.IsEnabled = false;
                buttonCancel.IsEnabled = false;
            }
        }

        private void AddStart()
        {
            listBox1.SelectedItem = null;
            groupBoxList.IsEnabled = false;
            buttonAdd.IsEnabled = false;
            buttonUpdate.IsEnabled = true;
            buttonCancel.IsEnabled = true;
            editorControl.ActiveTax = null;
            groupBoxProperties.IsEnabled = true;
            editorControl.textBoxDescription.Focus();
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                editorControl.ActiveTax = null;
                groupBoxProperties.IsEnabled = false;
            }
            else
            {
                FormattedListBoxItem tax = (FormattedListBoxItem)e.AddedItems[0];
                editorControl.ActiveTax = Tax.Get(tax.Id);
                tax.IsSelected = true;
                groupBoxProperties.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStart();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateTaxItem();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

    }
}
