using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TemposLibrary.Win32;

namespace TemposLibrary
{
    /// 
    /// Access the Win32 API
    /// 
    public class Win32Wrappers
    {
        #region Licensed Access Only
        static Win32Wrappers()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Win32Wrappers).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        //###############################################################
        #region Windows Constants

        // For WaitForSingleObject
        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        public const ulong ENDSESSION_CLOSEAPP = 0x1L;
        public const ulong ENDSESSION_CRITICAL = 0x40000000L;
        public const ulong ENDSESSION_LOGOFF = 0x80000000L;

        public const int MK_LBUTTON = 0x1;
        public const int SC_RESTORE = 0xF120;
        public const int SC_MINIMIZE = 0xF020;
        public const int EM_GETLINECOUNT = 0xBA;

        // SetWindowPos
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int SWP_NOSIZE = 0x1;
        public const int SWP_NOMOVE = 0x2;
        public const int SWP_NOZORDER = 0x4;
        public const int SWP_NOREDRAW = 0x8;


        // GetWindow() constants
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int GW_ENABLEDPOPUP = 6;

        /* ShowWindow() Commands */
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_MAX = 11;

        /* Window messages */
        public const int WM_ACTIVATE = 6;
        public const int WM_ACTIVATEAPP = 28;
        public const int WM_ASKCBFORMATNAME = 780;
        public const int WM_CANCELJOURNAL = 75;
        public const int WM_CANCELMODE = 31;
        public const int WM_CAPTURECHANGED = 533;
        public const int WM_CHANGECBCHAIN = 781;
        public const int WM_CHAR = 258;
        public const int WM_CHARTOITEM = 47;
        public const int WM_CHILDACTIVATE = 34;
        public const int WM_CHOOSEFONT_GETLOGFONT = 1025;
        public const int WM_CHOOSEFONT_SETLOGFONT = 1125;
        public const int WM_CHOOSEFONT_SETFLAGS = 1126;
        public const int WM_CLEAR = 771;
        public const int WM_CLOSE = 16;
        public const int WM_COMMAND = 273;
        public const int WM_COMPACTING = 65;
        public const int WM_COMPAREITEM = 57;
        public const int WM_CONTEXTMENU = 123;
        public const int WM_COPY = 769;
        public const int WM_COPYDATA = 74;
        public const int WM_CREATE = 1;
        public const int WM_CTLCOLORBTN = 309;
        public const int WM_CTLCOLORDLG = 310;
        public const int WM_CTLCOLOREDIT = 307;
        public const int WM_CTLCOLORLISTBOX = 308;
        public const int WM_CTLCOLORMSGBOX = 306;
        public const int WM_CTLCOLORSCROLLBAR = 311;
        public const int WM_CTLCOLORSTATIC = 312;
        public const int WM_CUT = 768;
        public const int WM_DEADCHAR = 259;
        public const int WM_DELETEITEM = 45;
        public const int WM_DESTROY = 2;
        public const int WM_DESTROYCLIPBOARD = 775;
        public const int WM_DEVICECHANGE = 537;
        public const int WM_DEVMODECHANGE = 27;
        public const int WM_DISPLAYCHANGE = 126;
        public const int WM_DRAWCLIPBOARD = 776;
        public const int WM_DRAWITEM = 43;
        public const int WM_DROPFILES = 563;
        public const int WM_ENABLE = 10;
        public const int WM_ENDSESSION = 22;
        public const int WM_ENTERIDLE = 289;
        public const int WM_ENTERMENULOOP = 529;
        public const int WM_ENTERSIZEMOVE = 561;
        public const int WM_ERASEBKGND = 20;
        public const int WM_EXITMENULOOP = 530;
        public const int WM_EXITSIZEMOVE = 562;
        public const int WM_FONTCHANGE = 29;
        public const int WM_GETDLGCODE = 135;
        public const int WM_GETFONT = 49;
        public const int WM_GETHOTKEY = 51;
        public const int WM_GETICON = 127;
        public const int WM_GETMINMAXINFO = 36;
        public const int WM_GETTEXT = 13;
        public const int WM_GETTEXTLENGTH = 14;
        public const int WM_HELP = 83;
        public const int WM_HOTKEY = 786;
        public const int WM_HSCROLL = 276;
        public const int WM_HSCROLLCLIPBOARD = 782;
        public const int WM_ICONERASEBKGND = 39;
        public const int WM_IME_CHAR = 646;
        public const int WM_IME_COMPOSITION = 271;
        public const int WM_IME_COMPOSITIONFULL = 644;
        public const int WM_IME_CONTROL = 643;
        public const int WM_IME_ENDCOMPOSITION = 270;
        public const int WM_IME_KEYDOWN = 656;
        public const int WM_IME_KEYUP = 657;
        public const int WM_IME_NOTIFY = 642;
        public const int WM_IME_SELECT = 645;
        public const int WM_IME_SETCONTEXT = 641;
        public const int WM_IME_STARTCOMPOSITION = 269;
        public const int WM_INITDIALOG = 272;
        public const int WM_INITMENU = 278;
        public const int WM_INITMENUPOPUP = 279;
        public const int WM_INPUTLANGCHANGE = 81;
        public const int WM_INPUTLANGCHANGEREQUEST = 80;
        public const int WM_KEYDOWN = 256;
        public const int WM_KEYUP = 257;
        public const int WM_KILLFOCUS = 8;
        public const int WM_LBUTTONDBLCLK = 515;
        public const int WM_LBUTTONDOWN = 513;
        public const int WM_LBUTTONUP = 514;
        public const int WM_MBUTTONDBLCLK = 521;
        public const int WM_MBUTTONDOWN = 519;
        public const int WM_MBUTTONUP = 520;
        public const int WM_MDIACTIVATE = 546;
        public const int WM_MDICASCADE = 551;
        public const int WM_MDICREATE = 544;
        public const int WM_MDIDESTROY = 545;
        public const int WM_MDIGETACTIVE = 553;
        public const int WM_MDIICONARRANGE = 552;
        public const int WM_MDIMAXIMIZE = 549;
        public const int WM_MDINEXT = 548;
        public const int WM_MDIREFRESHMENU = 564;
        public const int WM_MDIRESTORE = 547;
        public const int WM_MDISETMENU = 560;
        public const int WM_MDITILE = 550;
        public const int WM_MEASUREITEM = 44;
        public const int WM_MENUCHAR = 288;
        public const int WM_MENUSELECT = 287;
        public const int WM_MOUSEACTIVATE = 33;
        public const int WM_MOUSEMOVE = 512;
        public const int WM_MOVE = 3;
        public const int WM_MOVING = 534;
        public const int WM_NCACTIVATE = 134;
        public const int WM_NCCALCSIZE = 131;
        public const int WM_NCCREATE = 129;
        public const int WM_NCDESTROY = 130;
        public const int WM_NCHITTEST = 132;
        public const int WM_NCLBUTTONDBLCLK = 163;
        public const int WM_NCLBUTTONDOWN = 161;
        public const int WM_NCLBUTTONUP = 162;
        public const int WM_NCMBUTTONDBLCLK = 169;
        public const int WM_NCMBUTTONDOWN = 167;
        public const int WM_NCMBUTTONUP = 168;
        public const int WM_NCMOUSEMOVE = 160;
        public const int WM_NCPAINT = 133;
        public const int WM_NCRBUTTONDBLCLK = 166;
        public const int WM_NCRBUTTONDOWN = 164;
        public const int WM_NCRBUTTONUP = 165;
        public const int WM_NEXTDLGCTL = 40;
        public const int WM_NOTIFY = 78;
        public const int WM_NOTIFYFORMAT = 85;
        public const int WM_NULL = 0;
        public const int WM_PAINT = 15;
        public const int WM_PAINTCLIPBOARD = 777;
        public const int WM_PAINTICON = 38;
        public const int WM_PALETTECHANGED = 785;
        public const int WM_PALETTEISCHANGING = 784;
        public const int WM_PARENTNOTIFY = 528;
        public const int WM_PASTE = 770;
        public const int WM_PENWINFIRST = 896;
        public const int WM_PENWINLAST = 911;
        public const int WM_POWER = 72;
        public const int WM_POWERBROADCAST = 536;
        public const int WM_PRINT = 791;
        public const int WM_PRINTCLIENT = 792;
        public const int WM_PSD_ENVSTAMPRECT = 1029;
        public const int WM_PSD_FULLPAGERECT = 1025;
        public const int WM_PSD_GREEKTEXTRECT = 1028;
        public const int WM_PSD_MARGINRECT = 1027;
        public const int WM_PSD_MINMARGINRECT = 1026;
        public const int WM_PSD_PAGESETUPDLG = 1024;
        public const int WM_PSD_YAFULLPAGERECT = 1030;
        public const int WM_QUERYDRAGICON = 55;
        public const int WM_QUERYENDSESSION = 17;
        public const int WM_QUERYNEWPALETTE = 783;
        public const int WM_QUERYOPEN = 19;
        public const int WM_QUEUESYNC = 35;
        public const int WM_QUIT = 18;
        public const int WM_RBUTTONDBLCLK = 518;
        public const int WM_RBUTTONDOWN = 516;
        public const int WM_RBUTTONUP = 517;
        public const int WM_RENDERALLFORMATS = 774;
        public const int WM_RENDERFORMAT = 773;
        public const int WM_SETCURSOR = 32;
        public const int WM_SETFOCUS = 7;
        public const int WM_SETFONT = 48;
        public const int WM_SETHOTKEY = 50;
        public const int WM_SETICON = 128;
        public const int WM_SETREDRAW = 11;
        public const int WM_SETTEXT = 12;
        public const int WM_SETTINGCHANGE = 26;
        public const int WM_SHOWWINDOW = 24;
        public const int WM_SIZE = 5;
        public const int WM_SIZECLIPBOARD = 779;
        public const int WM_SIZING = 532;
        public const int WM_SPOOLERSTATUS = 42;
        public const int WM_STYLECHANGED = 125;
        public const int WM_STYLECHANGING = 124;
        public const int WM_SYSCHAR = 262;
        public const int WM_SYSCOLORCHANGE = 21;
        public const int WM_SYSCOMMAND = 274;
        public const int WM_SYSDEADCHAR = 263;
        public const int WM_SYSKEYDOWN = 260;
        public const int WM_SYSKEYUP = 261;
        public const int WM_TCARD = 82;
        public const int WM_TIMECHANGE = 30;
        public const int WM_TIMER = 275;
        public const int WM_UNDO = 772;
        public const int WM_USER = 1024;
        public const int WM_USERCHANGED = 84;
        public const int WM_VKEYTOITEM = 46;
        public const int WM_VSCROLL = 277;
        public const int WM_VSCROLLCLIPBOARD = 778;
        public const int WM_WINDOWPOSCHANGED = 71;
        public const int WM_WINDOWPOSCHANGING = 70;
        public const int WM_WININICHANGE = 26;

        // WM_USER message
        public const int EM_SETSCROLLPOS = WM_USER + 222;


        /* Window message ranges */
        public const int WM_KEYFIRST = 256;
        public const int WM_KEYLAST = 264;
        public const int WM_MOUSEFIRST = 512;
        public const int WM_MOUSELAST = 521;

        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;
        public const uint KEYEVENTF_SCANCODE = 0x0008;
        public const uint XBUTTON1 = 0x0001;
        public const uint XBUTTON2 = 0x0002;
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const uint MOUSEEVENTF_XDOWN = 0x0080;
        public const uint MOUSEEVENTF_XUP = 0x0100;
        public const uint MOUSEEVENTF_WHEEL = 0x0800;
        public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        /* GetScrollBarInfo */
        public const long OBJID_CLIENT = 0xFFFFFFFC;
        public const long OBJID_HSCROLL = 0xFFFFFFFB;
        public const long OBJID_VSCROLL = 0xFFFFFFFA;

        public const int SB_THUMBPOSITION = 4;
        public const int SB_VERT = 1;

        public enum USER_OBJECT_INFORMATION
        {
            Flags = 1,
            Name = 2,
            Type = 3,
            UserSID = 4
        }

        [Flags]
        public enum ACCESS_MASK : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000f0000,

            STANDARD_RIGHTS_READ = 0x00020000,
            STANDARD_RIGHTS_WRITE = 0x00020000,
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            STANDARD_RIGHTS_ALL = 0x001f0000,

            SPECIFIC_RIGHTS_ALL = 0x0000ffff,

            ACCESS_SYSTEM_SECURITY = 0x01000000,

            MAXIMUM_ALLOWED = 0x02000000,

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            DESKTOP_READOBJECTS = 0x00000001,
            DESKTOP_CREATEWINDOW = 0x00000002,
            DESKTOP_CREATEMENU = 0x00000004,
            DESKTOP_HOOKCONTROL = 0x00000008,
            DESKTOP_JOURNALRECORD = 0x00000010,
            DESKTOP_JOURNALPLAYBACK = 0x00000020,
            DESKTOP_ENUMERATE = 0x00000040,
            DESKTOP_WRITEOBJECTS = 0x00000080,
            DESKTOP_SWITCHDESKTOP = 0x00000100,

            WINSTA_ENUMDESKTOPS = 0x00000001,
            WINSTA_READATTRIBUTES = 0x00000002,
            WINSTA_ACCESSCLIPBOARD = 0x00000004,
            WINSTA_CREATEDESKTOP = 0x00000008,
            WINSTA_WRITEATTRIBUTES = 0x00000010,
            WINSTA_ACCESSGLOBALATOMS = 0x00000020,
            WINSTA_EXITWINDOWS = 0x00000040,
            WINSTA_ENUMERATE = 0x00000100,
            WINSTA_READSCREEN = 0x00000200,

            WINSTA_ALL_ACCESS = 0x0000037f
        }

        #endregion


        //###############################################################
        #region Structures
        /*
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public unsafe byte* lpSecurityDescriptor;
            public int bInheritHandle;
        }
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cbSize;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// LONG->int
            public int left;

            /// LONG->int
            public int top;

            /// LONG->int
            public int right;

            /// LONG->int
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            /// DWORD->unsigned int
            public uint cbSize;

            /// RECT->tagRECT
            public RECT rcWindow;

            /// RECT->tagRECT
            public RECT rcClient;

            /// DWORD->unsigned int
            public uint dwStyle;

            /// DWORD->unsigned int
            public uint dwExStyle;

            /// DWORD->unsigned int
            public uint dwWindowStatus;

            /// UINT->unsigned int
            public uint cxWindowBorders;

            /// UINT->unsigned int
            public uint cyWindowBorders;

            /// ATOM->WORD->unsigned short
            public ushort atomWindowType;

            /// WORD->unsigned short
            public ushort wCreatorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLBARINFO
        {
            public int cbSize;
            public RECT rcScrollBar;
            public int dxyLineButton;
            public int xyThumbTop;
            public int xyThumbBottom;
            public int reserved;
            public int[] rgstate;
        }
        #endregion


        //###############################################################
        #region DLL Imports

        #region user32
        //private const int WM_VSCROLL = 0x115;
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetScrollBarInfo(IntPtr hWnd, long idObject, ref SCROLLBARINFO barInfo);

        // ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/dllproc/base/createdesktop.htm
        [DllImport("user32.dll", EntryPoint = "CreateDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateDesktop(
                        [MarshalAs(UnmanagedType.LPWStr)] string desktopName,
                        [MarshalAs(UnmanagedType.LPWStr)] string device, // must be null.
                        [MarshalAs(UnmanagedType.LPWStr)] string deviceMode, // must be null,
                        [MarshalAs(UnmanagedType.U4)] int flags,  // use 0
                        [MarshalAs(UnmanagedType.U4)] ACCESS_MASK accessMask,
                        [MarshalAs(UnmanagedType.Struct)] SECURITY_ATTRIBUTES attributes);

        [DllImport("user32.dll", EntryPoint = "CreateDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateDesktop(
                        [MarshalAs(UnmanagedType.LPWStr)] string desktopName,
                        [MarshalAs(UnmanagedType.LPWStr)] string device, // must be null.
                        [MarshalAs(UnmanagedType.LPWStr)] string deviceMode, // must be null,
                        [MarshalAs(UnmanagedType.U4)] int flags,  // use 0
                        [MarshalAs(UnmanagedType.U4)] ACCESS_MASK accessMask,
                        [MarshalAs(UnmanagedType.Struct)] IntPtr attributes);

        // ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/dllproc/base/closedesktop.htm
        [DllImport("user32.dll", EntryPoint = "CloseDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseDesktop(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit,
           ACCESS_MASK dwDesiredAccess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetUserObjectInformation(IntPtr hObj, USER_OBJECT_INFORMATION nIndex,
           [Out] byte[] pvInfo, uint nLength, out uint lpnLengthNeeded);

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
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

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
        public static extern bool GetCursorPos(ref Point lpPoint);

        #endregion

        #region kernel32
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

        [DllImport("kernel32.dll", EntryPoint = "CreateProcessW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName,
           string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
           ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
           uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
           [In] ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", EntryPoint = "CreateProcessW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName,
           string lpCommandLine, IntPtr lpProcessAttributes,
           IntPtr lpThreadAttributes, bool bInheritHandles,
           uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
           [In] ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern uint QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region psapi
        //[DllImport("psapi.dll")]
        //static unsafe extern bool EnumProcessModules(IntPtr handle, int * modules, int size, ref int needed);

        [DllImport("psapi.dll")]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);
        #endregion

        #endregion


        //###############################################################
        #region Win32 Interface code

        public static Point GetCursorPos()
        {
            Point pt = new Point();
            User32.GetCursorPos(ref pt);
            return pt;
        }

        /*
        //  You can now use it like this:
        //
        // char c = Win32.ToAscii(Keys.OemQuestion, Keys.ShiftKey); // = '?'
        // If you need more than one modifier, just or them. Keys.ShiftKey | Keys.AltKey
        //
        public static char ToAscii(Keys key, Keys modifiers)
        {
            var outputBuilder = new StringBuilder(2);
            int result = ToAscii((uint)key, 0, GetKeyState(modifiers),
                         outputBuilder, 0);
            if (result == 1)
                return outputBuilder[0];
            else
                throw new Exception("Invalid key");
        }

        private const byte HighBit = 0x80;
        public static byte[] GetKeyState(Keys modifiers)
        {
            var keyState = new byte[256];
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if ((modifiers & key) == key)
                {
                    keyState[(int)key] = HighBit;
                }
            }
            return keyState;
        }
        */

        /// 
        /// Returns a string containing the title of the window with the
        /// specified handle
        /// 
        public static string SafeGetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// 
        /// Returns the program executable name ideally
        /// 
        public static string GetWindowModuleFileName(IntPtr hWnd)
        {
            uint processId = 0;
            const int nChars = 1024;
            StringBuilder filename = new StringBuilder(nChars);
            GetWindowThreadProcessId(hWnd, out processId);
            IntPtr hProcess = OpenProcess(0x410, 0, processId); // 0x410 or 0x1010
            if (hProcess == null)
                return "(Failed to find process)";
            if (GetProcessImageFileName(hProcess, filename, nChars) < 1)
            {
                // Fall back, not sure how to un-hide this info
                CloseHandle(hProcess);
                return "";
            }
            CloseHandle(hProcess);
            return filename.ToString();
        }

        /// 
        /// Returns the program executable name of the foreground window
        /// 
        public static string GetForegroundWindowModuleFileName()
        {
            return GetWindowModuleFileName(GetForegroundWindow());
        }

        /// 
        /// Returns the volume serial number for the specified drive
        /// 
        public static string GetVolumeSerial(string strDriveLetter)
        {
            uint serNum = 0;
            uint maxCompLen = 0;
            StringBuilder VolLabel = new StringBuilder(256); // Label
            UInt32 VolFlags = new UInt32();
            StringBuilder FSName = new StringBuilder(256); // File System Name
            strDriveLetter += ":\\"; // fix up the passed-in drive letter for the API call
            long Ret = GetVolumeInformation(strDriveLetter, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);

            return Convert.ToString(serNum);
        }

        public static Rectangle GetWindowClientRectangle(IntPtr hWnd)
        {
            WINDOWINFO info = new WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            if (GetWindowInfo(hWnd, ref info))
            {
                RECT rect = info.rcClient;
                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;
                return new Rectangle(rect.left, rect.top, width, height);
            }
            return new Rectangle(0, 0, 0, 0);
        }

        public static Rectangle GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect = new RECT();
            if (GetWindowRect(hWnd, out rect))
            {
                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;
                return new Rectangle(rect.left, rect.top, width, height);
            }
            return new Rectangle(0, 0, 0, 0);
        }

        #endregion

        public static void ForceSetForegroundWindow(IntPtr hWnd)
        {
            if (GetForegroundWindow() != hWnd)
            {
                IntPtr dwMyThreadID = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
                IntPtr dwOtherThreadID = GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                if (dwMyThreadID != dwOtherThreadID)
                {
                    AttachThreadInput(dwMyThreadID, dwOtherThreadID, true);
                    BringWindowToTop(hWnd);
                    SetForegroundWindow(hWnd);
                    IntPtr hMain = GetWindow(hWnd, GW_ENABLEDPOPUP);
                    SetFocus(hMain);
                    AttachThreadInput(dwMyThreadID, dwOtherThreadID, false);
                }
                else
                    SetForegroundWindow(hWnd);
            }
        }

        public static IntPtr GetChildHandle(string caption, string className, IntPtr topLevelHandle)
        {
            //Returns the handle to a child window
            IntPtr hwnd = IntPtr.Zero;

            hwnd = FindWindowEx(topLevelHandle, IntPtr.Zero, className, caption);
            if (hwnd == IntPtr.Zero)
            {
                //int ErrorNumber = 0;
                //ErrorNumber = Err.LastDllError;
                //if (HookError != null) {
                //    HookError(ErrorNumber, "Error getting child window.", GetAPIErrorText(ErrorNumber));
                //}
            }

            return hwnd;
        }

        public static IntPtr GetWindowHandle(string caption, string className)
        {
            //Returns the Handle of a top-level Window that has the caption and/or classname specified
            IntPtr HandleToWindow = IntPtr.Zero;
            HandleToWindow = FindWindow(className, caption);
            if (HandleToWindow == IntPtr.Zero)
            {
                //int ErrorNumber = 0;
                //ErrorNumber = Err.LastDllError;
                //if (HookError != null) {
                //    HookError(ErrorNumber, "Error Getting Top Level Handle", GetAPIErrorText(ErrorNumber));
                //}
            }
            return HandleToWindow;
        }

        //public const int WM_PAINT = 0x000F;
        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

    }
}
