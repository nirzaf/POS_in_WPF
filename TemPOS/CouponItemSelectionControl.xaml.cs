using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CouponItemSelectionControl.xaml
    /// </summary>
    public partial class CouponItemSelectionControl : UserControl
    {
        public Coupon ActiveCoupon { get; set; }

        private int[] _selectedItemIds = null;

        public int[] SelectedItemIds
        {
            get { return _selectedItemIds; }
            set
            {
                _selectedItemIds = value;

                List<Item> items = new List<Item>(Item.GetAll());
                List<Category> categories = new List<Category>(Category.GetAll());
                InitializeItemList(items, categories);
                InitializeSelectedItemList(items, categories);
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event CouponValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(CouponFieldName field)
        {
            if ((ValueChanged != null) && (ActiveCoupon != null))
                ValueChanged.Invoke(this, new CouponValueChangedArgs(ActiveCoupon, field));
        }
        #endregion

        public CouponItemSelectionControl()
        {
            InitializeComponent();
        }

        private void InitializeSelectedItemList(List<Item> items, List<Category> categories)
        {
            listBoxSelectedItems.Items.Clear();
            if (SelectedItemIds == null)
                return;
            foreach (Item item in SelectedItemIds.Select(id => Item.FindById(items, id)))
            {
                AddItemToList(listBoxSelectedItems, item, Category.FindById(categories, item.CategoryId));
            }
        }

        private void InitializeItemList(List<Item> items, List<Category> categories)
        {
            listBoxAllItems.Items.Clear();
            foreach (Item item in items)
            {
                AddItemToList(listBoxAllItems, item, Category.FindById(categories, item.CategoryId));
            }

        }

        private void AddItemToList(DragScrollListBox listBox, Item item, Category category)
        {
            string description = item.FullName + Environment.NewLine + Strings.CouponEditorCategory +
                category.NameValue;
            listBox.Items.Add(new FormattedListBoxItem(item.Id, description, true));
        }

        private void RemoveItem()
        {
            if (listBoxSelectedItems.SelectedItem == null)
                return;

            listBoxSelectedItems.Items.Remove(listBoxSelectedItems.SelectedItem);
            buttonAdd.IsEnabled = true;
            buttonRemove.IsEnabled = false;

            SetSelectedIds();
            DoChangedValueEvent(CouponFieldName.Items);
        }

        private void AddItem()
        {
            if (listBoxAllItems.SelectedItem == null)
                return;
            FormattedListBoxItem sourceItem =
                (FormattedListBoxItem)listBoxAllItems.SelectedItem;
            FormattedListBoxItem createdItem =
                new FormattedListBoxItem(sourceItem.Id, sourceItem.Text, true);
            listBoxSelectedItems.Items.Add(createdItem);
            listBoxSelectedItems.SelectedItem = createdItem;
            
            SetSelectedIds();
            DoChangedValueEvent(CouponFieldName.Items);
        }

        private void SetSelectedIds()
        {
            List<int> selected = (
                from FormattedListBoxItem item in listBoxSelectedItems.Items
                select item.Id).ToList();
            selected.Sort();
            if (selected.Count == 0)
                SelectedItemIds = null;
            SelectedItemIds = selected.ToArray();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxAllItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (listBoxAllItems.SelectedItem != null)
            {
                FormattedListBoxItem currentItem = (FormattedListBoxItem)e.AddedItems[0];
                FormattedListBoxItem priorItem = (FormattedListBoxItem)listBoxSelectedItems.SelectedItem;
                FormattedListBoxItem existingItem = null;
                foreach (FormattedListBoxItem item in listBoxSelectedItems.Items)
                {
                    if (item.Id == currentItem.Id)
                        existingItem = item;
                }
                
                if (existingItem != null)
                {
                    listBoxSelectedItems.SelectedItem = existingItem;
                    buttonRemove.IsEnabled = true;
                    buttonAdd.IsEnabled = false;
                    
                }
                else if (priorItem != null)
                {
                    listBoxSelectedItems.SelectedItem = null;
                    buttonRemove.IsEnabled = false;
                    buttonAdd.IsEnabled = true;
                }
                else
                {
                    buttonRemove.IsEnabled = false;
                    buttonAdd.IsEnabled = true;
                }
            }
            else
            {
                buttonAdd.IsEnabled = false;
            }

            SetViewable();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxSelectedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (listBoxSelectedItems.SelectedItem != null)
            {
                FormattedListBoxItem currentItem = (FormattedListBoxItem)e.AddedItems[0];
                FormattedListBoxItem existingItem = null;
                foreach (FormattedListBoxItem item in listBoxAllItems.Items.Cast<FormattedListBoxItem>()
                    .Where(item => item.Id == currentItem.Id))
                {
                    existingItem = item;
                }

                listBoxAllItems.SelectedItem = existingItem;
                buttonRemove.IsEnabled = true;
                buttonAdd.IsEnabled = false;
            }
            else
            {
                buttonRemove.IsEnabled = false;            
            }

            SetViewable();
        }

        private void SetViewable()
        {
            // Scroll into view
            if (listBoxSelectedItems.SelectedItem != null)
                listBoxSelectedItems.ScrollIntoView(listBoxSelectedItems.SelectedItem);
            if (listBoxAllItems.SelectedItem != null)
                listBoxAllItems.ScrollIntoView(listBoxAllItems.SelectedItem);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem();
        }

    }
}
