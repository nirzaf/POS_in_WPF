using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PosControls.Helpers;
using PosControls.Interfaces;
using PosControls.Types;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for CustomTextBox.xaml
    /// </summary>
    public partial class CustomTextBox : UserControl
    {
        #region Licensed Access Only
        static CustomTextBox()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(CustomTextBox).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        #region Fields
        private int changeCount = 0;
        private int touchOpenDelay = 0;
        private string touchPadTitleText = "Edit Text";
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private CustomTextBoxType promptType = CustomTextBoxType.Keyboard;
        private bool useKeyboardContextMenu = false;
        private int maxTextLength = 0;
        private int caretIndex = -1;
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        [Obfuscation(Exclude = true)]
        public event EventHandler CommitEdit;
        [Obfuscation(Exclude = true)]
        public event EventHandler ContextMenuInitialized;
        #endregion

        #region Properties
        public bool UseMultipleLines
        {
            get { return (textBlock.TextWrapping == TextWrapping.Wrap); }
            set
            {
                textBlock.TextWrapping = (value ? TextWrapping.Wrap : TextWrapping.NoWrap);
            }
        }

        public new ContextMenu ContextMenu
        {
            get { return borderControl.ContextMenu; }
            set { }
        }

        public bool UseContextMenuEditing
        {
            get { return useKeyboardContextMenu; }
            set
            {
                useKeyboardContextMenu = value;
                borderControl.ContextMenu = null;
            }
        }

        public TextWrapping TextWrapping
        {
            get;
            set;
        }

        public int MaxLength
        {
            get { return maxTextLength; }
            set
            {
                maxTextLength = value;                
                if ((borderControl.ContextMenu != null) &&
                    borderControl.ContextMenu.IsOpen)
                    borderControl.ContextMenu.IsOpen = false;
                borderControl.ContextMenu = null;
            }
        }

        public char? TextMaskCharacter
        {
            get;
            set;
        }

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
            "TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomTextBox));
        [Obfuscation(Exclude = true)]

        // Provide CLR accessors for the event
        [Obfuscation(Exclude = true)]
        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            "TextProperty", typeof(string), typeof(CustomTextBox),
            new UIPropertyMetadata(null,
                new PropertyChangedCallback(TextValueChanged),
                new CoerceValueCallback(TextCoerceValue)));

        protected static object TextCoerceValue(DependencyObject depObject, object value)
        {
            CustomTextBox myClass = (CustomTextBox)depObject;
            string newValue = (string)value;
            if ((newValue != null) && (myClass.MaxLength > 0) &&
                (newValue.Length > myClass.MaxLength))
                return newValue.Substring(0, myClass.MaxLength);
            return value;
        }

        protected static void TextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox myClass = (CustomTextBox)d;
            string newValue = (string)e.NewValue;
            string oldValue = (string)e.OldValue;
            if (myClass.TextMaskCharacter != null)
            {
                myClass.textBlock.Text = "";
                for (int i = 0; i < newValue.Length; i++)
                    myClass.textBlock.Text += myClass.TextMaskCharacter;
            }
            else
                myClass.textBlock.Text = newValue;
            if (myClass.PromptType == CustomTextBoxType.PhoneNumber)
                myClass.SetPhoneNumber(newValue);
            if (myClass.UseContextMenuEditing)
                myClass.UpdateCaret();
            myClass.changeCount++;
           
            if ((myClass.PromptType == CustomTextBoxType.PhoneNumber) && 
                (myClass.borderControl.ContextMenu != null) &&
                (newValue.Length == 10) && myClass.CaretIndex >= 9)
                myClass.borderControl.ContextMenu.IsOpen = false;

            myClass.OnTextValueChanged(oldValue, newValue);
        }

        protected void OnTextValueChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> newEventArgs =
                new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            newEventArgs.RoutedEvent = CustomTextBox.TextChangedEvent;
            RaiseEvent(newEventArgs);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty EnabledForegroundBrushProperty =
            DependencyProperty.Register(
            "EnabledForegroundBrushProperty", typeof(Brush), typeof(CustomTextBox),
            new UIPropertyMetadata(Brushes.Transparent,
                new PropertyChangedCallback(EnabledForegroundBrushValueChanged)));

        protected static void EnabledForegroundBrushValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox myClass = (CustomTextBox)d;
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            myClass.InitializeBrushes();
        }

        public Brush EnabledForeground
        {
            get { return (Brush)GetValue(EnabledForegroundBrushProperty); }
            set { SetValue(EnabledForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty DisabledForegroundBrushProperty =
            DependencyProperty.Register(
            "DisabledForegroundBrushProperty", typeof(Brush), typeof(CustomTextBox),
            new UIPropertyMetadata(Brushes.Transparent,
                new PropertyChangedCallback(DisabledForegroundBrushValueChanged)));

        protected static void DisabledForegroundBrushValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox myClass = (CustomTextBox)d;
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            myClass.InitializeBrushes();
        }

        public Brush DisabledForeground
        {
            get { return (Brush)GetValue(DisabledForegroundBrushProperty); }
            set { SetValue(DisabledForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty EnabledBackgroundBrushProperty =
            DependencyProperty.Register(
            "EnabledBackgroundBrushProperty", typeof(Brush), typeof(CustomTextBox),
            new UIPropertyMetadata(Brushes.Transparent,
                new PropertyChangedCallback(EnabledBackgroundBrushValueChanged)));

        protected static void EnabledBackgroundBrushValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox myClass = (CustomTextBox)d;
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            myClass.InitializeBrushes();
        }

        public Brush EnabledBackground
        {
            get { return (Brush)GetValue(EnabledBackgroundBrushProperty); }
            set { SetValue(EnabledBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundBrushProperty =
            DependencyProperty.Register(
            "DisabledBackgroundBrushProperty", typeof(Brush), typeof(CustomTextBox),
            new UIPropertyMetadata(Brushes.Transparent,
                new PropertyChangedCallback(DisabledBackgroundBrushValueChanged)));

        protected static void DisabledBackgroundBrushValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox myClass = (CustomTextBox)d;
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            myClass.InitializeBrushes();
        }

        public int CaretIndex
        {
            get { return caretIndex; }
            set
            {
                caretIndex = value;
            }
        }

        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundBrushProperty); }
            set { SetValue(DisabledBackgroundBrushProperty, value); }
        }

        public ShiftMode KeyboardShiftMode
        {
            get;
            set;
        }

        public int TouchOpenDelay
        {
            get { return touchOpenDelay; }
            set
            {
                touchOpenDelay = value;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, value);
            }
        }

        public string KeyboardTitleText
        {
            get { return touchPadTitleText; }
            set { touchPadTitleText = value; }
        }

        public CustomTextBoxType PromptType
        {
            get { return promptType; }
            set
            {
                CustomTextBoxType oldType = promptType;
                promptType = value;
                if (oldType == promptType)
                    return;
                if (Text != null)
                {
                    if (promptType == CustomTextBoxType.Percentage)
                        SetPercentagePromptType();
                    if (promptType == CustomTextBoxType.FloatNumeric)
                        SetFloatNumericPromptType();
                    if (promptType == CustomTextBoxType.Currency)
                        SetCurrencyPromptType();
                }
                SetPhoneNumberDisplay((promptType == CustomTextBoxType.PhoneNumber));
            }
        }
        #endregion

        #region PromptType helper methods
        private void SetCurrencyPromptType()
        {
            double amount = 0;
            bool wasPercentage = Text.Contains("%");
            try
            {
                amount = Convert.ToDouble(Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace("%", ""));
            }
            catch { }
            if (wasPercentage)
                amount = amount / 100;
            Text = amount.ToString("C2");
        }

        private void SetFloatNumericPromptType()
        {
            double amount = 0;
            try
            {
                amount = Convert.ToDouble(Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace("%", ""));
            }
            catch { }
            Text = amount.ToString("F2");
        }

        private void SetPercentagePromptType()
        {
            double amount = 0;
            try
            {
                amount = Convert.ToDouble(Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace("%", ""));
            }
            catch { }
            Text = (amount * 100).ToString("F1") + "%";
        }
        #endregion

        #region Initialization
        public CustomTextBox()
        {
            InitializeComponent();
           
            InitializeBrushes();
            textBlock.Background = Brushes.Transparent;

            // Bindings
            Binding binding = new Binding();
            binding.Source = ConfigurationManager.BorderBrush;
            BindingOperations.SetBinding(borderControl, Border.BorderBrushProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.TextboxForegroundBrush;
            BindingOperations.SetBinding(this, CustomTextBox.EnabledForegroundBrushProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledTextboxForegroundBrush;
            BindingOperations.SetBinding(this, CustomTextBox.DisabledForegroundBrushProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.TextboxBackgroundBrush;
            BindingOperations.SetBinding(this, CustomTextBox.EnabledBackgroundBrushProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledTextboxBackgroundBrush;
            BindingOperations.SetBinding(this, CustomTextBox.DisabledBackgroundBrushProperty, binding);

            borderControl.BorderThickness = new Thickness(1);
            borderControl.CornerRadius = new CornerRadius(4);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, TouchOpenDelay);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            IsEnabledChanged += CustomTextBox_IsEnabledChanged;
        }

        private void InitializeBrushes()
        {
            Background = (IsEnabled ? EnabledBackground : DisabledBackground);
            textBlock.Foreground =
                Foreground = (IsEnabled ? EnabledForeground : DisabledForeground);
        }
        #endregion

        #region Phone Number Specific Methods
        private void SetPhoneNumberDisplay(bool showPhoneNumber)
        {
            if (showPhoneNumber)
                MaxLength = 10;
            textBlock.Visibility = 
                (showPhoneNumber ? Visibility.Collapsed : Visibility.Visible);
            phoneNumberStackPanel.Visibility =
                (showPhoneNumber ? Visibility.Visible : Visibility.Collapsed);
            if (PromptType == CustomTextBoxType.PhoneNumber)
                SetPhoneNumberLines();
        }

        private void SetPhoneNumberLines()
        {
            if (Text == null)
            {
                phoneLine1.Visibility = Visibility.Collapsed;
                phoneLine2.Visibility = Visibility.Collapsed;
                return;
            }
            phoneLine1.Visibility =
                (Text.Length > 3) ? Visibility.Visible : Visibility.Collapsed;
            phoneLine2.Visibility =
                (Text.Length > 6) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Called when Text property changes and PromptType == PhoneNumber
        private void SetPhoneNumber(string newValue)
        {
            areaCodeText1.Text =
                (((newValue != null) && (newValue.Length > 0)) ? "" + newValue[0] : "");
            areaCodeText2.Text =
                (((newValue != null) && (newValue.Length > 1)) ? "" + newValue[1] : "");
            areaCodeText3.Text =
                (((newValue != null) && (newValue.Length > 2)) ? "" + newValue[2] : "");
            phoneText1.Text =
                (((newValue != null) && (newValue.Length > 3)) ? "" + newValue[3] : "");
            phoneText2.Text =
                (((newValue != null) && (newValue.Length > 4)) ? "" + newValue[4] : "");
            phoneText3.Text =
                (((newValue != null) && (newValue.Length > 5)) ? "" + newValue[5] : "");
            phoneText4.Text =
                (((newValue != null) && (newValue.Length > 6)) ? "" + newValue[6] : "");
            phoneText5.Text =
                (((newValue != null) && (newValue.Length > 7)) ? "" + newValue[7] : "");
            phoneText6.Text =
                (((newValue != null) && (newValue.Length > 8)) ? "" + newValue[8] : "");
            phoneText7.Text =
                (((newValue != null) && (newValue.Length > 9)) ? "" + newValue[9] : "");
            SetPhoneNumberLines();
        }
        #endregion

        #region Caret Handling
        /// <summary>
        /// Set the visibility of the caret line
        /// </summary>
        /// <param name="show"></param>
        private void ShowCaret(bool show)
        {
            if ((PromptType == CustomTextBoxType.Currency) ||
                (PromptType == CustomTextBoxType.Percentage))
            {
                caretLine.Visibility = Visibility.Hidden;
                return;
            }
            if (show)
                UpdateCaret();
            caretLine.Visibility =
                (show ? Visibility.Visible : Visibility.Hidden);
        }

        /// <summary>
        /// Called anytime a context menu editing control wants to update the caret position.
        /// </summary>
        private void UpdateCaret()
        {
            if (testEvent != null)
                testEvent.Invoke(this, new EventArgs());
            if (Text != null)
                CaretIndex = CaretIndex.Clamp(0, Text.Length);
            if ((Text == null) || ((Text != null) && (Text.Length == 0)))
                CaretIndex = 0;
            if (CaretIndex == 0)
            {
                MoveCaret(0);
                return;
            }
            double hyphenLength = 0;
            if (MeasureText("-", out hyphenLength))
            {
                double lengthToCaret = 0;
                if (MeasureText(textBlock.Text.Substring(0, CaretIndex), out lengthToCaret))
                {
                    if (PromptType == CustomTextBoxType.PhoneNumber)
                        lengthToCaret +=
                            ((CaretIndex > 3) ? hyphenLength - 2 : 0) +
                            ((CaretIndex > 6) ? hyphenLength - 1 : 0);
                    MoveCaret(lengthToCaret);
                }
            }
        }

        private bool MeasureText(string text, out double width)
        {
            width = 0;
            Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle,
                    textBlock.FontWeight, textBlock.FontStretch);
            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                return false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                ushort glyph = glyphTypeface.CharacterToGlyphMap[ch];
                width += glyphTypeface.AdvanceWidths[glyph];
            }
            width *= textBlock.FontSize;
            return true;
        }

        private void MoveCaret(double xPos)
        {
            caretLine.X1 =
                caretLine.X2 =
                (xPos + 5); 
            // +5 is for margin differences
        }

        private void ChangeSelectedIndex(MouseButtonEventArgs e)
        {
            if (Text == null)
            {
                SetCaretIndex(0);
                ShowCaret(true);
                return;
            }
            Point pt = MouseUtilities.CorrectGetPosition(this);
            if (PromptType == CustomTextBoxType.PhoneNumber)
                ChangeSelectedIndexPhoneNumber(pt.X);
            else
                ChangeSelectedIndexNormally(pt.X);
            ShowCaret(true);
        }

        private void ChangeSelectedIndexPhoneNumber(double offset)
        {
            double hyphenLength = 0;
            if (!MeasureText("-", out hyphenLength))
                return;
            
            for (int i = 1; i <= Text.Length; i++)
            {
                double lengthToCaret = 0;
                if (MeasureText(textBlock.Text.Substring(0, i), out lengthToCaret))
                {
                    if (i > 3)
                        lengthToCaret += hyphenLength - 2;
                    if (i > 6)
                        lengthToCaret += (hyphenLength * 2) - 1;
                    if (offset < lengthToCaret)
                    {
                        i--;
                        if (MeasureText(textBlock.Text.Substring(0, i), out lengthToCaret))
                        {
                            if (i > 3)
                                lengthToCaret += hyphenLength - 2;
                            if (i > 6)
                                lengthToCaret += (hyphenLength * 2) - 1;
                            SetCaretIndex(i, false);
                            MoveCaret(lengthToCaret);
                        }
                        return;
                    }
                    if (i == Text.Length)
                    {
                        SetCaretIndex(i);
                        break;
                    }
                }
            }
        }

        private void ChangeSelectedIndexNormally(double offset)
        {
            for (int i = 1; i <= Text.Length; i++)
            {
                double lengthToCaret = 0;
                if (MeasureText(textBlock.Text.Substring(0, i), out lengthToCaret))
                {
                    if (offset < lengthToCaret)
                    {
                        SetCaretIndex(i - 1);
                        break;
                    }
                    if (i == Text.Length)
                    {
                        SetCaretIndex(i);
                        break;
                    }
                }
            }
        }

        private void SetCaretIndex(int value, bool update = true)
        {
            CaretIndex = value;
            if (borderControl.ContextMenu.Tag.Equals("Keyboard"))
                GetKeyboardControl(borderControl.ContextMenu).textBoxLine.CaretIndex = value;
            else if (borderControl.ContextMenu.Tag.Equals("NumberPad"))
                GetNumberPadControl(borderControl.ContextMenu).textBoxValue.CaretIndex = value;
            if (update)
                UpdateCaret();
        }

        [Obfuscation(Exclude = true)]
        void Keyboard_CaretUpdateNeeded(object sender, EventArgs e)
        {
            CaretIndex = (sender as KeyboardEntryControl).textBoxLine.CaretIndex;
            UpdateCaret();
        }

        [Obfuscation(Exclude = true)]
        void NumberPad_CaretUpdateNeeded(object sender, EventArgs e)
        {
            CaretIndex = (sender as NumberEntryControl).textBoxValue.CaretIndex;
            UpdateCaret();
        }
        #endregion

        #region Context Menu Related Methods
        private void SetKeyboardContextMenu()
        {
            
            ControlTemplate controlTemplate =
                Resources.GetControlTemplate("keyboardControlTemplate");
            if (controlTemplate != null)
            {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Tag = "Keyboard";
                contextMenu.Template = controlTemplate;
                contextMenu.Placement = PlacementMode.Bottom;
                contextMenu.PlacementTarget = borderControl;
                contextMenu.LostKeyboardFocus += Keyboard_LostKeyboardFocus;
                contextMenu.GotKeyboardFocus += Keyboard_GotKeyboardFocus;
                borderControl.ContextMenu = contextMenu;
                contextMenu.ApplyTemplate();
                KeyboardEntryControl control =
                    GetKeyboardControl(contextMenu);
                control.MaxLength = MaxLength;
                control.CompactModeOwner = this;
                control.MaxLength = MaxLength;
                control.CaretUpdateNeeded += Keyboard_CaretUpdateNeeded;
            }
        }

                [Obfuscation(Exclude = true)]
        EventHandler testEvent;
                [Obfuscation(Exclude = true)]
        public void SetNotifyCaretUpdate(EventHandler handler)
        {
            testEvent = handler;
        }

        private void SetNumberPadContextMenu()
        {
            ControlTemplate controlTemplate =
                Resources.GetControlTemplate("numberPadControlTemplate");
            if (controlTemplate != null)
            {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Tag = "NumberPad";
                contextMenu.Template = controlTemplate;
                contextMenu.Placement = PlacementMode.Bottom;
                contextMenu.PlacementTarget = borderControl;
                contextMenu.Opened += NumberPad_ContextMenu_Opened;
                contextMenu.Closed += NumberPad_ContextMenu_Closed;
                borderControl.ContextMenu = contextMenu;
                contextMenu.ApplyTemplate();
                NumberEntryControl control = GetNumberPadControl(contextMenu);
                control.CompactModeOwner = this;
                control.MaxLength = MaxLength;
                control.CaretUpdateNeeded +=
                    NumberPad_CaretUpdateNeeded;
            }
        }

        public KeyboardEntryControl GetKeyboardControl()
        {
            return GetKeyboardControl(ContextMenu);
        }

        private KeyboardEntryControl GetKeyboardControl(DependencyObject parentDependencyObject)
        {
            if (parentDependencyObject == null)
                return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
            {
                DependencyObject depObject = VisualTreeHelper.GetChild(parentDependencyObject, i);
                if (depObject is KeyboardEntryControl)
                    return depObject as KeyboardEntryControl;
                if (VisualTreeHelper.GetChildrenCount(depObject) > 0)
                {
                    KeyboardEntryControl childFound = GetKeyboardControl(depObject);
                    if (childFound != null)
                        return childFound;
                }
            }
            return null;
        }

        public KeyboardEntryControl GetNumberPadControl()
        {
            return GetKeyboardControl(ContextMenu);
        }

        private NumberEntryControl GetNumberPadControl(DependencyObject parentDependencyObject)
        {
            if (parentDependencyObject == null)
                return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
            {
                DependencyObject depObject = VisualTreeHelper.GetChild(parentDependencyObject, i);
                if (depObject is NumberEntryControl)
                    return depObject as NumberEntryControl;
                if (VisualTreeHelper.GetChildrenCount(depObject) > 0)
                {
                    NumberEntryControl childFound = GetNumberPadControl(depObject);
                    if (childFound != null)
                        return childFound;
                }
            }
            return null;
        }
        #endregion

        #region Event Handling
        [Obfuscation(Exclude = true)]
        void borderControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPhoneNumberLines();
        }

        // Context menu is now opened
        [Obfuscation(Exclude = true)]
        private void Keyboard_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            KeyboardEntryControl keyboard = GetKeyboardControl(borderControl.ContextMenu);
            CaretIndex = keyboard.textBoxLine.CaretIndex;
            UpdateCaret();
            changeCount = 0;
            //ShowCaret(true);
        }

        // Context menu is now opened
        [Obfuscation(Exclude = true)]
        void NumberPad_ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            NumberEntryControl numPad = GetNumberPadControl(borderControl.ContextMenu);
            numPad.SetCaretIndexToEnd();
            CaretIndex = numPad.textBoxValue.CaretIndex;
            UpdateCaret();
            changeCount = 0;
            //ShowCaret(true);
            if (PromptType == CustomTextBoxType.IntegerNumeric)
                numPad.SetCaretIndexToEnd();
        }

        // Context menu is now closed
        [Obfuscation(Exclude = true)]
        private void Keyboard_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ShowCaret(false);
            DoCommitEditEvent();
        }

        private void DoCommitEditEvent()
        {
            if ((changeCount > 0) && (CommitEdit != null))
                CommitEdit.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        private void borderControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UseContextMenuEditing)
            {
                if ((PromptType == CustomTextBoxType.Keyboard) ||
                    (PromptType == CustomTextBoxType.Password))
                {
                    SetKeyboardContextMenu();
                    ContextMenu contextMenu = borderControl.ContextMenu;
                    KeyboardEntryControl keyboard = GetKeyboardControl(contextMenu);
                    if (PromptType == CustomTextBoxType.Password)
                        keyboard.UsePasswordTextField = true;
                    if (keyboard != null)
                        keyboard.OriginalText = Text;
                    contextMenu.IsOpen = true;
                    ChangeSelectedIndex(e);
                    e.Handled = true;
                }
                else if ((PromptType == CustomTextBoxType.Currency) ||
                    (PromptType == CustomTextBoxType.IntegerNumeric) ||
                    (PromptType == CustomTextBoxType.Percentage) ||
                    (PromptType == CustomTextBoxType.FloatNumeric) ||
                    (PromptType == CustomTextBoxType.PhoneNumber))
                {
                    SetNumberPadContextMenu();
                    ContextMenu contextMenu = borderControl.ContextMenu;
                    NumberEntryControl numPad = GetNumberPadControl(contextMenu);
                    numPad.UseDecimalPoint =
                        ((PromptType == CustomTextBoxType.FloatNumeric) ||
                        (PromptType == CustomTextBoxType.Percentage));
                    numPad.DisplayAsPercentage =
                        (PromptType == CustomTextBoxType.Percentage);
                    numPad.DisplayAsCurrency =
                        (PromptType == CustomTextBoxType.Currency);
                    numPad.buttonBackspace.IsEnabled =
                        ((PromptType == CustomTextBoxType.IntegerNumeric) ||
                        (PromptType == CustomTextBoxType.FloatNumeric) ||
                        (PromptType == CustomTextBoxType.PhoneNumber));
                    numPad.OriginalText = Text;
                    contextMenu.IsOpen = true;
                    ChangeSelectedIndex(e);
                    e.Handled = true;
                }
            }
        }

        [Obfuscation(Exclude = true)]
        void NumberPad_ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if ((PromptType == CustomTextBoxType.Percentage) && (Text != null))
            {
                Text = Text.Replace(".%", ".0%");
                if (!Text.Contains("."))
                    Text = Text.Replace("%", ".0%");
            }
            if ((borderControl.ContextMenu == null) || !borderControl.ContextMenu.IsOpen)
                ShowCaret(false);
            ContextMenu contextMenu = borderControl.ContextMenu;
            NumberEntryControl numPad = GetNumberPadControl(contextMenu);
            if (numPad.OriginalText != numPad.Text)
                DoCommitEditEvent();
        }

        [Obfuscation(Exclude = true)]
        private void KeyboardBorder_Initialized(object sender, EventArgs e)
        {
            Border border = (sender as Border);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(4);
            border.Background = ConfigurationManager.ApplicationBackgroundBrush;
            border.BorderBrush = ConfigurationManager.BorderBrush;
            KeyboardEntryControl keyboard = border.Child as KeyboardEntryControl;
            keyboard.CompactModeOwner = this;
            keyboard.ShiftMode =  KeyboardShiftMode;
            if (ContextMenuInitialized != null)
                ContextMenuInitialized.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        private void NumberPadBorder_Initialized(object sender, EventArgs e)
        {
            Border border = (sender as Border);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(4);
            border.Background = ConfigurationManager.ApplicationBackgroundBrush;
            border.BorderBrush = ConfigurationManager.BorderBrush;
            NumberEntryControl numberPad = border.Child as NumberEntryControl;
            numberPad.CompactModeOwner = this;
            numberPad.SetCaretIndex(CaretIndex);
            if (ContextMenuInitialized != null)
                ContextMenuInitialized.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        void CustomTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InitializeBrushes();
        }
        #endregion

        #region Prompted Editing
        private void StartTimer()
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
        }

        private void StopTimer()
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
            }
        }

        [Obfuscation(Exclude = true)]
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            StopTimer();
            ShowNow();
        }

        private void ShowNow()
        {
            switch (PromptType)
            {
                case CustomTextBoxType.FloatNumeric:
                    PromptFloat();
                    break;
                case CustomTextBoxType.IntegerNumeric:
                    PromptInteger();
                    break;
                case CustomTextBoxType.Currency:
                    PromptCurrency();
                    break;
                case CustomTextBoxType.Keyboard:
                    PromptKeyboard(false);
                    break;
                case CustomTextBoxType.Password:
                    PromptKeyboard(true);
                    break;
                case CustomTextBoxType.Percentage:
                    PromptPercentage();
                    break;
                case CustomTextBoxType.PhoneNumber:
                    PromptPhoneNumber();
                    break;
            }
        }

        private void PromptPhoneNumber()
        {
            throw new NotImplementedException();
        }

        private void PromptPercentage()
        {

            double? currentValue = null;
            try
            {
                currentValue = Convert.ToDouble(Text.Replace("%", ""));
            }
            catch
            {
            }
            double? prompt = PosDialogWindow.PromptPercentage(this, KeyboardTitleText, (double?)null);
            if (prompt != null && prompt.HasValue)
                Text = "" + (prompt.Value * 100).ToString("F1") + "%";
        }

        private void PromptCurrency()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = true;
            double? prompt = PosDialogWindow.PromptCurrency(KeyboardTitleText, (double?)null);
            if ((prompt != null) && prompt.HasValue)
                Text = prompt.Value.ToString("C2");
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = false;
        }

        private void PromptFloat()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = true;
            double? currentValue = null;
            try
            {
                currentValue = Convert.ToDouble(Text);
            }
            catch
            {
            }
            double? newValue = PosDialogWindow.PromptNumber(KeyboardTitleText, currentValue);
            if ((newValue != null) && newValue.HasValue)
            {
                Text = "" + newValue.Value;
            }
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = false;
        }

        private void PromptInteger()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = true;
            int? currentValue = null;
            try
            {
                currentValue = Convert.ToInt32(Text);
            }
            catch
            {
            }
            int? newValue = PosDialogWindow.PromptNumber(KeyboardTitleText, currentValue);
            if ((newValue != null) && newValue.HasValue)
                Text = "" + newValue;
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = false;
        }

        private void PromptKeyboard(bool isPassword)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = true;
            string prompt = PosDialogWindow.PromptKeyboard(KeyboardTitleText, Text, isPassword, KeyboardShiftMode);
            if (prompt != null)
                Text = prompt;
            if (parentWindow is IShadeable)
                ((IShadeable)parentWindow).ShowShadingOverlay = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!UseContextMenuEditing)
                StopTimer();
            base.OnMouseLeave(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!UseContextMenuEditing)
                StartTimer();
            base.OnPreviewMouseDown(e);
        }


        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!UseContextMenuEditing)
            {
                ShowNow();
                e.Handled = true;
            }
            else
                base.OnPreviewMouseDoubleClick(e);
        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (!UseContextMenuEditing)
                StopTimer();
            base.OnPreviewMouseUp(e);
        }
        #endregion

        #region MouseUtilities
        public static class MouseUtilities
        {
            public static Point CorrectGetPosition(Visual relativeTo)
            {
                Win32Point w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);
                return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct Win32Point
            {
                public Int32 X;
                public Int32 Y;
            };

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(ref Win32Point pt);
        }
        #endregion
    }
}
