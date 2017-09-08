using System;
using System.Reflection;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void CancelableEventHandler(object sender, CancelableEventArgs args);

    [Obfuscation(Exclude = true)]
    public class CancelableEventArgs : EventArgs
    {
        public bool Cancel
        {
            get; set;
        }
    }
}
