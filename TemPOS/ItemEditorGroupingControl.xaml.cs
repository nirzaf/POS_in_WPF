using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosModels;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorGroupingControl.xaml
    /// </summary>
    public partial class ItemEditorGroupingControl : UserControl
    {
        #region Fields
        private Item _activeItem;

        private readonly List<ItemGroup> _itemGroupsAdded =
            new List<ItemGroup>();
        private readonly List<ItemGroup> _itemGroupsNeedingUpdate =
            new List<ItemGroup>();
        private readonly List<ItemGroup> _itemGroupsRemoved =
            new List<ItemGroup>();
        #endregion

        #region Properties
        public Item ActiveItem
        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;
                InitializeListBoxes();
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoValueChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        #region Initialization
        public ItemEditorGroupingControl()
        {
            InitializeComponent();
        }

        private void InitializeListBoxes()
        {
            listBoxAvailableItems.SelectedItem = null;
            listBoxIncludedItems.SelectedItem = null;
            listBoxAvailableItems.Items.Clear();
            listBoxIncludedItems.Items.Clear();

            List<Item> items = ItemMaintenanceControl.ItemCache;
            if (_activeItem != null)
            {
                // Remove current item
                items.Remove(Item.FindById(items, _activeItem.Id));

                foreach (ItemGroup itemGroup in ItemGroup.GetAll(_activeItem.Id))
                {
                    bool added, changed, removed;
                    ItemGroup current = GetItemGroup(itemGroup.Id,
                        out added, out changed, out removed);
                    if (removed) continue;
                    Item item = Item.FindById(items, itemGroup.TargetItemId);
                    AddItemGroupToListBox(
                        (changed ? current : itemGroup), item);
                }
            }

            // Note: Added ones have an ItemId of zero so GetAll (above) will not find them
            foreach (ItemGroup itemGroup in _itemGroupsAdded)
            {
                Item item = Item.FindById(items, itemGroup.TargetItemId);
                AddItemGroupToListBox(itemGroup, item);
            }

            foreach (Item item in items)
            {
                if (ItemGroup.IsGroupMember(item.Id)) continue;
                listBoxAvailableItems.Items.Add(
                    new FormattedListBoxItem(item, item.FullName, true));
            }

            SetButtonsEnabled();
        }

        private void AddItemGroupToListBox(ItemGroup itemGroup, Item item)
        {
            string amount = "" + itemGroup.TargetItemQuantity;
            string tagLine = item.FullName + Environment.NewLine +
                Strings.ItemEditorQuantity + amount;
            listBoxIncludedItems.Items.Add(
                new FormattedListBoxItem(itemGroup, tagLine, true));
        }
        #endregion

        private void SetButtonsEnabled()
        {
            if (DayOfOperation.Today != null)
            {
                buttonAdd.Visibility =
                    buttonEditQuantity.Visibility =
                    buttonRemove.Visibility =
                    Visibility.Collapsed;
                labelReadOnly.Visibility = Visibility.Visible;
                return;
            }

            buttonAdd.IsEnabled = (listBoxAvailableItems.SelectedItem != null);
            buttonEditQuantity.IsEnabled =
                buttonRemove.IsEnabled = (listBoxIncludedItems.SelectedItem != null);
        }

        #region Event Handlers
        [Obfuscation(Exclude = true)]
        private void listBoxAvailableItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxAvailableItems.SelectedItem != null)
                listBoxIncludedItems.SelectedItem = null;
            SetButtonsEnabled();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void listBoxIncludedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxIncludedItems.SelectedItem != null)
                listBoxAvailableItems.SelectedItem = null;
            SetButtonsEnabled();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
#if DEMO
            if (listBoxIncludedItems.Items.Count >= 2)
            {
                PosDialogWindow.ShowDialog(Strings.ItemEditorErrorDemo,
                    Strings.ItemEditorDemoRestriction);
                return;
            }
#endif
            FormattedListBoxItem formattedListBoxItem = listBoxAvailableItems.SelectedItem as FormattedListBoxItem;
            if (formattedListBoxItem != null)
            {
                Item item = formattedListBoxItem.ReferenceObject as Item;
                if (item != null)
                    _itemGroupsAdded.Add(ItemGroup.Add(0, item.Id, 1));
            }
            InitializeListBoxes();
            DoValueChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEditQuantity_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem = listBoxIncludedItems.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;
            ItemGroup itemGroup = selectedItem.ReferenceObject as ItemGroup;
            if (itemGroup == null) return;
            int? newQuantity = PosDialogWindow.PromptNumber(Strings.ItemEditorEditQuantity, null);
            if (newQuantity.HasValue)
            {
                itemGroup.SetTargetItemQuantity(newQuantity.Value);
                if (EditQuantityNeedsUpdating(itemGroup))
                    _itemGroupsNeedingUpdate.Add(itemGroup);

                InitializeListBoxes();
                DoValueChangedEvent();
            }
        }

        private bool EditQuantityNeedsUpdating(ItemGroup itemGroup)
        {
            bool cachedAdd = false, cachedChange = false;
            foreach (ItemGroup checkItemGroup in _itemGroupsAdded
                .Where(checkItemGroup => checkItemGroup.Id == itemGroup.Id))
            {
                cachedAdd = true;
            }
            foreach (ItemGroup checkItemGroup in _itemGroupsNeedingUpdate
                .Where(checkItemGroup => checkItemGroup.Id == itemGroup.Id))
            {
                cachedChange = true;
            }
            return (!cachedAdd && !cachedChange);
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem = listBoxIncludedItems.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;
            ItemGroup itemGroup = selectedItem.ReferenceObject as ItemGroup;
            DialogButton result = PosDialogWindow.ShowDialog(
                Strings.ItemEditorConfirmRemove, Strings.Confirmation, DialogButtons.YesNo);
            if (result != DialogButton.Yes) return;
            if (itemGroup != null)
                RemoveItemGroup(itemGroup.Id);
            InitializeListBoxes();
            DoValueChangedEvent();
        }

        private void RemoveItemGroup(int id)
        {
            bool added, changed, removed;
            ItemGroup itemGroup = GetItemGroup(id,
                out added, out changed, out removed);
            if (added)
                _itemGroupsAdded.Remove(itemGroup);
            if (changed)
                _itemGroupsNeedingUpdate.Remove(itemGroup);
            if (!added && !removed)
                _itemGroupsRemoved.Add(itemGroup);
        }

        private ItemGroup GetItemGroup(int id,
            out bool cachedAdd, out bool cachedChange, out bool cachedRemove)
        {
            cachedAdd = false;
            cachedChange = false;
            cachedRemove = false;
            foreach (ItemGroup itemGroup in _itemGroupsAdded.Where(itemGroup => itemGroup.Id == id))
            {
                cachedAdd = true;
                return itemGroup;
            }
            foreach (ItemGroup itemGroup in _itemGroupsNeedingUpdate.Where(itemGroup => itemGroup.Id == id))
            {
                cachedChange = true;
                return itemGroup;
            }
            foreach (ItemGroup itemGroup in _itemGroupsRemoved.Where(itemGroup => itemGroup.Id == id))
            {
                cachedRemove = true;
                return itemGroup;
            }
            return ItemGroup.Get(id);
        }

        public void Update(int itemId)
        {
            // Added Ingredients
            foreach (ItemGroup itemGroup in _itemGroupsAdded)
            {
                itemGroup.SetSourceItemId(itemId);
                itemGroup.Update();
                //ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                //    itemId, itemIngredient.IngredientId, null,
                //    itemIngredient.Amount, itemIngredient.MeasurementUnit);
            }
            // Changed Ingredients
            foreach (ItemGroup itemGroup in _itemGroupsNeedingUpdate)
            {
                //ItemIngredient original = ItemIngredient.Get(itemIngredient.Id);
                //double oldAmount = UnitConversion.Convert(original.Amount, original.MeasurementUnit,
                //    itemIngredient.MeasurementUnit);
                itemGroup.Update();
                //ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                //    itemId, itemIngredient.IngredientId, oldAmount, itemIngredient.Amount,
                //    itemIngredient.MeasurementUnit);
            }
            // Removed Ingredients
            foreach (ItemGroup itemGroup in _itemGroupsRemoved)
            {
                //ItemIngredient original = ItemIngredient.Get(itemIngredient.Id);
                ItemGroup.Delete(itemGroup.Id);
                //ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                //    itemId, itemIngredient.IngredientId, itemIngredient.Amount, null,
                //    itemIngredient.MeasurementUnit);
            }
            _itemGroupsAdded.Clear();
            _itemGroupsNeedingUpdate.Clear();
            _itemGroupsRemoved.Clear();
        }

        public void Cancel()
        {
            foreach (ItemGroup itemGroup in _itemGroupsAdded)
            {
                ItemGroup.Delete(itemGroup.Id);
            }
            _itemGroupsAdded.Clear();
            _itemGroupsNeedingUpdate.Clear();
            _itemGroupsRemoved.Clear();

            // Reset UI
            InitializeListBoxes();
        }
        #endregion
    }
}
