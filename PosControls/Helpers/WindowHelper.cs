using System.Linq;
using System.Windows;
using PosControls.Interfaces;

namespace PosControls.Helpers
{
    public static class WindowHelper
    {
        public static bool? ShowDialogForActiveWindow(this Window thisWindow)
        {
            return thisWindow.ShowDialog(
                Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive));
        }

        public static bool? ShowDialog(this Window thisWindow, Window ownerWindow)
        {
            if (thisWindow != null)
            {
                thisWindow.Owner = ownerWindow;
                if (ownerWindow is IShadeable)
                {
                    IShadeable shadable = ownerWindow as IShadeable;
                    if (!shadable.ShowShadingOverlay)
                        shadable.ShowShadingOverlay = true;
                    bool? result = thisWindow.ShowDialog();
                    shadable.ShowShadingOverlay = false;
                    return result;
                }
                return thisWindow.ShowDialog();
            }
            return null;
        }
    }
}
