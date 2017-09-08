using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModeledDataClassAttribute : System.Attribute
    {
        public ModeledDataClassAttribute()
        {
        }
    }
}
