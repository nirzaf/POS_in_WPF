using System;
using System.Reflection;
using System.Linq;

namespace TemposLanguageProcessor.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings : StringsCore
    {
        #region Code
        #region Licensed Access Only / Static Initializer
        static Strings()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Strings).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
            InitializeEnglish();
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
                default:
                    throw new Exception("Invalid Language");
            }
        }
        #endregion

        #region General
        public static string Warning
        {
            get { return GetString("Warning"); }
        }
        #endregion

        #region TestClass
        public static string Number123
        {
            get { return GetString("Number123"); }
        }
        public static string Testing
        {
            get { return GetString("Testing"); }
        }
        public static string ThisIsATest
        {
            get { return GetString("ThisIsATest"); }
        }
        #endregion
    }
}
