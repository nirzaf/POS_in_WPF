using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemOptionEditorControl.xaml
    /// </summary>
    public partial class ItemOptionSetEditorControl : UserControl
    {
        #region Fields
        private bool _haltEvents;
        private ItemOptionSet _activeItemOptionSet;
        #endregion

        #region Properties
        public ItemOptionSet ActiveItemOptionSet
        {
            get
            {
                return _activeItemOptionSet;
            }
            set
            {
                _haltEvents = true;
                _activeItemOptionSet = value;
                if (value != null)
                {
                    textBoxName.Text = value.Name;
                    textBoxMin.Text = "" + value.SelectedMinimum;
                    textBoxMax.Text = "" + value.SelectedMaximum;
                    textBoxFree.Text = "" + value.SelectedFree;
                    textBoxExtraCost.Text = value.ExcessUnitCost.ToString("C2");
                    radioButtonIsPizzaStyle.IsSelected = value.IsPizzaStyle;
                    radioButtonIsNotPizzaStyle.IsSelected = !value.IsPizzaStyle;
                }
                else
                {
                    textBoxName.Text = "";
                    textBoxMin.Text = "0";
                    textBoxMax.Text = "0";
                    textBoxFree.Text = "0";
                    textBoxExtraCost.Text = Convert.ToDouble("0").ToString("C2");
                    radioButtonIsPizzaStyle.IsSelected = false;
                    radioButtonIsNotPizzaStyle.IsSelected = true;
                }
                _haltEvents = false;
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if ((ValueChanged != null) && (ActiveItemOptionSet != null) && !_haltEvents)
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public ItemOptionSetEditorControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxMin_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxFree_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxMax_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxExtraCost_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonPizzaStyle_SelectionChanged(object sender, EventArgs e)
        {
            DoChangedValueEvent();
        }

        public string GetNextValidationError()
        {
            return String.IsNullOrEmpty(textBoxName.Text) ?
                Strings.ItemSetupErrorSetNeedsName : null;
        }

        public void Update()
        {
            if (ActiveItemOptionSet != null)
            {
                ActiveItemOptionSet.SetName(textBoxName.Text);
                ActiveItemOptionSet.SetSelectedMinimum(GetSelectedMinimum());
                ActiveItemOptionSet.SetSelectedFree(GetSelectedFree());
                ActiveItemOptionSet.SetSelectedMaximum(GetSelectedMaximum());
                ActiveItemOptionSet.SetExcessUnitCost(GetExcessUnitCost());
                ActiveItemOptionSet.SetIsPizzaStyle(radioButtonIsPizzaStyle.IsSelected);
                ActiveItemOptionSet.Update();
            }
            else
            {
                ActiveItemOptionSet = ItemOptionSet.Add(textBoxName.Text, GetSelectedMinimum(),
                    GetSelectedFree(), GetSelectedMaximum(), GetExcessUnitCost(),
                    radioButtonIsPizzaStyle.IsSelected);
            }
        }

        private int GetSelectedMinimum()
        {
            try
            {
                return Convert.ToInt32(textBoxMin.Text);
            }
            catch
            {
                return 0;
            }
        }

        private int GetSelectedFree()
        {
            try
            {
                return Convert.ToInt32(textBoxFree.Text);
            }
            catch
            {
                return 0;
            }
        }

        private int GetSelectedMaximum()
        {
            try
            {
                return Convert.ToInt32(textBoxMax.Text);
            }
            catch
            {
                return 0;
            }
        }

        private double GetExcessUnitCost()
        {
            try
            {
                return Convert.ToDouble(textBoxExtraCost.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch
            {
                return 0;
            }
        }
    }
}
