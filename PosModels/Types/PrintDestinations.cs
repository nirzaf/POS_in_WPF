using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    [FlagsAttribute]
    public enum PrintDestination : short
    {
        None    = 0,
        Journal = 1,
        Receipt = 2,
        Kitchen1 = 4,
        Kitchen2 = 8,
        Kitchen3 = 16,
        Bar1 = 32,
        Bar2 = 64,
        Bar3 = 128
    }
}
