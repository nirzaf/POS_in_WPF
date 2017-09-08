using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CouponMaintenanceControl.xaml
    /// </summary>
    public partial class CouponMaintenanceControl : UserControl
    {
        public CouponMaintenanceControl()
        {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeCouponList();
        }

        private void InitializeCouponList()
        {
            foreach (var item in Coupon.GetAll()
                .Select(coupon => new FormattedListBoxItem(coupon, coupon.Description, true)))
            {
                listBox1.Items.Add(item);
            }
        }

        private void InitializeEventHandlers()
        {
            editorControl.ValueChanged += editorControl_ValueChanged;
        }

        [Obfuscation(Exclude = true)]
        void editorControl_ValueChanged(object sender, EventHandlers.CouponValueChangedArgs args)
        {
            SetEditMode(true);
        }

        private void DeleteCoupon()
        {
            var listItem = listBox1.SelectedItem as FormattedListBoxItem;
            if (listItem == null || (listItem.ReferenceObject == null)) return;
            if (PosDialogWindow.ShowDialog(
                Strings.AreYouSureYouWantToDeleteTheSelectedCoupon, Strings.Confirmation,
                DialogButtons.YesNo) != DialogButton.Yes) return;
            var coupon = listItem.ReferenceObject as Coupon;
            if (coupon != null) coupon.Discontinue();
            listBox1.Items.Remove(listItem);
            buttonDelete.IsEnabled = false;
            groupBoxProperties.IsEnabled = false;
            editorControl.ActiveCoupon = null;
        }

        private void CancelChanges()
        {
            // Reinitialize
            editorControl.ActiveCoupon = editorControl.ActiveCoupon;
            SetEditMode(false);
        }

        private void UpdateCoupon()
        {
            if (editorControl.UpdateCoupon())
            {
                SetEditMode(false);
                if (listBox1.SelectedIndex >= 0)
                {
                    var item = (FormattedListBoxItem)listBox1.SelectedItem;
                    item.Set(editorControl.ActiveCoupon, editorControl.ActiveCoupon.Description);
                }
                else
                {
                    var listItem = new FormattedListBoxItem(
                        editorControl.ActiveCoupon,
                        editorControl.ActiveCoupon.Description, true);
                    listBox1.Items.Add(listItem);
                    listBox1.SelectedItem = listItem;
                    listBox1.ScrollIntoView(listItem);
                    buttonDelete.IsEnabled = true;
                }
            }
            if (editorControl.ActiveCoupon != null)
            {
                var couponCategories =
                    new List<CouponCategory>(CouponCategory.GetAll(editorControl.ActiveCoupon.Id));
                foreach (int id in editorControl.couponCategorySelectionControl.SelectedCategoryIds
                    .Where(id => CouponCategory.FindByCategoryId(couponCategories, id) == null))
                {
                    CouponCategory.Add(editorControl.ActiveCoupon.Id, id);
                }
                foreach (CouponCategory category in couponCategories
                    .Where(category => !editorControl.couponCategorySelectionControl.SelectedCategoryIds
                        .Contains(category.CategoryId)))
                {
                    CouponCategory.Delete(editorControl.ActiveCoupon.Id, category.CategoryId);
                }
                
                var couponItems =
                    new List<CouponItem>(CouponItem.GetAll(editorControl.ActiveCoupon.Id));
                foreach (int id in editorControl.couponItemSelectionControl.SelectedItemIds
                    .Where(id => CouponItem.FindByItemId(couponItems, id) == null))
                {
                    CouponItem.Add(editorControl.ActiveCoupon.Id, id);
                }
                foreach (CouponItem item in couponItems
                    .Where(item => !editorControl.couponCategorySelectionControl.SelectedCategoryIds
                        .Contains(item.ItemId)))
                {
                    CouponItem.Delete(editorControl.ActiveCoupon.Id, item.ItemId);
                }
            }
        }

        private void AddCoupon()
        {
            listBox1.SelectedItem = null;
            editorControl.ActiveCoupon = null;
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            var couponItem = (FormattedListBoxItem)e.AddedItems[0];
            editorControl.ActiveCoupon = couponItem.ReferenceObject as Coupon;
            couponItem.IsSelected = true;
            groupBoxProperties.IsEnabled = true;
            buttonDelete.IsEnabled = true;
        }

        private void SetEditMode(bool inEditMode)
        {
            if (inEditMode || (listBox1.SelectedItem == null))
                groupBoxProperties.IsEnabled = inEditMode;
            listBox1.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonDelete.IsEnabled = (!inEditMode && (listBox1.SelectedItem != null));
            buttonUpdate.IsEnabled = inEditMode;
            buttonCancel.IsEnabled = inEditMode;
            var parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null) parentWindow.SetButtonsEnabled(!inEditMode);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCoupon();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCoupon();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateCoupon();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

    }
}
