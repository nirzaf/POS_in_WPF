using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using TemposLibrary;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Used to check if we can create a new mutex
        private static bool newMutexCreated;
        private static bool NewMutexCreated
        {
            get
            {
                return newMutexCreated;
            }
            set
            {
                newMutexCreated = value;
            }

        }

        static App()
        {
            // Set the current directory to the application path
            // So we don't start-up in the \windows\System32\ folder at windows login
            System.IO.Directory.SetCurrentDirectory(
                System.IO.Path.GetDirectoryName(VistaSecurity.GetExecutablePath())
            );

            // This works, where Muxtex didn't with XP
            if (GetDuplicateProcesses() != null)
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (!IsFirstInstance())
                Process.GetCurrentProcess().Kill();
        }

        private static Process GetDuplicateProcesses()
        {
            Process[] processes = Process.GetProcesses();
            //string currentProcessName = Process.GetCurrentProcess().MainModule.FileName;
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            foreach (Process process in processes)
            {
                try
                {
                    //if (process.MainModule.FileName.Equals(currentProcessName))
                    if (process.ProcessName.Equals(currentProcessName))
                    {
                        if (process.Id == Process.GetCurrentProcess().Id)
                            continue;
                        return process;
                    }
                }
                catch (Win32Exception)
                {

                }
            }
            return null;
        }

        private static bool IsFirstInstance()
        {
            // The name of the mutex is to be prefixed with Local\ to make
            // sure that its is created in the per-session namespace,
            // not in the global namespace.
            string mutexName = "Local\\" +
              System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            Mutex mutex = null;
            try
            {
                // Create a new mutex object with a unique name
                NewMutexCreated = false;
                mutex = new Mutex(false, mutexName, out newMutexCreated);
                //if ((args.Length == 1) && args[0].Equals("/RESTART"))
                //    mutex.WaitOne();
            }
            catch (Exception)
            {
                return false;
            }

            // When the mutex is created for the first time
            // we run the program since it is the first instance.
            if (newMutexCreated)
            {
                // Set task priority to realtime
                //Process thisProc = Process.GetCurrentProcess();
                //thisProc.PriorityClass = ProcessPriorityClass.AboveNormal;
                return true;
            }

            return false;
        }
    }
}
