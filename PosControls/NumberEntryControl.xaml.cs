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
using System.Globalization;
using PosControls.Types;
using System.Reflection;
using PosControls.Helpers;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for NumberEntryControl.xaml
    /// </summary>
    public partial class NumberEntryControl : UserControl
    {
        #region Licensed Access Only
        static NumberEntryControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(NumberEntryControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool useAllButtons = false;
        private bool useDecimalPoint = false;
        private bool displayAsCurrency = false;
        private bool compactMode = false;
        private CustomTextBox compactModeOwner = null;
        private string originalText = null;

        [Obfuscation(Exclude = true)]
        public event EventHandler CaretUpdateNeeded;
        [Obfuscation(Exclude = true)]
        public event EventHandler EnterPressed;

        public int MaxLength
        {
            get { return textBoxValue.MaxLength; }
            set { textBoxValue.MaxLength = value; }
        }

        public bool FireEventOnEnter
        {
            get;
            set;
        }

        public bool UseAllButtons
        {
            get { return useAllButtons; }
            set
            {
                useAllButtons = value;
                buttonBackspace.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                buttonCancel.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                buttonClearCompact.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                buttonClear.Visibility =
                    (value ? Visibility.Collapsed : Visibility.Visible);
                if (value)
                {
                    textBoxValue.Width = 280;
                    buttonEnter.Width = 155;
                    //gridControl.RowDefinitions[0].Height =
                    //    new GridLength(0, GridUnitType.Pixel);
                }
                else
                {
                    textBoxValue.Width = 120;
                    buttonEnter.Width = 50;
                    //gridControl.RowDefinitions[0].Height =
                    //    new GridLength(40, GridUnitType.Pixel);
                }
                gridControl.UpdateLayout();
            }
        }

        public bool CompactMode
        {
            get { return compactMode; }
            set
            {
                compactMode = value;
                buttonBackspace.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                buttonCancel.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                buttonClearCompact.Visibility =
                    (value ? Visibility.Visible : Visibility.Collapsed);
                if (value)
                {
                    buttonEnter.Width = 155;
                    gridControl.RowDefinitions[0].Height =
                        new GridLength(0, GridUnitType.Pixel);
                }
                else
                {
                    buttonEnter.Width = 50;
                    gridControl.RowDefinitions[0].Height =
                        new GridLength(40, GridUnitType.Pixel);
                }
                gridControl.UpdateLayout();
            }
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

        public bool EnterButtonVisible
        {
            get { return (buttonEnter.Visibility == Visibility.Visible); }
            set
            {
                buttonEnter.Visibility =
                    (value ? Visibility.Visible : Visibility.Hidden);
            }
        }

        public bool DisplayAsCurrency
        {
            get { return displayAsCurrency; }
            set
            {
                displayAsCurrency = value;
            }
        }

        public bool DisplayAsPercentage { get; set; }

        public bool UseDecimalPoint
        {
            get { return useDecimalPoint; }
            set
            {
                useDecimalPoint = value;
                if (value)
                    button00.Text = ".";
                else
                    button00.Text = "00";
            }
        }

        public string Text
        {
            get
            {
                return textBoxValue.Text;
            }
            set
            {
                if ((value != null) && (MaxLength > 0) && (value.Length > MaxLength))
                    value = value.Substring(0, MaxLength);
                textBoxValue.Text = value;
                CheckForInsertDisable();
            }
        }

        public int? IntValue
        {
            get
            {
                try
                {
                    string stripText = Text.Replace(
                        CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                        Replace("%", "");
                    return Convert.ToInt32(stripText);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue && DisplayAsCurrency)
                    Text = "" + value.Value.ToString("C2");
                else if (value.HasValue)
                    Text = "" + value.Value;
                else if (DisplayAsCurrency)
                    Text = "$0.00";
                else
                    Text = "";
            }
        }

        public double? FloatValue
        {
            get
            {
                try
                {
                    string stripText = Text.Replace(
                        CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                        Replace("%", "");
                    return Convert.ToDouble(stripText);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if ((value == null) || !value.HasValue)
                    ClearText();
                else if (DisplayAsPercentage)
                    Text = "" + value.Value.ToString("F1") + "%";
                else if (DisplayAsCurrency)
                    Text = "" + value.Value.ToString("C2");
                else
                    Text = "" + value.Value;
            }
        }

        public NumberEntryControl()
        {
            FireEventOnEnter = false;
            InitializeComponent();
        }

        public void SetCaretIndex(int index)
        {
            if (textBoxValue.Text != null)
            {
                index = index.Clamp(0, textBoxValue.Text.Length);
                textBoxValue.Select(index, 0);
            }
            DoCaretUpdateNeededEvent();
        }

        public void SetCaretIndexToEnd()
        {
            if (textBoxValue.Text != null)
                textBoxValue.Select(textBoxValue.Text.Length, 0);
            DoCaretUpdateNeededEvent();
        }

        public ContextMenu NumberEntryContextMenu
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

        private void OnCancelButtonPressed()
        {
            textBoxValue.Text = OriginalText;
            if (!CompactMode)
                Window.GetWindow(this).Close();
            else if (NumberEntryContextMenu != null)
                NumberEntryContextMenu.IsOpen = false;
        }

        private void OnEnterButtonPressed()
        {
            ContextMenu contextMenu = NumberEntryContextMenu;
            if (contextMenu != null)
                contextMenu.IsOpen = false;
            if (FireEventOnEnter)
                if (EnterPressed != null)
                    EnterPressed.Invoke(this, new EventArgs());
                else if (!CompactMode)
                    Window.GetWindow(this).Close();
                else
                    contextMenu.IsOpen = false;
        }

        private void ClearText()
        {
            if (DisplayAsPercentage)
                Text = "0%";
            else if (DisplayAsCurrency)
            {
                Text = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "0.00";
                if (compactModeOwner != null)
                    textBoxValue.Select(textBoxValue.Text.Length, 0);
            }
            else
                Text = "";
        }

        private void EnterValue(string value)        
        {
            if (value == null)
                return;
            if (compactModeOwner != null)
            {
                int index = compactModeOwner.CaretIndex;
                if (compactModeOwner.PromptType == CustomTextBoxType.PhoneNumber)
                {
                    if (string.IsNullOrEmpty(Text) || (Text.Length == index))
                        Text += value;
                    else
                        Text = Text.Remove(index, 1).Insert(index, value);
                    textBoxValue.Select(index + 1, 0);
                }
                else if (compactModeOwner.PromptType == CustomTextBoxType.Currency)
                {
                    string subString = Text.Replace(
                            CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                            Replace(",", "").
                            Replace("%", "");
                    EnterValueDisplayAsCurrency(subString, value);
                }
                else if (compactModeOwner.PromptType == CustomTextBoxType.Percentage)
                {
                    string subString = Text.Replace(
                            CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                            Replace(",", "").
                            Replace("%", "");
                    EnterValueDisplayAsPercentage(subString, value);
                }
                else
                {
                    if (string.IsNullOrEmpty(Text) || (Text.Length == index))
                        Text += value;
                    else
                        Text = Text.Insert(index, value);
                    textBoxValue.Select(index + value.Length, 0);
                }
                return;
            }
            if (!DisplayAsCurrency && !DisplayAsPercentage)
            {
                Text += value;
                return;
            }
            string substring = Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace(",", "").
                    Replace("%", "");
            // Display as currency
            if (DisplayAsCurrency)
            {
                EnterValueDisplayAsCurrency(substring, value);
            }
            else if (DisplayAsPercentage)
            {
                EnterValueDisplayAsPercentage(substring, value);
            }
        }

        private void EnterValueDisplayAsPercentage(string substring, string value)
        {
            string text = substring + value + "%";
            if (text.StartsWith("0") && text.Length > 2)
                text = text.Substring(1);
            Text = text;
        }

        private void EnterValueDisplayAsCurrency(string substring, string value)
        {
            string[] tokens = substring.Split('.');
            string integerPart = tokens[0];
            string decimalPart = ((tokens.Length > 1) ? tokens[1] : "00");
            if (!value.Equals("00"))
            {
                integerPart += decimalPart[0];
                decimalPart = decimalPart.Substring(1) + value;
            }
            else
            {
                integerPart += decimalPart;
                decimalPart = "00";
            }
            int decimalValue = 0;
            int integerValue = 0;
            try
            {
                integerValue = Convert.ToInt32(integerPart);
            }
            catch { };
            try
            {
                decimalValue = Convert.ToInt32(decimalPart);
            }
            catch { };
            while (decimalValue > 100) { decimalValue /= 10; }
            double doubleValue = integerValue + (decimalValue / 100.0f);
            Text = doubleValue.ToString("C2");
        }

        [Obfuscation(Exclude = true)]
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxValue.Focus();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (compactModeOwner != null)
                compactModeOwner.Text = textBoxValue.Text;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (compactModeOwner == null)
                return;

            if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
            {
                int value = (int)e.Key - (int)Key.D0;
                EnterValue("" + value);
                DoCaretUpdateNeededEvent();
                e.Handled = true;
            }
            else if ((e.Key >= Key.NumPad0) && (e.Key <= Key.NumPad9))
            {
                int value = (int)e.Key - (int)Key.NumPad0;
                EnterValue("" + value);
                DoCaretUpdateNeededEvent();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                OnEnterButtonPressed();
            }
            else if (e.Key == Key.Escape)
            {
                OnCancelButtonPressed();
            }

            if ((compactModeOwner.PromptType == CustomTextBoxType.Currency) ||
                (compactModeOwner.PromptType == CustomTextBoxType.Percentage))
                return;

            if (e.Key == Key.Home)
            {
                textBoxValue.Select(0, 0);
                DoCaretUpdateNeededEvent();
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                if (textBoxValue.Text != null)
                {
                    textBoxValue.Select(textBoxValue.Text.Length, 0);
                    DoCaretUpdateNeededEvent();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                if (textBoxValue.Text != null)
                {
                    int index = (textBoxValue.CaretIndex - 1).Clamp(0, textBoxValue.Text.Length);
                    textBoxValue.Select(index, 0);
                    DoCaretUpdateNeededEvent();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (textBoxValue.Text != null)
                {
                    int index = (textBoxValue.CaretIndex + 1).Clamp(0, textBoxValue.Text.Length);
                    textBoxValue.Select(index, 0);
                    DoCaretUpdateNeededEvent();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                if ((textBoxValue.Text != null) && (textBoxValue.CaretIndex > 0))
                {
                    textBoxValue.Select(textBoxValue.CaretIndex - 1, 1);
                    textBoxValue.SelectedText = "";
                    DoCaretUpdateNeededEvent();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                if ((textBoxValue.Text != null) && (textBoxValue.CaretIndex < textBoxValue.Text.Length))
                {
                    textBoxValue.Select(textBoxValue.CaretIndex, 1);
                    textBoxValue.SelectedText = "";
                    DoCaretUpdateNeededEvent();
                }
                e.Handled = true;
            }
            CheckForInsertDisable();
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textBoxValue.Focus();
            if (sender == buttonEnter)
            {
                OnEnterButtonPressed();
                return;
            }
            else if ((sender == buttonClear) || (sender == buttonClearCompact))
            {
                ClearText();
            }
            else if (sender == buttonCancel)
            {
                OnCancelButtonPressed();
            }
            else if (sender == buttonBackspace)
            {
                if (compactModeOwner != null)
                {
                    int index = compactModeOwner.CaretIndex;
                    if (index <= 0)
                        return;
                    textBoxValue.Text = textBoxValue.Text.Remove(index - 1, 1);
                    textBoxValue.Select(index - 1, 0);
                }
                else
                {
                    int index = textBoxValue.CaretIndex;
                    if (index <= 0)
                        return;
                    textBoxValue.Text = textBoxValue.Text.Remove(index - 1, 1);
                    textBoxValue.Select(index, 0);
                }
            }
            else
            {
                TextBlockButton button = (TextBlockButton)sender;
                EnterValue(button.Text);
            }
            CheckForInsertDisable();
            DoCaretUpdateNeededEvent();
        }

        private void DoCaretUpdateNeededEvent()
        {
            if (CaretUpdateNeeded != null)
                CaretUpdateNeeded.Invoke(this, new EventArgs());
        }

        private void CheckForInsertDisable()
        {
            bool isPhonePrompt = ((compactModeOwner != null) && 
                compactModeOwner.PromptType == CustomTextBoxType.PhoneNumber);
            bool isMaxLength = (!isPhonePrompt &&
                ((Text != null) && (MaxLength > 0) && (Text.Length >= MaxLength)));
            bool isMaxLength00 = (!isPhonePrompt &&
                ((Text != null) && (MaxLength > 0) && (Text.Length >= (MaxLength - 1))));
            if (!button00.Text.Contains("."))
                button00.IsEnabled = !isMaxLength00;
            else 
                button00.IsEnabled = (!isMaxLength && ((Text == null) || !Text.Contains(".")));
            button0.IsEnabled = button1.IsEnabled =
                button2.IsEnabled = button3.IsEnabled = button4.IsEnabled =
                button5.IsEnabled = button6.IsEnabled = button7.IsEnabled =
                button8.IsEnabled = button9.IsEnabled = !isMaxLength;
        }

        public void SetDefaultEnterEventHandler()
        {
            if (FireEventOnEnter)
                return;
            FireEventOnEnter = true;
            EnterPressed += new EventHandler(NumberEntryControl_EnterPressed);
        }

        [Obfuscation(Exclude = true)]
        void NumberEntryControl_EnterPressed(object sender, EventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
