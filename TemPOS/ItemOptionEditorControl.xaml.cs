using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosModels.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemOptionEditorControl.xaml
    /// </summary>
    public partial class ItemOptionEditorControl : UserControl
    {
        #region Fields
        private bool _haltEvents;
        private MeasurementUnit _measurementUnit = MeasurementUnit.None;
        private ItemOption _activeItemOption;
        #endregion

        #region Properties
        public ItemOptionSet ActiveItemOptionSet
        {
            get;
            set;
        }

        public ItemOption ActiveItemOption
        {
            get
            {
                return _activeItemOption;
            }
            set
            {
                _haltEvents = true;
                _activeItemOption = value;
                InitializeFields();
                _haltEvents = false;
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if ((ValueChanged != null) && (ActiveItemOption != null) && !_haltEvents)
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public ItemOptionEditorControl()
        {
            InitializeComponent();
            InitializeIngredients();
            InitializeItems();
        }

        private void InitializeFields()
        {            
            if (ActiveItemOption != null)
            {
                textBoxName.Text = ActiveItemOption.Name;
                textBoxExtraCost.Text = ActiveItemOption.CostForExtra.HasValue ?
                    ActiveItemOption.CostForExtra.Value.ToString("C2") : null;
                textBoxPortionAmount.Text = ActiveItemOption.ProductAmount.HasValue ?
                    ActiveItemOption.ProductAmount.Value.ToString("F2") : null;
                if (ActiveItemOption.UsesIngredient)
                {
                    InitializeUnitsComboBox(true);
                    SelectRadioButton(radioButtonUsesIngredient);
                    if (ActiveItemOption.ProductId != null)
                    {
                        Ingredient ingredient = Ingredient.Get(ActiveItemOption.ProductId.Value);
                        comboBoxAddsIngredientChoice.SelectedItem = ingredient.FullName;
                    }
                    SelectComboBoxIndexOf(ActiveItemOption.ProductMeasurementUnit.ToString());
                    ShowProductEntry(true);
                }
                else if (ActiveItemOption.UsesItem)
                {
                    SelectRadioButton(radioButtonUsesItem);
                    if (ActiveItemOption.ProductId != null)
                    {
                        Item item = Item.Get(ActiveItemOption.ProductId.Value);
                        comboBoxItemChoice.SelectedItem = item.FullName;
                    }
                    ShowProductEntry(false);
                }
                else
                {
                    SelectRadioButton(radioButtonUsesNothing);
                    ShowProductEntry(null);
                }
            }
            else
            {
                textBoxName.Text = "";
                textBoxExtraCost.Text = "";
                textBoxPortionAmount.Text = "";
                comboBoxAddsIngredientChoice.SelectedIndex = -1;
                comboBoxItemChoice.SelectedIndex = -1;
                SelectRadioButton(radioButtonUsesNothing);
                ShowProductEntry(null);
            }
        }

        private void InitializeUnitsComboBox(bool setMeasurementUnit)
        {
            if (setMeasurementUnit)
                _measurementUnit = (ActiveItemOption.ProductMeasurementUnit.HasValue ?
                    ActiveItemOption.ProductMeasurementUnit.Value : MeasurementUnit.None);
            comboBoxMeasuringUnits.Items.Clear();
            if (!_measurementUnit.IsVolume() &&
                !_measurementUnit.IsWeight())
            {
                comboBoxMeasuringUnits.Items.Add(Types.Strings.UnmeasuredUnits);
            }
            else if (_measurementUnit.IsWeight())
            {
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightPound);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightOunce);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightGram);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightMilligram);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightKilogram);
            }
            else if (_measurementUnit.IsVolume())
            {
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeGallon);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeQuart);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumePint);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeCup);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeTablespoon);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeTeaspoon);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeLiter);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeFluidOunce);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeMilliliter);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.VolumeKiloliter);
            }
        }

        private void SelectComboBoxIndexOf(string containingText)
        {
            comboBoxMeasuringUnits.SelectedItem = containingText;
            for (int i = 0; i < comboBoxMeasuringUnits.Items.Count; i++)
            {
                string text = comboBoxMeasuringUnits.Items[i];
                if (!text.Contains(containingText)) continue;
                comboBoxMeasuringUnits.SelectedIndex = i;
                return;
            }
            comboBoxMeasuringUnits.SelectedItem = null;
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxMeasuringUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_measurementUnit.IsWeight())
            {
                if (comboBoxMeasuringUnits.SelectedIndex == 0)
                    _measurementUnit = MeasurementUnit.Pound;
                else if (comboBoxMeasuringUnits.SelectedIndex == 1)
                    _measurementUnit = MeasurementUnit.Ounce;
                else if (comboBoxMeasuringUnits.SelectedIndex == 2)
                    _measurementUnit = MeasurementUnit.Gram;
                else if (comboBoxMeasuringUnits.SelectedIndex == 3)
                    _measurementUnit = MeasurementUnit.Milligram;
                else if (comboBoxMeasuringUnits.SelectedIndex == 4)
                    _measurementUnit = MeasurementUnit.Kilogram;
            }
            else if (_measurementUnit.IsVolume())
            {
                if (comboBoxMeasuringUnits.SelectedIndex == 0)
                    _measurementUnit = MeasurementUnit.Gallon;
                else if (comboBoxMeasuringUnits.SelectedIndex == 1)
                    _measurementUnit = MeasurementUnit.Quart;
                else if (comboBoxMeasuringUnits.SelectedIndex == 2)
                    _measurementUnit = MeasurementUnit.Pint;
                else if (comboBoxMeasuringUnits.SelectedIndex == 3)
                    _measurementUnit = MeasurementUnit.Cup;
                else if (comboBoxMeasuringUnits.SelectedIndex == 4)
                    _measurementUnit = MeasurementUnit.Tablespoon;
                else if (comboBoxMeasuringUnits.SelectedIndex == 5)
                    _measurementUnit = MeasurementUnit.Teaspoon;
                else if (comboBoxMeasuringUnits.SelectedIndex == 6)
                    _measurementUnit = MeasurementUnit.Liter;
                else if (comboBoxMeasuringUnits.SelectedIndex == 7)
                    _measurementUnit = MeasurementUnit.FluidOunce;
                else if (comboBoxMeasuringUnits.SelectedIndex == 8)
                    _measurementUnit = MeasurementUnit.Milliliter;
                else if (comboBoxMeasuringUnits.SelectedIndex == 9)
                    _measurementUnit = MeasurementUnit.Kiloliter;
            }
            else
                _measurementUnit = MeasurementUnit.Unit;
        }

        private void InitializeItems()
        {
            comboBoxItemChoice.Items.Clear();
            foreach (Item item in Item.GetAll())
            {
                if (comboBoxItemChoice.Items.Count == 0)
                {
                    comboBoxItemChoice.Items.Add(item.FullName);
                    continue;
                }
                for (int i = 0; i < comboBoxItemChoice.Items.Count; i++)
                {
                    string first = comboBoxItemChoice.Items[i];
                    if (i == (comboBoxItemChoice.Items.Count - 1))
                    {
                        if (String.Compare(first, item.FullName, StringComparison.Ordinal) > 0)
                        {
                            comboBoxItemChoice.Items.Insert(i, item.FullName);
                            break;
                        }
                        comboBoxItemChoice.Items.Add(item.FullName);
                        break;
                    }
                    if (String.Compare(first, item.FullName, StringComparison.Ordinal) > 0)
                    {
                        comboBoxItemChoice.Items.Insert(i, item.FullName);
                        break;
                    }
                    string second = comboBoxItemChoice.Items[i + 1];
                    // Insertion Point between i and i + 1
                    if ((String.Compare(first, item.FullName, StringComparison.Ordinal) < 0) &&
                        (String.Compare(second, item.FullName, StringComparison.Ordinal) > 0))
                    {
                        comboBoxItemChoice.Items.Insert(i + 1, item.FullName);
                        break;
                    }
                }
            }
            //comboBoxItemChoice.Items.Insert(0, "");
        }

        private void InitializeIngredients()
        {
            comboBoxAddsIngredientChoice.Items.Clear();
            foreach (Ingredient ingredient in Ingredient.GetAll())
            {
                if (comboBoxAddsIngredientChoice.Items.Count == 0)
                {
                    comboBoxAddsIngredientChoice.Items.Add(ingredient.FullName);
                    continue;
                }
                for (int i = 0; i < comboBoxAddsIngredientChoice.Items.Count; i++)
                {
                    string first = comboBoxAddsIngredientChoice.Items[i];
                    if (i == (comboBoxAddsIngredientChoice.Items.Count - 1))
                    {
                        if (String.Compare(first, ingredient.FullName, StringComparison.Ordinal) > 0)
                        {
                            comboBoxAddsIngredientChoice.Items.Insert(i, ingredient.FullName);
                            break;
                        }
                        comboBoxAddsIngredientChoice.Items.Add(ingredient.FullName);
                        break;
                    }
                    if (String.Compare(first, ingredient.FullName, StringComparison.Ordinal) > 0)
                    {
                        comboBoxAddsIngredientChoice.Items.Insert(i, ingredient.FullName);
                        break;
                    }
                    string second = comboBoxAddsIngredientChoice.Items[i + 1];
                    // Insertion Point between i and i + 1
                    if ((String.Compare(first, ingredient.FullName, StringComparison.Ordinal) < 0) &&
                        (String.Compare(second, ingredient.FullName, StringComparison.Ordinal) > 0))
                    {
                        comboBoxAddsIngredientChoice.Items.Insert(i + 1, ingredient.FullName);
                        break;
                    }
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxExtraCost_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxItemChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxAddsIngredientChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = comboBoxAddsIngredientChoice.SelectedItem;
            Ingredient ingredient = Ingredient.Get(name);
            _measurementUnit = ingredient != null ? ingredient.MeasurementUnit : MeasurementUnit.None;
            InitializeUnitsComboBox(false);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPortionAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        public bool HasValidationError()
        {
            // Check if an item id is specified, and if so, that item doesn't have 
            // this ItemOption in it's sets
            //if (!ActiveItemOption.UsesItem || (ActiveItemOption.ProductId != null))
            if (!radioButtonUsesItem.IsSelected)
                return false;
            Item item = Item.Get(GetItemId());
            if ((item.ItemOptionSetIds[0] == ActiveItemOptionSet.Id) ||
                (item.ItemOptionSetIds[1] == ActiveItemOptionSet.Id) ||
                (item.ItemOptionSetIds[2] == ActiveItemOptionSet.Id))
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.ItemSetupItemRecursionError, Types.Strings.ItemSetupValidationError);
                return true;
            }
            return false;

        }

        public void Update()
        {
            if (ActiveItemOption == null)
            {
                if (radioButtonUsesIngredient.IsSelected)
                    ActiveItemOption = ItemOption.Add(ActiveItemOptionSet.Id,
                        textBoxName.Text, GetCostForExtra(), true, GetIngredientId(),
                        GetPortionAmount(), _measurementUnit);
                else if (radioButtonUsesItem.IsSelected)
                    ActiveItemOption = ItemOption.Add(ActiveItemOptionSet.Id,
                        textBoxName.Text, GetCostForExtra(), false, GetItemId(),
                        GetPortionAmount(), _measurementUnit);
                else
                    ActiveItemOption = ItemOption.Add(ActiveItemOptionSet.Id,
                        textBoxName.Text, GetCostForExtra(), false, null, null, null);
            }
            else
            {                
                ActiveItemOption.SetName(textBoxName.Text);
                ActiveItemOption.SetCostForExtra(GetCostForExtra());
                if (radioButtonUsesIngredient.IsSelected)
                {
                    ActiveItemOption.SetUsesIngredient(true);
                    ActiveItemOption.SetProductId(GetIngredientId());
                    ActiveItemOption.SetProductAmount(GetPortionAmount());
                    ActiveItemOption.SetProductMeasurementUnit(_measurementUnit);
                }
                else if (radioButtonUsesItem.IsSelected)
                {
                    ActiveItemOption.SetUsesIngredient(false);
                    ActiveItemOption.SetProductId(GetItemId());
                    ActiveItemOption.SetProductAmount(GetPortionAmount());
                    ActiveItemOption.SetProductMeasurementUnit(MeasurementUnit.Unit);
                }
                else
                {
                    ActiveItemOption.SetUsesIngredient(false);
                    ActiveItemOption.SetProductId(null);
                    ActiveItemOption.SetProductAmount(null);
                    ActiveItemOption.SetProductMeasurementUnit(null);
                }
                ActiveItemOption.Update();
            }
        }

        private double? GetCostForExtra()
        {

            try
            {
                return Convert.ToDouble(textBoxExtraCost.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch
            {
                return null;
            }
        }

        private int GetItemId()
        {
            int result = 0;

            string name = comboBoxItemChoice.SelectedItem;
            Item item = Item.Get(name);
            if (item != null)
                result = item.Id;

            return result;
        }

        private int GetIngredientId()
        {
            int result = 0;

            string name = comboBoxAddsIngredientChoice.SelectedItem;
            Ingredient ingredient = Ingredient.Get(name);
            if (ingredient != null)
                result = ingredient.Id;

            return result;
        }

        private double GetPortionAmount()
        {
            try
            {
                return Convert.ToDouble(textBoxPortionAmount.Text);
            }
            catch
            {
                return 0;
            }
        }

        private void SelectRadioButton(PushRadioButton radioButton)
        {
            radioButtonUsesNothing.IsSelected = Equals(radioButton, radioButtonUsesNothing);
            radioButtonUsesItem.IsSelected = Equals(radioButton, radioButtonUsesItem);
            radioButtonUsesIngredient.IsSelected = Equals(radioButton, radioButtonUsesIngredient);
        }

        private void ShowProductEntry(bool? showIngregients)
        {
            labelAmount.Visibility =
                labelSpecifiedUse.Visibility =
                textBoxPortionAmount.Visibility =
                ((showIngregients != null) ? Visibility.Visible : Visibility.Hidden);
            labelMeasurementUnit.Visibility =
                comboBoxMeasuringUnits.Visibility =
                comboBoxAddsIngredientChoice.Visibility = (((showIngregients != null) &&
                    showIngregients.Value) ? Visibility.Visible : Visibility.Hidden);
            comboBoxItemChoice.Visibility = (((showIngregients != null) &&
                !showIngregients.Value) ? Visibility.Visible : Visibility.Hidden);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonUsesNothing_SelectionGained(object sender, EventArgs e)
        {
            DoChangedValueEvent();
            radioButtonUsesIngredient.IsSelected =
                radioButtonUsesItem.IsSelected = false;
            textBoxPortionAmount.Text = null;
            comboBoxItemChoice.SelectedIndex = -1;
            comboBoxAddsIngredientChoice.SelectedIndex = -1;
            ShowProductEntry(null);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonUsesIngredient_SelectionGained(object sender, EventArgs e)
        {
            DoChangedValueEvent();
            radioButtonUsesNothing.IsSelected =
                radioButtonUsesItem.IsSelected = false;
            labelSpecifiedUse.Content = Types.Strings.ItemSetupIngredient;
            textBoxPortionAmount.Text = null;
            comboBoxItemChoice.SelectedIndex = -1;
            comboBoxAddsIngredientChoice.SelectedIndex = -1;
            InitializeUnitsComboBox(true);
            ShowProductEntry(true);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonUsesItem_SelectionGained(object sender, EventArgs e)
        {
            DoChangedValueEvent();
            radioButtonUsesNothing.IsSelected =
                radioButtonUsesIngredient.IsSelected = false;
            labelSpecifiedUse.Content = Types.Strings.ItemSetupItem;
            textBoxPortionAmount.Text = null;
            comboBoxItemChoice.SelectedIndex = -1;
            comboBoxAddsIngredientChoice.SelectedIndex = -1;
            ShowProductEntry(false);
        }

    }
}
