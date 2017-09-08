using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace TaskManagerAccessServiceUninstaller
{
    public class VistaSecurity
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern StringBuilder GetCommandLine();

        public static bool IsAdmin
        {
            get
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                if (id == null) return false;
                var p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool IsVista
        {
            get
            {
                OperatingSystem osInfo = Environment.OSVersion;
                return (osInfo.Version.Major >= 6);
            }
        }

        public static string GetExecutablePath()
        {
            try
            {
                string path = GetCommandLine().ToString();
                return path.Substring(1, path.IndexOf("\"", 1, StringComparison.Ordinal) - 1);
            }
            catch
            {
                return null;
            }
        }

        public static bool Restart(string arguments, bool wait = false)
        {
            return StartProgram(null, arguments, false, wait);
        }

        public static bool RestartElevated(string arguments, bool wait = false)
        {
            return StartElevated(null, arguments, wait);
        }

        public static bool StartElevated(string fileName, string arguments,
            bool wait = false, bool hiddenWindow = false)
        {
            return StartProgram(fileName, arguments, true, wait, hiddenWindow);
        }

        public static bool StartProgram(string fileName, string arguments,
            bool asAdmin, bool wait, bool hiddenWindow = false)
        {
            
            if (fileName == null)
                fileName = Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", "");

            if ((fileName.Length > 1) && (fileName[1] != ':'))
                fileName = Directory.GetCurrentDirectory() + @"\" + fileName;

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = fileName,
                Arguments = arguments
            };

            if (hiddenWindow)
            {
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            if (asAdmin && IsVista)
                startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
                if (wait)
                {
                    p.WaitForExit();
                    return (p.ExitCode == 0);
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }
    }
}
