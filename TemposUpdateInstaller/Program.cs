using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using TemposLibrary;

namespace TemposUpdateInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                string restartPath = null;
                int processId = 0;
                if ((args != null) && (args.Length >= 1) && (args[0].Length > 0))
                    processId = Convert.ToInt32(args[0]);
                if ((args != null) && (args.Length >= 2) && (args[0].Length > 0))
                    restartPath = args[1];
                if (VistaSecurity.IsVista && !VistaSecurity.IsAdmin)
                {                    
                    // Run the elevated version of this program (and wait for it to exit)
                    VistaSecurity.RestartElevated(processId.ToString(), true);

                    // Restart PointOfSale.exe
                    VistaSecurity.StartProgram("PointOfSale.exe", "", false, false);
                    return;
                }

                // Wait for PointOfSale.exe to exit
                WaitForApplicationShutdown(processId);

                // Install the update
                StartInstall(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    @"\TemPOS Update\Update");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TemposUpdateInstaller Exception");
            }
        }

        private static void WaitForApplicationShutdown(int processId)
        {
            if (processId == 0)
                return;
            try
            {
                Process process = Process.GetProcessById(processId);
                while ((process != null) && !process.HasExited)
                {
                    Thread.Sleep(200);
                    process = Process.GetProcessById(processId);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Copies all files from the current folder to the parent folder,
        /// except for this application's executable; existing files are replaced.
        /// </summary>
        private static void StartInstall(string sourceDirectory)
        {            
            string targetDirectory = Directory.GetCurrentDirectory();

            string[] files = Directory.GetFiles(sourceDirectory);
            foreach (string file in files)
            {
                string[] filenameSplit = file.Split('\\');
                string filename = filenameSplit[filenameSplit.Length - 1];
                // Don't copy this file
                if (file.Contains(Application.ExecutablePath))
                    continue;
                // Don't install a TemposProcs.dll unless it already exists in the
                // target folder. Only the client on the SQL server's computer
                // will have this file installed.
                if (file.Contains("TemposProcs") &&
                    !File.Exists(targetDirectory + @"\" + filename))
                    continue;
                File.Copy(file, targetDirectory + @"\" + filename, true);
            }
        }
    }
}
