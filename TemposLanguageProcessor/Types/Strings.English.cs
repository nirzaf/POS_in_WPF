using System.Collections.Generic;
using System.Reflection;

namespace TemposLanguageProcessor.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings
    {
        public static readonly Dictionary<string, string> English = new Dictionary<string, string>();

        public static void InitializeEnglish()
        {
            #region General
            English.Add("Warning", "Warning");
            #endregion

            #region TestClass
            English.Add("Number123", "123");
            English.Add("Testing", "Testing");
            English.Add("ThisIsATest", "this is a test");
            #endregion
        }
    }
}
