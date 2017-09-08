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
using PosControls;
using TemPOS.Managers;
using TemPOS.Types;

#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DeviceSelectionDetails.xaml
    /// </summary>
    public partial class DeviceSelectionDetails : UserControl
    {
#if !DEMO
        private PosDeviceTypes selectedDeviceType = PosDeviceTypes.None;
        public PosDeviceTypes SelectedDeviceType
        {
            get { return selectedDeviceType; }
            set
            {
                selectedDeviceType = value;
                if (!ConfigurationManager.IsInDesignMode)
                    InitializeDeviceList(value.GetPosClassType(),
                        value.GetDeviceCollection());
            }
        }
#else
        public PosDeviceTypes SelectedDeviceType
        {
            get { return PosDeviceTypes.None; }
            set
            {
            }
        }
#endif
        public DeviceSelectionDetails()
        {
            InitializeComponent();
        }
#if !DEMO
        private void InitializeDeviceList(Type type, DeviceCollection deviceCollection)
        {
            // Setup the choice control
            choiceControl.Visibility = Visibility.Hidden;
            choiceControl.SelectedType = type;

            // Initialize the device list
            listBoxDevices.Items.Clear();
            if ((deviceCollection != null) && (deviceCollection.Count > 0))
            {
                foreach (DeviceInfo deviceInfo in deviceCollection)
                {
                    listBoxDevices.Items.Add(new FormattedListBoxItem(deviceInfo,
                        deviceInfo.Description, true));
                }
            }
        }
#endif

        [Obfuscation(Exclude = true)]
        private void listBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !DEMO
            if ((e.AddedItems == null) && e.AddedItems.Count == 0)
                return;
            FormattedListBoxItem selectedItem = listBoxDevices.SelectedItem as FormattedListBoxItem;
            choiceControl.Visibility = ((listBoxDevices.SelectedItem != null) ?
                Visibility.Visible : Visibility.Hidden);
            choiceControl.SelectedDevice = selectedItem.ReferenceObject as DeviceInfo;
#endif
        }

    }
}
