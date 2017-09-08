using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;

namespace TemposLibrary
{
    public partial class HookManager
    {
        #region Licensed Access Only
        static HookManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(HookManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        public static bool HandleAllKeys
        {
            get;
            set;
        }

        #region Keyboard Hook
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private HookProc s_KeyboardDelegate;

        /// <summary>
        /// Stores the handle to the Keyboard hook procedure.
        /// </summary>
        private static int s_KeyboardHookHandle;

        /// <summary>
        /// A callback function which is called every time keyboard activity is detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;

            if (nCode >= 0)
            {
#if DEBUG
                //read structure KeyboardHookStruct at lParam
                KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                ProcessKey(new KeyPressChange(keyboardHookStruct.VirtualKeyCode,
                    !(wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)));
#endif
                if (HandleAllKeys)
                    handled = true;
            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                //    return -1;
                return 1;

            //forward to other application
            return CallNextHookEx(s_KeyboardHookHandle, nCode, wParam, lParam);
        }

#if DEBUG
        public static event EventHandler HotKeyExit;
        private static void DoHotKeyExitEvent()
        {
            if (HotKeyExit != null)
                HotKeyExit.Invoke(null, new EventArgs());
        }
        private static void ProcessKey(KeyPressChange keyEvent)
        {
            if ((keyEvent.Key == VirtualKeys.Escape) &&
                (keyEvent.IsKeyPressed))
                DoHotKeyExitEvent();
        }

#endif

        public void SubscribeToGlobalKeyboardEvents()
        {
            // install Keyboard hook only if it is not installed and must be installed
            if (s_KeyboardHookHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                s_KeyboardDelegate = KeyboardHookProc;

                //install hook
                s_KeyboardHookHandle = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    s_KeyboardDelegate,
                    IntPtr.Zero,
                    0);

                //If SetWindowsHookEx fails.
                if (s_KeyboardHookHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }

                // For Windows 7
                GC.KeepAlive(s_KeyboardDelegate);
                GC.KeepAlive(s_KeyboardHookHandle);
            }
        }

        public void UnsubscribeFromGlobalKeyboardEvents()
        {
            if (s_KeyboardHookHandle != 0)
            {
                //uninstall hook
                int result = UnhookWindowsHookEx(s_KeyboardHookHandle);
                //reset invalid handle
                s_KeyboardHookHandle = 0;
                //Free up for GC
                s_KeyboardDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }
        #endregion

#if NOT_DEFINED
        private static bool HandleKeyDown(KeyboardHookStruct keyboardHookStruct)
        {
            bool found = false;
            bool handled = false;
            bool keyPressIsUpEvent = false;
            Keys keyPressSurpressionKey = Keys.None;
            Keys keyPressKey = Keys.None;
            Keys keyData = (Keys)keyboardHookStruct.VirtualKeyCode;
            localKeyMap[((int)keyData & 0xFF)] = SetBit(0x80, localKeyMap[((int)keyData & 0xFF)]);

            try
            {
                KeyQueueEntry entry = KeyQueue.Peek();
                keyPressSurpressionKey = entry.suppressionKey;
                keyPressIsUpEvent = entry.isUpEvent;
                keyPressKey = entry.key;
                if (keyData == entry.suppressionKey)
                {
                    // Supresses repeating keystrokes while in a beginkeypress
                    // Only down events repeat like this
                    handled = true;
                }
                found = true;
            }
            catch { }

            // Handle and generated key first, we don't want the events from
            // those EVER! It will cause recursive issues.
            if (found)
            {
                ReportMessage("KeyDown-DATA: " + keyData);
                ReportMessage("KeyDown-KEY: " + keyPressKey);
                if ((KeyPressPhase == KeyPressPhases.WaitInitialize) && (AddModifiers(Keys.None) == KeyPressModifiers))
                {
                    KeyPressPhase = KeyPressPhases.DoKey;
                    KeyQueue.NextCallback = DoKeyPress;
                    return false;
                }
                else if (KeyPressPhase == KeyPressPhases.WaitKey)
                {
                    if ((AddModifiers(Keys.None) == KeyPressModifiers) &&
                        ((Keys)((int)keyboardHookStruct.VirtualKeyCode & 0xFF) == keyPressKey) &&
                        !keyPressIsUpEvent)
                    {
                        //KeyPressPhase = KeyPressPhases.CleanUp;
                        //KeyQueue.NextCallback = CleanUpKeyPress;
                    }
                    return false;
                }
                else if ((KeyPressPhase == KeyPressPhases.WaitCleanUp) && (Control.ModifierKeys == KeyPressRestoreModifiers))
                {
                    KeyPressPhase = KeyPressPhases.Completed;
                    ProtectedInvoke(s_CompletedKeyPress, IntPtr.Zero,
                        new KeyEventArgs(keyPressKey));
                    KeyQueue.NextCallback = DoNextQueue;
                    return false;
                }
            }

            if (!handled)
            {
                bool isModifierKey = ((keyData == Keys.LControlKey) ||
                                      (keyData == Keys.RControlKey) ||
                                      (keyData == Keys.LMenu) ||
                                      (keyData == Keys.RMenu) ||
                                      (keyData == Keys.LShiftKey) ||
                                      (keyData == Keys.RShiftKey) ||
                                      IsWindowsKeyDown);
                //keyData = AddModifiers(keyData);
                if (IsWindowsKeyDown)
                    keyData |= (Keys)((int)Control.ModifierKeys | (int)KeyModifier.System);
                else
                    keyData |= Control.ModifierKeys;
                KeyEventArgs e = new KeyEventArgs(keyData);

                // Invoke handlers
                if ((keyPressSurpressionKey == Keys.None) || !isModifierKey)
                    ProtectedInvoke(s_KeyDown, null, e);
                handled = e.Handled;
                if (!handled)
                {
                    if (((keyPressSurpressionKey == Keys.None) || !isModifierKey))
                        ProtectedInvoke(s_ProtectedKeyDown, null, e);
                    handled = e.Handled;
                    if (!handled)
                    {
                        if (((keyPressSurpressionKey == Keys.None) || !isModifierKey))
                            ProtectedInvoke(s_SafeKeyDown, null, e);
                        handled = e.Handled;
                    }
                }
            }
            return handled;
        }

        private static bool HandleKeyPress(KeyboardHookStruct keyboardHookStruct)
        {
            bool handled = false;

            bool isDownShift = ((GetKeyState(VirtualKeys.Shift) & 0x80) == 0x80 ? true : false);
            bool isDownCapslock = (GetKeyState(VirtualKeys.Capital) != 0 ? true : false);
            byte[] keyState = new byte[256];
            GetKeyboardState(keyState);
            byte[] inBuffer = new byte[2];
#if USE_UNICODE
            int localeId = (int)LoadKeyboardLayout(LANG_GEORGIAN_S, KLF_ACTIVATE);
            if (ToUnicodeEx(MyKeyboardHookStruct.VirtualKeyCode,
                      MyKeyboardHookStruct.ScanCode,
                      keyState,
                      inBuffer,
                      2,
                      0,
                      localeId) >= 1)
            {
                UnicodeEncoding a = new UnicodeEncoding();
                string s = a.GetString(inBuffer);
                foreach (char key in s)
                {
                    //if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                    KeyPressEventArgs e = new KeyPressEventArgs(key);
                    ERROR -> s_KeyPress.Invoke(null, e);
                    handled = handled || e.Handled;
                }
            }
#else
            if (ToAscii(keyboardHookStruct.VirtualKeyCode,
                      keyboardHookStruct.ScanCode,
                      keyState,
                      inBuffer,
                      keyboardHookStruct.Flags) == 1)
            {
                char key = (char)inBuffer[0];
                if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                KeyPressEventArgs e = new KeyPressEventArgs(key);

                // Invoke handlers
                ProtectedInvoke(s_KeyPress, null, e);
                handled = handled || e.Handled;
                if (!handled)
                {
                    ProtectedInvoke(s_KeyPress_Safe, null, e);
                    handled = e.Handled;
                }
            }
#endif
            return handled;
        }

        private static bool HandleKeyUp(KeyboardHookStruct keyboardHookStruct)
        {
            bool found = false;
            bool handled = false;

            bool keyPressIsUpEvent = false;
            Keys keyPressSurpressionKey = Keys.None;
            Keys keyPressKey = Keys.None;
            Keys keyData = (Keys)keyboardHookStruct.VirtualKeyCode;
            localKeyMap[((int)keyData & 0xFF)] = ClearBit(0x80, localKeyMap[((int)keyData & 0xFF)]);

            try
            {
                KeyQueueEntry entry = KeyQueue.Peek();
                keyPressIsUpEvent = entry.isUpEvent;
                keyPressKey = entry.key;
                keyPressSurpressionKey = entry.suppressionKey;
                if (keyData == entry.suppressionKey)
                {
                    // Supresses repeating keystrokes while in a beginkeypress
                    // Only down events repeat like this
                    handled = true;
                }
                found = true;
            }
            catch { }

            if (found)
            {
                ReportMessage("KeyUp-DATA: " + keyData);
                ReportMessage("KeyUp-KEY: " + keyPressKey);
                if ((KeyPressPhase == KeyPressPhases.WaitInitialize) && (AddModifiers(Keys.None) == KeyPressModifiers))
                {
                    KeyPressPhase = KeyPressPhases.DoKey;
                    KeyQueue.NextCallback = DoKeyPress;
                    return false;
                }

                else if (KeyPressPhase == KeyPressPhases.WaitKey)
                {
                    if ((AddModifiers(Keys.None) == KeyPressModifiers) &&
                        ((Keys)((int)keyboardHookStruct.VirtualKeyCode & 0xFF) == keyPressKey) &&
                        keyPressIsUpEvent)
                    {
                        //KeyPressPhase = KeyPressPhases.CleanUp;
                        //KeyQueue.NextCallback = CleanUpKeyPress;
                    }
                    return false;
                }
                else if ((KeyPressPhase == KeyPressPhases.WaitCleanUp) && (Control.ModifierKeys == KeyPressRestoreModifiers))
                {
                    KeyPressPhase = KeyPressPhases.Completed;
                    ProtectedInvoke(s_CompletedKeyPress, (IntPtr)0x01,
                        new KeyEventArgs(keyPressKey));
                    KeyQueue.NextCallback = DoNextQueue;
                    return false;
                }
            }

            bool isModifierKey = ((keyData == Keys.LControlKey) ||
                                  (keyData == Keys.RControlKey) ||
                                  (keyData == Keys.LMenu) ||
                                  (keyData == Keys.RMenu) ||
                                  (keyData == Keys.LShiftKey) ||
                                  (keyData == Keys.RShiftKey) ||
                                  IsWindowsKeyDown);
            //keyData = AddModifiers(keyData);
            //keyData = AddModifiers(keyData);
            if (IsWindowsKeyDown)
                keyData |= (Keys)((int)Control.ModifierKeys | (int)KeyModifier.System);
            else
                keyData |= Control.ModifierKeys;
            KeyEventArgs e = new KeyEventArgs(keyData);

            // Invoke handlers
            if ((keyPressSurpressionKey == Keys.None) || !isModifierKey)
                ProtectedInvoke(s_KeyUp, null, e);
            handled = handled || e.Handled;
            if (!handled)
            {
                if (((keyPressSurpressionKey == Keys.None) || !isModifierKey))
                    ProtectedInvoke(s_ProtectedKeyUp, null, e);
                handled = e.Handled;
                if (!handled)
                {
                    if (((keyPressSurpressionKey == Keys.None) || !isModifierKey))
                        ProtectedInvoke(s_SafeKeyUp, null, e);
                    handled = e.Handled;
                }
            }

            return handled;
        }

        private static void ProtectedInvoke(Delegate handler, object sender, EventArgs args)
        {
            if (handler == null)
                return;
            Delegate[] delegates = handler.GetInvocationList();
            if (delegates == null)
                return;
            foreach (Delegate d in delegates)
            {
                try
                {
                    d.Method.Invoke(d.Target, new object[] { sender, args });
                }
                catch (TargetInvocationException ex)
                {
                    s_Exception.Invoke(sender, new ExceptionEventArgs(args, ex.InnerException));
                }
            }
        }
#endif
    }
}
