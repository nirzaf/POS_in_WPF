using System;

namespace PosModels.Types
{
    /// <summary>
    /// 
    /// </summary>
    public enum Days : byte
    {
        Any = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7
    }

    public static class DayHelper
    {
        public static string ToLanguageSpecificString(this Days day)
        {
            switch (day)
            {
                case Days.Monday:
                    return Strings.DayMonday;
                case Days.Tuesday:
                    return Strings.DayTuesday;
                case Days.Wednesday:
                    return Strings.DayWednesday;
                case Days.Thursday:
                    return Strings.DayThursday;
                case Days.Friday:
                    return Strings.DayFriday;
                case Days.Saturday:
                    return Strings.DaySaturday;
                case Days.Sunday:
                    return Strings.DaySunday;
                case Days.Any:
                    return Strings.DayAny;
            }
            // Unreachable, but compiler doesn't know that
            return null;
        }

        /// <summary>
        /// Converts a Day to a DayOfWeek
        /// </summary>
        /// <param name="day">The day</param>
        /// <returns>Returns the DayOfWeek object. If the day is Days.Any, the return value is null.</returns>
        public static DayOfWeek? ToDayOfWeek(this Days day)
        {
            switch (day)
            {
                case Days.Sunday:
                    return DayOfWeek.Sunday;
                case Days.Monday:
                    return DayOfWeek.Monday;
                case Days.Tuesday:
                    return DayOfWeek.Tuesday;
                case Days.Wednesday:
                    return DayOfWeek.Wednesday;
                case Days.Thursday:
                    return DayOfWeek.Thursday;
                case Days.Friday:
                    return DayOfWeek.Friday;
                case Days.Saturday:
                    return DayOfWeek.Saturday;
            }
            return null;
        }

        public static Days ConvertToDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return Days.Sunday;
                case DayOfWeek.Monday:
                    return Days.Monday;
                case DayOfWeek.Tuesday:
                    return Days.Tuesday;
                case DayOfWeek.Wednesday:
                    return Days.Wednesday;
                case DayOfWeek.Thursday:
                    return Days.Thursday;
                case DayOfWeek.Friday:
                    return Days.Friday;
                case DayOfWeek.Saturday:
                    return Days.Saturday;
            }
            // Unreachable, but compiler doesn't know that
            return Days.Any;
        }
    }
}
