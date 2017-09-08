using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PosControls.Helpers;
using PosControls.Interfaces;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for TimeEditControl.xaml
    /// </summary>
    public partial class TimeEditControl : UserControl
    {
        #region Licensed Access Only
        static TimeEditControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TimeEditControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool haltEvents = false;
        private bool isPM = false;
        private int hour = DateTime.Now.Hour;
        private int minute = DateTime.Now.Minute;

        [Obfuscation(Exclude = true)]
        public event EventHandler TimeChanged;

        public int Hour
        {
            get { return hour; }
            set
            {
                HourString = "" + value;
                IsPM = (Hour >= 12);
            }
        }

        public string HourString
        {
            get { return hour.ToString(); }
            set
            {
                
                int setHour = 0;
                try
                {
                    setHour = Convert.ToInt32(value);
                }
                catch
                {
                    
                }
                if ((setHour < 0) || (setHour > 23))
                    setHour = 0;
                isPM = (setHour >= 12);
                hour = setHour;
                haltEvents = true;
                UpdateLableText();
                haltEvents = false;
                //InvokeTimeChanged();
            }
        }

        public int Minute
        {
            get { return minute; }
            set
            {
                MinuteString = "" + value;
            }
        }

        public string MinuteString
        {
            get { return minute.ToString("D2"); }
            set
            {
                int setMinute = 1;
                try
                {
                    setMinute = Convert.ToInt32(value);
                }
                catch
                {

                }
                if ((setMinute < 0) || (setMinute > 59))
                    setMinute = 0;
                minute = setMinute;
                haltEvents = true;
                UpdateLableText();
                haltEvents = false;
            }
        }

        public bool IsAM
        {
            get
            {
                return !isPM;
            }
            set
            {
                if (IsAM && !value)
                    hour += 12;
                if (!IsAM && value)
                    hour -= 12;
                isPM = !value;
                haltEvents = true;
                UpdateLableText();
                haltEvents = false;
            }
        }

        public bool IsPM
        {
            get
            {
                return isPM;
            }
            set
            {
                if (isPM && !value)
                    hour -= 12;
                if (!isPM && value)
                    hour += 12;
                isPM = value;
                haltEvents = true;
                UpdateLableText();
                haltEvents = false;
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                DateTime now = DateTime.Now;
                return new DateTime(now.Year, now.Month, now.Day, Hour, Minute, 0);
            }
        }

        public TimeEditControl()
        {
            InitializeComponent();
            SetContextMenu();
        }

        private void SetContextMenu()
        {
            ControlTemplate controlTemplate =
                Resources.GetControlTemplate("editControlTemplate");
            if (controlTemplate != null)
            {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Tag = "NumberPad";
                contextMenu.Template = controlTemplate;
                contextMenu.Placement = PlacementMode.Bottom;
                contextMenu.PlacementTarget = borderControl;
                contextMenu.Opened += new RoutedEventHandler(contextMenu_Opened);
                contextMenu.Closed += new RoutedEventHandler(contextMenu_Closed);
                borderControl.ContextMenu = contextMenu;
                contextMenu.ApplyTemplate();
                //TimeEntryControl control = GetTimeEntryControl(contextMenu);
            }
        }
        
        [Obfuscation(Exclude = true)]
        void contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = borderControl.ContextMenu;
            TimeEntryControl timeEntryControl = GetTimeEntryControl(contextMenu);
            timeEntryControl.TimeOfDay = new TimeSpan(Hour, Minute, 0);
        }

        [Obfuscation(Exclude = true)]
        void contextMenu_Closed(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = borderControl.ContextMenu;
            TimeEntryControl timeEntryControl = GetTimeEntryControl(contextMenu);
            if (timeEntryControl.IsOK)
            {
                Hour = timeEntryControl.TimeOfDay.Hours;
                Minute = timeEntryControl.TimeOfDay.Minutes;
                InvokeTimeChanged();
            }
        }

        private TimeEntryControl GetTimeEntryControl(
            DependencyObject parentDependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
            {
                DependencyObject depObject =
                    VisualTreeHelper.GetChild(parentDependencyObject, i);
                if (depObject is TimeEntryControl)
                    return depObject as TimeEntryControl;
                if (VisualTreeHelper.GetChildrenCount(depObject) > 0)
                {
                    TimeEntryControl childFound = GetTimeEntryControl(depObject);
                    if (childFound != null)
                        return childFound;
                }
            }
            return null;
        }

        private Border GetBorderControl(DependencyObject parentDependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
            {
                DependencyObject depObject =
                    VisualTreeHelper.GetChild(parentDependencyObject, i);
                if (depObject is Border)
                    return depObject as Border;
                if (VisualTreeHelper.GetChildrenCount(depObject) > 0)
                {
                    Border childFound = GetBorderControl(depObject);
                    if (childFound != null)
                        return childFound;
                }
            }
            return null;
        }

        private void UpdateLableText()
        {
            int convertedHour = Hour;
            if (convertedHour >= 12)
                convertedHour -= 12;
            if (convertedHour == 0)
                convertedHour = 12;
            labelTime.Content = "" + convertedHour + ":" + Minute.ToString("D2") +" " +
                (IsPM ? "PM" : "AM");
        }

        private void InvokeTimeChanged()
        {
            if ((!haltEvents) && (TimeChanged != null))
            {
                TimeChanged.Invoke(this, new EventArgs());
            }
        }

        private MouseButtonEventArgs lastArgs = null;
        [Obfuscation(Exclude = true)]
        private void dockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e == lastArgs)
                return;
            lastArgs = e;
            //TimeSpan? time = PromptForTime();

            SetContextMenu();
            ContextMenu contextMenu = borderControl.ContextMenu;
            TimeEntryControl timeEntryControl = GetTimeEntryControl(contextMenu);
            timeEntryControl.IsContextMenu = true;
            Border border = GetBorderControl(contextMenu);
            border.BorderBrush = ConfigurationManager.BorderBrush;
            border.Background = ConfigurationManager.ApplicationBackgroundBrush;
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(4);
            contextMenu.IsOpen = true;
            e.Handled = true;
        }

        private TimeSpan? PromptForTime()
        {
            TimeSpan? time = null;
            Window window = Window.GetWindow(this);
            if (window is IShadeable)
            {
                IShadeable dialogWindow = (IShadeable)window;
                dialogWindow.ShowShadingOverlay = true;
                time = PosDialogWindow.PromptForTime("Enter Time", CurrentDateTime.TimeOfDay);
                dialogWindow.ShowShadingOverlay = false;
            }
            else
            {
                time = PosDialogWindow.PromptForTime("Enter Time", CurrentDateTime.TimeOfDay);
            }
            return time;
        }

        [Obfuscation(Exclude = true)]
        private void Border_Initialized(object sender, EventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void TimeEntryControl_TimeChanged(object sender, EventArgs e)
        {

        }

    }
}
