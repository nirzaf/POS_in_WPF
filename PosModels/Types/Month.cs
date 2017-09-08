namespace PosModels.Types
{
    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public static class MonthHelper
    {
        public static string ToLanguageSpecificString(this Month month)
        {
            switch (month)
            {
                case Month.January:
                    return Strings.MonthJanuary;
                case Month.February:
                    return Strings.MonthFebruary;
                case Month.March:
                    return Strings.MonthMarch;
                case Month.April:
                    return Strings.MonthApril;
                case Month.May:
                    return Strings.MonthMay;
                case Month.June:
                    return Strings.MonthJune;
                case Month.July:
                    return Strings.MonthJuly;
                case Month.August:
                    return Strings.MonthAugust;
                case Month.September:
                    return Strings.MonthSeptember;
                case Month.October:
                    return Strings.MonthOctober;
                case Month.November:
                    return Strings.MonthNovember;
                case Month.December:
                    return Strings.MonthDecember;
            }
            // Unreachable, but compiler doesn't know that
            return null;
        }
    }
}
