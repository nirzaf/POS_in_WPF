using System;
using System.Reflection;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void ButtonTouchInputClickEventHandler(object sender, ButtonTouchInputClickArgs args);

    [Obfuscation(Exclude = true)]
    public class ButtonTouchInputClickArgs : EventArgs
    {
        public int ButtonIndex
        {
            get;
            set;
        }
        public ButtonTouchInputClickArgs(int buttonIndex)
        {
            ButtonIndex = buttonIndex;
        }
    }
}

