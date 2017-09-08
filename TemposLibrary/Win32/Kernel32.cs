using System;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposLibrary.Win32
{
    public class Kernel32 : WinBase
    {
        #region Delegates
        #endregion

        #region Constants
        #endregion

        #region Structures
        #endregion

        #region Externals

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WaitNamedPipe(string name, int timeout);
        
        [DllImport("kernel32.dll")]
        public static extern bool SetSystemPowerState(bool fSuspend, bool fForce);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern long GetVolumeInformation(string pathName, StringBuilder volumeNameBuffer,
            UInt32 volumeNameSize, ref UInt32 volumeSerialNumber, ref UInt32 maximumComponentLength,
            ref UInt32 fileSystemFlags, StringBuilder fileSystemNameBuffer, UInt32 fileSystemNameSize);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern int GetThreadId(IntPtr thread);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(IntPtr process);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern uint QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        #endregion

    }
}
