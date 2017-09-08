using System;
using System.Reflection;
using PosModels;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void UserLoginEventHandler(object sender, UserLoginEventArgs args);

    [Obfuscation(Exclude = true)]
    public class UserLoginEventArgs : EventArgs
    {
        public Employee Employee
        {
            get;
            private set;
        }

        public UserLoginEventArgs(Employee employee)
        {
            this.Employee = employee;
        }
    }
}
