using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Helpers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorIngredientsControl.xaml
    /// </summary>
    public partial class ItemEditorIngredientsControl : UserControl
    {
        private readonly List<ItemIngredient> _itemIngredientsAdded =
            new List<ItemIngredient>();
        private readonly List<ItemIngredient> _itemIngredientsNeedingUpdate =
            new List<ItemIngredient>();
        private readonly List<ItemIngredient> _itemIngredientsRemoved =
            new List<ItemIngredient>();
        private Item _activeItem;
        private readonly List<Ingredient> _cachedIngredients = new List<Ingredient>();

        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;

        public Item ActiveItem
        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;
                _cachedIngredients.Clear();
                _cachedIngredients.AddRange(Ingredient.GetAll());
                InitializeFields();
            }
        }

        public ItemEditorIngredientsControl()
        {
            InitializeComponent();
        }

        private void InitializeFields()
        {
            listBox.SelectedItem = null;
            listBoxItemIngredients.SelectedItem = null;
            listBox.Items.Clear();
            listBoxItemIngredients.Items.Clear();

            List<Ingredient> ingredients = new List<Ingredient>(_cachedIngredients);

            if (ActiveItem != null)
            {
                foreach (ItemIngredient itemIngredient in ItemIngredient.GetAll(ActiveItem.Id))
                {
                    bool added, changed, removed;
                    ItemIngredient current = GetItemIngredient(itemIngredient.Id,
                        out added, out changed, out removed);
                    if (!removed)
                    {
                        Ingredient ingredient = Ingredient.Find(ingredients, itemIngredient.IngredientId);
                        AddIngredientSetToListBox(
                            (changed ? current : itemIngredient), ingredient);
                        ingredients.Remove(ingredient);
                    }
                }
            }

            // Note: Added ones have an ItemId of zero so GetAll (above) will not find them
            foreach (ItemIngredient itemIngredient in _itemIngredientsAdded)
            {
                Ingredient ingredient = Ingredient.Find(ingredients, itemIngredient.IngredientId);
                AddIngredientSetToListBox(itemIngredient, ingredient);
                ingredients.Remove(ingredient);
            }

            foreach (Ingredient ingredient in ingredients)
            {
                listBox.Items.Add(new FormattedListBoxItem(ingredient.Id,
                    ingredient.FullName, true));
            }

            SetButtonsEnabled();
        }

        private void AddIngredientSetToListBox(ItemIngredient itemIngredient, Ingredient ingredient)
        {
            string amount = FormatSingleToString(itemIngredient.Amount);
            string tagLine = ingredient.FullName + Environment.NewLine +
                Strings.ItemEditorAmount + amount + " " + itemIngredient.MeasurementUnit.ToString() +
                ((amount.Equals("1.0") ? "" : Strings.S));
            listBoxItemIngredients.Items.Add(new FormattedListBoxItem(itemIngredient.Id,
                tagLine, true));
        }

        private void SetButtonsEnabled()
        {
            if (DayOfOperation.Today != null)
            {
                buttonAdd.Visibility =
                    buttonAmount.Visibility =
                    buttonRemove.Visibility =
                    Visibility.Collapsed;
                labelReadOnly.Visibility = Visibility.Visible;
                return;
            }

            buttonAdd.IsEnabled = (listBox.SelectedItem != null);
            buttonAmount.IsEnabled =
                buttonRemove.IsEnabled = (listBoxItemIngredients.SelectedItem != null);
        }

        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;

            listBoxItemIngredients.SelectedItem = null;
            SetButtonsEnabled();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void listBoxItemIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;

            listBox.SelectedItem = null;
            SetButtonsEnabled();
        }


        private static string FormatSingleToString(double single)
        {
            NumberFormatInfo nfi = new NumberFormatInfo {NumberDecimalDigits = 1};
            return single.ToString("F", nfi);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem =
                listBox.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;

            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(Strings.ItemEditorIngredientAmount);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;
            Ingredient ingredient = Ingredient.Get(selectedItem.Id);

            control.Amount = 0;
            control.MeasurementUnit = ingredient.MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser && (control.Amount > 0))
            {
                _itemIngredientsAdded.Add(
                    ItemIngredient.Add(0, selectedItem.Id, control.Amount,
                        control.MeasurementUnit));
                InitializeFields();
                DoValueChangedEvent();
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAmount_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem =
                (FormattedListBoxItem)listBoxItemIngredients.SelectedItem;
            bool added = false, changed = false, removed = false;
            ItemIngredient itemIngredient = GetItemIngredient(selectedItem.Id,
                out added, out changed, out removed);
            Ingredient ingredient = Ingredient.Get(itemIngredient.IngredientId);

            PosDialogWindow window = IngredientAmountControl
                .CreateInDefaultWindow(Strings.ItemEditorEditIngredient);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.Amount = itemIngredient.Amount;
            control.MeasurementUnit = itemIngredient.MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                if (control.Amount > 0)
                {
                    itemIngredient.SetAmount(control.Amount);
                    itemIngredient.SetMeasurementUnit(control.MeasurementUnit);
                    if (!added && !changed)
                        _itemIngredientsNeedingUpdate.Add(itemIngredient);
                }
                else
                {
                    RemoveItemIngredient(selectedItem.Id);
                }
                listBox.SelectedItem = null;
                InitializeFields();
                DoValueChangedEvent();
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem = listBoxItemIngredients.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;

            DialogButton result = PosDialogWindow.ShowDialog(
                Strings.ItemEditorConfirmIngredientRemove,
                Strings.Confirmation, DialogButtons.YesNo);
            if (result == DialogButton.Yes)
            {
                RemoveItemIngredient(selectedItem.Id);
                InitializeFields();
                DoValueChangedEvent();
            }
        }

        private void RemoveItemIngredient(int id)
        {
            bool added, changed, removed;
            ItemIngredient itemIngredient = GetItemIngredient(id,
                out added, out changed, out removed);
            if (added)
                _itemIngredientsAdded.Remove(itemIngredient);
            if (changed)
                _itemIngredientsNeedingUpdate.Remove(itemIngredient);
            if (!added && !removed)
                _itemIngredientsRemoved.Add(itemIngredient);
        }

        private ItemIngredient GetItemIngredient(int id,
            out bool cachedAdd, out bool cachedChange, out bool cachedRemove)
        {
            cachedAdd = false;
            cachedChange = false;
            cachedRemove = false;
            foreach (ItemIngredient itemIngredient in
                _itemIngredientsAdded.Where(itemIngredient => itemIngredient.Id == id))
            {
                cachedAdd = true;
                return itemIngredient;
            }
            foreach (ItemIngredient itemIngredient in
                _itemIngredientsNeedingUpdate.Where(itemIngredient => itemIngredient.Id == id))
            {
                cachedChange = true;
                return itemIngredient;
            }
            foreach (ItemIngredient itemIngredient in
                _itemIngredientsRemoved.Where(itemIngredient => itemIngredient.Id == id))
            {
                cachedRemove = true;
                return itemIngredient;
            }
            return ItemIngredient.Get(id);
        }

        public void Update(int itemId)
        {
            // Added Ingredients
            foreach (ItemIngredient itemIngredient in _itemIngredientsAdded)
            {
                itemIngredient.SetItemId(itemId);
                itemIngredient.Update();
                ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    itemId, itemIngredient.IngredientId, null,
                    itemIngredient.Amount, itemIngredient.MeasurementUnit);
            }
            // Changed Ingredients
            foreach (ItemIngredient itemIngredient in _itemIngredientsNeedingUpdate)
            {
                ItemIngredient original = ItemIngredient.Get(itemIngredient.Id);
                double oldAmount = UnitConversion.Convert(original.Amount, original.MeasurementUnit,
                    itemIngredient.MeasurementUnit);
                itemIngredient.Update();
                ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    itemId, itemIngredient.IngredientId, oldAmount, itemIngredient.Amount,
                    itemIngredient.MeasurementUnit);            
            }
            // Removed Ingredients
            foreach (ItemIngredient itemIngredient in _itemIngredientsRemoved)
            {
                ItemIngredient.Delete(itemIngredient.Id);
                ItemIngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    itemId, itemIngredient.IngredientId, itemIngredient.Amount, null,
                    itemIngredient.MeasurementUnit);
            }
            _itemIngredientsAdded.Clear();
            _itemIngredientsNeedingUpdate.Clear();
            _itemIngredientsRemoved.Clear();
        }

        public void Cancel()
        {
            foreach (ItemIngredient itemIngredient in _itemIngredientsAdded)
            {
                ItemIngredient.Delete(itemIngredient.Id);
            }
            _itemIngredientsAdded.Clear();
            _itemIngredientsNeedingUpdate.Clear();
            _itemIngredientsRemoved.Clear();

            // Reset UI
            InitializeFields();
        }

        private void DoValueChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }
    }
}
