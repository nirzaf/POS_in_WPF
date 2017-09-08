using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosControls.Interfaces
{
    public interface ISelectable
    {
        bool IsSelectable
        {
            get;
            set;
        }

        bool IsSelected
        {
            get;
            set;
        }
    }
}
