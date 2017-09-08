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
    /// Interaction logic for CouponCategorySelectionControl.xaml
    /// </summary>
    public partial class CouponCategorySelectionControl : UserControl
    {
        public Coupon ActiveCoupon { get; set; }

        private int[] _selectedCategoryIds;
        public int[] SelectedCategoryIds
        {
            get { return _selectedCategoryIds; }
            set
            {
                _selectedCategoryIds = value;
                List<Category> categories = new List<Category>(Category.GetAll());
                InitializeCategoryList(categories);
                InitializeSelectedCategoryList(categories);
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

        public CouponCategorySelectionControl()
        {
            InitializeComponent();
        }

        private void InitializeSelectedCategoryList(List<Category> categories)
        {
            listBoxSelectedCategories.Items.Clear();
            if (SelectedCategoryIds == null)
                return;
            foreach (int id in SelectedCategoryIds)
            {
                Category category = Category.FindById(categories, id);
                AddCategoryToList(listBoxSelectedCategories, category);
            }
        }

        private void InitializeCategoryList(IEnumerable<Category> categories)
        {
            listBoxAllCategories.Items.Clear();
            foreach (Category category in categories)
            {
                AddCategoryToList(listBoxAllCategories, category);
            }
        }

        private void AddCategoryToList(DragScrollListBox listBox, Category category)
        {
            listBox.Items.Add(
                new FormattedListBoxItem(category.Id, category.NameValue, true));
        }

        private void RemoveItem()
        {
            if (listBoxSelectedCategories.SelectedItem == null)
                return;

            listBoxSelectedCategories.Items.Remove(listBoxSelectedCategories.SelectedItem);
            buttonAdd.IsEnabled = false;
            buttonRemove.IsEnabled = false;

            SetSelectedIds();
            DoChangedValueEvent(CouponFieldName.Categories);
        }

        private void AddItem()
        {
            if (listBoxAllCategories.SelectedItem == null)
                return;
            FormattedListBoxItem sourceItem =
                (FormattedListBoxItem)listBoxAllCategories.SelectedItem;
            FormattedListBoxItem createdItem = 
                new FormattedListBoxItem(sourceItem.Id, sourceItem.Text, true);
            listBoxSelectedCategories.Items.Add(createdItem);
            listBoxSelectedCategories.SelectedItem = createdItem;

            buttonAdd.IsEnabled = false;
            buttonRemove.IsEnabled = false;

            SetSelectedIds();
            DoChangedValueEvent(CouponFieldName.Categories);
        }

        private void SetSelectedIds()
        {
            List<int> selected =
                (from FormattedListBoxItem item in listBoxSelectedCategories.Items
                 select item.Id).ToList();
            selected.Sort();
            if (selected.Count == 0)
                SelectedCategoryIds = null;
            SelectedCategoryIds = selected.ToArray();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxAllCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (listBoxAllCategories.SelectedItem != null)
            {
                FormattedListBoxItem currentItem = (FormattedListBoxItem)e.AddedItems[0];
                FormattedListBoxItem priorItem = (FormattedListBoxItem)listBoxSelectedCategories.SelectedItem;
                FormattedListBoxItem existingItem = null;
                foreach (FormattedListBoxItem item in listBoxSelectedCategories.Items)
                {
                    if (item.Id == currentItem.Id)
                        existingItem = item;
                }
                if (existingItem != null)
                {
                    listBoxSelectedCategories.SelectedItem = existingItem;
                    buttonRemove.IsEnabled = true;
                    buttonAdd.IsEnabled = false;
                }
                else if (priorItem != null)
                {
                    listBoxSelectedCategories.SelectedItem = null;
                    buttonAdd.IsEnabled = true;
                    buttonRemove.IsEnabled = false;
                }
                else
                {
                    buttonAdd.IsEnabled = true;
                    buttonRemove.IsEnabled = false;
                }
            }
            else
            {
                buttonAdd.IsEnabled = false;
            }

            SetViewable();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxSelectedCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (listBoxSelectedCategories.SelectedItem != null)
            {
                FormattedListBoxItem currentItem = (FormattedListBoxItem)e.AddedItems[0];
                FormattedListBoxItem existingItem = null;
                foreach (FormattedListBoxItem item in listBoxAllCategories.Items)
                {
                    if (item.Id == currentItem.Id)
                        existingItem = item;
                }

                listBoxAllCategories.SelectedItem = existingItem;
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
            if (listBoxSelectedCategories.SelectedItem != null)
                listBoxSelectedCategories.ScrollIntoView(listBoxSelectedCategories.SelectedItem);
            if (listBoxAllCategories.SelectedItem != null)
                listBoxAllCategories.ScrollIntoView(listBoxAllCategories.SelectedItem);
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
