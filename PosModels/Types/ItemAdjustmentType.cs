using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum ItemAdjustmentType : byte
    {
        Addition = 0,
        Discontinuation = 1,
        OptionSetAddition = 2,
        OptionSetDeletion = 3,
    }
}
