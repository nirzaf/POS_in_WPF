using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TemposLanguageProcessor.Types
{
    public abstract class StringsCore
    {
        #region Licensed Access Only / Static Initializer
        static StringsCore()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(StringsCore).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

#if DEBUG
        private static Languages _language = Languages.Debug;
#else
        private static Languages _language = Languages.English;
#endif

        public static event EventHandler LanguageChanged;

        public static Languages Language
        {
            get { return _language; }
            set
            {
                if (_language == value) return;
                _language = value;
                OnLanguageChanged();
            }
        }

        protected static void OnLanguageChanged()
        {
            if (LanguageChanged != null)
                LanguageChanged.Invoke(null, new EventArgs());
        }

        public static void SetDefaultLanguage()
        {
#if DEBUG
            Language = Languages.Debug;
#else
            if (CultureInfo.CurrentCulture.Name.StartsWith("es"))
                Language = Languages.Spanish;
            else if (CultureInfo.CurrentCulture.Name.StartsWith("de"))
                Language = Languages.German;
            else if (CultureInfo.CurrentCulture.Name.StartsWith("fr"))
                Language = Languages.French;
            else if (CultureInfo.CurrentCulture.Name.StartsWith("it"))
                Language = Languages.Italian;
            else if (CultureInfo.CurrentCulture.Name.StartsWith("nl"))
                Language = Languages.Dutch;
            else if (CultureInfo.CurrentCulture.Name.StartsWith("pt"))
                Language = Languages.Portuguese;
            else // Default: Unknown and all English languages
                Language = Languages.English;
#endif
        }        
    }
}
