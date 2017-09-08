using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DiscountMaintenanceControl.xaml
    /// </summary>
    public partial class DiscountMaintenanceControl
    {
        public Discount SelectedDiscount
        {
            get;
            private set;
        }

        public DiscountMaintenanceControl()
        {
            InitializeComponent();
            InitializeListbox();
        }

        private void InitializeListbox()
        {
            foreach (Discount discount in Discount.GetAll())
            {
                AddDiscountToList(discount, false);
            }
        }

        private void AddDiscountToList(Discount discount, bool select)
        {
            var newItem = new FormattedListBoxItem(discount, discount.Description, true);
            listBox1.Items.Add(newItem);
            if (select)
            {
                listBox1.SelectedItem = newItem;
            }
        }

        [Obfuscation(Exclude = true)]
        void editorControl_DetailsChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void DeleteDiscount()
        {
            var listItem = listBox1.SelectedItem as FormattedListBoxItem;
            if (listItem == null || (listItem.ReferenceObject == null)) return;
            if (PosDialogWindow.ShowDialog(
                Strings.AreYouSureYouWantToDeleteTheSelectedDiscount, Strings.Confirmation,
                DialogButtons.YesNo) != DialogButton.Yes) return;
            var coupon = listItem.ReferenceObject as Discount;
            if (coupon != null) coupon.Discontinue();
            listBox1.Items.Remove(listItem);
            buttonDelete.IsEnabled = false;
            groupBoxProperties.IsEnabled = false;
            SelectedDiscount = null;
        }

        private void AddDiscount()
        {
            SelectedDiscount = null;
            SetEditMode(true);
            editorControl.InitializeNew();
            listBox1.SelectedItem = null;
            foreach (FormattedListBoxItem item in listBox1.Items)
            {
                item.IsSelected = false;
            }
        }

        private void UpdateDiscount()
        {
            if (SelectedDiscount == null)
            {
                SelectedDiscount = Discount.Add(editorControl.Description,
                    editorControl.Amount, editorControl.IsPercentage,
                    editorControl.RequiresPermission, editorControl.ApplyToTicketItem,
                    0, 0);
                // By default IsActive is true (in Add), but encase not...
                if (!editorControl.IsActive)
                {
                    SelectedDiscount.SetIsActive(false);
                    SelectedDiscount.Update();
                }
                AddDiscountToList(SelectedDiscount, true);
                buttonDelete.IsEnabled = true;
            }
            else
            {
                SelectedDiscount.SetDescription(editorControl.Description);
                SelectedDiscount.SetAmount(editorControl.Amount);
                SelectedDiscount.SetAmountIsPercentage(editorControl.IsPercentage);
                SelectedDiscount.SetRequiresPermission(editorControl.RequiresPermission);
                SelectedDiscount.SetApplyToTicketItem(editorControl.ApplyToTicketItem);
                SelectedDiscount.SetIsActive(editorControl.IsActive);
                SelectedDiscount.Update();
                if (listBox1.SelectedItem != null)
                {
                    var formattedListBoxItem = listBox1.SelectedItem as FormattedListBoxItem;
                    if (formattedListBoxItem != null)
                        formattedListBoxItem.Text = editorControl.Description;
                }
            }
            SetEditMode(false);
        }

        private void CancelChanges()
        {
            if (SelectedDiscount != null)
                editorControl.InitializeDiscount(SelectedDiscount);
            else
                editorControl.InitializeNew();
            SetEditMode(false);
        }

        private void SetEditMode(bool inEditMode)
        {
            if (inEditMode || (listBox1.SelectedItem == null))
                groupBoxProperties.IsEnabled = inEditMode;
            listBox1.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonDelete.IsEnabled = (!inEditMode && (listBox1.SelectedItem != null));
            buttonCancel.IsEnabled = inEditMode;
            buttonUpdate.IsEnabled = inEditMode;
            PosDialogWindow parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null) parentWindow.SetButtonsEnabled(!inEditMode);
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            FormattedListBoxItem item = e.AddedItems[0] as FormattedListBoxItem;
            if (item != null)
            {
                Discount selectedDiscount = item.ReferenceObject as Discount;
                SelectedDiscount = selectedDiscount;
                editorControl.InitializeDiscount(selectedDiscount);
            }
            groupBoxProperties.IsEnabled = true;
            buttonDelete.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddDiscount();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteDiscount();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateDiscount();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

    }
}
