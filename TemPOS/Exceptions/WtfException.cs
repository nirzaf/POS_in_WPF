using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemPOS.Exceptions
{
    public class WtfException : Exception
    {
        public string Text
        {
            get;
            private set;
        }

        public WtfException()
        {
            Text = null;
        }

        public WtfException(string text)
        {
            Text = text;
        }
    }
}
