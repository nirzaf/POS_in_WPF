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
using PosControls.Interfaces;
using PosControls.Helpers;
using System.Data.SqlTypes;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateTimeEditControl.xaml
    /// </summary>
    public partial class DateTimeEditControl : UserControl
    {
        #region Licensed Access Only
        static DateTimeEditControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateTimeEditControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private DateTime? selectedDateTime = DateTime.Now;

[Obfuscation(Exclude = true)]
        public event EventHandler SelectedDateTimeChanged;

        private DateTime SafeSelectedDateTime
        {
            get
            {
                if ((selectedDateTime == null) || !selectedDateTime.HasValue)
                    return SqlDateTime.MinValue.Value;
                else
                    return selectedDateTime.Value;
            }
        }

        public DateTime? SelectedDateTime
        {
            get { return selectedDateTime; }
            set
            {
                selectedDateTime = value;
                if ((value == null) || !value.HasValue)
                    button.Text = "";
                else
                    button.Text = value.Value.ToString("h:mm tt on MMMM dd, yyyy");
                if (SelectedDateTimeChanged != null)
                    SelectedDateTimeChanged.Invoke(this, new EventArgs());
            }
        }

        public int Day
        {
            get { return SafeSelectedDateTime.Day; }
        }

        public int Month
        {
            get { return SafeSelectedDateTime.Month; }
        }

        public int Year
        {
            get { return SafeSelectedDateTime.Year; }
        }

        public int Hour
        {
            get { return SafeSelectedDateTime.Hour; }
        }

        public int Minute
        {
            get { return SafeSelectedDateTime.Minute; }
        }

        public int Second
        {
            get { return SafeSelectedDateTime.Second; }
        }

        public IShadeable ParentWindow
        {
            get { return (IShadeable)Window.GetWindow(this); }
        }

        public DateTimeEditControl()
        {
            InitializeComponent();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTimeComboControl editor = new DateTimeComboControl();
            PosDialogWindow window = new PosDialogWindow(editor, "Modify Date and Time");

            editor.SelectedDateTime = SelectedDateTime;
            ParentWindow.ShowShadingOverlay = true;
            window.Width = 560;
            window.Height = 475;
            window.ShowDialog((Window)ParentWindow);
            ParentWindow.ShowShadingOverlay = false;
            if (!window.ClosedByUser)
                SelectedDateTime = editor.SelectedDateTime;
        }
    }
}
