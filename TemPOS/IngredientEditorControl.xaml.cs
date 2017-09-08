using System;
using System.Reflection;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using PosModels.Helpers;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for IngredientEditorControl.xaml
    /// </summary>
    public partial class IngredientEditorControl : UserControl
    {
        private bool _haltEvents;
        private Ingredient _activeIngredient;

        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        
        [Obfuscation(Exclude = true)]
        public event EventHandler ModifiedIngredient;

        public Ingredient ActiveIngredient
        {
            get { return _activeIngredient; }
            set
            {
                _activeIngredient = value;
                _haltEvents = true;
                EditorPreparation.ActiveIngredient = value;
                EditorDetails.ActiveIngredient = value;
                _haltEvents = false;
            }
        }

        public IngredientEditorDetailsControl EditorDetails
        {
            get
            {
                return tabControl.Tab1.DockedControl as IngredientEditorDetailsControl;
            }
        }

        public IngredientEditorPreparationControl EditorPreparation
        {
            get
            {
                return tabControl.Tab2.DockedControl as IngredientEditorPreparationControl;
            }
        }

        public IngredientEditorControl()
        {
            InitializeComponent();
            EditorDetails.ValueChanged += editor_ValueChanged;
            EditorPreparation.ValueChanged += editor_ValueChanged;
            EditorPreparation.PrepDisable += editorPreparation_PrepDisable;
            EditorPreparation.PrepEnable += editorPreparation_PrepEnable;
        }

        [Obfuscation(Exclude = true)]
        void editorPreparation_PrepEnable(object sender, EventArgs e)
        {
            EditorDetails.ShowCostPerUnit(false);
        }

        [Obfuscation(Exclude = true)]
        void editorPreparation_PrepDisable(object sender, EventArgs e)
        {
            EditorDetails.ShowCostPerUnit(true);
        }

        [Obfuscation(Exclude = true)]
        void editor_ValueChanged(object sender, EventArgs e)
        {
            if (!_haltEvents)
                DoValueChangedEvent();
        }

        private void DoValueChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged.Invoke(this, new EventArgs());
        }


        internal bool UpdateItem()
        {
            string fullName = EditorDetails.FullName;
            string shortName = EditorDetails.ShortName;
            double inventoryAmount = EditorDetails.InventoryAmount;
            MeasurementUnit unit = EditorDetails.MeasurementUnit;
            double costPerUnit = EditorDetails.CostPerUnit;
            double? parQuantity = EditorDetails.ParQuantity;

            // Is there an ActiveItem?
            if (ActiveIngredient == null)
            {
                ActiveIngredient = Ingredient.Add(fullName, shortName,
                    inventoryAmount, unit, costPerUnit, parQuantity);
                EditorPreparation.Update(ActiveIngredient.Id);
                ActiveIngredient = Ingredient.Get(ActiveIngredient.Id);
            }
            else
            {
                if (Math.Abs(ActiveIngredient.InventoryAmount - inventoryAmount) > double.Epsilon)
                {
                    double originalAmount = UnitConversion.Convert(ActiveIngredient.InventoryAmount,
                        ActiveIngredient .MeasurementUnit, unit);
                    IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                        ActiveIngredient.Id, originalAmount, inventoryAmount, unit);
                }
                if (ActiveIngredient.MeasurementUnit != unit)
                {
                    IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id,
                        ActiveIngredient.Id, -1, -1, ActiveIngredient.MeasurementUnit, (int)unit);
                }

                if (EditorDetails.IsAdjustedByRecipe)
                    ProcessInventoryChangesForPrepIngredients(
                        ActiveIngredient.InventoryAmount,
                        inventoryAmount);
                // Update the category values for the ActiveItem
                ActiveIngredient.SetFullName(fullName);
                ActiveIngredient.SetShortName(shortName);                
                ActiveIngredient.SetInventoryAmount(inventoryAmount);
                ActiveIngredient.SetMeasurementUnit(unit);
                ActiveIngredient.SetCostPerUnit(costPerUnit);
                ActiveIngredient.SetParQuantity(parQuantity);

                // Update the database
                ActiveIngredient.Update();

                EditorPreparation.Update(ActiveIngredient.Id);
                ActiveIngredient = Ingredient.Get(ActiveIngredient.Id);
            }

            
            return true;
        }

        private void ProcessInventoryChangesForPrepIngredients(double originalAmount, double newAmount)
        {
            if ((Math.Abs(originalAmount - newAmount) < double.Epsilon) || !ActiveIngredient.ExtendedIngredientYield.HasValue)
                return;
            foreach (IngredientSet ingredientSet in IngredientSet.GetAll(ActiveIngredient.Id))
            {
                Ingredient prepIngredient = Ingredient.Get(ingredientSet.IngredientId);
                double difference = newAmount - originalAmount;
                double amount = UnitConversion.Convert(ingredientSet.Amount, ingredientSet.MeasurementUnit,
                    prepIngredient.MeasurementUnit) * difference / ActiveIngredient.ExtendedIngredientYield.Value;
                prepIngredient.SetInventoryAmount(prepIngredient.InventoryAmount - amount);
                prepIngredient.Update();
                if (ModifiedIngredient != null)
                    ModifiedIngredient.Invoke(prepIngredient, new EventArgs());
            }
        }

        [Obfuscation(Exclude = true)]
        private void IngredientEditorDetailsControl_UnitsChanged(object sender, EventArgs e)
        {
            MeasurementUnit newValue = (MeasurementUnit)sender;
            EditorPreparation.SetMeasurementUnit(newValue, (e != null));
        }

        [Obfuscation(Exclude = true)]
        private void IngredientEditorPreparationControl_YieldAmountChanged(object sender, EventArgs e)
        {
            IngredientEditorPreparationControl control = (IngredientEditorPreparationControl)sender;
            EditorDetails.SetExtendedIngredientYieldAmount(control.ExtendedIngredientYield);
        }
    }
}
