using System;
using TemPOS.Types;
using System.Windows.Input;
using TemPOS.EventHandlers;

namespace TemPOS.Commands
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public event EventHandler Executed;
        public event CancelableEventHandler Executing;

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                parameter = new object();
            return CanExecute(ref parameter);
        }
        protected abstract bool CanExecute(ref object parameter);

        public void Execute(object parameter)
        {
            if (parameter == null)
                parameter = new object();
            if (!OnExecuting())
                return; // Canceled
            if (Execute(ref parameter))
                OnExecuted();
        }
        protected abstract bool Execute(ref object parameter);

        protected void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged.Invoke(this, new EventArgs());
        }

        protected bool OnExecuting()
        {
            if (Executing != null)
            {
                bool cancel = false;
                CancelableEventHandler handler = Executing;
            
                if (handler != null)
                {
                    var args = new CancelableEventArgs();
                    foreach (CancelableEventHandler tmp in handler.GetInvocationList())
                    {
                        tmp(this, args);
                        if (!args.Cancel) continue;
                        cancel = true;
                        break;
                    }
                }
                return !cancel;
            }
            return true;
        }

        protected void OnExecuted()
        {
            if (Executed != null)
                Executed.Invoke(this, new EventArgs());
        }

        public void Update()
        {
            OnCanExecuteChanged();
        }
    }
}
