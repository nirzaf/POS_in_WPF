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
using System.ComponentModel;
using PosControls.Types;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DateEntryControl.xaml
    /// </summary>
    public partial class DateEntryControl : UserControl
    {
        #region Licensed Access Only
        static DateEntryControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DateEditControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public DateTime? SelectedDay
        {
            get { return dateControl.SelectedDay; }
            set { dateControl.SelectedDay = value; }
        }

        public DateEntryControl()
        {
            InitializeComponent();            
        }

        [Obfuscation(Exclude = true)]
        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedDay = null;
            Window.GetWindow(this).Close();
        }
    }
}
