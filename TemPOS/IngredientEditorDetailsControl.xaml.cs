using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Types;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Types;
using PosModels.Helpers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for IngredientEditorDetailsControl.xaml
    /// </summary>
    public partial class IngredientEditorDetailsControl : UserControl
    {
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;

        [Obfuscation(Exclude = true)]
        public event EventHandler UnitsChanged;

        private Ingredient _activeIngredient;

        public Ingredient ActiveIngredient
        {
            get { return _activeIngredient; }
            set
            {
                _activeIngredient = value;
                IsAdjustedByRecipe = false;
                if (_activeIngredient != null)
                {
                    FullName = _activeIngredient.FullName;
                    ShortName = _activeIngredient.ShortName;
                    InventoryAmount = _activeIngredient.InventoryAmount;
                    MeasurementUnit = _activeIngredient.MeasurementUnit;
                    CostPerUnit = _activeIngredient.CostPerUnit;
                    ParQuantity = _activeIngredient.ParQuantity;
                    ExtendedIngredientYieldAmount = _activeIngredient.ExtendedIngredientYield;
                }
                else
                {
                    FullName = "";
                    ShortName = "";
                    InventoryAmount = 0;
                    MeasurementUnit = MeasurementUnit.None;
                    CostPerUnit = 0;
                    ParQuantity = null;
                    ExtendedIngredientYieldAmount = null;
                }
                InitializeFields();
            }
        }

        public string FullName
        {
            get;
            private set;
        }

        public string ShortName
        {
            get;
            private set;
        }

        public double InventoryAmount
        {
            get;
            private set;
        }

        public MeasurementUnit MeasurementUnit
        {
            get;
            private set;
        }

        public double CostPerUnit
        {
            get;
            private set;
        }

        public double? ParQuantity
        {
            get;
            private set;
        }

        public bool IsAdjustedByRecipe
        {
            get;
            private set;
        }

        public double? ExtendedIngredientYieldAmount
        {
            get;
            private set;
        }

        public IngredientEditorDetailsControl()
        {
            InitializeComponent();
            InitializeUnitsComboBox();
            InitializeFields();
        }

        public void SetExtendedIngredientYieldAmount(double? amount)
        {
            ExtendedIngredientYieldAmount = amount;
        }

        public void ShowCostPerUnit(bool show)
        {
            textBoxCostPerUnit.Visibility =
                labelCostPerUnit.Visibility =
                (show ? Visibility.Visible : Visibility.Collapsed);
        }

        private void InitializeUnitsComboBox()
        {
            comboBoxMeasuringUnits.Items.Add(Types.Strings.UnmeasuredUnits);
            comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightPound);
            comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightOunce);
            comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightGram);
            comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightMilligram);
            comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightKilogram);
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

        private void InitializeFields()
        {
            if (ActiveIngredient == null)
            {
                ClearFields();
                return;
            }

            textBoxFullName.Text = ActiveIngredient.FullName;
            textBoxShortName.Text = ActiveIngredient.ShortName;
            textBoxInventoryAmount.Text =
                FormatDoubleToString(ActiveIngredient.InventoryAmount);
            textBoxCostPerUnit.Text =
                ActiveIngredient.CostPerUnit.ToString("C2");
            textBoxParAmount.Text = ActiveIngredient.ParQuantity.HasValue ?
                FormatDoubleToString(ActiveIngredient.ParQuantity.Value) : null;

            // Initialize combobox selection
            switch (ActiveIngredient.MeasurementUnit)
            {
                case MeasurementUnit.None:
                case MeasurementUnit.Unit:
                    comboBoxMeasuringUnits.SelectedIndex = 0;
                    break;
                case MeasurementUnit.Pound:
                    comboBoxMeasuringUnits.SelectedIndex = 1;
                    break;
                case MeasurementUnit.Ounce:
                    comboBoxMeasuringUnits.SelectedIndex = 2;
                    break;
                case MeasurementUnit.Gram:
                    comboBoxMeasuringUnits.SelectedIndex = 3;
                    break;
                case MeasurementUnit.Milligram:
                    comboBoxMeasuringUnits.SelectedIndex = 4;
                    break;
                case MeasurementUnit.Kilogram:
                    comboBoxMeasuringUnits.SelectedIndex = 5;
                    break;
                case MeasurementUnit.Gallon:
                    comboBoxMeasuringUnits.SelectedIndex = 6;
                    break;
                case MeasurementUnit.Quart:
                    comboBoxMeasuringUnits.SelectedIndex = 7;
                    break;
                case MeasurementUnit.Pint:
                    comboBoxMeasuringUnits.SelectedIndex = 8;
                    break;
                case MeasurementUnit.Cup:
                    comboBoxMeasuringUnits.SelectedIndex = 9;
                    break;
                case MeasurementUnit.Tablespoon:
                    comboBoxMeasuringUnits.SelectedIndex = 10;
                    break;
                case MeasurementUnit.Teaspoon:
                    comboBoxMeasuringUnits.SelectedIndex = 11;
                    break;
                case MeasurementUnit.Liter:
                    comboBoxMeasuringUnits.SelectedIndex = 12;
                    break;
                case MeasurementUnit.FluidOunce:
                    comboBoxMeasuringUnits.SelectedIndex = 13;
                    break;
                case MeasurementUnit.Milliliter:
                    comboBoxMeasuringUnits.SelectedIndex = 14;
                    break;
                case MeasurementUnit.Kiloliter:
                    comboBoxMeasuringUnits.SelectedIndex = 15;
                    break;
                default:
                    comboBoxMeasuringUnits.SelectedItem = null;
                    break;
            }
        }

        private void ClearFields()
        {
            textBoxFullName.Text = "";
            textBoxShortName.Text = "";
            textBoxInventoryAmount.Text = "";
            comboBoxMeasuringUnits.SelectedIndex = -1;
            textBoxCostPerUnit.Text = "";
        }

        private string FormatDoubleToString(double single)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalDigits = 2
            };
            return single.ToString("F", nfi);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxFullName_TextChanged(object sender, RoutedEventArgs e)
        {
            FullName = textBoxFullName.Text;
            DoValueChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxShortName_TextChanged(object sender, RoutedEventArgs e)
        {
            ShortName = textBoxFullName.Text;
            DoValueChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonIncreaseInventory_Click(object sender, RoutedEventArgs e)
        {
            if (!IngredientSet.HasEntries(ActiveIngredient.Id))
                AdjustInventory(Types.Strings.IngredientEditorIncreaseByAmount, 1);
            else
                AdjustInventoryByRecipe(Types.Strings.IngredientEditorIncreaseByRecipe, 1);
        }

        [Obfuscation(Exclude = true)]
        private void buttonDecreaseInventory_Click(object sender, RoutedEventArgs e)
        {
            if (!IngredientSet.HasEntries(ActiveIngredient.Id))
                AdjustInventory(Types.Strings.IngredientEditorDecreaseByAmount, -1);
            else
                AdjustInventoryByRecipe(Types.Strings.IngredientEditorDecreaseByRecipe, -1);
        }

        private void AdjustInventoryByRecipe(string windowTitle, int factor)
        {
            if (ActiveIngredient.ExtendedIngredientYield == null)
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.IngredientEditorNoYieldError,
                    Types.Strings.Error);
                return;
            }
            double? amount = PosDialogWindow.PromptNumber(windowTitle, (double)0);
            if ((amount != null) && (amount.Value > 0))
            {
                IsAdjustedByRecipe = true;
                if (ExtendedIngredientYieldAmount != null)
                    InventoryAmount += (ExtendedIngredientYieldAmount.Value * amount.Value * factor);
                textBoxInventoryAmount.Text = FormatDoubleToString(InventoryAmount);
                DoValueChangedEvent();
            }
        }

        private void AdjustInventory(string windowTitle, int factor)
        {
            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(windowTitle);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.Amount = 0;
            control.MeasurementUnit = MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                if (control.Amount > 0)
                {
                    double amount = UnitConversion.Convert(control.Amount, control.MeasurementUnit,
                        ActiveIngredient.MeasurementUnit) * factor;
                    InventoryAmount += amount;
                    textBoxInventoryAmount.Text = FormatDoubleToString(InventoryAmount);
                    DoValueChangedEvent();
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxInventoryAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryAmount = Convert.ToDouble(textBoxInventoryAmount.Text);
                DoValueChangedEvent();
            }
            catch
            {
            }
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxMeasuringUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            MeasurementUnit oldUnit = MeasurementUnit;
            string text = comboBoxMeasuringUnits.SelectedItem;
            if (text == null)
            {
                MeasurementUnit = MeasurementUnit.None;
            }
            else
            {
                switch (comboBoxMeasuringUnits.SelectedIndex)
                {
                    case 0:
                        MeasurementUnit = MeasurementUnit.Unit;
                        break;
                    case 1:
                        MeasurementUnit = MeasurementUnit.Pound;
                        break;
                    case 2:
                        MeasurementUnit = MeasurementUnit.Ounce;
                        break;
                    case 3:
                        MeasurementUnit = MeasurementUnit.Gram;
                        break;
                    case 4:
                        MeasurementUnit = MeasurementUnit.Kilogram;
                        break;
                    case 5:
                        MeasurementUnit = MeasurementUnit.Milligram;
                        break;
                    case 6:
                        MeasurementUnit = MeasurementUnit.Gallon;
                        break;
                    case 7:
                        MeasurementUnit = MeasurementUnit.Quart;
                        break;
                    case 8:
                        MeasurementUnit = MeasurementUnit.Pint;
                        break;
                    case 9:
                        MeasurementUnit = MeasurementUnit.Cup;
                        break;
                    case 10:
                        MeasurementUnit = MeasurementUnit.Tablespoon;
                        break;
                    case 11:
                        MeasurementUnit = MeasurementUnit.Teaspoon;
                        break;
                    case 12:
                        MeasurementUnit = MeasurementUnit.Liter;
                        break;
                    case 13:
                        MeasurementUnit = MeasurementUnit.FluidOunce;
                        break;
                    case 14:
                        MeasurementUnit = MeasurementUnit.Milliliter;
                        break;
                    case 15:
                        MeasurementUnit = MeasurementUnit.Kiloliter;
                        break;
                }
            }
            if ((oldUnit != MeasurementUnit) &&
                (MeasurementUnit != MeasurementUnit.Unit) && (MeasurementUnit != MeasurementUnit.None) &&
                ((oldUnit.IsVolume() && MeasurementUnit.IsVolume()) || (oldUnit.IsWeight() && MeasurementUnit.IsWeight())) &&
                PosDialogWindow.ShowDialog(
                Types.Strings.IngredientEditorConvert1 +
                (IngredientSet.HasEntries(ActiveIngredient.Id) ? Types.Strings.IngredientEditorConvert2 : "") +
                Types.Strings.IngredientEditorConvert3,
                Types.Strings.IngredientEditorUpdateInventory, DialogButtons.YesNo) == DialogButton.Yes)
            {
                InventoryAmount =
                    UnitConversion.Convert(InventoryAmount, oldUnit, MeasurementUnit);
                textBoxInventoryAmount.Text = FormatDoubleToString(InventoryAmount);
                if (UnitsChanged != null)
                    UnitsChanged.Invoke(MeasurementUnit, new EventArgs());
            }
            else
            {
                if (UnitsChanged != null)
                    UnitsChanged.Invoke(MeasurementUnit, null);
            }

            // Events
            DoValueChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxCostPerUnit_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                CostPerUnit = Convert.ToDouble(textBoxCostPerUnit.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
                DoValueChangedEvent();
            }
            catch
            {
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxParAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                ParQuantity = Convert.ToDouble(textBoxParAmount.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
                DoValueChangedEvent();
            }
            catch
            {
                ParQuantity = null;
                DoValueChangedEvent();
            }
        }

        private void DoValueChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }

    }
}
