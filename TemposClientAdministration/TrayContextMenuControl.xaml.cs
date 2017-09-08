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

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for TrayContextMenuControl.xaml
    /// </summary>
    public partial class TrayContextMenuControl : UserControl
    {
        public ContextMenu TrayContextMenu
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

        public TrayContextMenuControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            TrayContextMenu.IsOpen = false;
            TaskbarWindow.ShowWindow();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            TrayContextMenu.IsOpen = false;
            TaskbarWindow.Shutdown();
        }
    }
}
