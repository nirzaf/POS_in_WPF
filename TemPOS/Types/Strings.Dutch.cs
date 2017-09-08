using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PosModels.Types;

namespace TemPOS.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings : StringsCore
    {
        public static readonly Dictionary<string, string> Dutch = new Dictionary<string, string>();

        public static void InitializeDutch()
        {

        }
    }
}
