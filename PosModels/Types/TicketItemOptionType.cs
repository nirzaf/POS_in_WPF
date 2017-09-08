using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum TicketItemOptionType : byte
    {
        // For non-pizzas
        None = 0,

        // Add to recipe's ingredients
        Add = 1,
        // Add an extra portion to the recipe's ingredients
        AddExtra = 2,
        // A half portion of the recipe ingredient
        EzOn = 3,
        // Remove from recipe ingredients
        Remove = 4,

        // Add to recipe's ingredients (Applies to Side A only)
        AddSideA = 5,
        // Add an extra portion to the recipe's ingredients (Applies to Side A only)
        AddExtraSideA = 6,
        // A half portion of the recipe ingredient (Applies to Side A only)
        EzOnSideA = 7,
        // Remove from recipe ingredients (Applies to Side A only)
        RemoveSideA = 8,

        // Add to recipe's ingredients (Applies to Side B only)
        AddSideB = 9,
        // Add an extra portion to the recipe's ingredients (Applies to Side B only)
        AddExtraSideB = 10,
        // A half portion of the recipe ingredient (Applies to Side B only)
        EzOnSideB = 11,
        // Remove from recipe ingredients (Applies to Side B only)
        RemoveSideB = 12,
    }

    public static class TicketItemOptionTypeExtensions
    {
        public static TicketItemOptionType GetTicketItemOptionType(this byte ticketItemOptionType)
        {
            try
            {
                return (TicketItemOptionType)ticketItemOptionType;
            }
            catch
            {
                return TicketItemOptionType.None;
            }
        }
    }
}
