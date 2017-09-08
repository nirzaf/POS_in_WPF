namespace PosModels.Types
{
    public enum MeasurementUnit : short
    {
        // Weight
        None = 0,
        Unit = 1,
        Pound = 2,
        Ounce = 3,
        Gram = 4,
        Milligram = 5,
        Kilogram = 6,
        // Volume
        Pinch = 102,        // 1/5 teaspoon
        Teaspoon = 103,
        Tablespoon = 104,
        Cup = 105,
        Gallon = 106,
        Pint = 107,
        Quart = 108,
        FluidOunce = 109,
        Liter = 110,
        Centiliter = 111,
        Milliliter = 112,
        Kiloliter = 113,
    }

    public static class MeasurementUnitExtensions
    {
        public static MeasurementUnit GetMeasurementUnit(this short measurementUnitId)
        {
            try
            {
                return (MeasurementUnit)measurementUnitId;
            }
            catch
            {
                return MeasurementUnit.None;
            }
        }

        public static bool IsVolume(this MeasurementUnit unit)
        {
            return (
                (unit == MeasurementUnit.Pinch) ||
                (unit == MeasurementUnit.Centiliter) ||
                (unit == MeasurementUnit.Cup) ||
                (unit == MeasurementUnit.FluidOunce) ||
                (unit == MeasurementUnit.Gallon) ||
                (unit == MeasurementUnit.Kiloliter) ||
                (unit == MeasurementUnit.Liter) ||
                (unit == MeasurementUnit.Milliliter) ||
                (unit == MeasurementUnit.Pint) ||
                (unit == MeasurementUnit.Quart) ||
                (unit == MeasurementUnit.Tablespoon) ||
                (unit == MeasurementUnit.Teaspoon));
        }

        public static bool IsWeight(this MeasurementUnit unit)
        {
            return (
                (unit == MeasurementUnit.Gram) ||
                (unit == MeasurementUnit.Kilogram) ||
                (unit == MeasurementUnit.Milligram) ||
                (unit == MeasurementUnit.Ounce) ||
                (unit == MeasurementUnit.Pound));
        }
    }
}
