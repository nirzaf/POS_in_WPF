using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Types;
using TemPOS.EventHandlers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;
using TemPOS.Types;
using System.Threading;
using System.Windows.Threading;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemMaintenanceControl.xaml
    /// </summary>
    public partial class ItemMaintenanceControl
    {
        #region Fields
        private string _lastSearchString = "";
        private Category _selectedCategory;
        public static readonly List<Item> ItemCache = new List<Item>();
        private bool _itemCacheEmpty = true;
        #endregion

        #region Initialization
        public ItemMaintenanceControl()
        {
            InitializeComponent();
            ItemCache.Clear();
            Loaded += ItemMaintenanceControl_Loaded;            
        }

        [Obfuscation(Exclude = true)]
        void ItemMaintenanceControl_Loaded(object sender, RoutedEventArgs e)
        {
            buttonView.ContextMenu = GetViewContextMenu();
            buttonView.ContextMenu.Placement = PlacementMode.Top;
            buttonView.ContextMenu.PlacementTarget = buttonView;
            UpdateViewMode();
            ItemMaintenanceViewContextMenuControl.ViewModeChanged +=
                ItemMaintenanceViewContextMenuControl_ViewModeChanged;
            ingredientEditorControl.ModifiedIngredient += 
                ingredientEditorControl_ModifiedIngredient;
            InitializeListBox();
        }

        /// <summary>
        /// Happens when prep ingredients are modified because a prep ingredient's
        /// inventory amount has been either increased or decreased (by recipe).
        /// </summary>
        /// <param name="sender">An instance of the changed Ingredient</param>
        /// <param name="e"></param>
        [Obfuscation(Exclude = true)]
        void ingredientEditorControl_ModifiedIngredient(object sender, EventArgs e)
        {
            var prepIngredient = (sender as Ingredient);
            if (prepIngredient == null) return;
            foreach (Ingredient listIngredient in
                (from FormattedListBoxItem listItem in listBoxIngredient.Items
                 select listItem.ReferenceObject).OfType<Ingredient>())
            {
                listIngredient.SetInventoryAmount(prepIngredient.InventoryAmount);
            }
        }

        [Obfuscation(Exclude = true)]
        void ItemMaintenanceViewContextMenuControl_ViewModeChanged(object sender, EventArgs e)
        {
            UpdateViewMode();
        }

        private void UpdateViewMode()
        {
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems)
                ShowListBox(listBoxItems);
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                ShowListBox(listBoxFilteredItems);
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                ShowListBox(listBoxItemOption);
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                ShowListBox(listBoxCategories);
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                ShowListBox(listBoxIngredient);

            groupBoxItems.Visibility =
                (((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems) ||
                (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)) ?
                Visibility.Visible : Visibility.Collapsed);
            groupBoxIngredients.Visibility = 
                ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients) ?
                Visibility.Visible : Visibility.Collapsed);
            groupBoxItemOptions.Visibility = 
                ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets) ?
                Visibility.Visible : Visibility.Collapsed);
            groupBoxCategories.Visibility =
                ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories) ?
                Visibility.Visible : Visibility.Collapsed);
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                InitializeItemListBoxForCategory();
            InitializeListBoxHeader();
            InitializeButtons();
            SetTitle();
        }

        private void InitializeListBoxHeader()
        {
            switch (ItemMaintenanceViewContextMenuControl.ViewMode)
            {
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems:
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory:
                    groupBoxList.Header = Types.Strings.Items;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients:
                    groupBoxList.Header = Types.Strings.Ingredients;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories:
                    groupBoxList.Header = Types.Strings.Categories;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets:
                    groupBoxList.Header = Types.Strings.ItemOptionSets;
                    break;
            }
        }

        private void SetTitle()
        {
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems)
                SetWindowTitle(Types.Strings.ItemSetupItems);
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                SetWindowTitle(Types.Strings.ItemSetupItemsInCategory +
                    (_selectedCategory != null ? _selectedCategory.NameValue : "") + "\"");
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                SetWindowTitle(Types.Strings.ItemSetupCategories);
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                SetWindowTitle(Types.Strings.ItemSetupIngredients);
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                SetWindowTitle(Types.Strings.ItemSetupItemOptionSets);
        }

        public void SetWindowTitle(string sectionName)
        {
            var window = Window.GetWindow(this) as PosDialogWindow;
            if (window != null) 
                window.Title = Types.Strings.ItemSetup + " [" + sectionName + "]";
        }

        private DragScrollListBox GetVisibleListBox()
        {
            if (listBoxItems.Visibility == Visibility.Visible)
                return listBoxItems;
            if (listBoxItemOption.Visibility == Visibility.Visible)
                return listBoxItemOption;
            if (listBoxIngredient.Visibility == Visibility.Visible)
                return listBoxIngredient;
            if (listBoxFilteredItems.Visibility == Visibility.Visible)
                return listBoxFilteredItems;
            if (listBoxCategories.Visibility == Visibility.Visible)
                return listBoxCategories;
            return listBoxItems;
        }

        private void InitializeButtons()
        {
            SetFindNextVisibility(false);
            buttonDelete.Visibility =  ((ItemMaintenanceViewContextMenuControl.ViewMode !=
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients) ?
                Visibility.Visible : Visibility.Collapsed);
            buttonEdit.Visibility = (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets ?
                Visibility.Visible : Visibility.Collapsed);
            buttonDelete.IsEnabled =
                buttonEdit.IsEnabled = (GetVisibleListBox().SelectedItem != null);
            switch (ItemMaintenanceViewContextMenuControl.ViewMode)
            {
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems:
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory:
                    buttonUpdate.Text = Types.Strings.ItemSetupUpdateItem;
                    buttonAdd.Text = Types.Strings.ItemSetupAddItem;
                    buttonDelete.Text = Types.Strings.ItemSetupDeleteItem;
                    buttonSearch.Visibility = Visibility.Visible;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories:
                    buttonUpdate.Text = Types.Strings.ItemSetupUpdateCategory;
                    buttonAdd.Text = Types.Strings.ItemSetupAddCategory;
                    buttonDelete.Text = Types.Strings.ItemSetupDeleteCategory;
                    buttonSearch.Visibility = Visibility.Collapsed;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients:
                    buttonUpdate.Text = Types.Strings.ItemSetupUpdateIngredient;
                    buttonAdd.Text = Types.Strings.ItemSetupAddIngredient;
                    buttonSearch.Visibility = Visibility.Visible;
                    break;
                case ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets:
                    buttonUpdate.Text = Types.Strings.ItemSetupUpdateOptionSet;
                    buttonAdd.Text = Types.Strings.ItemSetupAddOptionSet;
                    buttonDelete.Text = Types.Strings.ItemSetupDeleteOptionSet;
                    buttonSearch.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private ContextMenu GetViewContextMenu()
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                var entry = (DictionaryEntry)e.Current;
                var name = entry.Key as string;
                if (name != "viewContextMenu") continue;
                contextMenu = entry.Value as ContextMenu;
                break;
            }
            return contextMenu;
        }

        private void InitializeListBox()
        {
            InitializeItemListBox();
            InitializeItemOptionListBox();
            InitializeCategoryListBox();
            InitializeIngredientListBox();
        }

        private void InitializeIngredientListBox()
        {
            new Thread(InitializeIngredientListBoxThreadStart).Start();
            ingredientEditorControl.ActiveIngredient = null;
            groupBoxIngredients.IsEnabled = false;
        }

        private void InitializeIngredientListBoxThreadStart()
        {
            foreach (Ingredient ingredient in Ingredient.GetAll())
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    listBoxIngredient.Items.Add(
                        new FormattedListBoxItem(ingredient, ingredient.FullName, true));
                }), DispatcherPriority.Background);
            }
        }

        private void InitializeCategoryListBox()
        {
            new Thread(InitializeCategoryListBoxThreadStart).Start();
            categoryEditorControl.ActiveCategory = null;
            groupBoxCategories.IsEnabled = false;
        }

        private void InitializeCategoryListBoxThreadStart()
        {
            foreach (Category category in Category.GetAll())
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var item = new FormattedListBoxItem(category, category.NameValue, true);
                    listBoxCategories.Items.Add(item);
                }), DispatcherPriority.Background);
            }
        }

        private void InitializeItemOptionListBox()
        {
            new Thread(InitializeItemOptionListBoxThreadStart).Start();
            itemOptionSetEditorControl.ActiveItemOptionSet = null;
            groupBoxItemOptions.IsEnabled = false;
        }

        private void InitializeItemOptionListBoxThreadStart()
        {
            foreach (ItemOptionSet set in ItemOptionSet.GetAll())
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    listBoxItemOption.Items.Add(new FormattedListBoxItem(set, set.Name, true));
                }), DispatcherPriority.Background);
            }
        }

        private void InitializeItemListBox()
        {
            new Thread(InitializeItemListBoxThreadStart).Start();
            itemEditorControl.ActiveItem = null;
            groupBoxItems.IsEnabled = false;
        }

        private void InitializeItemListBoxThreadStart()
        {
            lock (ItemCache)
            {
                if (_itemCacheEmpty)
                {
                    ItemCache.Clear();
                    foreach (Item item in Item.GetAll())
                    {
                        ItemCache.Add(item);
                        Dispatcher.Invoke((Action)(() =>
                        {
                            listBoxItems.Items.Add(new FormattedListBoxItem(item, item.FullName, true));
                        }), DispatcherPriority.Background);
                    }
                    _itemCacheEmpty = false;
                }
                else
                {
                    foreach (Item item in ItemCache)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            listBoxItems.Items.Add(new FormattedListBoxItem(item, item.FullName, true));
                        }), DispatcherPriority.Background);
                    }
                }
            }
        }

        private void InitializeItemListBoxForCategory()
        {
            listBoxFilteredItems.Items.Clear();
            itemEditorControl.ActiveItem = null;
            groupBoxItems.IsEnabled = false;
            if (_selectedCategory == null)
                return;
            new Thread(InitializeItemListBoxForCategoryThreadStart).Start();
        }

        private void InitializeItemListBoxForCategoryThreadStart()
        {
            lock (ItemCache)
            {
                foreach (Item item in ItemCache
                    .Where(item => item.CategoryId == _selectedCategory.Id))
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        listBoxFilteredItems.Items.Add(new FormattedListBoxItem(item, item.FullName, true));
                    }), DispatcherPriority.Background);
                }
            }
        }
        #endregion

        #region Button Handler
        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStart();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchList();
        }

        [Obfuscation(Exclude = true)]
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            PosDialogWindow window = ItemOptionMaintenanceControl.CreateInDefaultWindow(
                itemOptionSetEditorControl.ActiveItemOptionSet.Name);
            var control = window.DockedControl as ItemOptionMaintenanceControl;
            var parent = (PosDialogWindow)Window.GetWindow(this);

            if (control != null)
                control.ActiveItemOptionSet = itemOptionSetEditorControl.ActiveItemOptionSet;
            window.IsClosable = true;
            window.ShowDialog(parent);
        }

        [Obfuscation(Exclude = true)]
        private void buttonFindNext_Click(object sender, RoutedEventArgs e)
        {
            FindNext();
        }

        [Obfuscation(Exclude = true)]
        private void buttonView_Click(object sender, RoutedEventArgs e)
        {
            ShowContentMenu();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateStart();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

        [Obfuscation(Exclude = true)]
        private void ShowContentMenu()
        {
            if (buttonView.ContextMenu == null) return;
            UpdateLayout();
            if (buttonView.ContextMenu.IsOpen)
                buttonView.ContextMenu.IsOpen = false;
            buttonView.ContextMenu.Placement = PlacementMode.Top;
            buttonView.ContextMenu.PlacementTarget = buttonView;
            buttonView.ContextMenu.IsOpen = true;
        }
        #endregion

        #region Searching
        private void FindNext()
        {
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems)
                FindNextItem();
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                FindNextItemInCategory();
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                FindNextIngredient();
        }

        private void FindNextIngredient()
        {
            if (FindNextGeneric(listBoxIngredient))
            {
                var item = listBoxIngredient.SelectedItem as FormattedListBoxItem;
                if (item != null) ingredientEditorControl.ActiveIngredient = item.ReferenceObject as Ingredient;
                groupBoxIngredients.IsEnabled = true;
            }
            else
            {
                groupBoxIngredients.IsEnabled = false;
                ingredientEditorControl.ActiveIngredient = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoAdditialItemsFound + " \"" + _lastSearchString + "\"",
                    Types.Strings.ItemSetupFindNext);
            }
        }

        private void FindNextItemInCategory()
        {
            if (FindNextGeneric(listBoxFilteredItems))
            {
                var item = listBoxFilteredItems.SelectedItem as FormattedListBoxItem;
                if (item != null) itemEditorControl.ActiveItem = item.ReferenceObject as Item;
                groupBoxItems.IsEnabled = true;
            }
            else
            {
                groupBoxItems.IsEnabled = false;
                itemEditorControl.ActiveItem = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoAdditialItemsFound + " \"" + _lastSearchString + "\"",
                    Types.Strings.ItemSetupFindNext);
            }
        }

        private void FindNextItem()
        {
            if (FindNextGeneric(listBoxItems))
            {
                var item = listBoxItems.SelectedItem as FormattedListBoxItem;
                if (item != null) itemEditorControl.ActiveItem = item.ReferenceObject as Item;
                groupBoxItems.IsEnabled = true;
            }
            else
            {
                groupBoxItems.IsEnabled = false;
                itemEditorControl.ActiveItem = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoAdditialItemsFound + " \"" + _lastSearchString + "\"",
                    Types.Strings.ItemSetupFindNext);
            }
        }

        private bool FindNextGeneric(DragScrollListBox listBox)
        {
            bool found = false;
            string lowerLastSearchString = _lastSearchString.ToLower();
            for (int i = listBox.SelectedIndex + 1; i < listBox.Items.Count; i++)
            {
                var listItem = (FormattedListBoxItem)listBox.Items[i];
                if (listItem.Text.ToLower().Contains(lowerLastSearchString))
                {
                    listBox.SelectedItem = listItem;
                    found = true;
                    break;
                }
            }
            if (!found)
                listBox.SelectedItem = null;

            if (listBox.SelectedItem != null)
            {
                listBox.ScrollIntoView(listBox.SelectedItem);
                return true;
            }
            return false;
        }

        private void SearchList()
        {
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems)
                SearchListItem();
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                SearchListItemInCategory();
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                SearchListIngredient();
        }

        private void SearchListIngredient()
        {
            string searchText = PosDialogWindow.PromptKeyboard(Types.Strings.ItemSetupSearchItems, null);
            if (searchText == null)
                return;
            groupBoxIngredients.IsEnabled = false;
            if (SearchListGeneric(listBoxIngredient, searchText))
            {
                if (listBoxIngredient.SelectedItem != null)
                {
                    ingredientEditorControl.ActiveIngredient =
                        (listBoxIngredient.SelectedItem as FormattedListBoxItem)
                        .ReferenceObject as Ingredient;
                    groupBoxIngredients.IsEnabled = true;
                }
            }
            else
            {
                ingredientEditorControl.ActiveIngredient = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoItemsFound + " \"" + searchText + "\"", Types.Strings.ItemSetupFind);
            }
        }

        private void SearchListItemInCategory()
        {
            string searchText = PosDialogWindow.PromptKeyboard(Types.Strings.ItemSetupSearchItems, null);
            if (searchText == null)
                return;
            groupBoxItems.IsEnabled = false;
            if (SearchListGeneric(listBoxFilteredItems, searchText))
            {
                if (listBoxFilteredItems.SelectedItem != null)
                {
                    itemEditorControl.ActiveItem =
                        (listBoxFilteredItems.SelectedItem as FormattedListBoxItem)
                        .ReferenceObject as Item;
                    groupBoxItems.IsEnabled = true;
                }
            }
            else
            {
                itemEditorControl.ActiveItem = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoItemsFound + " \"" + searchText + "\"", Types.Strings.ItemSetupFind);
            }
        }

        private void SearchListItem()
        {
            string searchText = PosDialogWindow.PromptKeyboard(Types.Strings.ItemSetupSearchItems, null);
            if (searchText == null)
                return;
            groupBoxItems.IsEnabled = false;
            if (SearchListGeneric(listBoxItems, searchText))
            {
                if (listBoxItems.SelectedItem != null)
                {
                    itemEditorControl.ActiveItem =
                        (listBoxItems.SelectedItem as FormattedListBoxItem)
                        .ReferenceObject as Item;
                    groupBoxItems.IsEnabled = true;
                }
            }
            else
            {
                itemEditorControl.ActiveItem = null;
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupNotifyNoItemsFound + " \"" + searchText + "\"", Types.Strings.ItemSetupFind);
            }
        }

        private bool SearchListGeneric(DragScrollListBox listBox, string searchText)
        {
            _lastSearchString = searchText;
            searchText = searchText.ToLower();
            listBox.SelectedItem = null;
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                var listItem = (FormattedListBoxItem)listBox.Items[i];
                if (!listItem.Text.ToLower().Contains(searchText)) continue;
                listBox.SelectedItem = listItem;
                break;
            }
            if (listBox.SelectedItem != null)
            {
                var item = listBox.SelectedItem as FormattedListBoxItem;
                listBox.ScrollIntoView(listBox.SelectedItem);
                SetFindNextVisibility(true);
                return true;
            }
            else
            {
                SetFindNextVisibility(false);
                return false;
            }
        }

        private void SetFindNextVisibility(bool isVisible)
        {
            buttonFindNext.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            buttonSearch.Height = isVisible ? 30 : 64;
        }
        #endregion

        #region Delete / Discontinue
        private void Delete()
        {
            if ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems) ||
                (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory))
                DeleteItem();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                DeleteCategory();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                DeleteItemOptionSet();
        }

        private void DeleteItemOptionSet()
        {
            var listItem = listBoxItemOption.SelectedItem as FormattedListBoxItem;
            if (listItem != null && (listItem.ReferenceObject != null))
            {
                var itemOptionSet = listItem.ReferenceObject as ItemOptionSet;
                if ((itemOptionSet != null) && PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupConfirmDeleteOptionSet, Types.Strings.Confirmation, DialogButtons.YesNo) ==
                    DialogButton.Yes)
                {
                    foreach (Item item in Item.GetAllContainingItemOptionSet(itemOptionSet.Id))
                    {
                        var itemOptionSets = new List<int>(item.ItemOptionSetIds);
                        itemOptionSets.Remove(itemOptionSet.Id);
                        itemOptionSets.Add(0); // Keeps array the proper length
                        item.SetOptionSets(itemOptionSets.ToArray());
                        item.Update();
                    }
                    itemOptionSet.Discontinue();
                    listBoxItemOption.Items.Remove(listItem);
                    buttonDelete.IsEnabled = 
                        buttonEdit.IsEnabled = false;
                    groupBoxItemOptions.IsEnabled = false;
                    itemOptionSetEditorControl.ActiveItemOptionSet = null;
                }
            }
        }

        private void DeleteCategory()
        {
            var listItem = listBoxCategories.SelectedItem as FormattedListBoxItem;
            if (listItem == null || (listItem.ReferenceObject == null)) return;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.ItemSetupConfirmDeleteCategory,
                Types.Strings.Confirmation, DialogButtons.YesNo) == DialogButton.Yes)
            {
                var category = listItem.ReferenceObject as Category;
                int originalDisplayIndex = category.DisplayIndex;
                if (category != null)
                {
                    foreach (Item item in Item.GetAllForCategory(category.Id))
                    {
                        item.Discontinue();

                        ItemAdjustment.Add(SessionManager.ActiveEmployee.Id,
                                           ItemAdjustmentType.Discontinuation,
                                           item.Id);
                    }
                    category.Discontinue();
                }
                listBoxCategories.Items.Remove(listItem);
                buttonDelete.IsEnabled = false;
                buttonEdit.IsEnabled = false;
                groupBoxCategories.IsEnabled = false;
                categoryEditorControl.ActiveCategory = null;
                categoryEditorControl.ReindexDisplayIndices(category, originalDisplayIndex);
            }
        }

        private void DeleteItem()
        {
            var listItem = listBoxItems.SelectedItem as FormattedListBoxItem;
            if (listItem != null && (listItem.ReferenceObject != null))
            {
                if (PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupConfirmDeleteItem,
                    Types.Strings.Confirmation, DialogButtons.YesNo) ==
                    DialogButton.Yes)
                {
                    var item = listItem.ReferenceObject as Item;
                    if (item != null)
                    {
                        item.Discontinue();

                        ItemAdjustment.Add(SessionManager.ActiveEmployee.Id,
                                           ItemAdjustmentType.Discontinuation,
                                           item.Id);
                    }
                    ItemCache.Remove(item);
                    listBoxItems.Items.Remove(listItem);
                    buttonDelete.IsEnabled = false;
                    buttonEdit.IsEnabled = false;
                    groupBoxItems.IsEnabled = false;
                    itemEditorControl.ActiveItem = null;
                }
            }
        }
        #endregion

        #region Cancel Changes
        private void CancelChanges()
        {
            if ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems) ||
                (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory))
                CancelItemChanges();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                CancelCategoryChanges();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                CancelIngredientChanges();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                CancelItemOptionSetChanges();
        }

        private void CancelItemChanges()
        {
            // Reinitialize
            Item.Refresh(itemEditorControl.ActiveItem);

            itemEditorControl.itemEditorGroupingControl.Cancel();
            itemEditorControl.itemEditorIngredientsControl.Cancel();

            itemEditorControl.ActiveItem = itemEditorControl.ActiveItem;
            SetEditMode(false);
            if (listBoxItems.SelectedItem == null)
                groupBoxItems.IsEnabled = false;
        }

        private void CancelCategoryChanges()
        {
            // Reinitialize
            categoryEditorControl.ActiveCategory = categoryEditorControl.ActiveCategory;
            SetEditMode(false);
            if (listBoxCategories.SelectedItem == null)
                groupBoxCategories.IsEnabled = false;
        }

        private void CancelIngredientChanges()
        {
            // Reinitialize
            ingredientEditorControl.EditorPreparation.Cancel();
            ingredientEditorControl.ActiveIngredient = ingredientEditorControl.ActiveIngredient;
            SetEditMode(false);
            if (listBoxIngredient.SelectedItem == null)
                groupBoxIngredients.IsEnabled = false;
        }

        private void CancelItemOptionSetChanges()
        {
            var item = listBoxItemOption.SelectedItem as FormattedListBoxItem;
            ItemOptionSet set = null;
            if (item != null)
                set = item.ReferenceObject as ItemOptionSet;
            itemOptionSetEditorControl.ActiveItemOptionSet = set;
            SetEditMode(false);
        }
        #endregion

        #region Update
        private void UpdateStart()
        {
            if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems)
                UpdateItem(listBoxItems);                
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory)
                UpdateItem(listBoxFilteredItems);
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                UpdateCategory();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                UpdateIngredient();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                UpdateItemOptionSet();
            InitializeButtons();
        }

        private void UpdateItem(DragScrollListBox listBox)
        {
            if (itemEditorControl.UpdateItem())
            {
                var selectedItem = listBox.SelectedItem as FormattedListBoxItem;
                if (selectedItem != null)
                {
                    selectedItem.Set(itemEditorControl.ActiveItem,
                        itemEditorControl.ActiveItem.FullName);
                }
                else if ((_selectedCategory == null) ||
                    (itemEditorControl.ActiveItem.CategoryId == _selectedCategory.Id))
                {
                    var item = new FormattedListBoxItem(itemEditorControl.ActiveItem,
                        itemEditorControl.ActiveItem.FullName, true);
                    ItemCache.Add(itemEditorControl.ActiveItem);
                    listBox.Items.Add(item);
                    listBox.SelectedItem = item;
                    listBox.ScrollIntoView(item);
                }
                SetEditMode(false);
            }
        }

        private void UpdateCategory()
        {
            if (categoryEditorControl.UpdateCategory())
            {
                listBoxCategories.IsEnabled = true;
                if (listBoxCategories.SelectedIndex >= 0)
                {
                    var item = (FormattedListBoxItem)listBoxCategories.SelectedItem;
                    item.Set(categoryEditorControl.ActiveCategory,
                        categoryEditorControl.ActiveCategory.NameValue);
                }
                else
                {
                    var listItem = new FormattedListBoxItem(
                        categoryEditorControl.ActiveCategory,
                        categoryEditorControl.ActiveCategory.NameValue, true);
                    listBoxCategories.Items.Add(listItem);
                    listBoxCategories.SelectedItem = listItem;
                    listBoxCategories.ScrollIntoView(listItem);
                }
                SetEditMode(false);
            }
        }

        private void UpdateIngredient()
        {
            if (ingredientEditorControl.UpdateItem())
            {
                if (listBoxIngredient.SelectedItem != null)
                {
                    var item = listBoxIngredient.SelectedItem as FormattedListBoxItem;
                    if (item != null)
                        item.Set(ingredientEditorControl.ActiveIngredient,
                                 ingredientEditorControl.ActiveIngredient.FullName);
                }
                else
                {
                    var listItem = new FormattedListBoxItem(
                        ingredientEditorControl.ActiveIngredient,
                        ingredientEditorControl.ActiveIngredient.FullName, true);
                    listBoxIngredient.Items.Add(listItem);
                    listBoxIngredient.SelectedItem = listItem;
                    listBoxIngredient.ScrollIntoView(listItem);
                }
                SetEditMode(false);
            }
        }

        private void UpdateItemOptionSet()
        {
            bool isAdd = (listBoxItemOption.SelectedItem == null);
            string validationError = itemOptionSetEditorControl.GetNextValidationError();
            if (validationError == null)
            {
                itemOptionSetEditorControl.Update();
                SetEditMode(false);
                if (isAdd)
                {
                    var item = new FormattedListBoxItem(
                        itemOptionSetEditorControl.ActiveItemOptionSet,
                        itemOptionSetEditorControl.ActiveItemOptionSet.Name,
                        true);
                    listBoxItemOption.Items.Add(item);
                    listBoxItemOption.SelectedItem = item;
                    listBoxItemOption.ScrollIntoView(item);
                }
                else
                {
                    var selectedItem = listBoxItemOption.SelectedItem as FormattedListBoxItem;
                    if (selectedItem != null)
                    {
                        selectedItem.Set(itemOptionSetEditorControl.ActiveItemOptionSet,
                        itemOptionSetEditorControl.ActiveItemOptionSet.Name);
                    }
                }
                SetEditMode(false);
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupValidationError, validationError);
            }
        }
        #endregion

        #region Add
        private void AddStart()
        {
            SetFindNextVisibility(false);
            if ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems) ||
                (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory))
                AddItem();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets)
                AddItemOptionSet();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories)
                AddCategory();
            else if (ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients)
                AddIngredient();
        }

        private void AddIngredient()
        {
            listBoxIngredient.SelectedItem = null;
            ingredientEditorControl.ActiveIngredient = null;
            SetEditMode(true);
        }

        private void AddItemOptionSet()
        {
            listBoxItemOption.SelectedItem = null;
            itemOptionSetEditorControl.ActiveItemOptionSet = null;
            SetEditMode(true);
        }

        private void AddCategory()
        {
            listBoxCategories.SelectedItem = null;
            categoryEditorControl.ActiveCategory = null;
            SetEditMode(true);
            categoryEditorControl.textBoxName.Focus();
        }

        private void AddItem()
        {
            listBoxItems.SelectedItem = null;
            listBoxFilteredItems.SelectedItem = null;
            itemEditorControl.ActiveItem = null;
            SetEditMode(true);
            itemEditorControl.itemEditorDetailsControl.textBoxFullName.Focus();
        }
        #endregion

        #region Listbox Selection Event
        [Obfuscation(Exclude = true)]
        private void listBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PreProcessSelectionChanged(listBoxItems, e)) return;
            ProcessListBoxSelectionChangeForItems(e);
        }

        private bool PreProcessSelectionChanged(DragScrollListBox listBox, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return false;
            if (listBox.SelectedItem == null)
            {
                buttonDelete.IsEnabled = false;
                buttonEdit.IsEnabled = false;
                return false;
            }
            buttonDelete.IsEnabled = true;
            buttonEdit.IsEnabled = true;
            return true;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxFilteredItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PreProcessSelectionChanged(listBoxFilteredItems, e)) return;
            ProcessListBoxSelectionChangeForItems(e);
        }

        [Obfuscation(Exclude = true)]
        private void listBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PreProcessSelectionChanged(listBoxCategories, e)) return;
            ProcessListBoxSelectionChangeForCategories(e);
        }

        [Obfuscation(Exclude = true)]
        private void listBoxItemOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PreProcessSelectionChanged(listBoxItemOption, e)) return;
            ProcessListBoxSelectionChangeForItemOptionSets(e);
        }

        [Obfuscation(Exclude = true)]
        private void listBoxIngredient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PreProcessSelectionChanged(listBoxIngredient, e)) return;
            ProcessListBoxSelectionChangeForIngredients(e);
        }

        private void ShowListBox(DragScrollListBox listBox)
        {
            listBoxItems.Visibility = (listBox == listBoxItems) ? Visibility.Visible : Visibility.Collapsed;
            listBoxFilteredItems.Visibility = (listBox == listBoxFilteredItems) ? Visibility.Visible : Visibility.Collapsed;
            listBoxIngredient.Visibility = (listBox == listBoxIngredient) ? Visibility.Visible : Visibility.Collapsed;
            listBoxCategories.Visibility = (listBox == listBoxCategories) ? Visibility.Visible : Visibility.Collapsed;
            listBoxItemOption.Visibility = (listBox == listBoxItemOption) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ProcessListBoxSelectionChangeForItems(SelectionChangedEventArgs e)
        {
            var item = (FormattedListBoxItem)e.AddedItems[0];
            itemEditorControl.ActiveItem = item.ReferenceObject as Item;
            groupBoxItems.IsEnabled = true;
        }

        private void ProcessListBoxSelectionChangeForItemOptionSets(SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            var item = e.AddedItems[0] as FormattedListBoxItem;
            if (item == null) return;
            itemOptionSetEditorControl.ActiveItemOptionSet = item.ReferenceObject as ItemOptionSet;
            groupBoxItemOptions.IsEnabled = true;
        }

        private void ProcessListBoxSelectionChangeForCategories(SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            var category = (FormattedListBoxItem)e.AddedItems[0];
            _selectedCategory =
                categoryEditorControl.ActiveCategory =
                category.ReferenceObject as Category;
            groupBoxCategories.IsEnabled = true;
        }

        private void ProcessListBoxSelectionChangeForIngredients(SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            
            var selectedItem = (FormattedListBoxItem)listBoxIngredient.SelectedItem;
            var ingredient = selectedItem.ReferenceObject as Ingredient;

            groupBoxIngredients.IsEnabled = true;
            ingredientEditorControl.ActiveIngredient = ingredient;
            buttonDelete.IsEnabled =
                buttonEdit.IsEnabled = 
                (buttonAdd.IsEnabled && (selectedItem != null));
        }
        #endregion

        #region SetEditMode
        private void SetEditMode(bool inEditMode)
        {
            groupBoxList.IsEnabled = !inEditMode;
            buttonSearch.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonEdit.IsEnabled =
                buttonDelete.IsEnabled =
                (!inEditMode && (listBoxItems.SelectedItem != null));
            buttonUpdate.IsEnabled = inEditMode;
            buttonCancel.IsEnabled = inEditMode;
            buttonView.IsEnabled = !inEditMode;
            if (inEditMode && (buttonView.ContextMenu != null) && buttonView.ContextMenu.IsOpen)
                buttonView.ContextMenu.IsOpen = false;
            SetEditModeFromEditors(inEditMode);
            var parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null) parentWindow.SetButtonsEnabled(!inEditMode);
        }

        private void SetEditModeFromEditors(bool inEditMode)
        {
            bool enableEditor = (inEditMode || (GetVisibleListBox().SelectedItem != null));
            groupBoxItems.IsEnabled =
                ((ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.AllItems ||
                ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemsInCategory) && enableEditor);

            groupBoxItemOptions.IsEnabled = 
                ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.ItemOptionSets && enableEditor;
            
            groupBoxCategories.IsEnabled =
                ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Categories && enableEditor;

            groupBoxIngredients.IsEnabled = 
                ItemMaintenanceViewContextMenuControl.ViewMode ==
                ItemMaintenanceViewContextMenuControl.ItemViewMode.Ingredients && enableEditor;
        }

        [Obfuscation(Exclude = true)]
        void editorControl_ValueChanged(object sender, ItemValueChangedArgs args)
        {
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void itemOptionSetEditorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void categoryEditorControl_ValueChanged(object sender, CategoryValueChangedArgs args)
        {
            SetEditMode(true);
        }

        [Obfuscation(Exclude = true)]
        private void ingredientEditorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }
        #endregion
    }
}
