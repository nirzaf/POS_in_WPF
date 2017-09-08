namespace PosModels.Types
{
    public enum TemperatureScale
    {
            Fahrenheit = 1,
            Celsius = 2,
            Kelvin = 3
    }

    public static class TemperatureScaleExtensions
    {
        public static double ConvertTo(this TemperatureScale source, TemperatureScale target, double amount)
        {
            if ((source == TemperatureScale.Fahrenheit) &&
                (target == TemperatureScale.Celsius))
            {
                amount = (amount - 32)*5/9;
            }
            else if ((source == TemperatureScale.Fahrenheit) &&
                     (target == TemperatureScale.Kelvin))
            {
                amount = ((amount - 32)*5/9) + 273.15;
            }
            else if ((source == TemperatureScale.Celsius) &&
                     (target == TemperatureScale.Fahrenheit))
            {
                amount = (amount*9/5) + 32;
            }
            else if ((source == TemperatureScale.Celsius) &&
                     (target == TemperatureScale.Kelvin))
            {
                amount = amount + 273.15;
            }
            else if ((source == TemperatureScale.Kelvin) &&
                     (target == TemperatureScale.Fahrenheit))
            {
                amount = ((amount - 273.15)*9/5) + 32;
            }
            else if ((source == TemperatureScale.Kelvin) &&
                     (target == TemperatureScale.Celsius))
            {
                amount = amount - 273.15;
            }
            return amount;
        }

        public static string ToString(this TemperatureScale scale, double amount)
        {
            switch (scale)
            {
                case TemperatureScale.Fahrenheit:
                    return ((int)amount) + "°F";
                case TemperatureScale.Celsius:
                    return ((int)amount) + "°C";
                case TemperatureScale.Kelvin:
                    return ((int)amount) + "K";
            }
            // Complier doesn't know this won't happen
            return null;
        }
    }
}
