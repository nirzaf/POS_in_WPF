using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposLibrary.Win32
{
    public static class PsApi
    {
        //[DllImport("psapi.dll")]
        //static unsafe extern bool EnumProcessModules(IntPtr handle, int * modules, int size, ref int needed);

        [DllImport("psapi.dll")]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);
    }
}
