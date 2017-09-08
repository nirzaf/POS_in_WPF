using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModeledDataTypeAttribute : System.Attribute
    {
        public string Alias1Type
        {
            get;
            private set;
        }

        public string Alias2Type
        {
            get;
            private set;
        }

        public string Alias3Type
        {
            get;
            private set;
        }

        public string Alias4Type
        {
            get;
            private set;
        }

        public ModeledDataTypeAttribute()
        {
            Alias1Type = null;
            Alias2Type = null;
            Alias3Type = null;
            Alias4Type = null;
        }

        public ModeledDataTypeAttribute(string type1)
        {
            Alias1Type = type1;
            Alias2Type = null;
            Alias3Type = null;
            Alias4Type = null;
        }

        public ModeledDataTypeAttribute(string type1, string type2)
        {
            Alias1Type = type1;
            Alias2Type = type2;
            Alias3Type = null;
            Alias4Type = null;
        }

        public ModeledDataTypeAttribute(string type1, string type2, string type3)
        {
            Alias1Type = type1;
            Alias2Type = type2;
            Alias3Type = type3;
            Alias4Type = null;
        }

        public ModeledDataTypeAttribute(string type1, string type2, string type3, string type4)
        {
            Alias1Type = type1;
            Alias2Type = type2;
            Alias3Type = type3;
            Alias4Type = type4;
        }
    }
}
