using System;
using System.Reflection;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void CommandEventHandler(object sender, CommandEventArgs args);

    [Obfuscation(Exclude = true)]
    public class CommandEventArgs : EventArgs
    {
        public string Command
        {
            get;
            private set;
        }
        public string Arguments
        {
            get;
            private set;
        }
        public CommandEventArgs(string command)
        {
            Command = command;
            Arguments = "";
        }
        public CommandEventArgs(string command, string arguments)
        {
            Command = command;
            Arguments = arguments;
        }
    }
}
