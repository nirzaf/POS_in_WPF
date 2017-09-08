// Type: System.ServiceProcess.SafeNativeMethods
// Assembly: System.ServiceProcess, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceProcess.dll

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.ServiceProcess
{
  [SuppressUnmanagedCodeSecurity]
  [ComVisible(false)]
  internal static class SafeNativeMethods
  {
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static bool CloseServiceHandle(IntPtr handle);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static IntPtr OpenSCManager(string machineName, string databaseName, int access);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    public static int LsaClose(IntPtr objectHandle);

    [DllImport("advapi32.dll")]
    public static int LsaFreeMemory(IntPtr ptr);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    public static int LsaNtStatusToWinError(int ntStatus);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static bool GetServiceKeyName(IntPtr SCMHandle, string displayName, StringBuilder shortName, ref int shortNameLength);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static bool GetServiceDisplayName(IntPtr SCMHandle, string shortName, StringBuilder displayName, ref int displayNameLength);
  }
}
