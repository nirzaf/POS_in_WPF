using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModeledDataNullableAttribute : System.Attribute
    {
        public ModeledDataNullableAttribute()
        {

        }
    }
}
