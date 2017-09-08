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
using TemPOS.Managers;
using PosControls;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DeviceSelectionControl.xaml
    /// </summary>
    public partial class DeviceSelectionControl : UserControl
    {
        public DeviceSelectionControl()
        {
            InitializeComponent();
        }
    }
}
