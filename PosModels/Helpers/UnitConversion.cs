using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;

namespace PosModels.Helpers
{
    public class UnitConversion
    {
        public static double Convert(double amount,
            MeasurementUnit sourceUnit, MeasurementUnit targetUnit)
        {
            if (sourceUnit == targetUnit)
                return amount;
            if (sourceUnit.IsVolume() && !targetUnit.IsVolume())
                throw new InvalidOperationException("Trying to convent from a volume to a weight");
            if (sourceUnit.IsWeight() && !targetUnit.IsWeight())
                throw new InvalidOperationException("Trying to convent from a weight to a volume");
            // Cups to X
            if (sourceUnit == MeasurementUnit.Cup)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 236.588237;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 23.6588236;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.236588236;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 240; 
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 48;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 16;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 8;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount / 2;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount / 4;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 16;
            }

            // Pints to X
            else if (sourceUnit == MeasurementUnit.Pint)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 473.176473;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 47.3176473;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.473176473;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 480;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 96;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 32;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 16;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 2;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount / 2;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 8;
            }

            // Quarts to X
            else if (sourceUnit == MeasurementUnit.Quart)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 946.352946;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 94.6352946;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.946362946;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 960;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 192;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 64;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 32;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 4;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount * 2;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 4;
            }

            // Gallons to X
            else if (sourceUnit == MeasurementUnit.Gallon)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 3785.41178;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 378.541178;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 3.78541178;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 3840;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 768;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 256;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 128;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 16;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount * 8;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount * 4;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount;
            }

            // Mililiters to X
            else if (sourceUnit == MeasurementUnit.Milliliter)
            {
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount / 10;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount / 1000;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 1.01442068;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 0.202884136;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 0.0676280454;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 0.0338140227;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 0.00422675284;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount * 0.00211337642;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount * 0.00105668821;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount * 0.000264172052;
            }

            // Centiliters to X
            else if (sourceUnit == MeasurementUnit.Centiliter)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 10;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount / 100;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 10.1442068;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 2.02884136;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 0.676280454;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 0.338140227;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 0.0422675284;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount * 0.0211337642;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount * 0.0105668821;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount * 0.00264172052;
            }

            // Liters to X
            else if (sourceUnit == MeasurementUnit.Liter)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 1000;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 100;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 1014.42068;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 202.884136;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount * 67.6280454;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount * 33.8140227;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount * 4.22675284;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount * 2.11337642;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount * 1.05668821;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount * 0.264172052;
            }
            
            // Pinch to X
            else if (sourceUnit == MeasurementUnit.Pinch)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 24.64460795;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 2.464460795;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.02464460795;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount / 5;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount / 15;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount / 30;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount / 240;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount / 480;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount / 960;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 3840;
            }

            // Teaspoon to X
            else if (sourceUnit == MeasurementUnit.Teaspoon)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 4.92892159;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 0.492892159;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.00492892159;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 15;
                if (targetUnit == MeasurementUnit.Tablespoon)
                    return amount / 3;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount / 6;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount / 48;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount / 96;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount / 192;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 768;
            }

            // Tablespoon to X
            else if (sourceUnit == MeasurementUnit.Tablespoon)
            {
                if (targetUnit == MeasurementUnit.Milliliter)
                    return amount * 14.7867648;
                if (targetUnit == MeasurementUnit.Centiliter)
                    return amount * 1.47867648;
                if (targetUnit == MeasurementUnit.Liter)
                    return amount * 0.147867648;
                if (targetUnit == MeasurementUnit.Pinch)
                    return amount * 15;
                if (targetUnit == MeasurementUnit.Teaspoon)
                    return amount * 3;
                if (targetUnit == MeasurementUnit.FluidOunce)
                    return amount / 2;
                if (targetUnit == MeasurementUnit.Cup)
                    return amount / 16;
                if (targetUnit == MeasurementUnit.Pint)
                    return amount / 32;
                if (targetUnit == MeasurementUnit.Quart)
                    return amount / 64;
                if (targetUnit == MeasurementUnit.Gallon)
                    return amount / 256;
            }

            else if (sourceUnit == MeasurementUnit.Ounce)
            {
                if (targetUnit == MeasurementUnit.Milligram)
                    return amount * 0.0283495231;
                if (targetUnit == MeasurementUnit.Gram)
                    return amount * 28.3495231;
                if (targetUnit == MeasurementUnit.Kilogram)
                    return amount * 28349.5231;
                if (targetUnit == MeasurementUnit.Pound)
                    return amount / 16;
            }

            else if (sourceUnit == MeasurementUnit.Pound)
            {
                if (targetUnit == MeasurementUnit.Milligram)
                    return amount * 453592.37;
                if (targetUnit == MeasurementUnit.Gram)
                    return amount * 453.59237;
                if (targetUnit == MeasurementUnit.Kilogram)
                    return amount * 0.45359237;
                if (targetUnit == MeasurementUnit.Ounce)
                    return amount * 16;
            }

            else if (sourceUnit == MeasurementUnit.Milligram)
            {
                if (targetUnit == MeasurementUnit.Gram)
                    return amount / 1000;
                if (targetUnit == MeasurementUnit.Kilogram)
                    return amount / 1000000;
                if (targetUnit == MeasurementUnit.Ounce)
                    return amount / 28349.5231;
                if (targetUnit == MeasurementUnit.Pound)
                    return amount / 453592.37;
            }

            else if (sourceUnit == MeasurementUnit.Gram)
            {
                if (targetUnit == MeasurementUnit.Milligram)
                    return amount * 1000;
                if (targetUnit == MeasurementUnit.Kilogram)
                    return amount / 1000;
                if (targetUnit == MeasurementUnit.Ounce)
                    return amount / 28.3495231;
                if (targetUnit == MeasurementUnit.Pound)
                    return amount / 453.59237;
            }

            else if (sourceUnit == MeasurementUnit.Kilogram)
            {
                if (targetUnit == MeasurementUnit.Milligram)
                    return amount * 1000000;
                if (targetUnit == MeasurementUnit.Gram)
                    return amount * 1000;
                if (targetUnit == MeasurementUnit.Ounce)
                    return amount / 0.0283495231;
                if (targetUnit == MeasurementUnit.Pound)
                    return amount / 0.45359237;
            }

            throw new NotImplementedException("Conversion not implemented: " + sourceUnit.ToString() + " to " + targetUnit.ToString());
        }

    }
}
