using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using PosControls.Interfaces;
using PosControls.Helpers;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for PosDialogWindow.xaml
    /// </summary>
    public partial class PosDialogWindow : Window, IShadeable
    {
        #region Licensed Access Only
        static PosDialogWindow()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PosDialogWindow).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool _showingShadingOverlay;
        private ShadingAdorner _adorner;
        private bool _isClosable;
        private string _windowTitle = "Window";

        [Obfuscation(Exclude = true)]
        private event EventHandler TabIndexChanged;

        public static bool ForceTopMost
        {
            get;
            set;
        }

        public static bool HasPosDialogWindowsOpen
        {
            get
            {
                return Application.Current.Windows.Cast<Window>()
                    .Any(window => (window is PosDialogWindow) && (window.IsVisible));
            }
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            Rect borderRect = new Rect(0, 0, layoutSlotSize.Width, layoutSlotSize.Height);
            Geometry borderClip = new RectangleGeometry(borderRect, 8.5, 8.5);
            borderClip.Freeze();

            Geometry baseClip = base.GetLayoutClip(layoutSlotSize);
            if (baseClip == null)
                return borderClip;

            CombinedGeometry mergedClip = new CombinedGeometry(
                GeometryCombineMode.Intersect, baseClip, borderClip);

            mergedClip.Freeze();
            return mergedClip;
        }

        public int WindowTabIndex
        {
            get
            {
                if (buttonTab1.IsChecked == true)
                    return 1;
                if (buttonTab2.IsChecked == true)
                    return 2;
                if (buttonTab3.IsChecked == true)
                    return 3;
                if (buttonTab4.IsChecked == true)
                    return 4;
                if (buttonTab5.IsChecked == true)
                    return 5;
                return 0;
            }
        }

        public new string Title
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                labelTitle.Content = value;
                labelTitle.Visibility = (!string.IsNullOrEmpty(value) ?
                    Visibility.Visible : Visibility.Collapsed);
            }
        }

        /// <summary>
        /// True to display the close button in the title bar
        /// </summary>
        public bool IsClosable
        {
            get { return _isClosable; }
            set
            {
                _isClosable = value;
                buttonClose.Visibility =
                    (value ? Visibility.Visible : Visibility.Hidden);                    
            }
        }

        public bool CenterTitleBarText
        {
            get;
            set;
        }

        public bool ClosedByUser
        {
            get;
            private set;
        }

        public FrameworkElement DockedControl
        {
            get;
            private set;
        }

        public FrameworkElement[] DockedControls
        {
            get;
            private set;
        }
        
        /// <summary>
        /// This is used to gray out the parent window when showing a dialog
        /// </summary>
        public bool ShowShadingOverlay
        {
            get
            {
                return _showingShadingOverlay;
            }
            set
            {
                if (_showingShadingOverlay == value)
                    return;
                _showingShadingOverlay = value;
                if (value)
                {
                    if (_adorner == null)
                        _adorner = new ShadingAdorner(mainPane);
                    AdornerLayer.GetAdornerLayer(mainPane).Add(_adorner);
                }
                else
                {
                    if (_adorner != null)
                        AdornerLayer.GetAdornerLayer(mainPane).Remove(_adorner);
                }
            }
        }

        private PosDialogWindow(Window parent, bool shadeParent = false)
        {
            _isClosable = true;
            ClosedByUser = false;
            AllowsTransparency = false;
            InitializeComponent();
            if (ForceTopMost)
                Topmost = true;
            Owner = parent;
            DockedControl = new DialogBoxControl();
            dockPanel1.Children.Add(DockedControl);
            if (!shadeParent) return;
            Loaded += PosDialogWindow_Loaded;
            Closed += PosDialogWindow_Closed;
        }
        
        [Obfuscation(Exclude = true)]
        void PosDialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is IShadeable)
                ((IShadeable)Owner).ShowShadingOverlay = true;
        }

        [Obfuscation(Exclude = true)]
        void PosDialogWindow_Closed(object sender, EventArgs e)
        {
            if (Owner is IShadeable)
                ((IShadeable)Owner).ShowShadingOverlay = false;
        }

        public PosDialogWindow(FrameworkElement control, string title)
        {
            _isClosable = true;
            ClosedByUser = false;
            InitializeComponent();
            DockedControl = control;            
            dockPanel1.Children.Add(control);
            Title = title;
        }

        public PosDialogWindow(FrameworkElement control,
            string title, double width, double height)
            : this(control, title)
        {
            Height = height;
            Width = width;
        }

        public PosDialogWindow(string title, FrameworkElement[] controls,
            string[] tabNames, double[] tabWidths, double width, double height)            
        {
            _isClosable = true;
            ClosedByUser = false;
            InitializeComponent();
            Width = width;
            Height = height;
            if (((controls == null) || (controls.Length == 0)) ||
                ((tabNames == null) || (tabNames.Length == 0)))
                return;
            DockedControls = controls;
            Title = title;
            SetTab(buttonTab1,
                ((tabWidths.Length > 0) ? tabWidths[0] : 0),
                ((controls.Length > 0) ? controls[0] : null),
                ((tabNames.Length > 0) ? tabNames[0] : null), 1);
            SetTab(buttonTab2,
                ((tabWidths.Length > 1) ? tabWidths[1] : 0),
                ((controls.Length > 1) ? controls[1] : null),
                ((tabNames.Length > 1) ? tabNames[1] : null), 2);
            SetTab(buttonTab3,
                ((tabWidths.Length > 2) ? tabWidths[2] : 0),
                ((controls.Length > 2) ? controls[2] : null),
                ((tabNames.Length > 2) ? tabNames[2] : null), 3);
            SetTab(buttonTab4,
                ((tabWidths.Length > 3) ? tabWidths[3] : 0),
                ((controls.Length > 3) ? controls[3] : null),
                ((tabNames.Length > 3) ? tabNames[3] : null), 4);
            SetTab(buttonTab5,
                ((tabWidths.Length > 4) ? tabWidths[4] : 0),
                ((controls.Length > 4) ? controls[4] : null),
                ((tabNames.Length > 4) ? tabNames[4] : null), 5);
            buttonTab1.IsChecked = true;
        }

        public static bool? ShowPosDialogWindow(PosDialogWindow window)
        {
            DependencyObject control =
                Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive) ??
                Application.Current.MainWindow;
            return control == null ? null : ShowPosDialogWindow(control, window);
        }

        [Obsolete("Remove the (parent) control parameter")]
        public static bool? ShowPosDialogWindow(DependencyObject control, 
            PosDialogWindow window)
        {
            Window parentWindow =
                (control is Window ? (Window)control : GetWindow(control));
            var shadeable = parentWindow as IShadeable;
            if (shadeable != null)
                shadeable.ShowShadingOverlay = true;

            bool? result = window.ShowDialog(parentWindow);

            if (shadeable != null)
                shadeable.ShowShadingOverlay = false;

            return window.ClosedByUser ? null : result;
        }

        private void ShowDockPanel(object sender)
        {
            dockPanel1.Visibility = (Equals(sender, buttonTab1) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel2.Visibility = (Equals(sender, buttonTab2) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel3.Visibility = (Equals(sender, buttonTab3) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel4.Visibility = (Equals(sender, buttonTab4) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel5.Visibility = (Equals(sender, buttonTab5) ?
                Visibility.Visible : Visibility.Collapsed);
        }

        private void SetTab(ToggleButton buttonTab, double buttonWidth, FrameworkElement tab, string tabTitle, int index)
        {
            if (tabTitle == null)
            {
                buttonTab.Visibility = Visibility.Collapsed;
                buttonTab.Content = "";
                return;
            }
            buttonTab.Width = buttonWidth;
            buttonTab.Visibility = Visibility.Visible;
            buttonTab.Content = tabTitle;
            if (index == 1)
                dockPanel1.Children.Add(tab);
            if (index == 2)
                dockPanel2.Children.Add(tab);
            if (index == 3)
                dockPanel3.Children.Add(tab);
            if (index == 4)
                dockPanel4.Children.Add(tab);
            if (index == 5)
                dockPanel5.Children.Add(tab);
        }

        #region static
        private static Window _StartupWindow;

        public static void SetStartupWindow(Window startupWindow)
        {
            _StartupWindow = startupWindow;
        }

        public static DialogButton ShowDialog(string text, string title, bool shadeParent = true)
        {
            return ShowDialog(Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive),
                       text, title, shadeParent);
        }

        [Obsolete("Remove the Window parameter")]
        public static DialogButton ShowDialog(Window owner, string text, string title, bool shadeParent = true)
        {
            return ShowDialog(owner, text, title, DialogButtons.Ok, shadeParent);
        }

        public static DialogButton ShowDialog(string text, string title, DialogButtons buttons, bool shadeParent = true)
        {
            return ShowDialog(Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive),
                              text, title, buttons, shadeParent);
        }

        [Obsolete("Remove the Window parameter")]
        public static DialogButton ShowDialog(Window owner, string text, string title, DialogButtons buttons, bool shadeParent)
        {
            var window = new PosDialogWindow(owner, shadeParent);
            var control = (DialogBoxControl)window.DockedControl;
            control.InitilizeButtonChoices(buttons);

            double width, height;
            MeasureText(control.textBox1, text, out width, out height);

            control.textBox1.Text = text;
            window.IsClosable = false;
            window.Width = width;
            window.Height = height;
            window.labelTitle.Content = title;
            if (owner.Topmost)
            {
                window.Topmost = true;
                owner.Topmost = false;
            }
            var shadeable = owner as IShadeable;
            if (shadeable != null)
                shadeable.ShowShadingOverlay = true;
            window.ShowDialog();
            if (shadeable != null)
                ((IShadeable)owner).ShowShadingOverlay = false;
            if (window.Topmost)
                owner.Topmost = true;
            if (control.IsOK)
                return DialogButton.Ok;
            if (control.IsYes)
                return DialogButton.Yes;
            if (buttons == DialogButtons.OkCancel)
                return DialogButton.Cancel;
            return DialogButton.No;            
        }

        /// <summary>
        /// ToDo: Unfinished
        /// </summary>
        #warning This ShowDialog method is unfinished
        public static int ShowDialog(Window owner, string text, string title,
            string[] buttonChoices)
        {
            PosDialogWindow window = new PosDialogWindow(owner);
            DialogBoxControl control = (DialogBoxControl)window.DockedControl;

            double width, height;
            MeasureText(control.textBox1, text, out width, out height);

            control.textBox1.Text = text;
            window.IsClosable = false;
            window.Width = width;
            window.Height = height;
            window.labelTitle.Content = title;
            window.ShowDialog();

            // Return the selected button index, not -1
            return -1; 
        }

        private static bool MeasureText(TextBlock control, string text, out double width, out double height)
        {
            height = 0;
            width = 0;
            Typeface typeface = new Typeface(control.FontFamily, control.FontStyle,
                    control.FontWeight, control.FontStretch);
            GlyphTypeface glyphTypeface;
            if ((text == null) || !typeface.TryGetGlyphTypeface(out glyphTypeface))
                return false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch.Equals('\n')) continue;
                // ToDo: This next line can't handle \t
                ushort glyph = glyphTypeface.CharacterToGlyphMap[ch];
                width += glyphTypeface.AdvanceWidths[glyph];
                height = glyphTypeface.AdvanceHeights[glyph];
            }
            width *= control.FontSize;
            height *= control.FontSize;

            double originalWidth = width;
            int min = 375;
            int max = 625;
            int range = max - min;
            int mid = max - (range / 2);
            int desiredLines = 1;
            if (width < min)
                width = min;
            if (width > max)
            {
                desiredLines = (int)(width / mid);
                width /= desiredLines;
            }
            height = (height * desiredLines) + 180;
            width += 30;
            return true;
        }

        public static string PromptKeyboard(string fieldName, string defaultValue)
        {
            return PromptKeyboard(fieldName, defaultValue, false, ShiftMode.None);
        }

        public static string PromptKeyboard(string fieldName, string defaultValue, bool isPassword, ShiftMode keyboardShiftMode)
        {
            KeyboardEntryControl control = new KeyboardEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            control.UsePasswordTextField = isPassword;
            control.ShiftMode = keyboardShiftMode;
            control.Text = defaultValue;
            window.IsClosable = false;
            window.Width = 835;
            window.Height = 390;
            window.ShowDialogForActiveWindow();
            if (control.WasCanceled)
                return null;
            if (control.Text == null)
                return "";
            return control.Text;
        }

        public static int? PromptNumber(string fieldName, int? defaultValue)
        {
            NumberEntryControl control = new NumberEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            if (defaultValue.HasValue)
                control.Text = "" + defaultValue.Value;
            else
                control.Text = "";
            control.SetDefaultEnterEventHandler();
            window.IsClosable = true;
            window.Width = 210;
            window.Height = 340;
            window.ShowDialogForActiveWindow();
            int? value = null;
            if (!window.ClosedByUser)
            {
                try
                {
                    value = Convert.ToInt32(control.Text);
                }
                catch
                {
                }
            }
            return value;
        }

        public static double? PromptNumber(string fieldName, double? defaultValue)
        {
            NumberEntryControl control = new NumberEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            control.FloatValue = defaultValue;
            control.UseDecimalPoint = true;
            control.SetDefaultEnterEventHandler();
            window.IsClosable = true;
            window.Width = 210;
            window.Height = 340;
            window.ShowDialogForActiveWindow();
            double? value = null;
            if (!window.ClosedByUser)
            {
                try
                {
                    value = Convert.ToDouble(control.Text);
                }
                catch
                {
                }
            }
            return value;
        }

        public static double? PromptPercentage(DependencyObject parentControl, string fieldName, double? defaultValue)
        {            
            NumberEntryControl control = new NumberEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            Window ownerWindow = GetWindow(parentControl);
            control.SetDefaultEnterEventHandler(); 
            control.DisplayAsPercentage = true;
            control.UseDecimalPoint = true;
            control.FloatValue = defaultValue;
            window.IsClosable = true;
            window.Width = 210;
            window.Height = 340;
            if (ownerWindow is IShadeable)
                (ownerWindow as IShadeable).ShowShadingOverlay = true;
            window.ShowDialog(ownerWindow);
            if (ownerWindow is IShadeable)
                (ownerWindow as IShadeable).ShowShadingOverlay = false;
            double? value = null;
            if (!window.ClosedByUser)
            {
                try
                {
                    value = Convert.ToDouble(control.Text.Replace("%", "")) / 100;
                }
                catch
                {
                }
            }
            return value;
        }

        public static double? PromptCurrency(string fieldName, double? defaultValue)
        {
            NumberEntryControl control = new NumberEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            control.SetDefaultEnterEventHandler(); 
            control.DisplayAsCurrency = true;
            control.FloatValue = defaultValue;
            window.IsClosable = true;
            window.Width = 210;
            window.Height = 340;
            window.ShowDialogForActiveWindow();
            double? value = null;
            if (!window.ClosedByUser)
            {
                try
                {
                    value = Convert.ToDouble(StripCurrencySymbols(control.Text));
                }
                catch
                {
                }
            }
            return value;
        }

        private static string StripCurrencySymbols(string value)
        {
            return value.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace(",", "");
        }

        public static bool PromptDateRange(string fieldName, ref DateTime startDate, ref DateTime endDate)
        {
            StartDateEndDateControl control = new StartDateEndDateControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName, 670, 500)
            {
                IsClosable = true
            };
            window.ShowDialogForActiveWindow();
            if (window.ClosedByUser)
                return false;
            startDate = control.StartDate;
            endDate = control.EndDate;
            return true;
        }

        /*
        public static PhoneNumber PromptPhoneNumber(string fieldName, PhoneNumber phoneNumber)
        {
            // Todo: PromptPhoneNumber
            throw new NotImplementedException();
        }
        */

        public static TimeSpan? PromptForTime(string fieldName, TimeSpan? timeOfDay)
        {
            TimeEntryControl control = new TimeEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            if (timeOfDay.HasValue)
                control.TimeOfDay = timeOfDay.Value;
            //control.UseMilitaryFormat = true;
            window.IsClosable = false;
            window.Width = 250;
            window.Height = 390;
            window.ShowDialogForActiveWindow();
            if (!control.IsOK)
                return null;
            return control.TimeOfDay;
        }

        public static DateTime? PromptForDay(string fieldName, DateTime? day = null)
        {
            DateEntryControl control = new DateEntryControl();
            PosDialogWindow window = new PosDialogWindow(control, fieldName);
            control.SelectedDay = day.HasValue ? day.Value : DateTime.Now;
            window.IsClosable = false;
            window.Width = 410;
            window.Height = 430;
            window.ShowDialogForActiveWindow();

            return control.SelectedDay;
        }
        #endregion

        public void SetButtonsEnabled(bool isEnabled)
        {
            buttonClose.IsEnabled = 
                buttonTab1.IsEnabled =
                buttonTab2.IsEnabled =
                buttonTab3.IsEnabled =
                buttonTab4.IsEnabled =
                buttonTab5.IsEnabled = isEnabled;
        }

        [Obfuscation(Exclude = true)]
        private void buttonTab_Click(object sender, RoutedEventArgs e)
        {
            buttonTab1.IsChecked = Equals(sender, buttonTab1);
            buttonTab2.IsChecked = Equals(sender, buttonTab2);
            buttonTab3.IsChecked = Equals(sender, buttonTab3);
            buttonTab4.IsChecked = Equals(sender, buttonTab4);
            buttonTab5.IsChecked = Equals(sender, buttonTab5);
            if (TabIndexChanged != null)
                TabIndexChanged.Invoke(this, new EventArgs());
            ShowDockPanel(sender);
        }

        [Obfuscation(Exclude = true)]
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            ClosedByUser = true;
            Close();
        }
    }
}
