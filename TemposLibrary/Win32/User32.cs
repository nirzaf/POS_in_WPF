using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposLibrary.Win32
{
    public class User32 : WinBase
    {
        #region Delegates
        public delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
        public delegate bool EnumDesktopWindowsProc(IntPtr desktopHandle, IntPtr lParam);
        #endregion

        #region Constants
        #endregion

        #region Externals

        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        /// <summary>
        /// Exit Windows
        /// </summary>
        /// <param name="uFlags">0 (normal logoff), 1 (shutdown), 2 (reboot), 4 (force logoff)</param>
        /// <param name="dwReason"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(int uFlags, int dwReason);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetScrollBarInfo(IntPtr hWnd, long idObject, ref SCROLLBARINFO barInfo);

        [DllImport("user32", EntryPoint = "CreateDesktopW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags,
                                                  int dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll")]
        public static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags,
           bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit,
           uint dwDesiredAccess);

        [DllImport("user32.dll")]
        public static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetUserObjectInformation(IntPtr hObj, USER_OBJECT_INFORMATION nIndex,
           [Out] byte[] pvInfo, uint nLength, out uint lpnLengthNeeded);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit,
           ACCESS_MASK dwDesiredAccess);

        //delegate used for EnumWindows() callback function
        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        ///
        /// Returns the handle to the foreground window
        /// 
        // Get a handle to an application window.
        // NOTE: Better to use this.Handle (on forms)
        [DllImport("user32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, IntPtr lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        //[DllImport("user32.dll")]
        //public static extern void ToggleDesktop();

        // Activate an application window.
        [DllImport("user32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Activate an application window.
        [DllImport("user32.DLL")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo,
            bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x,
            int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool ShowOwnedPopups(IntPtr hWnd, bool fShow);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        // Registers window as a viewer of clipboard events
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        // Unregisters a window as a viewer of clipboard events
        [DllImport("user32.dll")]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        // Get window at top in Z-Order, of specified window
        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        // Used to traverse Z-Order, of specified window
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        // Get the next window in Z-Order, of specified window
        [DllImport("user32.dll")]
        public static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

        // Depreciated, Win32 API
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);

        [DllImport("user32.DLL", CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        //[DllImport("user32.DLL", CharSet = CharSet.Unicode)]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowHandle();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc ewp, int lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(
                IntPtr hWnd,    // HWND handle to destination window
                int Msg,     // UINT message
                int wParam,  // WPARAM first message parameter
                int lParam   // LPARAM second message parameter
                );

        [DllImport("user32.dll")]
        public static extern int PostMessage(
                IntPtr hWnd,    // HWND handle to destination window
                int Msg,     // UINT message
                int wParam,  // WPARAM first message parameter
                int lParam   // LPARAM second message parameter
                );

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode,
            byte[] lpKeyState, [Out] StringBuilder lpChar, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        //[DllImport("user32.dll", SetLastError = true)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

        //public const int WM_PAINT = 0x000F;
        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref WinBase.PAINTSTRUCT lpPaint);
        
        #endregion

    }
}
