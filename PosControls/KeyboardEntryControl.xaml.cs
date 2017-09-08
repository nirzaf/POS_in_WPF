using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PosControls.Helpers;
using System.Windows.Threading;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace PosControls
{
    public enum ShiftMode
    {
        None,
        ShiftLock,
        CapsLock,
        SoftCapsLock
    }

    /// <summary>
    /// Interaction logic for KeyboardEntryControl.xaml
    /// </summary>
    public partial class KeyboardEntryControl : UserControl
    {
        #region Licensed Access Only
        static KeyboardEntryControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(KeyboardEntryControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool usePasswordTextField = false;
        private bool compactMode = false;
        private DispatcherTimer timer = new DispatcherTimer();
        private CustomTextBox compactModeOwner = null;
        private string originalText = null;

        [Obfuscation(Exclude = true)]
        [Obfuscation(Exclude = true)]
        [Obfuscation(Exclude = true)]
        public event EventHandler EnterPressed;
        [Obfuscation(Exclude = true)]
        public event EventHandler ConsoleClear;
        [Obfuscation(Exclude = true)]
        public event EventHandler CaretUpdateNeeded;

        #region Properties
        public int MaxLength
        {
            get { return textBoxLine.MaxLength; }
            set
            {
                textBoxLine.MaxLength = value;
                passwordBoxLine.MaxLength = value;
            }
        }

        public bool UseEnterEvents
        {
            get;
            set;
        }

        public CustomTextBox CompactModeOwner
        {
            get { return compactModeOwner; }
            set
            {
                compactModeOwner = value;
                if (compactModeOwner != null)
                    Text = compactModeOwner.Text;
            }
        }

        public bool CompactMode
        {
            get { return compactMode; }
            set
            {
                compactMode = value;
                if (value)
                    gridControl.RowDefinitions[0].Height =
                        new GridLength(0, GridUnitType.Pixel);
                else
                    gridControl.RowDefinitions[0].Height = 
                        new GridLength(40, GridUnitType.Pixel);
                gridControl.UpdateLayout();
            }
        }

        public bool UsePasswordTextField
        {
            get { return usePasswordTextField; }
            set
            {
                usePasswordTextField = value;
                textBoxLine.Visibility =
                    (!value ? Visibility.Visible : Visibility.Hidden);
                passwordBoxLine.Visibility =
                    (value ? Visibility.Visible : Visibility.Hidden);
                TabButton.IsEnabled =
                    LeftButton.IsEnabled =
                    RightButton.IsEnabled = !value;
                if (Buttons != null)
                    foreach (ButtonBase button in Buttons)
                    {
                        if (button is ToggleButton)
                            continue;
                        if (value)
                        {
                            button.Style =
                                (Style)button.FindResource("buttonStylePlain");
                        }
                        else
                        {
                            button.Style =
                                (Style)button.FindResource("buttonStyleNormal"); ;
                        }
                    }
            }
        }

        public ButtonBase[] Buttons
        {
            get
            {
                return new ButtonBase[] {
                    button00, button01, button02, button03, button04, button05, button06, button07, button08, button09,
                    button10, button11, button12, button13, button14, button15, button16, button17, button18, button19,
                    button20, button21, button22, button23, button24, button25, button26, button27, button28, button29,
                    button30, button31, button32, button33, button34, button35, button36, button37, button38, button39,
                    button40, button41, button42, button43, button44, button45, button46, button47, button48, button49,
                    button50, button51, button52, button53, button54, button55, button56, button57
                };
            }
        }

        public Button EnterButton
        {
            get { return button41; }
        }

        public Button CancelButton
        {
            get { return button54; }
        }

        public Button BackspaceButton
        {
            get { return button14; }
        }

        public Button ClearButton
        {
            get { return button00; }
        }

        public Button SpaceButton
        {
            get { return button55; }
        }

        public Button TabButton
        {
            get { return button15; }
        }

        public ToggleButton ShiftLockButton
        {
            get { return button29; }
        }

        public ToggleButton CapsLockButton
        {
            get { return button42; }
        }

        public ToggleButton SoftCapsLockButton
        {
            get { return button53; }
        }

        public Button LeftButton
        {
            get { return button56; }
        }

        public Button RightButton
        {
            get { return button57; }
        }

        public bool WasCanceled
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                if (UsePasswordTextField)
                    return passwordBoxLine.Password;
                return textBoxLine.Text;
            }
            set
            {
                bool isTooLong = ((value != null) && (MaxLength > 0) && (value.Length > MaxLength));
                if (isTooLong)
                    value = value.Substring(0, MaxLength);
                SetTextForBoth(value);
                if (value != null)
                {
                    textBoxLine.Select(textBoxLine.Text.Length, 0);
                }
                BackspaceButton.IsEnabled = (!string.IsNullOrEmpty(value));
            }
        }

        /// <summary>
        /// This should be initialized before IsOpen = true (in CompactMode)
        /// </summary>
        public string OriginalText
        {
            get { return originalText; }
            set
            {
                originalText = value;
                Text = value;
            }
        }

        public ContextMenu KeyboardContextMenu
        {
            get
            {
                DependencyObject depObject = VisualTreeHelper.GetParent(this);
                while (depObject != null)
                {
                    if (depObject is ContextMenu)
                        return depObject as ContextMenu;
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
                return null;
            }
        }

        private ShiftMode shiftMode;
        public ShiftMode ShiftMode
        {
            get { return shiftMode; }
            set
            {
                shiftMode = value;
                if (value == ShiftMode.SoftCapsLock)
                    SetSoftCaps(true);
                else
                {
                    SetCase((value == ShiftMode.CapsLock));
                    SetShift((value == ShiftMode.ShiftLock));
                }
                CapsLockButton.IsChecked = (value == ShiftMode.CapsLock);
                ShiftLockButton.IsChecked = (value == ShiftMode.ShiftLock);
                SoftCapsLockButton.IsChecked = (value == ShiftMode.SoftCapsLock);
            }
        }
        #endregion

        #region Initialization
        public KeyboardEntryControl()
        {
            UseEnterEvents = false;
            WasCanceled = false;
            InitializeComponent();

            DataObject.AddPastingHandler(textBoxLine, this.OnCancelCommand);
            DataObject.AddCopyingHandler(textBoxLine, this.OnCancelCommand);
            DataObject.AddPastingHandler(passwordBoxLine, this.OnCancelCommand);
            DataObject.AddCopyingHandler(passwordBoxLine, this.OnCancelCommand);

            this.Loaded += KeyboardEntryControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }

        [Obfuscation(Exclude = true)]
        void KeyboardEntryControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UsePasswordTextField)
                textBoxLine.Focus();
            else
                passwordBoxLine.Focus();
        }
        #endregion

        private void HandleEnterButton()
        {
            if (UseEnterEvents && (EnterPressed != null))
                EnterPressed.Invoke(this, new EventArgs());
            else if (!UseEnterEvents)
            {
                if (CompactMode && (KeyboardContextMenu != null))
                    KeyboardContextMenu.IsOpen = false;
                else if (!CompactMode)
                    Window.GetWindow(this).Close();
            }
        }

        [Obfuscation(Exclude = true)]
        protected void Enter_Button_Click(object sender, RoutedEventArgs e)
        {
            HandleEnterButton();
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCanceled = true;
            if (!CompactMode)
                Window.GetWindow(this).Close();
            else if (KeyboardContextMenu != null)
            {
                textBoxLine.Text = OriginalText;
                KeyboardContextMenu.IsOpen = false;
            }
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Left_Button_Click(object sender, RoutedEventArgs e)
        {
            DoLeft();
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Right_Button_Click(object sender, RoutedEventArgs e)
        {
            DoRight();
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Tab_Button_Click(object sender, RoutedEventArgs e)
        {
            int index = textBoxLine.CaretIndex;
            textBoxLine.Text = textBoxLine.Text.Insert(index, "    ");
            // Clean-up focus and textbox cursor location, it
            // not otherwise going to happen on the return;
            int position = textBoxLine.CaretIndex + 4;
            textBoxLine.Select(position, 0);
            textBoxLine.Focus();
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void ShiftLock_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isShiftLocked = (ShiftLockButton.IsChecked != null && ShiftLockButton.IsChecked.Value);
            ShiftMode = (isShiftLocked ? ShiftMode.ShiftLock : ShiftMode.None);
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void CapsLock_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isCapsLock = (CapsLockButton.IsChecked != null && CapsLockButton.IsChecked.Value);
            ShiftMode = (isCapsLock ? ShiftMode.CapsLock : ShiftMode.None);
            CleanUpButtonPress();
        }
        
        [Obfuscation(Exclude = true)]
        protected void SoftCapsLock_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isSoftCapsLock = (SoftCapsLockButton.IsChecked != null && SoftCapsLockButton.IsChecked.Value);
            ShiftMode = (isSoftCapsLock ? ShiftMode.SoftCapsLock : ShiftMode.None);
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            SetTextForBoth("");
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Backspace_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxLine.Text))
            {
                if (UsePasswordTextField)
                {
                    SetTextForBoth(
                        passwordBoxLine.Password.Substring(0,
                        passwordBoxLine.Password.Length - 1));
                }
                else
                {
                    int index = textBoxLine.CaretIndex;
                    if (textBoxLine.SelectionLength > 0)
                    {
                        textBoxLine.Text = textBoxLine.Text.Remove(textBoxLine.SelectionStart, textBoxLine.SelectionLength);
                        if (index > textBoxLine.Text.Length)
                            index = textBoxLine.Text.Length;
                        textBoxLine.Select(index, 0);
                    }
                    else if (index > 0)
                    {
                        textBoxLine.Select(index - 1, 1);
                        textBoxLine.SelectedText = "";
                    }
                }
            }
            CleanUpButtonPress();
        }

        [Obfuscation(Exclude = true)]
        protected void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonBase button = (ButtonBase)sender;                
            string ch = (button.Content as string);
            if (button == SpaceButton)
                ch = " ";
            if (UsePasswordTextField)
            {
                SetTextForBoth(passwordBoxLine.Password + ch);
            }
            else
            {
                int index = textBoxLine.CaretIndex;
                if (textBoxLine.SelectionLength > 0)
                    textBoxLine.Text = textBoxLine.Text.Remove(textBoxLine.SelectionStart, textBoxLine.SelectionLength).Insert(index, ch);
                else
                    textBoxLine.Text = textBoxLine.Text.Insert(index, ch);

                // Clean-up focus and textbox cursor location
                textBoxLine.Select(index + 1, 0);
            }
            CleanUpButtonPress();
        }

        private void CleanUpButtonPress()
        {
            if (!UsePasswordTextField)
                textBoxLine.Focus();
            else
                passwordBoxLine.Focus();
            SetSoftCaps();
            OnCaretUpdateNeeded();
        }

        private void OnCaretUpdateNeeded()
        {
            UpdateLayout();
            if (CaretUpdateNeeded != null)
                CaretUpdateNeeded.Invoke(this, new EventArgs());
        }

        private void DoLeft()
        {
            if (textBoxLine.CaretIndex > 0)
                textBoxLine.CaretIndex--;
            ButtonBase button = PointHitsButton(Mouse.GetPosition(this));
            if ((button == LeftButton) &&
                (Mouse.LeftButton == MouseButtonState.Pressed))
            {
                if (!timer.IsEnabled)
                {
                    timer.Tick += timer_Tick_Left;
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                    timer.IsEnabled = true;
                }
            }
            else
            {
                timer.Tick -= timer_Tick_Left;
                timer.IsEnabled = false;
            }
        }

        private void DoRight()
        {
            if (textBoxLine.CaretIndex < textBoxLine.Text.Length)
                textBoxLine.CaretIndex++;
            ButtonBase button = PointHitsButton(Mouse.GetPosition(this));
            if ((button == RightButton) &&
                (Mouse.LeftButton == MouseButtonState.Pressed))
            {
                if (!timer.IsEnabled)
                {
                    timer.Tick += timer_Tick_Right;
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                    timer.IsEnabled = true;
                }
            }
            else
            {
                timer.Tick -= timer_Tick_Right;
                timer.IsEnabled = false;
            }
        }

        #region Repeating Timer
        [Obfuscation(Exclude = true)]
        void timer_Tick_Right(object sender, EventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            DoRight();
        }

        [Obfuscation(Exclude = true)]
        void timer_Tick_Left(object sender, EventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            DoLeft();
        }

        [Obfuscation(Exclude = true)]
        private void button56_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            timer.Tick -= timer_Tick_Left;
            timer.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void button57_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            timer.Tick -= timer_Tick_Right;
            timer.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void button56_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Tick -= timer_Tick_Left;
            timer.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void button57_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Tick -= timer_Tick_Right;
            timer.IsEnabled = false;
        }
        #endregion

        private void SetSoftCaps()
        {
            if ((SoftCapsLockButton.IsChecked != null) && SoftCapsLockButton.IsChecked.Value)
                SetSoftCaps(true);
        }

        private void SetSoftCaps(bool isEnabled)
        {
            if (!isEnabled)
            {
                SetShift(false);
                SetCase(false);
                return;
            }

            SetShift(false);
            SetCase(CanSoftLock());
        }

        ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Soft caps locks should be enabled anytime there is a space
        /// To the left of the cursor. This is only the start... Any
        /// event that changes the caret, needs to be monitored.
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ///
        private bool CanSoftLock()
        {
            int index = textBoxLine.CaretIndex;
            //button56.Content = "" + index;

            if (String.IsNullOrEmpty(textBoxLine.Text) || (index == 0))
                return true;
            if ((index > 0) &&
                (index <= textBoxLine.Text.Length) &&
                (textBoxLine.Text[index - 1] == ' '))
                return true;

            return false;
        }

        private void SetShift(bool isShifted)
        {
            foreach (ButtonBase button in Buttons)
            {
                if ((button.Content as string).Length != 1) continue;
                if (SetShift(button, isShifted, "1", "!")) continue;
                if (SetShift(button, isShifted, "2", "@")) continue;
                if (SetShift(button, isShifted, "3", "#")) continue;
                if (SetShift(button, isShifted, "4",
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol)) continue;
                if (SetShift(button, isShifted, "5", "%")) continue;
                if (SetShift(button, isShifted, "6", "^")) continue;
                if (SetShift(button, isShifted, "7", "&")) continue;
                if (SetShift(button, isShifted, "8", "*")) continue;
                if (SetShift(button, isShifted, "9", "(")) continue;
                if (SetShift(button, isShifted, "0", ")")) continue;
                if (SetShift(button, isShifted, "-", "_")) continue;
                if (SetShift(button, isShifted, "=", "+")) continue;
                if (SetShift(button, isShifted, "`", "~")) continue;
                if (SetShift(button, isShifted, "[", "{")) continue;
                if (SetShift(button, isShifted, "]", "}")) continue;
                if (SetShift(button, isShifted, @"\", "|")) continue;
                if (SetShift(button, isShifted, ";", ":")) continue;
                if (SetShift(button, isShifted, "'", "\"")) continue;
                if (SetShift(button, isShifted, ",", "<")) continue;
                if (SetShift(button, isShifted, ".", ">")) continue;
                if (SetShift(button, isShifted, "/", "?")) continue;
            }
        }

        private bool SetShift(ButtonBase button, bool isShifted,
            string unShifted, string shifted)
        {
            string buttonText = button.Content as string;
            bool wasUnShifted = buttonText.Equals(unShifted);
            bool wasShifted = buttonText.Equals(shifted);
            if (wasUnShifted && isShifted)
                button.Content = shifted;
            else if (wasShifted && !isShifted)
                button.Content = unShifted;
            if (wasShifted || wasUnShifted)
                return true;
            return false;
        }

        private void SetCase(bool toUpper)
        {
            foreach (ButtonBase button in Buttons)
            {
                if (!(button.Content is string))
                    continue;
                string buttonText = (button.Content as string);
                if (buttonText.Length != 1)
                    continue;
                if (toUpper)
                    button.Content = buttonText.ToUpper();
                else
                    button.Content = buttonText.ToLower();
            }
        }

        [Obfuscation(Exclude = true)]
        private void passwordBoxLine_PasswordChanged(object sender, RoutedEventArgs e)
        {
            BackspaceButton.IsEnabled =
                (!string.IsNullOrEmpty(Text));
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CompactModeOwner != null)
            {
                CompactModeOwner.CaretIndex = textBoxLine.CaretIndex;
                CompactModeOwner.Text = Text;
                bool isMaxLength = ((CompactModeOwner.Text != null) && (MaxLength > 0) &&
                    (CompactModeOwner.Text.Length >= MaxLength));
                InsertionButtonsEnabled(!isMaxLength);
                BackspaceButton.IsEnabled =
                    (!string.IsNullOrEmpty(CompactModeOwner.Text));
            }
            SetSoftCaps();
            OnCaretUpdateNeeded();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //SetSoftCaps();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLine_KeyUp(object sender, KeyEventArgs e)
        {
            SetSoftCaps();
            OnCaretUpdateNeeded();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLine_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetSoftCaps();
            OnCaretUpdateNeeded();
        }

        [Obfuscation(Exclude = true)]
        private void passwordBoxLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void textBoxLine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleEnterButton();
                e.Handled = true;
            }
            OnCaretUpdateNeeded();
        }

        [Obfuscation(Exclude = true)]
        private void passwordBoxLine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleEnterButton();
                e.Handled = true;
            }
        }

        #region Helper Methods
        private void SetTextForBoth(string text)
        {
            if ((ConsoleClear != null) &&
                String.IsNullOrEmpty(textBoxLine.Text) &&
                String.IsNullOrEmpty(text))
            {
                ConsoleClear.Invoke(this, new EventArgs());
            }

            textBoxLine.Text = text;
            passwordBoxLine.Password = text;
        }

        private ButtonBase PointHitsButton(Point point)
        {
            HitTestResult elementobj = VisualTreeHelper.HitTest(this, point);
            if (elementobj == null)
                return null;

            UIElement element = elementobj.VisualHit as UIElement;
            if (element != null)
            {
                while (true)
                {
                    if (element is ButtonBase)
                    {
                        return (ButtonBase)element;
                    }
                    element = (UIElement)VisualTreeHelper.GetParent(element);
                    if (element == null)
                        return null;
                }
            }
            return null;
        }

        private void InsertionButtonsEnabled(bool isEnabled)
        {
            // 1-13, 16-28, 30-40, 43-52
            button01.IsEnabled = button02.IsEnabled = button03.IsEnabled =
                button04.IsEnabled = button05.IsEnabled = button06.IsEnabled =
                button07.IsEnabled = button08.IsEnabled = button09.IsEnabled =
                button10.IsEnabled = button11.IsEnabled = button12.IsEnabled =
                button13.IsEnabled = button16.IsEnabled = button17.IsEnabled =
                button18.IsEnabled = button19.IsEnabled = button20.IsEnabled =
                button21.IsEnabled = button22.IsEnabled = button23.IsEnabled =
                button24.IsEnabled = button25.IsEnabled = button26.IsEnabled =
                button27.IsEnabled = button28.IsEnabled = button30.IsEnabled =
                button31.IsEnabled = button32.IsEnabled = button33.IsEnabled =
                button34.IsEnabled = button35.IsEnabled = button36.IsEnabled =
                button37.IsEnabled = button38.IsEnabled = button39.IsEnabled =
                button40.IsEnabled = button43.IsEnabled = button44.IsEnabled =
                button45.IsEnabled = button46.IsEnabled = button47.IsEnabled =
                button48.IsEnabled = button49.IsEnabled = button50.IsEnabled =
                button51.IsEnabled = button52.IsEnabled = isEnabled;
        }
        #endregion

    }
}
