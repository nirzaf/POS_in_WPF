using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TaskManagerAccessService
{
    public static class Program
    {
        public static int ExitCode;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            ExitCode = 0;
            var servicesToRun = new ServiceBase[] 
            { 
                new TaskManagerHandlerService() 
            };
            ServiceBase.Run(servicesToRun);
            return ExitCode;
        }        
    }
}
