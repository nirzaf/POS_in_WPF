using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using PosControls;
using TemPOS.Types;
using TemposLibrary;
using TemposLibrary.Win32;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TemPOS.Managers
{
    public static class UserControlManager
    {
        #region Licensed Access Only
        static UserControlManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(UserControlManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        private static DateTime lastActivity = DateTime.Now;
        private static bool isEnabled = false;
        private static HookManager hookManager = new HookManager();

        public static TimeSpan UserInactivity
        {
            get
            {
                return DateTime.Now - lastActivity;
            }
            set
            {
                if (value != TimeSpan.Zero)
                    throw new InvalidOperationException();
                lastActivity = DateTime.Now;
            }
        }

        public static bool IsEnabled
        {
            get { return isEnabled; }
            private set { isEnabled = value; }
        }

        public static void Enable()
        {
            Enable(HookManager.HandleAllKeys);
        }
        public static void Enable(bool handleAllKeys)
        {
            if (IsEnabled)
                return;
            IsEnabled = true;
            if (VistaSecurity.IsVista)
            {
#if DEBUG
                HookManager.HandleAllKeys = false;
                HookManager.HotKeyExit += OnHotKeyExit;
#else
                HookManager.HandleAllKeys = handleAllKeys;
#endif
                hookManager.SubscribeToGlobalKeyboardEvents();

                PushCheckBox.Changed += UserActivity_Update;
                PushRadioButton.Changed += UserActivity_Update;
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Strings.YouAreNotUsingWindowsVistaOrNewerWindowsKeyboardProtectionIsDisabled,
                    Strings.Warning);
            }

            // Global event hook for TouchDown events
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.TouchDownEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
            // Global event hooks for Mouse events
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseMoveEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseDownEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseUpEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
            // Global event hooks for Keyboard events
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.KeyDownEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.KeyUpEvent,
                new RoutedEventHandler(UserActivity_RoutedUpdate), true);
        }

        static void UserActivity_RoutedUpdate(object sender, RoutedEventArgs e)
        {
            UserInactivity = TimeSpan.Zero;
        }

        static void UserActivity_Update(object sender, EventArgs e)
        {
            UserInactivity = TimeSpan.Zero;
        }

        public static void Disable()
        {
            if (!IsEnabled)
                return;
            IsEnabled = false;
            if (VistaSecurity.IsVista)
            {
                PushCheckBox.Changed -= UserActivity_Update;
                PushRadioButton.Changed -= UserActivity_Update;
#if DEBUG
                HookManager.HotKeyExit -= OnHotKeyExit;
#endif
                hookManager.UnsubscribeFromGlobalKeyboardEvents();
            }
        }

#if DEBUG
        public static void OnHotKeyExit(object sender, EventArgs args)
        {
            App.SwitchToDefaultDesktopOnClose = true;
            App.ShutdownApplication();
        }
#endif

        public static void ShowTaskbar(bool show)
        {
            int showValue = (show ? WinBase.SW_SHOW : WinBase.SW_HIDE);
            IntPtr taskBarWnd = User32.FindWindow("Shell_TrayWnd", null);
            IntPtr startButton = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);
            User32.ShowWindow(taskBarWnd, showValue);
            User32.ShowWindow(startButton, showValue);
        }

    }
}
