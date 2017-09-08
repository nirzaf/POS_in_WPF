using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PosModels.Types
{
    [Obfuscation(Exclude = true)]
    public class Strings : StringsCore
    {
        #region Fields
        public static readonly Dictionary<string, string> English = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> German = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> French = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> Spanish = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> Italian = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> Dutch = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> Portuguese = new Dictionary<string, string>();
        #endregion

        #region Code
        #region Licensed Access Only / Static Initializer
        static Strings()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Category).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
            InitializeAll();
        }
        #endregion

        private static string GetString(string name)
        {
            switch (Language)
            {
#if DEBUG
                case Languages.Debug:
                    return "(" + name + ")";
#endif
                case Languages.English:
                    return English[name];
                case Languages.Spanish:
                    return Spanish[name];
                case Languages.French:
                    return French[name];
                case Languages.Italian:
                    return Italian[name];
                case Languages.German:
                    return German[name];
                case Languages.Dutch:
                    return Dutch[name];
                case Languages.Portuguese:
                    return Portuguese[name];
                default:
                    return null;
            }
        }
        #endregion

        #region Initialize
        private static void InitializeAll()
        {
            InitializeEnglish();
            InitializeSpanish();
            InitializeItalian();
            InitializeGerman();
            InitializeFrench();
            InitializeDutch();
            InitializePortuguese();
        }

        private static void InitializePortuguese()
        {
            Portuguese.Add("DayAny", "Qualquer");
            Portuguese.Add("DayMonday", "Segunda-feira");
            Portuguese.Add("DayTuesday", "Terça-feira");
            Portuguese.Add("DayWednesday", "Quarta-feira");
            Portuguese.Add("DayThursday", "Quinta-feira");
            Portuguese.Add("DayFriday", "Sexta-feira");
            Portuguese.Add("DaySaturday", "Sábado");
            Portuguese.Add("DaySunday", "Domingo");

            Portuguese.Add("MonthJanuary", "Janeiro");
            Portuguese.Add("MonthFebruary", "Fevereiro");
            Portuguese.Add("MonthMarch", "Março");
            Portuguese.Add("MonthApril", "Abril");
            Portuguese.Add("MonthMay", "Maio");
            Portuguese.Add("MonthJune", "Junho");
            Portuguese.Add("MonthJuly", "Julho");
            Portuguese.Add("MonthAugust", "Agosto");
            Portuguese.Add("MonthSeptember", "Setembro");
            Portuguese.Add("MonthOctober", "Outubro");
            Portuguese.Add("MonthNovember", "Novembro");
            Portuguese.Add("MonthDecember", "Dezembro");
        }

        private static void InitializeDutch()
        {
            Dutch.Add("DayAny", "Elke");
            Dutch.Add("DayMonday", "Maandag");
            Dutch.Add("DayTuesday", "Dinsdag");
            Dutch.Add("DayWednesday", "Woensdag");
            Dutch.Add("DayThursday", "Donderdag");
            Dutch.Add("DayFriday", "Vrijdag");
            Dutch.Add("DaySaturday", "Zaterdag");
            Dutch.Add("DaySunday", "Zondag");

            Dutch.Add("MonthJanuary", "Januari");
            Dutch.Add("MonthFebruary", "Februari");
            Dutch.Add("MonthMarch", "Maart");
            Dutch.Add("MonthApril", "April");
            Dutch.Add("MonthMay", "Mei");
            Dutch.Add("MonthJune", "Juni");
            Dutch.Add("MonthJuly", "Juli");
            Dutch.Add("MonthAugust", "Augustus");
            Dutch.Add("MonthSeptember", "September");
            Dutch.Add("MonthOctober", "Oktober");
            Dutch.Add("MonthNovember", "November");
            Dutch.Add("MonthDecember", "December");
        }

        private static void InitializeFrench()
        {
            French.Add("DayAny", "Tout");
            French.Add("DayMonday", "Lundi");
            French.Add("DayTuesday", "Mardi");
            French.Add("DayWednesday", "Mercredi");
            French.Add("DayThursday", "Jeudi");
            French.Add("DayFriday", "Vendredi");
            French.Add("DaySaturday", "Samedi");
            French.Add("DaySunday", "Dimanche");

            French.Add("MonthJanuary", "Janvier");
            French.Add("MonthFebruary", "Février");
            French.Add("MonthMarch", "Mars");
            French.Add("MonthApril", "Avril");
            French.Add("MonthMay", "Mai");
            French.Add("MonthJune", "Juin");
            French.Add("MonthJuly", "Juillet");
            French.Add("MonthAugust", "Août");
            French.Add("MonthSeptember", "Septembre");
            French.Add("MonthOctober", "Octobre");
            French.Add("MonthNovember", "Novembre");
            French.Add("MonthDecember", "Décembre");
        }

        private static void InitializeGerman()
        {
            German.Add("DayAny", "Jeder");
            German.Add("DayMonday", "Montag");
            German.Add("DayTuesday", "Dienstag");
            German.Add("DayWednesday", "Mittwoch");
            German.Add("DayThursday", "Donnerstag");
            German.Add("DayFriday", "Freitag");
            German.Add("DaySaturday", "Samstag");
            German.Add("DaySunday", "Sonntag");

            German.Add("MonthJanuary", "Januar");
            German.Add("MonthFebruary", "Februar");
            German.Add("MonthMarch", "März");
            German.Add("MonthApril", "April");
            German.Add("MonthMay", "Mai");
            German.Add("MonthJune", "Juni");
            German.Add("MonthJuly", "Juli");
            German.Add("MonthAugust", "August");
            German.Add("MonthSeptember", "September");
            German.Add("MonthOctober", "Oktober");
            German.Add("MonthNovember", "November");
            German.Add("MonthDecember", "Dezember");
        }

        private static void InitializeItalian()
        {
            Italian.Add("DayAny", "Qualsiasi");
            Italian.Add("DayMonday", "Lunedi");
            Italian.Add("DayTuesday", "Martedì");
            Italian.Add("DayWednesday", "Mercoledì");
            Italian.Add("DayThursday", "Giovedi");
            Italian.Add("DayFriday", "Venerdì");
            Italian.Add("DaySaturday", "Sabato");
            Italian.Add("DaySunday", "Domenica");

            Italian.Add("MonthJanuary", "Gennaio");
            Italian.Add("MonthFebruary", "Febbraio");
            Italian.Add("MonthMarch", "Marzo");
            Italian.Add("MonthApril", "Aprile");
            Italian.Add("MonthMay", "Maggio");
            Italian.Add("MonthJune", "Giugno");
            Italian.Add("MonthJuly", "Luglio");
            Italian.Add("MonthAugust", "Agosto");
            Italian.Add("MonthSeptember", "Settembre");
            Italian.Add("MonthOctober", "Ottobre");
            Italian.Add("MonthNovember", "Novembre");
            Italian.Add("MonthDecember", "Dicembre");
        }

        private static void InitializeSpanish()
        {
            Spanish.Add("DayAny", "Cualquier");
            Spanish.Add("DayMonday", "Lunes");
            Spanish.Add("DayTuesday", "Martes");
            Spanish.Add("DayWednesday", "Miércoles");
            Spanish.Add("DayThursday", "Jueves");
            Spanish.Add("DayFriday", "Viernes");
            Spanish.Add("DaySaturday", "Sábado");
            Spanish.Add("DaySunday", "Domingo");

            Spanish.Add("MonthJanuary", "Enero");
            Spanish.Add("MonthFebruary", "Febrero");
            Spanish.Add("MonthMarch", "Marzo");
            Spanish.Add("MonthApril", "Abril");
            Spanish.Add("MonthMay", "Mayo");
            Spanish.Add("MonthJune", "Junio");
            Spanish.Add("MonthJuly", "Julio");
            Spanish.Add("MonthAugust", "Agosto");
            Spanish.Add("MonthSeptember", "Septiembre");
            Spanish.Add("MonthOctober", "Octubre");
            Spanish.Add("MonthNovember", "Noviembre");
            Spanish.Add("MonthDecember", "Diciembre");
        }

        private static void InitializeEnglish()
        {
            English.Add("DayAny", "Any");
            English.Add("DayMonday", "Monday");
            English.Add("DayTuesday", "Tuesday");
            English.Add("DayWednesday", "Wednesday");
            English.Add("DayThursday", "Thursday");
            English.Add("DayFriday", "Friday");
            English.Add("DaySaturday", "Saturday");
            English.Add("DaySunday", "Sunday");

            English.Add("MonthJanuary", "January");
            English.Add("MonthFeburary", "Feburary");
            English.Add("MonthMarch", "March");
            English.Add("MonthApril", "April");
            English.Add("MonthMay", "May");
            English.Add("MonthJune", "June");
            English.Add("MonthJuly", "July");
            English.Add("MonthAugust", "August");
            English.Add("MonthSeptember", "September");
            English.Add("MonthOctober", "October");
            English.Add("MonthNovember", "November");
            English.Add("MonthDecember", "December");
        }
        #endregion

        #region Properties
        public static string DayAny
        {
            get { return GetString("DayAny"); }
        }
        public static string DayMonday
        {
            get { return GetString("DayMonday"); }
        }
        public static string DayTuesday
        {
            get { return GetString("DayTuesday"); }
        }
        public static string DayWednesday
        {
            get { return GetString("DayWednesday"); }
        }
        public static string DayThursday
        {
            get { return GetString("DayThursday"); }
        }
        public static string DayFriday
        {
            get { return GetString("DayFriday"); }
        }
        public static string DaySaturday
        {
            get { return GetString("DaySaturday"); }
        }
        public static string DaySunday
        {
            get { return GetString("DaySunday"); }
        }
        public static string MonthJanuary
        {
            get { return GetString("MonthJanuary"); }
        }
        public static string MonthFebruary
        {
            get { return GetString("MonthFebruary"); }
        }
        public static string MonthMarch
        {
            get { return GetString("MonthMarch"); }
        }
        public static string MonthApril
        {
            get { return GetString("MonthApril"); }
        }
        public static string MonthMay
        {
            get { return GetString("MonthMay"); }
        }
        public static string MonthJune
        {
            get { return GetString("MonthJune"); }
        }
        public static string MonthJuly
        {
            get { return GetString("MonthJuly"); }
        }
        public static string MonthAugust
        {
            get { return GetString("MonthAugust"); }
        }
        public static string MonthSeptember
        {
            get { return GetString("MonthSeptember"); }
        }
        public static string MonthOctober
        {
            get { return GetString("MonthOctober"); }
        }
        public static string MonthNovember
        {
            get { return GetString("MonthNovember"); }
        }
        public static string MonthDecember
        {
            get { return GetString("MonthDecember"); }
        }
        #endregion
    }
}
