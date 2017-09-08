using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemPOS.Managers;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS.Types
{
    public enum PosDeviceTypes
    {
        None,
        BumpBar,
        CardScanner,
        CashDrawer,
        CoinDispenser,
        ReceiptPrinter,
    }

    public static class PosDeviceTypeExtensions
    {
#if !DEMO
        public static Type GetPosClassType(this PosDeviceTypes type)
        {
            if (type == PosDeviceTypes.BumpBar)
                return typeof(BumpBar);
            if (type == PosDeviceTypes.CardScanner)
                return typeof(Scanner);
            if (type == PosDeviceTypes.CashDrawer)
                return typeof(CashDrawer);
            if (type == PosDeviceTypes.CoinDispenser)
                return typeof(CoinDispenser);
            if (type == PosDeviceTypes.ReceiptPrinter)
                return typeof(PosPrinter);
            return null;
        }

        public static DeviceCollection GetDeviceCollection(this PosDeviceTypes type)
        {
            if (type == PosDeviceTypes.BumpBar)
                return DeviceManager.BumpBarDeviceCollection;
            if (type == PosDeviceTypes.CardScanner)
                return DeviceManager.ScannerDeviceCollection;
            if (type == PosDeviceTypes.CashDrawer)
                return DeviceManager.CashDrawerDeviceCollection;
            if (type == PosDeviceTypes.CoinDispenser)
                return DeviceManager.CoinDispenserDeviceCollection;
            if (type == PosDeviceTypes.ReceiptPrinter)
                return DeviceManager.PosPrinterDeviceCollection;
            return null;
        }
#endif
    }
}
