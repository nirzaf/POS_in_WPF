using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for TimeEntryControl.xaml
    /// </summary>
    public partial class TimeEntryControl : UserControl
    {
        #region Licensed Access Only
        static TimeEntryControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TimeEntryControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public event EventHandler TimeChanged;

        public bool IsContextMenu
        {
            get;
            set;
        }

        public bool IsOK
        {
            get;
            private set;
        }

        public TimeSpan TimeOfDay
        {
            get { return timeEntryControl.TimeOfDay; }
            set { timeEntryControl.TimeOfDay = value; }
        }

        public bool UseMilitaryFormat
        {
            get { return timeEntryControl.UseMilitaryFormat; }
            set { timeEntryControl.UseMilitaryFormat = value; }
        }

        public TimeEntryControl()
        {
            IsOK = false;
            InitializeComponent();
        }

        public ContextMenu TimeEditContextMenu
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

        private void ButtonCleanup()
        {
            if (!IsContextMenu)
                Window.GetWindow(this).Close();
            else
                TimeEditContextMenu.IsOpen = false;
        }
        
        [Obfuscation(Exclude = true)]
        private void timeEntryControl_TimeChanged(object sender, EventArgs e)
        {
            if (TimeChanged != null)
                TimeChanged.Invoke(this, e);
        }

        [Obfuscation(Exclude = true)]
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IsOK = true;
            ButtonCleanup();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            ButtonCleanup();
        }

    }
}
