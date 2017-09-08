using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PosControls;
using PosControls.Helpers;
using PosControls.Interfaces;
using PosModels;
using TemPOS.Types;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS.Helpers
{
    /// <summary>
    /// This static class contains functions that are used to simplifiy 
    /// common actions and maintain database referencal integrity.
    /// </summary>
    public static class PosHelper
    {
        #region Confirmation Prompts
        public static bool ConfirmReturnPrompt()
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToReturnTheSelectedTicketItems, Types.Strings.ReturnTicketItems,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        public static TicketRefundType? ConfirmRefundPrompt()
        {
            PosDialogWindow window = CancelMadeUnmadeControl.CreateInDefaultWindow(Types.Strings.RefundTicket, true);
            var control = window.DockedControl as CancelMadeUnmadeControl;
            if (control == null) return null;
            window.IsClosable = false;
            PosDialogWindow.ShowPosDialogWindow(window);
            return control.RefundMode;
        }

        public static bool? ConfirmCancelPrompt()
        {
            PosDialogWindow window = CancelMadeUnmadeControl.CreateInDefaultWindow(Types.Strings.CancelTicket);
            var control = window.DockedControl as CancelMadeUnmadeControl;
            if (control == null) return null;
            window.IsClosable = false;
            PosDialogWindow.ShowPosDialogWindow(window);
            return control.IsMade;
        }

        public static bool? ConfirmCancelPrompt(TicketItem ticketItem)
        {
            PosDialogWindow window = CancelMadeUnmadeControl.CreateInDefaultWindow(Types.Strings.CancelTicketItem);
            var control = window.DockedControl as CancelMadeUnmadeControl;
            if (control == null) return null;
            window.IsClosable = false;
            PosDialogWindow.ShowPosDialogWindow(window);
            return control.IsMade;
        }

        public static bool ConfirmEarlyCancelPrompt()
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToCancelThisTicket, Types.Strings.CancelTicket,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        public static bool ConfirmEarlyCancelPrompt(TicketItem ticketItem)
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToCancelThisTicketItem, Types.Strings.CancelTicketItem,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        private static bool ConfirmUnCancelPrompt()
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToUncancelThisTicket, Types.Strings.UncancelTicket,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        private static bool ConfirmUnCancelPrompt(TicketItem ticketItem)
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToUncancelThisTicketItem, Types.Strings.UncancelTicketItem,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        public static bool ConfirmVoidPrompt()
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToVoidThisTicket, Types.Strings.VoidTicket,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }

        public static bool ConfirmVoidPrompt(TicketItem ticketItem)
        {
            bool result = (PosDialogWindow.ShowDialog(
                Types.Strings.AreYouSureYouWantToVoidThisTicketItem, Types.Strings.VoidTicket,
                DialogButtons.YesNo) == DialogButton.Yes);
            return result;
        }
        #endregion

        #region Coupons and Discounts
        public static void ClearCoupons(TicketItem ticketItem)
        {
            TicketCoupon.DeleteForTicketItem(ticketItem.PrimaryKey);
        }

        public static void ClearTicketCoupons(YearId ticketPrimaryKey)
        {
            IEnumerable<TicketCoupon> coupons = TicketCoupon.GetAll(ticketPrimaryKey);
            foreach (TicketCoupon coupon in coupons)
            {
                TicketCoupon.Delete(coupon.PrimaryKey);
            }
        }

        public static void ClearDiscounts(TicketItem ticketItem)
        {
            TicketDiscount.DeleteForTicketItem(ticketItem.PrimaryKey);
        }

        public static void ClearTicketDiscounts(YearId ticketPrimaryKey)
        {
            IEnumerable<TicketDiscount> discounts = TicketDiscount.GetAll(ticketPrimaryKey);
            foreach (TicketDiscount discount in discounts)
            {
                TicketDiscount.Delete(discount.PrimaryKey);
            }
        }
        #endregion

        #region Command functions for ticket voids, cancels, and refunds (public)
        // Rule: This should not be called on a cashed-out ticket. A ticket needs to
        // be refunded before it can be voided.
        public static bool VoidTicketCommand(Ticket selectedTicket)
        {
            IEnumerable<TicketPayment> ticketPayments =
                TicketPayment.GetAll(selectedTicket.PrimaryKey);
            if (ticketPayments.Any())
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouCantVoidATicketThatIsCashedoutTheTicketMustBeRefundedFirst,
                    Types.Strings.VoidError);
            }
            else if (ConfirmVoidPrompt())
            {
                VoidTicket(selectedTicket);
                return true;
            }
            return false;
        }

        public static bool CancelTicketCommand(Ticket selectedTicket)
        {
            bool? cancelMade = ConfirmCancelPrompt();
            if (cancelMade != null)
            {
                CancelTicket(selectedTicket, cancelMade.Value);
                return true;
            }
            return false;
        }

        public static bool UnCancelTicketCommand(Ticket selectedTicket)
        {
            if (ConfirmUnCancelPrompt())
            {
                selectedTicket.UnCancel();
                return true;
            }
            return false;
        }

        public static double RefundTicketCommand(Ticket selectedTicket, TicketRefundType refundType)
        {
            double refundAmount = RefundTicket(selectedTicket, refundType);

            if (refundType == TicketRefundType.Reopened)
            {
                selectedTicket.SetCloseTime(null);
                selectedTicket.Update();
            }
            else if (refundType == TicketRefundType.CancelMade)
            {
                CancelTicket(selectedTicket, true);
            }
            else if (refundType == TicketRefundType.CancelUnmade)
            {
                CancelTicket(selectedTicket, false);
            }
            else if (refundType == TicketRefundType.Void)
            {
                VoidTicket(selectedTicket);
            }
            return refundAmount;
        }
        #endregion

        #region Core Functions for ticket voids, cancels, and refunds (private)
        private static void VoidTicket(Ticket selectedTicket)
        {
            // Print out hard copys for record keeping
            PrinterManager.Print(selectedTicket, TicketItemPrintOptions.AllAsVoid);

            double amount = selectedTicket.GetSubTotal();
            TicketVoid.Add(SessionManager.ActiveEmployee.Id, selectedTicket.PrimaryKey, null, amount);

            ClearTicketCoupons(selectedTicket.PrimaryKey);
            ClearTicketDiscounts(selectedTicket.PrimaryKey);

            TicketDelivery.DeleteByTicket(selectedTicket.PrimaryKey);
            foreach (TicketItem ticketItem in
                TicketItem.GetAll(selectedTicket.PrimaryKey))
            {
                bool updateInventory = !(ticketItem.IsCanceled && !ticketItem.IsWasted);
                ticketItem.Delete(updateInventory);
            }
            TicketManager.Delete(selectedTicket.PrimaryKey);
        }

        private static double RefundTicket(Ticket selectedTicket, TicketRefundType refundType)
        {
            double sumOfPayments = TicketPayment.GetAll(selectedTicket.PrimaryKey)
                .Sum(payment => payment.Amount);
            
            // Don't refund money for items that were already returned
            sumOfPayments = TicketItemReturn.GetAllForTicket(selectedTicket.PrimaryKey)
                .Aggregate(sumOfPayments, (current, ticketItemReturn) => current - ticketItemReturn.Amount);
            
            RegisterManager.ActiveRegisterDrawer.RemoveFromCurrentAmount(sumOfPayments);
            TicketPayment.DeleteAll(selectedTicket.PrimaryKey);
            TicketRefund.Add(selectedTicket.PrimaryKey, SessionManager.ActiveEmployee.Id,
                RegisterManager.ActiveRegisterDrawer.Id, sumOfPayments, refundType);
            return sumOfPayments;
        }

        private static void CancelTicket(Ticket selectedTicket, bool wasMade)
        {
            // Print out hard copys for record keeping
            PrinterManager.Print(selectedTicket, (wasMade ?
                TicketItemPrintOptions.AllAsCancelMade :
                TicketItemPrintOptions.AllAsCancelUnmade));

            selectedTicket.Cancel(SessionManager.ActiveEmployee.Id, wasMade);
            ClearTicketCoupons(selectedTicket.PrimaryKey);
            ClearTicketDiscounts(selectedTicket.PrimaryKey);
        }
        #endregion

        #region Core functions for ticket item cancels, voids, and returns
        public static void CancelTicketItem(TicketItem ticketItem, bool cancelMade)
        {
            var ticketItems = new List<TicketItem>(
                TicketItem.GetAllChildTicketItems(ticketItem.PrimaryKey));
            ticketItems.Add(ticketItem);
            foreach (TicketItem currentTicketItem in ticketItems)
            {
                currentTicketItem.Cancel(CancelType.TicketItemCancel,
                                  SessionManager.ActiveEmployee.Id, cancelMade);
            }
            ClearCoupons(ticketItem);
            ClearDiscounts(ticketItem);
        }
        
        public static void VoidTicketItem(TicketItem ticketItem)
        {
            List<TicketItem> ticketItems = new List<TicketItem>(
                TicketItem.GetAllChildTicketItems(ticketItem.PrimaryKey));
            ticketItems.Add(ticketItem);
            foreach (TicketItem currentTicketItem in ticketItems)
            {
                // Delete the ticket item and it's ticket item options
                currentTicketItem.Delete();
            }
            ClearCoupons(ticketItem);
            ClearDiscounts(ticketItem);

        }
        public static void ReturnTicketItem(TicketItem ticketItem, double amount)
        {
            TicketItemReturn.Add(RegisterManager.ActiveRegisterDrawer.Id,
                SessionManager.ActiveEmployee.Id, SessionManager.ActiveTicket.PrimaryKey,
                ticketItem.ItemId, ticketItem.QuantityPendingReturn,
                amount);
            //AdjustInventory(ticketItem, increase)
        }
        #endregion

        #region Locks
        public static bool IsLocked(TableName lockType, int tableId)
        {
#if DEMO
            return false;
#else
            PosModels.Lock tableLock = PosModels.Lock.Get(lockType, tableId);
            if (tableLock == null)
                return false;
            if (lockType == TableName.Employee || SessionManager.ActiveEmployee == null)
                return true;
            return (tableLock.EmployeeId != SessionManager.ActiveEmployee.Id);
#endif
        }

        public static bool IsLocked(Ticket ticket)
        {
            if (ticket == null)
                return false;
            return IsLocked(TableName.Ticket, ticket.PrimaryKey.Id);
        }

        public static bool IsLocked(Party party)
        {
            if (party == null)
                return false;
            return IsLocked(TableName.Party, party.Id);
        }

        public static void Lock(TableName lockType, int tableId, int employeeId)
        {
#if !DEMO
            PosModels.Lock.Add(lockType, tableId, employeeId, null);
#endif
        }

        public static void Lock(Ticket ticket, Employee employee)
        {
            Lock(TableName.Ticket, ticket.PrimaryKey.Id, employee.Id);
        }

        public static void Lock(Party party, Employee employee)
        {
            Lock(TableName.Party, party.Id, employee.Id);
        }

        public static void Unlock(TableName lockType, int tableId)
        {
            PosModels.Lock.Delete(lockType, tableId);
        }

        public static void Unlock(Ticket ticket)
        {
            if (ticket != null)
                Unlock(TableName.Ticket, ticket.PrimaryKey.Id);
        }

        public static void Unlock(Party party)
        {
            if (party != null)
                Unlock(TableName.Party, party.Id);
        }
        #endregion

        #region SumOf Functions
        public static double GetSumOfPayouts(IEnumerable<RegisterPayout> payouts)
        {
            return payouts == null ? 0 : payouts.Sum(payout => payout.Amount);
        }

        public static double GetSumOfDrops(IEnumerable<RegisterDrop> safeDrops)
        {
            return safeDrops.Sum(drop => drop.Amount);
        }

        public static double GetSumOfDeposits(IEnumerable<RegisterDeposit> deposits)
        {
            return deposits.Sum(deposit => deposit.Amount);
        }

        public static double GetSumOfRefunds(IEnumerable<TicketRefund> refunds)
        {
            return refunds.Sum(refund => refund.Amount);
        }

        public static double GetSumOfReturns(IEnumerable<TicketItemReturn> returns)
        {
            return returns.Sum(itemReturn => itemReturn.Amount);
        }

        public static double GetSumOfPayments(IEnumerable<TicketPayment> payments, PaymentSource? source = null)
        {
            return payments.Where(ticketPayment => (source == null) || (ticketPayment.PaymentType == source)).Sum(ticketPayment => ticketPayment.Amount);
        }

        #endregion

        #region Start-of-Day, End-of-Day, End-of-Year
        public static void EndOfDay()
        {
            if (TicketManager.GetOpenTickets().Any())
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouCanNotRunTheEndofdayReportUntilAllOpenTicketsAreClosedout,
                    Types.Strings.EndofdayReport, DialogButtons.Ok);
                return;
            }
            if (RegisterManager.OpenRegisterExists)
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouCanNotRunTheEndofdayReportUntilYouCloseoutAllActiveRegisterDrawers,
                    Types.Strings.EndofdayReport, DialogButtons.Ok);
                return;
            }
            DialogButton dialogButton = PosDialogWindow.ShowDialog(
                Types.Strings.TheEndOfDayReportShouldOnlyBeRunAtTheEndOfTheDay +
                Types.Strings.AreYouSureYouWantToRunThisReport, Types.Strings.Confirmation, DialogButtons.YesNo);
            if (dialogButton == DialogButton.Yes)
                PrintEndOfDay(MainWindow.Singleton);
        }

        private static IShadeable _endOfDayReportParentWindow;
        private static void PrintEndOfDay(IShadeable parentWindow)
        {
            // Setup shading overlay
            _endOfDayReportParentWindow = parentWindow;
            _endOfDayReportParentWindow.ShowShadingOverlay = true;

            // Reset order id's and process the DayOfOperation
            DayOfOperation today = DayOfOperation.Today;
            DayOfOperation.SetTodayNull();
            TicketManager.SetOrderIdOffset();
            DayOfOperation.ProcessEndOfDay(today, SessionManager.ActiveEmployee.Id,
                TicketManager.GetOrderIdOffset());

            OrderEntryCommands.SetupNoOrderCommands();

            // Show Report
            ReportManager.PrintEndOfDay(today, EODReportClosed_EventHandler);
        }

        private static void EODReportClosed_EventHandler(object sender, EventArgs args)
        {
            // Remove shading overlay
            _endOfDayReportParentWindow.ShowShadingOverlay = false;
            _endOfDayReportParentWindow = null;
        }

        public static void EndOfYear()
        {
            DialogButton dialogButton = PosDialogWindow.ShowDialog(
                Types.Strings.TheEndOfYearReportShouldOnlyBeRunAtTheBeginningOfANewYear +
                Types.Strings.AreYouSureYouWantToRunThisReport, Types.Strings.Confirmation, DialogButtons.YesNo);
            if (dialogButton == DialogButton.Yes)
                PrintEndOfYear(MainWindow.Singleton);
        }

        private static IShadeable _endOfYearReportParentWindow;
        private static void PrintEndOfYear(IShadeable parentWindow)
        {
            _endOfYearReportParentWindow = parentWindow;
            _endOfYearReportParentWindow.ShowShadingOverlay = true;

            Ticket.ResetAutoIdentity();
            TicketItem.ResetAutoIdentity();
            TicketItemOption.ResetAutoIdentity();
            SettingManager.SetStoreSetting("DailyIdOffset", 0);
            OrderEntryCommands.SetupNoOrderCommands();

            // Show Report
            ReportManager.PrintEndOfYear(DayOfOperation.YearOfLastStartOfDay, EOYReportClosed_EventHandler);
        }

        private static void EOYReportClosed_EventHandler(object sender, EventArgs args)
        {
            // Remove shading overlay
            _endOfYearReportParentWindow.ShowShadingOverlay = false;
            _endOfYearReportParentWindow = null;
        }


        public static void StartOfDay()
        {
            DayOfOperation.ProcessStartOfDay();
#if !DEMO
            PrinterManager.PrintLineToReceipt(DeviceManager.ActivePosPrinterLocal,
                Strings.StartOfDay + ": " + DateTime.Now);
            PrinterManager.PrintLineToReceipt(DeviceManager.ActivePosPrinterLocal);
#endif
            OrderEntryCommands.SetupNoOrderCommands();
        }
        #endregion

        #region Ordering processing and firing
        public static void UpdateTicketItems(Ticket ticket, bool setOrderTime = false)
        {
            foreach (TicketItem ticketItem in
                TicketItem.GetAll(ticket.PrimaryKey, false))
            {
                // Log a decrease in quantity
                LogDecreasedTicketItemLateCancels(ticket, ticketItem);

                // Mark the item's order time
                if (setOrderTime)
                    ticketItem.SetOrderTime(DateTime.Now);
                ticketItem.Update();
            }
        }

        private static void LogDecreasedTicketItemLateCancels(Ticket ticket, TicketItem ticketItem)
        {
            int originalQuantity = ticketItem.GetCurrentQuantity();
            if (ticketItem.QuantityPending.HasValue && (ticketItem.QuantityPending < originalQuantity))
            {
                int difference = originalQuantity - ticketItem.QuantityPending.Value;
                Item item = Item.Get(ticketItem.ItemId);
                PosDialogWindow window =                    
                    CancelMadeUnmadeControl.CreateInDefaultWindow("Cancel: " + item.FullName +
                    " (" + difference + ")");
                var control = window.DockedControl as CancelMadeUnmadeControl;
                window.ShowDialog();
                if (control != null && !control.IsMade.HasValue)
                {
                    ticketItem.SetQuantity(originalQuantity);
                }
                else
                {
                    TicketItem canceledTicketItem = 
                        TicketItem.Add(ticket.PrimaryKey, ticketItem.ItemId, difference, ticketItem.Price,
                        ticketItem.OrderTime, ticketItem.PreparedTime);
                    DuplicateItemOptions(ticketItem, canceledTicketItem);
                    Employee employee = SessionManager.PseudoEmployee ?? SessionManager.ActiveEmployee;
                    canceledTicketItem.Cancel(CancelType.DecreasedQuantity,
                        employee.Id, control != null && control.IsMade.Value);
                }
            }
        }

        private static void DuplicateItemOptions(TicketItem ticketItem, TicketItem canceledTicketItem)
        {
            foreach (TicketItemOption ticketItemOption in
                TicketItemOption.GetAll(ticketItem.PrimaryKey))
            {
                TicketItemOption.Add(canceledTicketItem.PrimaryKey.Id,
                    ticketItemOption.ItemOptionId, ticketItemOption.Type,
                    ticketItemOption.ChangeCount);
            }
        }

        public static void FireEntrees(Ticket ticket)
        {
            PrinterManager.Print(ticket, TicketItemPrintOptions.AllUnfired);
            foreach (TicketItem ticketItem in
                from ticketItem in TicketItem.GetAll(ticket.PrimaryKey)
                let item = Item.Get(ticketItem.ItemId)
                where item.IsFired select ticketItem)
            {
                ticketItem.Fire();
            }
        }
        #endregion

        #region Register Related
        public static bool CashOutTicket(Ticket ticket)
        {
            PosDialogWindow window = TicketCashoutControl.CreateInDefaultWindow();
            var control = window.DockedControl as TicketCashoutControl;
            if (control == null) return false;
            control.SelectedTicket = ticket;

            PosDialogWindow.ShowPosDialogWindow(window);

            // Ticket was cashed-out
            if (ticket.CloseTime != null)
            {
                OrderEntryControl.InitializeTicketSelection(null);
                return true;
            }
            return false;
        }
        #endregion

        #region Permission Prompting
        public static Employee GetPermission(Permissions permission)
        {
            if (SessionManager.PseudoEmployee != null)
                return SessionManager.PseudoEmployee;

            Employee employee = null;
            Window window = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            if (window is IShadeable)
                (window as IShadeable).ShowShadingOverlay = true;
            string password = PosDialogWindow.PromptKeyboard(Types.Strings.PermissionRequiredEnterPassword, "", true, ShiftMode.None);
            if (window is IShadeable)
                (window as IShadeable).ShowShadingOverlay = false;
            if (App.IsAppShuttingDown)
                return null;
            
            if (password != null)
                employee = EmployeeManager.LookupByScanCode(password);
            
            if ((employee == null) || !employee.HasPermission(permission))
                return null;

            SessionManager.PseudoEmployee = employee;
            return employee;
        }
        #endregion

    }
}
