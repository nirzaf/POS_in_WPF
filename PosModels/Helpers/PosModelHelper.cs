using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;
using System.Reflection;

namespace PosModels.Helpers
{
    public static class PosModelHelper
    {
        #region Licensed Access Only
        static PosModelHelper()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PosModelHelper).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        public static void AdjustInventoryByIngredient(int ingredientId, bool increase,
            double amount, MeasurementUnit amountUnit)
        {
            if (ingredientId > 0)
            {
                Ingredient ingredient = Ingredient.Get(ingredientId);
                if (ingredient.MeasurementUnit != amountUnit)
                    amount = UnitConversion.Convert(amount, amountUnit, ingredient.MeasurementUnit);
                if (increase)
                    ingredient.SetInventoryAmount(ingredient.InventoryAmount + amount);
                else
                    ingredient.SetInventoryAmount(ingredient.InventoryAmount - amount);
                ingredient.Update();
            }
        }

        public static void AdjustInventoryByItem(int itemId, double itemPortionSize,
            bool increase)
        {
            if (itemId > 0)
            {
                foreach (ItemIngredient itemIngredientIngredient in
                    ItemIngredient.GetAll(itemId))
                {
                    AdjustInventoryByIngredient(
                        itemIngredientIngredient.IngredientId,
                        increase,
                        itemIngredientIngredient.Amount * itemPortionSize,
                        itemIngredientIngredient.MeasurementUnit);
                }
            }
        }
    }
}
