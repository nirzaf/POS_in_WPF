using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Types;
using PosModels.Types;
using PosControls;
using PosModels.Helpers;
using Strings = TemPOS.Types.Strings;


namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemIngredientAddControl.xaml
    /// </summary>
    public partial class IngredientAmountControl : UserControl
    {
        private MeasurementUnit _measurementUnit;

        public MeasurementUnit MeasurementUnit
        {
            get { return _measurementUnit; }
            set
            {
                InitializeUnitsComboBox(value);
                _measurementUnit = value;
                SelectComboBoxIndexOf(value.ToString());
            }
        }

        public double Amount
        {
            get
            {
                try
                {
                    return Convert.ToDouble(numberEntryControl.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                numberEntryControl.Text = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private IngredientAmountControl()
        {
            InitializeComponent();
        }

        // SelectedIngredient.MeasurementUnit
        private void InitializeUnitsComboBox(MeasurementUnit measurementUnit)
        {
            _measurementUnit = measurementUnit;
            comboBoxMeasuringUnits.Items.Clear();
            if (!measurementUnit.IsVolume() &&
                !measurementUnit.IsWeight())
            {
                comboBoxMeasuringUnits.Items.Add(Types.Strings.UnmeasuredUnits);
            }
            else if (measurementUnit.IsWeight())
            {
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightPound);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightOunce);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightGram);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightMilligram);
                comboBoxMeasuringUnits.Items.Add(Types.Strings.WeightKilogram);
            }
            else if (measurementUnit.IsVolume())
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
                if (text.Contains(containingText))
                {
                    comboBoxMeasuringUnits.SelectedIndex = i;
                    return;
                }
            }
            comboBoxMeasuringUnits.SelectedItem = null;
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxMeasuringUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            MeasurementUnit origUnit = _measurementUnit;
            if (_measurementUnit.IsWeight())
            {
                switch (comboBoxMeasuringUnits.SelectedIndex)
                {
                    case 0:
                        _measurementUnit = MeasurementUnit.Pound;
                        break;
                    case 1:
                        _measurementUnit = MeasurementUnit.Ounce;
                        break;
                    case 2:
                        _measurementUnit = MeasurementUnit.Gram;
                        break;
                    case 3:
                        _measurementUnit = MeasurementUnit.Milligram;
                        break;
                    case 4:
                        _measurementUnit = MeasurementUnit.Kilogram;
                        break;
                }
            }
            else if (_measurementUnit.IsVolume())
            {
                switch (comboBoxMeasuringUnits.SelectedIndex)
                {
                    case 0:
                        _measurementUnit = MeasurementUnit.Gallon;
                        break;
                    case 1:
                        _measurementUnit = MeasurementUnit.Quart;
                        break;
                    case 2:
                        _measurementUnit = MeasurementUnit.Pint;
                        break;
                    case 3:
                        _measurementUnit = MeasurementUnit.Cup;
                        break;
                    case 4:
                        _measurementUnit = MeasurementUnit.Tablespoon;
                        break;
                    case 5:
                        _measurementUnit = MeasurementUnit.Teaspoon;
                        break;
                    case 6:
                        _measurementUnit = MeasurementUnit.Liter;
                        break;
                    case 7:
                        _measurementUnit = MeasurementUnit.FluidOunce;
                        break;
                    case 8:
                        _measurementUnit = MeasurementUnit.Milliliter;
                        break;
                    case 9:
                        _measurementUnit = MeasurementUnit.Kiloliter;
                        break;
                }
            }
            Amount = UnitConversion.Convert(Amount, origUnit, _measurementUnit);
        }

        [Obfuscation(Exclude = true)]
        private void numberEntryControl_EnterPressed(object sender, EventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        public static PosDialogWindow CreateInDefaultWindow(string title)
        {
            IngredientAmountControl control = new IngredientAmountControl();
            return new PosDialogWindow(control, title, 330, 455);
        }
    }
}
