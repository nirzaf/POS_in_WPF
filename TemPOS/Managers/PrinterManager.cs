using System;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using TemPOS.Commands;
using TemposLibrary.Win32;
using TemPOS.Types;
using PosModels.Types;
using PosControls;
using PosModels;
using PosModels.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Strings = TemPOS.Types.Strings;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS.Managers
{
    public static class PrinterManager
    {
        #region Licensed Access Only
        static PrinterManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PrinterManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        #region Core Print Functions
#if !DEMO
        /// <summary>
        /// Print Empty Line
        /// </summary>
        public static void PrintLineToReceipt(PosPrinter printer)
        {
            PrintLineToReceipt(printer, "");
        }

        /// <summary>
        /// Print a line, reseting formating before and adding a newline after
        /// </summary>
        /// <param name="text"></param>
        public static void PrintLineToReceipt(PosPrinter printer, string text)
        {
            if (printer == null)
                return;
#if DEBUG
            ShowWindow();
#endif
            printer.PrintNormal(PrinterStation.Receipt, PrinterEscapeCodes.ResetFormatting + text + Environment.NewLine);
        }

        /// <summary>
        /// Print the raw text to the receipt (as is)
        /// </summary>
        /// <param name="text"></param>
        public static void PrintToReceipt(PosPrinter printer, string text)
        {
            if (printer == null)
                return;
#if DEBUG
            ShowWindow();
#endif
            printer.PrintNormal(PrinterStation.Receipt, text);
        }

#if DEBUG
        private static void ShowWindow()
        {
            // Microsoft PosPrinter Simulator
            IntPtr hWnd = User32.FindWindow(null, "Microsoft PosPrinter Simulator");
            if (hWnd != null)
                DeviceManager.ShowWindow(hWnd);
        }
#endif
#endif
        /// <summary>
        /// Print a fixed number of space
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FixedNumberOfSpaces(int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
                result += " ";
            return result;
        }
        #endregion

        #region Print Ticket
#if !DEMO
        public static void Print(PrintDestination printDestination, PosPrinter printer,
            Ticket ticket, TicketItemPrintOptions printOptions)
        {
            List<TicketItem> ticketItems = new List<TicketItem>(TicketItem.GetAll(ticket.PrimaryKey));
            Print(printDestination, printer, ticket, ticketItems, printOptions, false);
        }

        private static void Print(PrintDestination printDestination, PosPrinter printer,
            Ticket ticket, List<TicketItem> ticketItems, TicketItemPrintOptions printOptions, bool checkPrintDestination)
        {
            bool isKitchen = ((printDestination == PrintDestination.Kitchen1) ||
                    (printDestination == PrintDestination.Kitchen2) ||
                    (printDestination == PrintDestination.Kitchen3));

            if (!HasPrintableTicketItems(ticketItems, printDestination, printOptions, checkPrintDestination)) return;
#if DEBUG
            PrintLineToReceipt(printer, "** Debug: Printer Destination is \'" + printDestination + "\' **");
#endif
            // Header
            PrintHeaderInformation(printDestination, printer, ticket, printOptions);

            // Ticket Items
            PrintTicketItems(ticketItems, printDestination, printer, ticket, printOptions);

            // Footer
            if (!isKitchen)
                PrintFooterInformation(printer, ticket);
        }
#endif
        /// <summary>
        /// Print for item voids and item returns
        /// </summary>
        public static void Print(Ticket ticket, TicketItemPrintOptions printOptions,
            TicketItem ticketItem)
        {
            Print(ticket, printOptions, new [] { ticketItem });
        }

        /// <summary>
        /// Print for item voids and item returns
        /// </summary>
        public static void Print(Ticket ticket, TicketItemPrintOptions printOptions,
            TicketItem[] ticketItems)
        {
#if !DEMO
            Print(PrintDestination.Receipt, DeviceManager.ActivePosPrinterLocal, ticket, printOptions, ticketItems,
                (((printOptions == TicketItemPrintOptions.TicketItemVoid) ||
                (printOptions == TicketItemPrintOptions.TicketItemReturn)) ? false : true));
            Print(PrintDestination.Journal, DeviceManager.ActivePosPrinterJournal, ticket, printOptions, ticketItems, true);
            Print(PrintDestination.Kitchen1, DeviceManager.ActivePosPrinterKitchen1, ticket, printOptions, ticketItems, true);
            Print(PrintDestination.Kitchen2, DeviceManager.ActivePosPrinterKitchen2, ticket, printOptions, ticketItems, true);
            Print(PrintDestination.Kitchen3, DeviceManager.ActivePosPrinterKitchen3, ticket, printOptions, ticketItems, true);
#endif
        }

#if !DEMO
        /// <summary>
        /// Print for item voids and item returns
        /// </summary>
        public static void Print(PrintDestination printDestination, PosPrinter printer,
            Ticket ticket, TicketItemPrintOptions printOptions, TicketItem[] ticketItems)
        {
            Print(printDestination, printer, ticket, printOptions, ticketItems, false);
        }

        /// <summary>
        /// Print for item voids and item returns
        /// </summary>
        private static void Print(PrintDestination printDestination, PosPrinter printer,
            Ticket ticket, TicketItemPrintOptions printOptions, TicketItem[] ticketItems,
            bool checkPrintDestination)
        {
            if (checkPrintDestination)
            {
                bool found = false;
                foreach (TicketItem ticketItem in ticketItems)
                {
                    if (ticketItem.PrintsTo(printDestination))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return;
            }

            bool isKitchen = ((printDestination == PrintDestination.Kitchen1) ||
                    (printDestination == PrintDestination.Kitchen2) ||
                    (printDestination == PrintDestination.Kitchen3));

#if DEBUG
            PrintLineToReceipt(printer, "** Debug: Printer Destination is \'" + printDestination + "\' **");
#endif
            // Header
            PrintHeaderInformation(printDestination, printer, ticket, printOptions);

            // Ticket Items
            double totalReturnAmount = 0;
            double totalTaxAmount = 0;
            foreach (TicketItem ticketItem in ticketItems)
            {
                double[] results = PrintTicketItemToReceipt(printer, ticketItem,
                    false, printOptions, ticket.Type);
                if (results == null)
                    continue;
                totalReturnAmount += results[0];
                totalTaxAmount += results[1];
            }
            totalReturnAmount += totalTaxAmount;

            // Footer
            if ((printOptions != TicketItemPrintOptions.TicketItemReturn) && !isKitchen)
                PrintFooterInformation(printer, ticket);
            else if (!isKitchen)
            {
                PrintLineToReceipt(printer);
                PrintLineToReceipt(printer, Strings.TaxTotal +
                    PrinterEscapeCodes.SetRight + totalTaxAmount.ToString("C2"));
                PrintLineToReceipt(printer, Strings.ReturnTotal +
                    PrinterEscapeCodes.SetRight + totalReturnAmount.ToString("C2"));
            }
        }

        private static void PrintTicketItems(IEnumerable<TicketItem> ticketItems, PrintDestination printDestination,
            PosPrinter printer, Ticket ticket, TicketItemPrintOptions printOptions)
        {
            DateTime realStartTime = SqlDateTime.MaxValue.Value;
            if (ticket.StartTime != null)
            {
                int? intValue = OrderEntryCommands.GetPrepTime().IntValue;
                if (intValue != null)
                    realStartTime = (ticket.StartTime.Value - new TimeSpan(0, 0, ticket.StartTime.Value.Second) -
                                     new TimeSpan(0, intValue.Value, 0));
            }
            bool showMakeTags = (((printDestination == PrintDestination.Kitchen1) ||
                    (printDestination == PrintDestination.Kitchen2) ||
                    (printDestination == PrintDestination.Kitchen3)) &&
                    ((ticket.StartTime == null) || (realStartTime <= DateTime.Now)));

            // Hide make tags on ticket voids and cancels
            if ((printOptions == TicketItemPrintOptions.AllAsVoid) ||
                (printOptions == TicketItemPrintOptions.AllAsCancelMade) ||
                (printOptions == TicketItemPrintOptions.AllAsCancelUnmade) ||
                (printOptions == TicketItemPrintOptions.TicketRefund) ||
                (printOptions == TicketItemPrintOptions.TicketItemReturn) ||
                (printOptions == TicketItemPrintOptions.TicketItemVoid))
                showMakeTags = false;

            foreach (TicketItem ticketItem in ticketItems)
            {
                PrintTicketItemToReceipt(printer, ticketItem, showMakeTags,
                    printOptions, ticket.Type);
            }
        }
#endif
        private static bool HasPrintableTicketItems(IEnumerable<TicketItem> ticketItems, PrintDestination printDestination,
            TicketItemPrintOptions printOptions, bool checkPrintDestination)
        {
            foreach (TicketItem ticketItem in ticketItems)
            {
                if (checkPrintDestination && !ticketItem.PrintsTo(printDestination))
                    continue;
                if ((printOptions == TicketItemPrintOptions.All) ||
                    (printOptions == TicketItemPrintOptions.AllAsVoid) ||
                    (printOptions == TicketItemPrintOptions.AllAsCancelMade) ||
                    (printOptions == TicketItemPrintOptions.AllAsCancelUnmade))
                    return true;
                if ((printOptions == TicketItemPrintOptions.OnlyChanged) &&
                     (ticketItem.IsChanged || ticketItem.OrderTime == null))
                    return true;
                if ((printOptions == TicketItemPrintOptions.AllNonCanceled) &&
                    !ticketItem.IsCanceled)
                    return true;
                if ((printOptions == TicketItemPrintOptions.AllUnfired) &&
                    (ticketItem.FireTime == null) && !ticketItem.IsCanceled &&
                    Item.GetIsFired(ticketItem.ItemId))
                    return true;
            }
            return false;
        }

        public static void Print(Ticket ticket, TicketItemPrintOptions printOptions)
        {
#if !DEMO
            List<TicketItem> ticketItems = new List<TicketItem>(TicketItem.GetAll(ticket.PrimaryKey));
            Print(PrintDestination.Receipt, DeviceManager.ActivePosPrinterLocal, ticket,
                ticketItems, printOptions,
                (((printOptions == TicketItemPrintOptions.AllAsVoid) ||
                (printOptions == TicketItemPrintOptions.AllAsCancelMade) ||
                (printOptions == TicketItemPrintOptions.AllAsCancelUnmade) ||
                (printOptions == TicketItemPrintOptions.TicketRefund)) ? false : true));
            Print(PrintDestination.Journal, DeviceManager.ActivePosPrinterJournal, ticket, ticketItems, printOptions, true);
            Print(PrintDestination.Kitchen1, DeviceManager.ActivePosPrinterKitchen1, ticket, ticketItems, printOptions, true);
            Print(PrintDestination.Kitchen2, DeviceManager.ActivePosPrinterKitchen2, ticket, ticketItems, printOptions, true);
            Print(PrintDestination.Kitchen3, DeviceManager.ActivePosPrinterKitchen3, ticket, ticketItems, printOptions, true);
#endif
        }

#if !DEMO
        private static void PrintHeaderInformation(PrintDestination printDestination,
            PosPrinter printer, Ticket ticket, TicketItemPrintOptions printOptions)
        {
            // Print the receipt header image
            string filename = "receipt-header.gif";
            if (File.Exists(filename))
            {
                PrintToReceipt(printer, PrinterEscapeCodes.SetCenter);
                printer.PrintBitmap(PrinterStation.Receipt, filename, 200, PosPrinter.PrinterBitmapCenter);
            }

            // Occasion / Seating Information
            if (ConfigurationManager.UseSeating)
            {
                PrintLineToReceipt(printer,
                    PrinterEscapeCodes.SetCenter + ticket.Type.GetFriendlyName());
            }
                        
            // Print Ticket Number
            PrintLineToReceipt(printer, ((ticket.OrderId == null) ? "" : Strings.Order +
                ticket.OrderId.Value + ", ") + Strings.Ticket + ticket.PrimaryKey.Id);

            Person person = PersonManager.GetPersonByEmployeeId(ticket.EmployeeId);
            PrintLineToReceipt(printer, Strings.Employee + ": " + person.FirstName + " " +
                person.LastName.Substring(0, 1));
            if (ticket.SeatingId > 0)
            {
                Seating seating = SeatingManager.GetSeating(ticket.SeatingId);
                PrintLineToReceipt(printer, Strings.Table + seating.Description);
            }

            // Print date and time (now, future time, or ticket close time)
            DateTime time = DateTime.Now;
            int? intValue = OrderEntryCommands.GetPrepTime().IntValue;
            if (printOptions == TicketItemPrintOptions.AllAsVoid)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketVoid);
            }
            else if (printOptions == TicketItemPrintOptions.TicketRefund)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketRefund);
            }
            else if (printOptions == TicketItemPrintOptions.AllAsCancelMade)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketCancelMade);
            }
            else if (printOptions == TicketItemPrintOptions.AllAsCancelUnmade)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketCancelUnmade);
            }
            else if (printOptions == TicketItemPrintOptions.TicketItemVoid)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketItemVoid);
            }
            else if (printOptions == TicketItemPrintOptions.TicketItemReturn)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.TicketItemReturn);
            }
            else if (ticket.StartTime.HasValue && intValue.HasValue &&
                ((ticket.StartTime.Value - new TimeSpan(0, 0, ticket.StartTime.Value.Second) -
                new TimeSpan(0, intValue.Value, 0)) >= time) &&
                !ticket.IsClosed)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.FutureOrder);
                time = ticket.StartTime.Value;
            }
            else if (ticket.StartTime.HasValue && !ticket.IsClosed)
            {
                PrintLineToReceipt(printer, PrinterEscapeCodes.SetCenter + Strings.MakeNow);
                time = ticket.StartTime.Value;
            }
            if (ticket.IsClosed && ticket.CloseTime.HasValue)
                time = ticket.CloseTime.Value;
            PrintLineToReceipt(printer, time.ToShortDateString() +
                PrinterEscapeCodes.SetRight + time.ToShortTimeString());

            // Newline
            PrintLineToReceipt(printer);

            // Customer's Information
            bool isKitchen = ((printDestination == PrintDestination.Kitchen1) ||
                    (printDestination == PrintDestination.Kitchen2) ||
                    (printDestination == PrintDestination.Kitchen3));
            if (!isKitchen && (ticket.CustomerId > 0))
            {
                person = PersonManager.GetPersonByCustomerId(ticket.CustomerId);
                PrintLineToReceipt(printer, person.FirstName + " " + person.LastName);
                if ((ticket.Type == TicketType.Delivery) ||
                    (ticket.Type == TicketType.Catering))
                {
                    ZipCode zipCode = ZipCode.Get(person.ZipCodeId);
                    ZipCodeCity zipCodeCity = ZipCodeCity.Get(zipCode.CityId);
                    ZipCodeState zipCodeState = ZipCodeState.Get(zipCodeCity.StateId);
                    PrintLineToReceipt(printer, person.AddressLine1);
                    if (!string.IsNullOrEmpty(person.AddressLine2))
                        PrintLineToReceipt(printer, person.AddressLine2);
                    PrintLineToReceipt(printer, zipCodeCity.City + ", " +
                        zipCodeState.Abbreviation + " " +
                        zipCode.PostalCode.ToString("D5"));
                }
                if (person.PhoneNumberId1 > 0)
                {
                    PhoneNumber phoneNumber = PhoneNumber.Get(person.PhoneNumberId1);
                    PrintLineToReceipt(printer, phoneNumber.GetFormattedPhoneNumber());
                }

                // Newline
                PrintLineToReceipt(printer);
            }
        }

        private static double[] PrintTicketItemToReceipt(PosPrinter printer,
            TicketItem ticketItem, bool showMakeTags, TicketItemPrintOptions printOptions,
            TicketType ticketType)
        {
            Item item = Item.Get(ticketItem.ItemId);
            int quantity = ticketItem.Quantity;

            // Reasons to skip this TicketItem
            if ((printOptions == TicketItemPrintOptions.OnlyChanged) &&
                !ticketItem.IsChanged &&
                (ticketItem.OrderTime != null))
                return null;
            if ((printOptions == TicketItemPrintOptions.AllNonCanceled) &&
                ticketItem.IsCanceled)
                return null;
            if ((printOptions == TicketItemPrintOptions.AllUnfired) &&
                (ticketItem.IsCanceled || !item.IsFired || (ticketItem.FireTime != null)))
                return null;

            // Return verse actual quantity
            if (printOptions == TicketItemPrintOptions.TicketItemReturn)
                quantity = ticketItem.QuantityPendingReturn;

            // Setup make-tags
            string cost = null;
            if (showMakeTags || (printOptions == TicketItemPrintOptions.AllAsVoid) ||
                (printOptions == TicketItemPrintOptions.TicketItemVoid))
            {
                if (ticketItem.IsCanceled && ticketItem.PreparedTime.HasValue)
                    cost = "[" + Strings.CancelMade + "]";
                else if (ticketItem.IsCanceled)
                    cost = "[" + Strings.CancelUnmade  + "]";
                else if (showMakeTags)
                {
                    if ((ticketType == TicketType.DineIn) && (item.IsFired))
                    {
                        cost = (printOptions != TicketItemPrintOptions.AllUnfired)
                            ? Strings.Hold : Strings.Fired;
                    }
                    else if (ticketItem.OrderTime == null)
                        cost = Strings.Make;
                    else
                        cost = Strings.Changed;
                }
            }
            
            // Actually want to display cost, and not a make-tag
            if (cost == null)
                cost = ticketItem.GetTotalCost(ticketItem.QuantityPendingReturn).
                    ToString("C2");

            // Print the item's quantity, name, and cost
            PrintLineToReceipt(printer,
                quantity +
                PrinterEscapeCodes.SetSize(5) +
                item.FullName +
                PrinterEscapeCodes.SetRight +
                cost);

            if (printOptions != TicketItemPrintOptions.TicketItemReturn)
            {
                // Print TicketItemOption(s) for this TicketItem
                PrintTicketItemOptions(printer, ticketItem);

                if (ticketItem.SpecialInstructions != null)
                {
                    // ToDo: This line needs to wrap if receipt tape is not wide enough
                    // for the entire special comment
                    PrintLineToReceipt(printer,
                        PrinterEscapeCodes.SetSize(5) +
                        "+[" +
                        ticketItem.SpecialInstructions +
                        "]");
                }
            }
            else
            {
                double returnAmount = ticketItem.GetTotalCost(ticketItem.QuantityPendingReturn);
                double couponAmount = ticketItem.GetTotalCoupon(returnAmount);
                double discountAmount = ticketItem.GetTotalDiscount(returnAmount);
                returnAmount -= couponAmount - discountAmount;
                double taxAmount = ticketItem.GetTax(returnAmount);
                if (couponAmount > 0)
                    PrintLineToReceipt(printer, PrinterEscapeCodes.SetSize(5) + Strings.Coupons +
                        PrinterEscapeCodes.SetRight + couponAmount.ToString("C2"));
                if (discountAmount > 0)
                    PrintLineToReceipt(printer, PrinterEscapeCodes.SetSize(5) + Strings.Discounts +
                        PrinterEscapeCodes.SetRight + discountAmount.ToString("C2"));
                return new [] {
                    returnAmount,
                    taxAmount
                };
            }
            return null;
        }

        private static void PrintTicketItemOptions(PosPrinter printer, TicketItem ticketItem)
        {
            Item item = Item.Get(ticketItem.ItemId);

            // Get the option sets for this category
            List<ItemOption> options1 = new List<ItemOption>(ItemOption.GetInSet(item.ItemOptionSetIds[0]));
            List<ItemOption> options2 = new List<ItemOption>(ItemOption.GetInSet(item.ItemOptionSetIds[1]));
            List<ItemOption> options3 = new List<ItemOption>(ItemOption.GetInSet(item.ItemOptionSetIds[2]));

            // Get the options for this ticket item's category
            List<TicketItemOption> ticketItemOptions = new List<TicketItemOption>(
                TicketItemOption.GetAll(ticketItem.PrimaryKey));

            if (options1.Count > 0)
            {
                string line = "";
                if (ProcessItemOption(ticketItemOptions, options1, ref line))
                    PrintLineToReceipt(printer, PrinterEscapeCodes.SetSize(5) +
                        "[" + line + "]");
            }
            if (options2.Count > 0)
            {
                string line = "";
                if (ProcessItemOption(ticketItemOptions, options2, ref line))
                    PrintLineToReceipt(printer, PrinterEscapeCodes.SetSize(5) +
                        "[" + line + "]");
            }
            if (options3.Count > 0)
            {
                string line = "";
                if (ProcessItemOption(ticketItemOptions, options3, ref line))
                    PrintLineToReceipt(printer, PrinterEscapeCodes.SetSize(5) +
                        "[" + line + "]");
            }
        }

        private static bool ProcessItemOption(IEnumerable<TicketItemOption> ticketItemOptions,
            IEnumerable<ItemOption> inSetOptions, ref string itemOptions)
        {
            bool found = false;
            foreach (TicketItemOption ticketItemOption in ticketItemOptions)
            {
                foreach (ItemOption itemOption in inSetOptions)
                {
                    if (itemOption.Id == ticketItemOption.ItemOptionId)
                    {
                        if (found)
                            itemOptions += ", ";
                        found = true;
                        itemOptions += itemOption.Name;
                    }
                }
            }
            return found;
        }

        private static void PrintFooterInformation(PosPrinter printer, Ticket selectedTicket)
        {
            double couponAmount = selectedTicket.GetCouponTotal();
            double discountAmount = selectedTicket.GetDiscountTotal();
            PrintLineToReceipt(printer);
            PrintLineToReceipt(printer, Strings.Subtotal + PrinterEscapeCodes.SetRight + selectedTicket.GetSubTotal().ToString("C2"));
            if (couponAmount > 0)
                PrintLineToReceipt(printer, Strings.Coupons + PrinterEscapeCodes.SetRight + couponAmount.ToString("C2"));
            if (discountAmount > 0)
                PrintLineToReceipt(printer, Strings.Discounts + PrinterEscapeCodes.SetRight + discountAmount.ToString("C2"));
            PrintLineToReceipt(printer, Strings.Tax + PrinterEscapeCodes.SetRight + selectedTicket.GetTax().ToString("C2"));
            PrintLineToReceipt(printer, Strings.Total + PrinterEscapeCodes.SetRight + selectedTicket.GetTotal().ToString("C2"));
        }
#endif
        #endregion

        #region Unused
        private static void LoadFormatsDocument()
        {
            FormatsXmlDocument = new XmlDocument { PreserveWhitespace = false };
            string text = (string)ConfigurationManager.GetSetting("PrinterReceiptFormats.xml") ?? "";
            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
            XmlTextReader textReader = new XmlTextReader(stream);
            FormatsXmlDocument.Load(textReader);
        }

        private static void ParseDemo()
        {
            XmlNode node = FormatsXmlDocument.FirstChild;
            while (node != null)
            {
                if (node.LocalName.Equals("schema"))
                {
                    //ProcessSchemaChildNodes(FormatsXmlDocument, node);
                }
                node = node.NextSibling;
            }
        }

        /*
        private static void ProcessSimpleTypeNode(XmlNode node)
        {
            XmlAttribute attrib = node.Attributes["name"];
            List<string> list = new List<string>();
            XmlNode child = node.FirstChild.FirstChild;
            while (child != null)
            {
                list.Add(child.Attributes[0].Value);
                child = child.NextSibling;
            }
            XmlAttribute baseAttribute = node.FirstChild.Attributes["base"];
            Restriction restriction = new Restriction(baseAttribute.Value, list.ToArray());
            //list.Sort();
            if (attrib.Value.Equals("FRAMEPOINT"))
                FramePoints = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("FRAMESTRATA"))
                FrameStratas = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("DRAWLAYER"))
                DrawLayers = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ALPHAMODE"))
                AlphaModes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("OUTLINETYPE"))
                OutlineTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("JUSTIFYVTYPE"))
                JustifyVTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("JUSTIFYHTYPE"))
                JustifyVTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("INSERTMODE"))
                InsertModes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ORIENTATION"))
                Orientations = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ATTRIBUTETYPE"))
                Orientations = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ANIMLOOPTYPE"))
                AnimLoopTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ANIMSMOOTHTYPE"))
                AnimSmoothTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ANIMCURVETYPE"))
                AnimCurveTypes = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("AnimOrderType"))
                AnimOrderType = new SimpleType(attrib.Value, baseAttribute.Value, restriction);
            else if (attrib.Value.Equals("ColorFloat"))
                ColorFloat = new SimpleType(attrib.Value, baseAttribute.Value, list[0], list[1]);
            //else
            //    throw new Exception(Strings.Unhandled + attrib.Value);
        }
        */
        public static XmlDocument FormatsXmlDocument
        {
            get;
            private set;
        }
        #endregion
    }

    public enum TicketItemPrintOptions
    {
        All,
        AllNonCanceled,
        AllAsVoid,
        AllAsCancelUnmade,
        AllAsCancelMade,
        OnlyChanged,
        TicketItemVoid,
        TicketItemReturn,
        TicketRefund,
        AllUnfired
    }

}
