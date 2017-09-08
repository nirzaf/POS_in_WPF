using System;
using System.Linq;
using PosModels;
using TemposLibrary.Win32;
using System.Reflection;
using TemPOS.Types;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS.Managers
{
    // Set the BinaryConversions property for Ingenico SigCap
    // http://social.msdn.microsoft.com/Forums/en-US/posfordotnet/thread/884a13b7-0031-42a2-99b5-0074b0e56017
    // http://blogs.msdn.com/pointofservice/archive/2009/09/18/setting-the-binaryconversions-property-for-legacy-opos-service-objects.aspx

    // OPOS SigCap error on EndCapture
    // http://social.msdn.microsoft.com/Forums/en-US/posfordotnet/thread/5686e3eb-50b7-4f19-aa82-29804ddda459

    public static class DeviceManager
    {
        #region Licensed Access Only / Static Initializer
        static DeviceManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DeviceManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
#if !DEMO
            InitializePosDevices();
#endif
        }
        #endregion

#if !DEMO
        private static PosExplorer explorer;
        private static Scanner activeScanner;
        private static CashDrawer activeCashDrawer1;
        private static CashDrawer activeCashDrawer2;
        private static BumpBar activeBumpBar;
        private static CoinDispenser activeCoinDispenser;
        private static PosPrinter activePosPrinterJournal;
        private static PosPrinter activePosPrinterLocal;
        private static PosPrinter activePosPrinterKitchen1;
        private static PosPrinter activePosPrinterKitchen2;
        private static PosPrinter activePosPrinterKitchen3;
        private static PosPrinter activePosPrinterBar1;
        private static PosPrinter activePosPrinterBar2;
        private static PosPrinter activePosPrinterBar3;

        public static BumpBar ActiveBumpBar
        {
            get { return activeBumpBar; }
        }

        public static CashDrawer ActiveCashDrawer1
        {
            get { return activeCashDrawer1; }
        }

        public static CashDrawer ActiveCashDrawer2
        {
            get { return activeCashDrawer2; }
        }

        public static CoinDispenser ActiveCoinDispenser
        {
            get { return activeCoinDispenser; }
        }

        public static Scanner ActiveScanner
        {
            get { return activeScanner; }
        }

        public static PosPrinter ActivePosPrinterJournal
        {
            get { return activePosPrinterJournal; }
        }

        public static PosPrinter ActivePosPrinterLocal
        {
            get { return activePosPrinterLocal; }
        }

        public static PosPrinter ActivePosPrinterKitchen1
        {
            get { return activePosPrinterKitchen1; }
        }

        public static PosPrinter ActivePosPrinterKitchen2
        {
            get { return activePosPrinterKitchen2; }
        }

        public static PosPrinter ActivePosPrinterKitchen3
        {
            get { return activePosPrinterKitchen3; }
        }

        public static PosPrinter ActivePosPrinterBar1
        {
            get { return activePosPrinterBar1; }
        }

        public static PosPrinter ActivePosPrinterBar2
        {
            get { return activePosPrinterBar2; }
        }

        public static PosPrinter ActivePosPrinterBar3
        {
            get { return activePosPrinterBar3; }
        }

        public static DeviceCollection BumpBarDeviceCollection
        {
            get { return explorer.GetDevices(DeviceType.BumpBar); }
        }

        public static DeviceCollection CashDrawerDeviceCollection
        {
            get { return explorer.GetDevices(DeviceType.CashDrawer); }
        }

        public static DeviceCollection CoinDispenserDeviceCollection
        {
            get { return explorer.GetDevices(DeviceType.CoinDispenser); }
        }

        public static DeviceCollection PosPrinterDeviceCollection
        {
            get { return explorer.GetDevices(DeviceType.PosPrinter); }
        }

        public static DeviceCollection ScannerDeviceCollection
        {
            get { return explorer.GetDevices(DeviceType.Scanner); }
        }

        private static void InitializePosDevices()
        {
            explorer = new PosExplorer();
            InitializeCashDrawer1();
            InitializeCashDrawer2();
            InitializeScanners();
#if DEBUG
            activePosPrinterLocal =
                activePosPrinterJournal =
                activePosPrinterKitchen1 =
                activePosPrinterKitchen2 =
                activePosPrinterKitchen3 =
                activePosPrinterBar1 =
                activePosPrinterBar2 =
                activePosPrinterBar3 = InitializePosPrinter("PrinterDebugAll");
            if (activePosPrinterLocal != null)
                activePosPrinterLocal.PrintNormal(PrinterStation.Receipt, "** Debug Mode: Capturing all printer outputs **" + Environment.NewLine);
#else
            activePosPrinterLocal = InitializePosPrinter("PrinterName");
            activePosPrinterJournal = InitializePosPrinter("PrinterNameJournal");
            activePosPrinterKitchen1 = InitializePosPrinter("PrinterNameKitchen1");
            activePosPrinterKitchen2 = InitializePosPrinter("PrinterNameKitchen2");
            activePosPrinterKitchen3 = InitializePosPrinter("PrinterNameKitchen3");
            activePosPrinterBar1 = InitializePosPrinter("PrinterNameBar1");
            activePosPrinterBar2 = InitializePosPrinter("PrinterNameBar2");
            activePosPrinterBar3 = InitializePosPrinter("PrinterNameBar3");
#endif
            InitializeCoinDispenser();
            InitializeBumpBar();
        }

        private static void InitializeBumpBar()
        {
            DeviceCollection bumpBarList = BumpBarDeviceCollection;
            if (bumpBarList.Count > 0)
            {
                try
                {
                    DeviceInfo bumpBar = GetActiveDeviceInfo(bumpBarList, "BumpBarName", true);
                    if (bumpBar != null)
                    {
                        activeBumpBar = (BumpBar)explorer.CreateInstance(bumpBar);
                        activeBumpBar.Open();
                        activeBumpBar.Claim(1000);
                        activeBumpBar.DeviceEnabled = true;
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    activeBumpBar = null;
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
            }
        }

        private static void InitializeCoinDispenser()
        {
            DeviceCollection coinDispenserList = CoinDispenserDeviceCollection;
            if (coinDispenserList.Count > 0)
            {
                try
                {
                    DeviceInfo coinDispenser = GetActiveDeviceInfo(coinDispenserList,
                        "CoinDispenserName", true);
                    if (coinDispenser != null)
                    {
                        activeCoinDispenser =
                            (CoinDispenser)explorer.CreateInstance(coinDispenser);
                        activeCoinDispenser.Open();
                        activeCoinDispenser.Claim(1000);
                        activeCoinDispenser.DeviceEnabled = true;
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    activeCoinDispenser = null;
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
            }
        }

        private static void InitializeScanners()
        {
            DeviceCollection scannerList = ScannerDeviceCollection;
            if (scannerList.Count > 0)
            {
                try
                {
                    DeviceInfo scanner = GetActiveDeviceInfo(scannerList, "ScannerName", true);
                    if (scanner != null)
                    {
                        activeScanner = (Scanner)explorer.CreateInstance(scanner);
                        activeScanner.Open();
                        activeScanner.Claim(1000);
                        activeScanner.DeviceEnabled = true;
                        activeScanner.DecodeData = true;
                        activeScanner.DataEventEnabled = true;
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    activeScanner = null;
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
            }
        }

        private static void InitializeCashDrawer1()
        {
            DeviceCollection cashDrawerList = CashDrawerDeviceCollection;
            if (cashDrawerList.Count > 0)
            {
                try
                {
                    DeviceInfo cashDrawer = GetActiveDeviceInfo(cashDrawerList,
                        "CashDrawerName1", true);
                    if (cashDrawer != null)
                    {
                        activeCashDrawer1 = (CashDrawer)explorer.CreateInstance(cashDrawer);
                        activeCashDrawer1.Open();
                        activeCashDrawer1.Claim(1000);
                        activeCashDrawer1.DeviceEnabled = true;
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    activeCashDrawer1 = null;
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
            }
        }

        private static void InitializeCashDrawer2()
        {
            DeviceCollection cashDrawerList = CashDrawerDeviceCollection;
            if (cashDrawerList.Count > 0)
            {
                try
                {
                    DeviceInfo cashDrawer = GetActiveDeviceInfo(cashDrawerList,
                        "CashDrawerName2", true);
                    if (cashDrawer != null)
                    {
                        activeCashDrawer2 = (CashDrawer)explorer.CreateInstance(cashDrawer);
                        activeCashDrawer2.Open();
                        activeCashDrawer2.Claim(1000);
                        activeCashDrawer2.DeviceEnabled = true;
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    activeCashDrawer2 = null;
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
            }
        }

        private static PosPrinter InitializePosPrinter(string configSettingName)
        {
            PosPrinter result = null;
            DeviceCollection printerList = PosPrinterDeviceCollection;
            if (printerList.Count > 0)
            {
                try
                {
#if DEBUG
                    //DeviceInfo printer = GetActiveDeviceInfo(printerList, configSettingName, true);
                    DeviceInfo printer = printerList[0];
#else
                    DeviceInfo printer = GetActiveDeviceInfo(printerList, configSettingName);
#endif
                    if (printer != null)
                    {
                        result = (PosPrinter)explorer.CreateInstance(printer);
                        result.Open();
                        result.Claim(1000);
                        result.DeviceEnabled = true;                        
                    }
                }
                catch (PosControlException)
                {
                    // Log error and set the active to nil
                    Logger.Error("InitializePosDevices", Strings.InitializationException);
                }
           }
           return result;
        }
#endif
        public static void ClosePosDevices()
        {
#if !DEMO
            CloseDevice(activeCashDrawer1);
            CloseDevice(activeScanner);
            CloseDevice(activePosPrinterJournal);
            CloseDevice(activePosPrinterLocal);
            CloseDevice(activePosPrinterKitchen1);
            CloseDevice(activePosPrinterKitchen2);
            CloseDevice(activePosPrinterKitchen3);
            CloseDevice(activePosPrinterBar1);
            CloseDevice(activePosPrinterBar2);
            CloseDevice(activePosPrinterBar3);
            CloseDevice(activeBumpBar);
            CloseDevice(activeCoinDispenser);
#endif
        }

#if !DEMO
        private static void CloseDevice(PosCommon device)
        {
            if (device == null)
                return;
            try
            {
                if (device.Claimed)
                    device.Release();
            }
            catch
            {
            }
            try
            {
                device.Close();
            }
            catch
            {
            }
        }

        public static DeviceInfo GetActiveDeviceInfo(DeviceCollection deviceCollection, string configSettingName, bool useDefault = false)
        {
            if (deviceCollection.Count == 0)
                return null;
            string currentDevice = LocalSetting.Values.String[configSettingName];
            if (currentDevice == null)
                return (useDefault ? deviceCollection[0] : null);
            foreach (DeviceInfo deviceInfo in deviceCollection)
            {
                if (deviceInfo.Description.Equals(currentDevice))
                    return deviceInfo;
            }
            //return deviceCollection[0];
            return (useDefault ? deviceCollection[0] : null);
        }
#endif
        public static void OpenCashDrawer1()
        {
#if !DEMO
            if ((activeCashDrawer1 != null) && (activeCashDrawer1.State == ControlState.Idle))
            {
#if DEBUG
                ShowActiveCashDrawerWindow();
#endif
                activeCashDrawer1.OpenDrawer();
            }
#endif
        }

        public static void OpenCashDrawer2()
        {
#if !DEMO
            if ((activeCashDrawer2 != null) && (activeCashDrawer2.State == ControlState.Idle))
            {
#if DEBUG
                ShowActiveCashDrawerWindow();
#endif
                activeCashDrawer2.OpenDrawer();
            }
#endif
        }

#if DEBUG
        private static void ShowActiveCashDrawerWindow()
        {
            IntPtr hWnd = User32.FindWindow(null, "Microsoft CashDrawer Simulator");
            if (hWnd != null)
                ShowWindow(hWnd);
        }

        public static void ShowWindow(IntPtr hWnd)
        {            
            if (User32.GetForegroundWindow() != hWnd)
            {
                IntPtr dwMyThreadID = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), IntPtr.Zero);
                IntPtr dwOtherThreadID = User32.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                if (dwMyThreadID != dwOtherThreadID)
                {
                    User32.AttachThreadInput(dwMyThreadID, dwOtherThreadID, true);
                    User32.BringWindowToTop(hWnd);
                    User32.SetForegroundWindow(hWnd);
                    IntPtr hMain = User32.GetWindow(hWnd, WinBase.GW_ENABLEDPOPUP);
                    User32.SetFocus(hMain);
                    User32.AttachThreadInput(dwMyThreadID, dwOtherThreadID, false);
                }
                else
                    User32.SetForegroundWindow(hWnd);
            }
        }
#endif

#if !DEMO
        public static void SetPrinter(string printerTargetName, DeviceInfo deviceInfo)
        {
            if (printerTargetName.Equals(Strings.Local))
            {
                CloseDevice(activePosPrinterLocal);
                LocalSetting.Values.String["PrinterName"] = deviceInfo.Description;
                activePosPrinterLocal = InitializePosPrinter("PrinterName");
            }
            else if (printerTargetName.Equals(Strings.Journal))
            {
                CloseDevice(activePosPrinterJournal);
                LocalSetting.Values.String["PrinterNameJournal"] = deviceInfo.Description;
                activePosPrinterJournal = InitializePosPrinter("PrinterNameJournal");
            }
            else if (printerTargetName.Equals(Strings.Kitchen1))
            {
                CloseDevice(activePosPrinterKitchen1);
                LocalSetting.Values.String["PrinterNameKitchen1"] = deviceInfo.Description;
                activePosPrinterKitchen1 = InitializePosPrinter("PrinterNameKitchen1");
            }
            else if (printerTargetName.Equals(Strings.Kitchen2))
            {
                CloseDevice(activePosPrinterKitchen2);
                LocalSetting.Values.String["PrinterNameKitchen2"] = deviceInfo.Description;
                activePosPrinterKitchen2 = InitializePosPrinter("PrinterNameKitchen2");
            }
            else if (printerTargetName.Equals(Strings.Kitchen3))
            {
                CloseDevice(activePosPrinterKitchen3);
                LocalSetting.Values.String["PrinterNameKitchen3"] = deviceInfo.Description;
                activePosPrinterKitchen3 = InitializePosPrinter("PrinterNameKitchen3");
            }
            else if (printerTargetName.Equals(Strings.Bar1))
            {
                CloseDevice(activePosPrinterKitchen1);
                LocalSetting.Values.String["PrinterNameBar1"] = deviceInfo.Description;
                activePosPrinterKitchen1 = InitializePosPrinter("PrinterNameBar1");
            }
            else if (printerTargetName.Equals(Strings.Bar2))
            {
                CloseDevice(activePosPrinterKitchen2);
                LocalSetting.Values.String["PrinterNameBar2"] = deviceInfo.Description;
                activePosPrinterKitchen2 = InitializePosPrinter("PrinterNameBar2");
            }
            else if (printerTargetName.Equals(Strings.Bar3))
            {
                CloseDevice(activePosPrinterKitchen3);
                LocalSetting.Values.String["PrinterNameBar3"] = deviceInfo.Description;
                activePosPrinterKitchen3 = InitializePosPrinter("PrinterNameBar3");
            }
            LocalSetting.Update();
        }

        public static string GetPrinterTargetName(DeviceInfo deviceInfo)
        {
            string localPrinter = LocalSetting.Values.String["PrinterName"];
            string journalPrinter = LocalSetting.Values.String["PrinterNameJournal"];
            string kitchen1Printer = LocalSetting.Values.String["PrinterNameKitchen1"];
            string kitchen2Printer = LocalSetting.Values.String["PrinterNameKitchen2"];
            string kitchen3Printer = LocalSetting.Values.String["PrinterNameKitchen3"];
            string bar1Printer = LocalSetting.Values.String["PrinterNameBar1"];
            string bar2Printer = LocalSetting.Values.String["PrinterNameBar2"];
            string bar3Printer = LocalSetting.Values.String["PrinterNameBar3"];
            if (deviceInfo.Description.Equals(localPrinter))
                return Strings.Local;
            if (deviceInfo.Description.Equals(journalPrinter))
                return Strings.Journal;
            if (deviceInfo.Description.Equals(kitchen1Printer))
                return Strings.Kitchen1;
            if (deviceInfo.Description.Equals(kitchen2Printer))
                return Strings.Kitchen2;
            if (deviceInfo.Description.Equals(kitchen3Printer))
                return Strings.Kitchen3;
            if (deviceInfo.Description.Equals(bar1Printer))
                return Strings.Bar1;
            if (deviceInfo.Description.Equals(bar2Printer))
                return Strings.Bar2;
            if (deviceInfo.Description.Equals(bar3Printer))
                return Strings.Bar3;
            return Strings.Unused;
        }

        public static void SetPrinterToUnused(string printerName)
        {
            if (printerName.Equals(Strings.Local))
            {
                CloseDevice(activePosPrinterLocal);
                activePosPrinterLocal = null;
                LocalSetting.Values.String["PrinterName"] = null;
            }
            else if (printerName.Equals(Strings.Journal))
            {
                CloseDevice(activePosPrinterJournal);
                activePosPrinterJournal = null;
                LocalSetting.Values.String["PrinterNameJournal"] = null;
            }
            else if (printerName.Equals(Strings.Kitchen1))
            {
                CloseDevice(activePosPrinterKitchen1);
                activePosPrinterKitchen1 = null;
                LocalSetting.Values.String["PrinterNameKitchen1"] = null;
            }
            else if (printerName.Equals(Strings.Kitchen2))
            {
                CloseDevice(activePosPrinterKitchen2);
                activePosPrinterKitchen2 = null;
                LocalSetting.Values.String["PrinterNameKitchen2"] = null;
            }
            else if (printerName.Equals(Strings.Kitchen3))
            {
                CloseDevice(activePosPrinterKitchen3);
                activePosPrinterKitchen3 = null;
                LocalSetting.Values.String["PrinterNameKitchen3"] = null;
            }
            else if (printerName.Equals(Strings.Bar1))
            {
                CloseDevice(activePosPrinterKitchen1);
                activePosPrinterKitchen1 = null;
                LocalSetting.Values.String["PrinterNameKitchen1"] = null;
            }
            else if (printerName.Equals(Strings.Bar2))
            {
                CloseDevice(activePosPrinterKitchen2);
                activePosPrinterKitchen2 = null;
                LocalSetting.Values.String["PrinterNameKitchen2"] = null;
            }
            else if (printerName.Equals(Strings.Bar3))
            {
                CloseDevice(activePosPrinterKitchen3);
                activePosPrinterKitchen3 = null;
                LocalSetting.Values.String["PrinterNameKitchen3"] = null;
            }
            LocalSetting.Update();
        }
#endif
    }
}
