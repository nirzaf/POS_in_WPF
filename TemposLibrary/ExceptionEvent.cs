using System;

namespace TemposLibrary
{
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs args);

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception
        {
            get;
            private set;
        }
        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
