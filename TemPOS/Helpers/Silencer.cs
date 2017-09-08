using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TemPOS.Helpers
{
    /// <summary>
    /// This class disables sound from application events
    /// </summary>
    public class Silencer : DependencyObject
    {
        // Silence
        public static bool GetSilence(DependencyObject obj) { return (bool)obj.GetValue(SilenceProperty); }
        public static void SetSilence(DependencyObject obj, bool value) { obj.SetValue(SilenceProperty, value); }
        public static readonly DependencyProperty SilenceProperty = DependencyProperty.RegisterAttached("Silence", typeof(bool), typeof(Silencer), new FrameworkPropertyMetadata
        {
            Inherits = true,
            PropertyChangedCallback = (obj, e) =>
            {
                var element = obj as MediaElement; if (element == null) return;
                if ((bool)e.NewValue)
                {
                    element.SetBinding(UnmuteDetectedProperty, new Binding("IsMuted") { RelativeSource = RelativeSource.Self });
                    element.IsMuted = true;
                }
                else
                {
                    element.ClearValue(UnmuteDetectedProperty);
                    element.IsMuted = false;
                }
            }
        });

        // UnmuteDetected
        public static readonly DependencyProperty UnmuteDetectedProperty = DependencyProperty.RegisterAttached("UnmuteDetected", typeof(bool), typeof(Silencer), new PropertyMetadata
        {
            PropertyChangedCallback = (obj, e) =>
            {
                ((MediaElement)obj).IsMuted = GetSilence(obj);
            }
        });
    }
}
