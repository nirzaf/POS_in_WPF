using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModeledDataAttribute : System.Attribute
    {
        public string Alias1Name
        {
            get;
            private set;
        }

        public string Alias2Name
        {
            get;
            private set;
        }

        public string Alias3Name
        {
            get;
            private set;
        }

        public string Alias4Name
        {
            get;
            private set;
        }

        public ModeledDataAttribute()
        {
            Alias1Name = null;
            Alias2Name = null;
            Alias3Name = null;
            Alias4Name = null;
        }

        public ModeledDataAttribute(string alias1)
        {
            Alias1Name = alias1;
            Alias2Name = null;
            Alias3Name = null;
            Alias4Name = null;
        }

        public ModeledDataAttribute(string alias1, string alias2)
        {
            Alias1Name = alias1;
            Alias2Name = alias2;
            Alias3Name = null;
            Alias4Name = null;
        }

        public ModeledDataAttribute(string alias1, string alias2, string alias3)
        {
            Alias1Name = alias1;
            Alias2Name = alias2;
            Alias3Name = alias3;
            Alias4Name = null;
        }

        public ModeledDataAttribute(string alias1, string alias2,
            string alias3, string alias4)
        {
            Alias1Name = alias1;
            Alias2Name = alias2;
            Alias3Name = alias3;
            Alias4Name = alias4;
        }
    }
}
