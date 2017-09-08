using System;

namespace TaskManagerAccessService
{
    public delegate void TextEventHandler(object sender, TextEventArgs args);

    public class TextEventArgs : EventArgs
    {
        public string Text
        {
            get;
            private set;
        }
        public TextEventArgs(string text)
        {
            Text = text;
        }
    }
}
