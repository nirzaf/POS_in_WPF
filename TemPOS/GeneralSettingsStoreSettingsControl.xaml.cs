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
using TemposLibrary;
using PosControls.Interfaces;
using PosControls;
using System.Threading;
using TemPOS.Managers;
using PosModels;
using PosModels.Managers;
using PosControls.Helpers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for GeneralSettingsStoreSettingsControl.xaml
    /// </summary>
    public partial class GeneralSettingsStoreSettingsControl : UserControl
    {
        public GeneralSettingsStoreSettingsControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
        }

    }
}
