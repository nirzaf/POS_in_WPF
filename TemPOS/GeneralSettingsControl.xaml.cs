using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TemPOS.Types;
using PosModels.Types;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for GeneralSettingsControl.xaml
    /// </summary>
    public partial class GeneralSettingsControl : UserControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
#if DEMO
            tabControl.Tab5 = null;
#endif
            if (!SessionManager.ActiveEmployee.HasPermission(Permissions.ExitProgram))
                tabControl.Tab5 = null;
        }


        [Obfuscation(Exclude = true)]
        private void button_Selected(object sender, EventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                return;
            }
        }

    }
}
