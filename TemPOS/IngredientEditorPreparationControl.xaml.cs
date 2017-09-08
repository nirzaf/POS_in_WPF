using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TemPOS.Types;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Helpers;
using PosModels.Types;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for IngredientEditorPreparationControl.xaml
    /// </summary>    
    public partial class IngredientEditorPreparationControl : UserControl
    {
        private readonly List<IngredientSet> _ingredientSetsAdded =
            new List<IngredientSet>();
        private readonly List<IngredientSet> _ingredientSetsNeedingUpdate =
            new List<IngredientSet>();
        private readonly List<IngredientSet> _ingredientSetsRemoved =
            new List<IngredientSet>();
        private double? _originalExtendedIngredientYield;
        private double? _extendedIngredientYield;
        private Ingredient _activeIngredient;

        [Obfuscation(Exclude = true)]
        public event EventHandler YieldAmountChanged;
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        [Obfuscation(Exclude = true)]
        public event EventHandler PrepDisable;
        [Obfuscation(Exclude = true)]
        public event EventHandler PrepEnable;

        public Ingredient ActiveIngredient
        {
            get { return _activeIngredient; }
            set
            {
                _activeIngredient = value;
                if (_activeIngredient != null)
                {
                    _originalExtendedIngredientYield =
                        ExtendedIngredientYield = 
                        _activeIngredient.ExtendedIngredientYield;
                    MeasurementUnit = _activeIngredient.MeasurementUnit;
                    if (ExtendedIngredientYield.HasValue)
                    {
                        buttonYield.Text = ExtendedIngredientYield.Value.ToString(CultureInfo.InvariantCulture);
                        labelUnits.Content = value.MeasurementUnit +
                                (Math.Abs(ExtendedIngredientYield.Value - 1) > double.Epsilon ? Types.Strings.S : "");
                    }
                    else
                    {
                        buttonYield.Text = "0";
                        labelUnits.Content = value.MeasurementUnit.ToString() + Types.Strings.S;
                    }
                }
                else
                {
                    _originalExtendedIngredientYield =
                        ExtendedIngredientYield = null;
                    MeasurementUnit = MeasurementUnit.Unit;
                    buttonYield.Text = "0";
                    labelUnits.Content = Types.Strings.IngredientEditorUnits;
                }
                checkBoxEnabled.SelectionChanged -= checkBoxEnabled_SelectionChanged;
                checkBoxEnabled.IsSelected = false;
                checkBoxEnabled.SelectionChanged += checkBoxEnabled_SelectionChanged;

                InitializeFields();
            }
        }

        public MeasurementUnit MeasurementUnit
        {
            get;
            private set;
        }

        public double? ExtendedIngredientYield
        {
            get { return _extendedIngredientYield; }
            set
            {
                _originalExtendedIngredientYield =
                    _extendedIngredientYield = value;
            }
        }

        public IngredientEditorPreparationControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            int oldCount = listBoxItemIngredients.Items.Count;
            listBoxAvailableIngredients.Items.Clear();
            listBoxItemIngredients.Items.Clear();

            List<Ingredient> ingredients = new List<Ingredient>(Ingredient.GetAll());

            if (ActiveIngredient != null)
            {
                foreach (IngredientSet setIngredient in IngredientSet.GetAll(ActiveIngredient.Id))
                {
                    bool added, changed, removed;
                    IngredientSet current = GetIngredientSet(setIngredient.Id,
                        out added, out changed, out removed);
                    if (!removed)
                    {
                        Ingredient ingredient = Ingredient.Find(ingredients, setIngredient.IngredientId);
                        AddIngredientSetToListBox(
                            (changed ? current : setIngredient), ingredient);
                        ingredients.Remove(ingredient);
                    }
                }
            }

            // Note: Added ones have an ExtendedIngredientId of zero so GetAll (above) will not find them
            foreach (IngredientSet setIngredient in _ingredientSetsAdded)
            {
                Ingredient ingredient = Ingredient.Find(ingredients, setIngredient.IngredientId);
                AddIngredientSetToListBox(setIngredient, ingredient);
                ingredients.Remove(ingredient);
            }

            if (ActiveIngredient != null)
            {
                foreach (Ingredient ingredient in ingredients)
                {
                    if ((ingredient.Id == ActiveIngredient.Id) ||
                        ActiveIngredient.ContainsIngredient(ingredient.Id))
                        continue;
                    listBoxAvailableIngredients.Items.Add(
                        new FormattedListBoxItem(ingredient.Id,
                        ingredient.FullName, true));
                }
            }
            SetReadOnly();
            SetupButtons();
            ProcessEvents(oldCount);
        }

        private void AddIngredientSetToListBox(IngredientSet setIngredient, Ingredient ingredient)
        {
            string tagLine = ingredient.FullName + Environment.NewLine +
                Types.Strings.IngredientEditorAmount + FormatDoubleToString(setIngredient.Amount) + " " +
                setIngredient.MeasurementUnit +
                (Math.Abs(setIngredient.Amount - 1) < double.Epsilon ? "" : Types.Strings.S);
            listBoxItemIngredients.Items.Add(
                new FormattedListBoxItem(setIngredient.Id, tagLine, true));
        }

        private void ProcessEvents(int oldCount)
        {
            if ((oldCount == 0) && (listBoxItemIngredients.Items.Count > 0))
                DoPrepEnable();
            else if ((oldCount > 0) && (listBoxItemIngredients.Items.Count == 0))
                DoPrepDisable();
        }

        private void DoPrepEnable()
        {
            if (PrepEnable != null)
                PrepEnable.Invoke(this, new EventArgs());
        }

        private void DoPrepDisable()
        {
            if (PrepDisable != null)
                PrepDisable.Invoke(this, new EventArgs());
        }

        private void SetReadOnly()
        {
            bool isEnabled = 
                ((listBoxItemIngredients.Items.Count > 0) || checkBoxEnabled.IsSelected);

            listBoxAvailableIngredients.IsEnabled =
                listBoxItemIngredients.IsEnabled =
                buttonAdd.IsEnabled =
                buttonAmount.IsEnabled =
                buttonRemove.IsEnabled =
                buttonYield.IsEnabled =
                label1.IsEnabled =
                label2.IsEnabled =
                label3.IsEnabled =
                labelUnits.IsEnabled =
                labelReadOnly.IsEnabled = isEnabled;            

            if (!checkBoxEnabled.IsSelected && isEnabled)
                checkBoxEnabled.IsSelected = true;
        }

        private void SetupButtons()
        {
            if (DayOfOperation.Today != null)
            {
                checkBoxEnabled.IsEnabled = false;
                buttonYield.IsEnabled = false;
                buttonAdd.Visibility =
                    buttonAmount.Visibility =
                    buttonRemove.Visibility =
                    Visibility.Collapsed;
                labelReadOnly.Visibility = Visibility.Visible;
                return;
            }

            if (!checkBoxEnabled.IsSelected)
                return;
            buttonAdd.IsEnabled = (listBoxAvailableIngredients.SelectedItem != null);
            buttonAmount.IsEnabled =
                buttonRemove.IsEnabled = (listBoxItemIngredients.SelectedItem != null);
        }

        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxItemIngredients.SelectedItem = null;
            SetupButtons();
        }

        [Obfuscation(Exclude = true)]
        private void listBoxItemIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxAvailableIngredients.SelectedItem = null;
            SetupButtons();
        }

        private string FormatDoubleToString(double doubleValue)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalDigits = 3
            };
            return doubleValue.ToString("F3", nfi);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxEnabled_SelectionChanged(object sender, EventArgs e)
        {
            if (!checkBoxEnabled.IsSelected)
            {
                if (listBoxItemIngredients.Items.Count > 0)
                {
                    DialogButton result = PosDialogWindow.ShowDialog(
                        Types.Strings.IngredientEditorWarningUncheck,
                        Types.Strings.Confirmation, DialogButtons.OkCancel);
                    if (result == DialogButton.Cancel)
                    {
                        checkBoxEnabled.IsSelected = true;
                    }
                    else
                    {
                        foreach (FormattedListBoxItem item in listBoxItemIngredients.Items)
                        {
                            RemoveIngredientSet(item.Id);
                        }
                        listBoxAvailableIngredients.SelectedItem = null;
                        listBoxItemIngredients.SelectedItem = null;
                        InitializeFields();
                        DoValueChangedEvent();
                    }
                }
            }
            else
            {
                listBoxAvailableIngredients.SelectedItem = null;
                listBoxItemIngredients.SelectedItem = null;
                InitializeFields();
            }
        }

        private IngredientSet GetIngredientSet(int id, out bool cachedAdd,
            out bool cachedChange, out bool cachedRemove)
        {
            cachedAdd = false;
            cachedChange = false;
            cachedRemove = false;
            foreach (IngredientSet ingredientSet in _ingredientSetsAdded)
                if (ingredientSet.Id == id)
                {
                    cachedAdd = true;
                    return ingredientSet;
                }
            foreach (IngredientSet ingredientSet in _ingredientSetsNeedingUpdate)
                if (ingredientSet.Id == id)
                {
                    cachedChange = true;
                    return ingredientSet;
                }
            foreach (IngredientSet ingredientSet in _ingredientSetsRemoved)
                if (ingredientSet.Id == id)
                {
                    cachedRemove = true;
                    return ingredientSet;
                }
            return IngredientSet.Get(id);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedIngredient = (FormattedListBoxItem)
                listBoxAvailableIngredients.SelectedItem;
            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(Types.Strings.IngredientEditorAddIngredient);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;
            Ingredient ingredient = Ingredient.Get(selectedIngredient.Id);
            
            control.Amount = 0;
            control.MeasurementUnit = ingredient.MeasurementUnit;

            window.ShowDialog(parent);
            if (window.ClosedByUser || (!(control.Amount > 0))) return;
            
            _ingredientSetsAdded.Add(
                IngredientSet.Add(0, selectedIngredient.Id,
                                  control.Amount, control.MeasurementUnit));
            listBoxAvailableIngredients.SelectedItem = null;
            listBoxItemIngredients.SelectedItem = null;
            InitializeFields();
            DoValueChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonAmount_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedIngredient =
                listBoxItemIngredients.SelectedItem as FormattedListBoxItem;
            if (selectedIngredient == null) return;
            bool added, changed, removed;
            IngredientSet ingredientSet = GetIngredientSet(selectedIngredient.Id,                
                out added, out changed, out removed);
            //Ingredient ingredient = Ingredient.Get(ingredientSet.IngredientId);

            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(Types.Strings.IngredientEditorEditAmount);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.Amount = ingredientSet.Amount;
            control.MeasurementUnit = ingredientSet.MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                if (control.Amount > 0)
                {
                    ingredientSet.SetAmount(control.Amount);
                    ingredientSet.SetMeasurementUnit(control.MeasurementUnit);
                    if (!added && !changed)
                        _ingredientSetsNeedingUpdate.Add(ingredientSet);
                }
                else
                {
                    RemoveIngredientSet(ingredientSet.Id);
                    ResetYieldIfEmpty();
                }
                listBoxAvailableIngredients.SelectedItem = null;
                listBoxItemIngredients.SelectedItem = null;
                InitializeFields();
                DoValueChangedEvent();
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem =
                listBoxItemIngredients.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null) return;

            DialogButton result = PosDialogWindow.ShowDialog(
                Types.Strings.IngredientEditorConfirmRemove,
                Types.Strings.Confirmation, DialogButtons.YesNo);
            if (result != DialogButton.Yes) return;
            
            RemoveIngredientSet(selectedItem.Id);
            listBoxItemIngredients.SelectedItem = null;
            ResetYieldIfEmpty();
            InitializeFields();
            DoValueChangedEvent();
        }

        private void RemoveIngredientSet(int id)
        {
            bool added, changed, removed;
            IngredientSet ingredientSet = GetIngredientSet(id,
                out added, out changed, out removed);
            if (added)
                _ingredientSetsAdded.Remove(ingredientSet);
            if (changed)
                _ingredientSetsNeedingUpdate.Remove(ingredientSet);
            if (!added && !removed)
                _ingredientSetsRemoved.Add(ingredientSet);
        }

        private void ResetYieldIfEmpty()
        {
            if (listBoxItemIngredients.Items.Count > 1)
                return;
            checkBoxEnabled.SelectionChanged -= checkBoxEnabled_SelectionChanged;
            checkBoxEnabled.IsSelected = false;
            checkBoxEnabled.SelectionChanged += checkBoxEnabled_SelectionChanged;
            buttonYield.Text = "0";
            labelUnits.Content = MeasurementUnit.ToString() + Types.Strings.S;
            _extendedIngredientYield = null;
        }

        [Obfuscation(Exclude = true)]
        private void buttonYield_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(Types.Strings.IngredientEditorEditRecipeYield);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.MeasurementUnit = MeasurementUnit;
            control.Amount = (ExtendedIngredientYield.HasValue ?
                ExtendedIngredientYield.Value : 0);

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                double amount = 0;
                if (control.Amount > 0)
                {
                    amount = UnitConversion.Convert(control.Amount,
                        control.MeasurementUnit, MeasurementUnit);
                    _extendedIngredientYield = amount;
                }
                else
                {
                    _extendedIngredientYield = null;
                }
                buttonYield.Text = amount.ToString(CultureInfo.InvariantCulture);
                labelUnits.Content = MeasurementUnit.ToString() +
                    (Math.Abs(amount - 1) > double.Epsilon ? Types.Strings.S : "");
                if (YieldAmountChanged != null)
                    YieldAmountChanged.Invoke(this, new EventArgs());
                DoValueChangedEvent();
            }
            e.Handled = true;
        }

        public void Update(int activeIngredientId)
        {
            Ingredient ingredient = Ingredient.Get(activeIngredientId);

            // Yield
            if (ingredient.ExtendedIngredientYield != ExtendedIngredientYield)
            {
                IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id, ingredient.Id,
                    ingredient.ExtendedIngredientYield, ExtendedIngredientYield,
                    MeasurementUnit.None, -1);
                ingredient.UpdateExtendedIngredientYield(ExtendedIngredientYield);
            }

            // Added Ingredients
            foreach (IngredientSet ingredientSet in _ingredientSetsAdded)
            {
                ingredientSet.SetExtendedIngredientId(activeIngredientId);
                ingredientSet.Update();
                IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    ingredientSet.IngredientId, null, ingredientSet.Amount, ingredientSet.MeasurementUnit,
                    activeIngredientId);
            }

            // Changed Ingredients
            foreach (IngredientSet ingredientSet in _ingredientSetsNeedingUpdate)
            {
                IngredientSet original = IngredientSet.Get(ingredientSet.Id);
                double oldAmount = UnitConversion.Convert(original.Amount, original.MeasurementUnit,
                    ingredientSet.MeasurementUnit);
                ingredientSet.Update();
                IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    ingredientSet.IngredientId, oldAmount, ingredientSet.Amount, ingredientSet.MeasurementUnit,
                    activeIngredientId);
            }

            // Removed Ingredients
            foreach (IngredientSet ingredientSet in _ingredientSetsRemoved)
            {
                IngredientSet original = IngredientSet.Get(ingredientSet.Id);
                IngredientSet.Delete(ingredientSet.Id);
                IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                    ingredientSet.IngredientId, original.Amount, null, original.MeasurementUnit,
                    activeIngredientId);
            }

            _ingredientSetsAdded.Clear();
            _ingredientSetsNeedingUpdate.Clear();
            _ingredientSetsRemoved.Clear();
        }

        public void Cancel()
        {
            foreach (IngredientSet ingredientSet in _ingredientSetsAdded)
            {
                IngredientSet.Delete(ingredientSet.Id);
            }
            _extendedIngredientYield = _originalExtendedIngredientYield;
            _ingredientSetsAdded.Clear();
            _ingredientSetsNeedingUpdate.Clear();
            _ingredientSetsRemoved.Clear();

            // Reset UI
            buttonYield.Text = (_extendedIngredientYield.HasValue ?
                _extendedIngredientYield.Value.ToString(CultureInfo.InvariantCulture) : "0");
            InitializeFields();
        }

        public void SetMeasurementUnit(MeasurementUnit unit, bool update)
        {
            if (update && (MeasurementUnit != unit) && ExtendedIngredientYield.HasValue &&
                (ExtendedIngredientYield.Value > 0) &&
                (unit != MeasurementUnit.Unit) && (unit != MeasurementUnit.None) &&
                ((unit.IsVolume() && MeasurementUnit.IsVolume()) || (unit.IsWeight() && MeasurementUnit.IsWeight())))
            {
                ExtendedIngredientYield = UnitConversion.Convert(ExtendedIngredientYield.Value,
                    MeasurementUnit, unit);
                buttonYield.Text = (ExtendedIngredientYield.Value.ToString(CultureInfo.InvariantCulture));
            }
            MeasurementUnit = unit;
            labelUnits.Content = MeasurementUnit.ToString() + Types.Strings.S;
        }

        private void DoValueChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }

    }
}
