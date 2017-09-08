using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum ItemOptionAdjustmentType
    {
		MinimumRequiredOptions,    // (ItemOptionSet)
		NumberOfFreeOptions,       // (ItemOptionSet)
		MaximumAllowedOptions,     // (ItemOptionSet)
		CostForExtra,              // (both tables)
		UsesItem,                  // (ItemOption)
		UsesIngredient,            // (ItemOption)
		UsesItemNowUsesIngredient, // (ItemOption)
		UsesIngredientNowUsesItem  // (ItemOption)
    }
}
