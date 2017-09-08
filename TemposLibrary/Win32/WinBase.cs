using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposLibrary.Win32
{
    public class WinBase
    {
        #region Delegates
        #endregion

        #region Constants

        public const int STARTF_USESTDHANDLES = 0x00000100;
        public const int STARTF_USESHOWWINDOW = 0x00000001;
        public const int UOI_NAME = 2;
        public const int STARTF_USEPOSITION = 0x00000004;
        public const int NORMAL_PRIORITY_CLASS = 0x00000020;
        
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

        public enum USER_OBJECT_INFORMATION : int
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

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
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
        #endregion
    }
}
