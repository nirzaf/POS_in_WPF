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
using TemPOS.Managers;
using TemPOS.Types;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DeviceChoiceControl.xaml
    /// </summary>
    public partial class DeviceChoiceControl : UserControl
    {
#if !DEMO
        private bool haltSelectionChangeEvent = false;
        private Type selectedType;
        private DeviceInfo deviceInfo;

        public Type SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                gridPosPrinter.Visibility = (value == typeof(PosPrinter)) ? 
                    Visibility.Visible :Visibility.Collapsed;
                gridCashDrawer.Visibility = (value == typeof(CashDrawer)) ?
                    Visibility.Visible : Visibility.Collapsed;
                gridScanner.Visibility = (value == typeof(Scanner)) ?
                    Visibility.Visible : Visibility.Collapsed;
                gridCoinDispenser.Visibility = (value == typeof(CoinDispenser)) ?
                    Visibility.Visible : Visibility.Collapsed;
                gridBumpBar.Visibility = (value == typeof(BumpBar)) ?
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        public DeviceInfo SelectedDevice
        {
            get { return deviceInfo; }
            set
            {
                deviceInfo = value;
                if (selectedType == typeof(PosPrinter))
                    InitializeSelectedPosPrinter();
            }
        }

        private void InitializeSelectedPosPrinter()
        {
            haltSelectionChangeEvent = true;
            string printTargetName = DeviceManager.GetPrinterTargetName(deviceInfo);
            if (printTargetName.Equals(Strings.Local))
                comboBoxPrinterOutput.SelectedIndex = 1;
            else if (printTargetName.Equals(Strings.Journal))
                comboBoxPrinterOutput.SelectedIndex = 2;
            else if (printTargetName.Equals(Strings.Kitchen1))
                comboBoxPrinterOutput.SelectedIndex = 3;
            else if (printTargetName.Equals(Strings.Kitchen2))
                comboBoxPrinterOutput.SelectedIndex = 4;
            else if (printTargetName.Equals(Strings.Kitchen3))
                comboBoxPrinterOutput.SelectedIndex = 5;
            else if (printTargetName.Equals(Strings.Bar1))
                comboBoxPrinterOutput.SelectedIndex = 6;
            else if (printTargetName.Equals(Strings.Bar2))
                comboBoxPrinterOutput.SelectedIndex = 7;
            else if (printTargetName.Equals(Strings.Bar3))
                comboBoxPrinterOutput.SelectedIndex = 8;
            else
                comboBoxPrinterOutput.SelectedIndex = 0;
            haltSelectionChangeEvent = false;
        }
#endif
        public DeviceChoiceControl()
        {
            InitializeComponent();
#if !DEMO
            InitializePosPrinterTargetsComboBox();
#else
            gridDemo.Visibility = Visibility.Visible;
#endif
        }

#if !DEMO
        private void InitializePosPrinterTargetsComboBox()
        {
            comboBoxPrinterOutput.Items.Add(Strings.Unused);
            comboBoxPrinterOutput.Items.Add(Strings.Local);
            comboBoxPrinterOutput.Items.Add(Strings.Journal);
            comboBoxPrinterOutput.Items.Add(Strings.Kitchen1);
            comboBoxPrinterOutput.Items.Add(Strings.Kitchen2);
            comboBoxPrinterOutput.Items.Add(Strings.Kitchen3);
            comboBoxPrinterOutput.Items.Add(Strings.Bar1);
            comboBoxPrinterOutput.Items.Add(Strings.Bar2);
            comboBoxPrinterOutput.Items.Add(Strings.Bar3);
        }
#endif
        [Obfuscation(Exclude = true)]
        private void comboBoxPrinterOutput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !DEMO
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0) || haltSelectionChangeEvent)
                return;
            if ((e.RemovedItems != null) && (e.RemovedItems.Count > 0))
            {
                string oldPrinterName = (e.RemovedItems[0] as ComboBoxItem).Content as string;
                DeviceManager.SetPrinterToUnused(oldPrinterName);
            }
            string printerName = (e.AddedItems[0] as ComboBoxItem).Content as string;
            DeviceManager.SetPrinter(printerName, deviceInfo);
#endif
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxPrinterOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
#if !DEMO

#endif
        }
    }
}
