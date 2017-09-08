using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosControls.Helpers;
using PosModels.Helpers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for InventoryEditorControl.xaml
    /// </summary>
    public partial class InventoryEditorControl : UserControl
    {
        public static Ingredient ActiveIngredient
        {
            get;
            private set;
        }

        public InventoryEditorControl()
        {
            InitializeComponent();
            InitializeListbox();
        }

        private void InitializeListbox()
        {
            foreach (Ingredient ingredient in Ingredient.GetAll())
            {
                listBox1.Items.Add(
                    new FormattedListBoxItem(ingredient, GetText(ingredient), true));
            }
        }

        private string GetText(Ingredient ingredient)
        {
            return ingredient.FullName +
                Environment.NewLine + ingredient.InventoryAmount + " " +
                ingredient.MeasurementUnit.ToString() +
                (Math.Abs(ingredient.InventoryAmount - 1) < double.Epsilon ? "" : Strings.S);
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isEnabled = ((e.AddedItems != null) && (e.AddedItems.Count > 0));
            buttonDecreaseBy.IsEnabled =
                buttonIncreaseBy.IsEnabled =
                buttonSet.IsEnabled = isEnabled;
            if (listBox1.SelectedItem == null)
                ActiveIngredient = null;
            else
            {
                FormattedListBoxItem formattedListBoxItem = listBox1.SelectedItem as FormattedListBoxItem;
                if (formattedListBoxItem != null)
                    ActiveIngredient = formattedListBoxItem.ReferenceObject as Ingredient;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonIncreaseBy_Click(object sender, RoutedEventArgs e)
        {
            if (!IngredientSet.HasEntries(ActiveIngredient.Id))
                AdjustInventory(Strings.InventoryIncreaseByAmount, 1);
            else
                AdjustInventoryByRecipe(Strings.InventoryIncreaseByRecipe, 1);
            UpdateListBoxItem();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDecreaseBy_Click(object sender, RoutedEventArgs e)
        {
            if (!IngredientSet.HasEntries(ActiveIngredient.Id))
                AdjustInventory(Strings.InventoryDecreaseByAmount, -1);
            else
                AdjustInventoryByRecipe(Strings.InventoryDecreaseByRecipe, -1);
            UpdateListBoxItem();
        }

        [Obfuscation(Exclude = true)]
        private void buttonSet_Click(object sender, RoutedEventArgs e)
        {
            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(Strings.InventorySetAmount);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.Amount = 0;
            control.MeasurementUnit = ActiveIngredient.MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                if (control.Amount > 0)
                {
                    double amount = UnitConversion.Convert(control.Amount, control.MeasurementUnit,
                        ActiveIngredient.MeasurementUnit);

                    IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id, ActiveIngredient.Id,
                         ActiveIngredient.InventoryAmount, amount, ActiveIngredient.MeasurementUnit);

                    ActiveIngredient.SetInventoryAmount(amount);
                    ActiveIngredient.Update();
                }
                UpdateListBoxItem();
            }
        }

        private void UpdateListBoxItem()
        {
            if (listBox1.SelectedItem == null)
                return;
            FormattedListBoxItem selectedItem = listBox1.SelectedItem as FormattedListBoxItem;
            if (selectedItem != null) 
                selectedItem.Set(selectedItem.ReferenceObject, GetText(ActiveIngredient));
        }

        private void AdjustInventoryByRecipe(string windowTitle, int factor)
        {
            if (ActiveIngredient.ExtendedIngredientYield == null)
            {
                PosDialogWindow.ShowDialog(Strings.InventoryError, Strings.Error);
                return;
            }
            double? amount = PosDialogWindow.PromptNumber(windowTitle, (double)0);
            if ((amount != null) && (amount.Value > 0))
            {
                double amountDelta = (ActiveIngredient.ExtendedIngredientYield.Value * amount.Value * factor);

                IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id, ActiveIngredient.Id,
                    ActiveIngredient.InventoryAmount,
                    ActiveIngredient.InventoryAmount + amountDelta,
                    ActiveIngredient.MeasurementUnit);

                ActiveIngredient.SetInventoryAmount(ActiveIngredient.InventoryAmount + amountDelta);
                ActiveIngredient.Update();

                AddIngredientPreparation(ActiveIngredient.Id, amountDelta);
            }
        }

        private void AdjustInventory(string windowTitle, int factor)
        {
            PosDialogWindow window = IngredientAmountControl.CreateInDefaultWindow(windowTitle);
            IngredientAmountControl control = window.DockedControl as IngredientAmountControl;
            PosDialogWindow parent = Window.GetWindow(this) as PosDialogWindow;

            control.Amount = 0;
            control.MeasurementUnit = ActiveIngredient.MeasurementUnit;

            window.ShowDialog(parent);
            if (!window.ClosedByUser)
            {
                if (control.Amount > 0)
                {
                    double amount = UnitConversion.Convert(control.Amount, control.MeasurementUnit,
                        ActiveIngredient.MeasurementUnit) * factor;

                    IngredientAdjustment.Add(SessionManager.ActiveEmployee.Id, ActiveIngredient.Id,
                        ActiveIngredient.InventoryAmount,
                        ActiveIngredient.InventoryAmount + amount,
                        ActiveIngredient.MeasurementUnit);

                    ActiveIngredient.SetInventoryAmount(ActiveIngredient.InventoryAmount + amount);
                    ActiveIngredient.Update();
                }
            }
        }

        private static void AddIngredientPreparation(int ingredientId, double amount)
        {
            foreach (IngredientSet ingredientSet in IngredientSet.GetAll(ingredientId))
            {
                Ingredient ingredient = Ingredient.Get(ingredientSet.IngredientId);
                ingredient.SetInventoryAmount(ingredient.InventoryAmount - amount);
                ingredient.Update();
            }
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            InventoryEditorControl control = new InventoryEditorControl();
            return new PosDialogWindow(control, Strings.InventoryEdit, 330, 550);
        }
    }
}
