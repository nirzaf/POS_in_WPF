using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using PosControls;
using PosModels;
using PosModels.Managers;
using TemPOS.Types;
using PosModels.Types;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS.Commands
{
    [Obfuscation(Exclude = true)]
    public static class OrderEntryCommands
    {
        #region Licensed Access Only / Static Initializer
        static OrderEntryCommands()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(OrderEntryCommands).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        #region Private Command Classes
        #region ReportsMenuCommand
        private class ReportsMenuCommand : CommandBase
        {
            public ReportsMenuCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                       (SessionManager.ActiveTicket == null) &&
                       (SessionManager.ActiveEmployee.HasPermission(Permissions.ReportsMenu));
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = ReportsMenuControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                return true;
            }
        }
        #endregion

        #region ChangePasswordCommand
        private class ChangePasswordCommand : CommandBase
        {
            public ChangePasswordCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) && (SessionManager.ActiveTicket == null);
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = ChangePasswordControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                return true;
            }
        }
        #endregion

        #region ExitCommand
        private class ExitCommand : CommandBase
        {
            public ExitCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                       (SessionManager.ActiveTicket == null) &&
                       (SessionManager.ActiveEmployee.HasPermission(Permissions.ExitProgram));
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = ExitControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                return true;
            }
        }
        #endregion

        #region LogoutCommand
        private class LogoutCommand : CommandBase
        {
            public LogoutCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) && (SessionManager.ActiveTicket == null);
            }

            protected override bool Execute(ref object parameter)
            {
                Lock.DeleteAllEmployeeLocks(SessionManager.ActiveEmployee.Id);
                SessionManager.Reset();
                return true;
            }
        }
        #endregion

        #region PersonalSettingsCommand
        private class PersonalSettingsCommand : CommandBase
        {
            public PersonalSettingsCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
#if !DEBUG
                return false;
#else
                return (SessionManager.ActiveEmployee != null) && (SessionManager.ActiveTicket == null);
#endif
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = PersonalSettingsControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                return true;
            }
        }
        #endregion

        #region ClockOutCommand
        private class ClockOutCommand : CommandBase
        {
            public ClockOutCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) && (SessionManager.ActiveTicket == null);
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = EmployeeClockOutControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                if (window.ClosedByUser)
                    return false;
                Logout.Execute(this);
                return true;
            }
        }
        #endregion

        #region SetupMenuCommand
        private class SetupMenuCommand : CommandBase
        {
            public SetupMenuCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (parameter is FrameworkElement) &&
                    ((SessionManager.ActiveEmployee != null) &&
                    SessionManager.ActiveEmployee.HasPermission(new []
                    {
                        Permissions.SystemMaintenance,
                        Permissions.VendorMaintenance,
                        Permissions.EmployeeMaintenance
                    })
                    && (SessionManager.ActiveTicket == null));
            }

            protected override bool Execute(ref object parameter)
            {
                var element = (FrameworkElement) parameter;
                if (element.ContextMenu == null) return false;
                element.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.IsOpen = true;
                return true;
            }
        }
        #endregion

        #region SystemMenuCommand
        private class SystemMenuCommand : CommandBase
        {
            public SystemMenuCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (parameter is FrameworkElement) &&
                    ((SessionManager.ActiveEmployee != null) &&
                    SessionManager.ActiveEmployee.HasPermission(new[]
                    {
                        Permissions.EmployeeMaintenance, Permissions.EmployeeScheduleMaintenance,
                        Permissions.SystemMaintenance, Permissions.VendorMaintenance
                    }) &&
                    (SessionManager.ActiveTicket == null));
            }

            protected override bool Execute(ref object parameter)
            {
                var element = (FrameworkElement)parameter;
                if (element.ContextMenu == null) return false;
                element.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.IsOpen = true;
                return true;
            }
        }
        #endregion

        #region RegisterMenuCommand
        private class RegisterMenuCommand : CommandBase
        {
            public RegisterMenuCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (parameter is FrameworkElement) &&
                    ((SessionManager.ActiveEmployee != null) &&
                    SessionManager.ActiveEmployee.HasPermission(new[]
                    {
                        Permissions.RegisterDeposit, 
                        Permissions.RegisterNoSale, 
                        Permissions.RegisterClose,
                        Permissions.RegisterReport
                    }) &&
                    (SessionManager.ActiveTicket == null));
            }

            protected override bool Execute(ref object parameter)
            {
                var element = (FrameworkElement)parameter;
                if (element.ContextMenu == null) return false;
                element.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.IsOpen = true;
                return true;
            }

        }
        #endregion

        #region ConsoleCommand
        private class ConsoleCommand : CommandBase
        {
            public ConsoleCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
#if DEMO
                return false;
#else
                return (SessionManager.ActiveEmployee != null) &&
                    SessionManager.ActiveEmployee.HasPermission(Permissions.CommandShell) &&
                    (SessionManager.ActiveTicket == null);
#endif
            }

            protected override bool Execute(ref object parameter)
            {
                PosDialogWindow window = CommandShellControl.CreateInDefaultWindow();
                PosDialogWindow.ShowPosDialogWindow(window);
                return true;
            }
        }
        #endregion

        #region ChangeTicketSeatingCommand
        private class ChangeTicketSeatingCommand : CommandBase
        {
            public ChangeTicketSeatingCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&

                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed) ||

                    ((SessionManager.ActiveTicket == null) &&
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed));
            }

            protected override bool Execute(ref object parameter)
            {
                return (SessionManager.ActiveTicket != null) ? ExecuteInOrder() : ExecuteNonOrder();
            }

            private bool ExecuteInOrder()
            {
                PosDialogWindow window = SeatingSelectionControl.CreateInDefaultWindow();
                var control = window.DockedControl as SeatingSelectionControl;
                if (control == null) return false;

                control.InitializeFromTicket(SessionManager.ActiveTicket);

                // Show the dialog
                PosDialogWindow.ShowPosDialogWindow(window);

                if (window.ClosedByUser) return false;

                // Process Results
                if (SessionManager.ActiveTicket != null)
                {
                    SessionManager.ActiveTicket.SetSeatingId(control.SelectedSeatingId);
                    SessionManager.ActiveTicket.SetType(control.TicketType);
                    if (control.SelectedCustomer != null)
                        SessionManager.ActiveTicket.SetCustomerId(control.SelectedCustomer.Id);
                }
                if (control.TicketType == TicketType.DineIn)
                    OrderEntryControl.SetSeating(control.SelectedTableName);
                else if (SessionManager.ActiveTicket != null)
                    OrderEntryControl.SetSeating(SessionManager.ActiveTicket.Type.GetFriendlyName(),
                                control.personInformationControl.textBoxCustomerName.Text);
                TextBlock textBlockOrderAmount = MainWindow.Singleton.orderEntryControl.textBlockOrderAmount;
                OrderEntryReceiptTape receiptTape1 = MainWindow.Singleton.orderEntryControl.receiptTape1;
                textBlockOrderAmount.Text = Types.Strings.OrderEntrySubTotal +
                                            receiptTape1.TicketSubTotal.ToString("C2");
                if (SessionManager.ActiveTicket != null)
                {
                    SessionManager.ActiveTicket.Update();
                }
                else
                {
                    SessionManager.ActiveTicket = TicketManager.Add(
                        control.TicketType, Party.NoPartyId,
                        control.SelectedSeatingId,
                        SessionManager.ActiveEmployee.Id,
                        ((control.SelectedCustomer != null)
                                ? control.SelectedCustomer.Id
                                : Customer.NoCustomerId));
                    OrderEntryControl.ShowTicketSelection(false);
                    SetupInOrderCommands();
                }
                return true;
            }

            private bool ExecuteNonOrder()
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;

                if (details.SelectedTicket == null)
                    return false;

                // Create the seating selection control
                PosDialogWindow window = SeatingSelectionControl.CreateInDefaultWindow();
                var control = window.DockedControl as SeatingSelectionControl;
                if (control == null) return false;
                control.InitializeFromTicket(details.SelectedTicket);

                // Show the dialog            
                PosDialogWindow.ShowPosDialogWindow(window);

                if (!window.ClosedByUser)
                {
                    if (details.SelectedTicket != null)
                    {
                        if (control.SelectedCustomer != null)
                            details.SelectedTicket.SetCustomerId(control.SelectedCustomer.Id);
                        details.SelectedTicket.SetType(control.TicketType);
                        details.SelectedTicket.SetSeatingId(control.SelectedSeatingId);
                        details.SelectedTicket.Update();
                    }
                }

                OrderEntryControl.InitializeTicketSelection(details.SelectedTicket);
                return true;
            }
        }
        #endregion

        #region CouponCommand
        private class CouponCommand : CommandBase
        {
            public CouponCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed;
            }

            protected override bool Execute(ref object parameter)
            {
                if (!(parameter is TextBlockButton)) return false;
                var button = (TextBlockButton) parameter;
                TicketCouponControl couponEntryControl =
                    MainWindow.Singleton.orderEntryControl.couponEntryControl;
                bool show = (couponEntryControl.Visibility == Visibility.Hidden);
                button.IsChecked = show;
                MainWindow.Singleton.orderEntryControl.orderCommandControl.buttonDiscount.IsChecked = false;
                OrderEntryControl.ShowCouponControl(show);
                return true;
            }
        }
        #endregion

        #region DiscountCommand
        private class DiscountCommand : CommandBase
        {
            public DiscountCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed;
            }

            protected override bool Execute(ref object parameter)
            {
                if (!(parameter is TextBlockButton)) return false;
                var button = (TextBlockButton)parameter;
                TicketDiscountControl discountControl =
                    MainWindow.Singleton.orderEntryControl.discountControl;
                bool show = (discountControl.Visibility == Visibility.Hidden);
                button.IsChecked = show;
                MainWindow.Singleton.orderEntryControl.orderCommandControl.buttonCoupon.IsChecked = false;
                OrderEntryControl.ShowDiscountControl(show);
                return true;
            }
        }
        #endregion

        #region FutureTimeCommand
        private class FutureTimeCommand : CommandBase
        {
            public FutureTimeCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    
                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&
                    (SessionManager.ActiveTicket.PrepareTime == null)) ||
                    
                    ((SessionManager.ActiveTicket == null) && 
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed &&
                    (details.SelectedTicket.PrepareTime == null)));
            }

            protected override bool Execute(ref object parameter)
            {
                return (SessionManager.ActiveTicket != null) ? ExecuteInOrder() : ExecuteNonOrder();
            }

            private bool ExecuteInOrder()
            {
                PosDialogWindow window = FutureTimeEditControl.CreateInDefaultWindow();
                var control = window.DockedControl as FutureTimeEditControl;
                if (control == null) return false;
                int? intValue = GetPrepTime().IntValue;
                if (intValue != null)
                {
                    DateTime? startTime = SessionManager.ActiveTicket.StartTime ??
                                          DateTime.Now + new TimeSpan(0, intValue.Value, 0);
                    control.SelectedDateTime = startTime;
                }
                PosDialogWindow.ShowPosDialogWindow(window);
                if (control.KeepChanges)
                {
                    SessionManager.ActiveTicket.SetStartTime(control.SelectedDateTime);
                }
                return true;
            }

            private bool ExecuteNonOrder()
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                DateTime? originalDateTime = details.SelectedTicket.StartTime;
                PosDialogWindow window = FutureTimeEditControl.CreateInDefaultWindow();
                var control = window.DockedControl as FutureTimeEditControl;
                DateTime? startTime = details.SelectedTicket.StartTime;
                if (startTime == null)
                {
                    int? intValue = GetPrepTime().IntValue;
                    if (intValue != null)
                        startTime = DateTime.Now + new TimeSpan(0, intValue.Value, 0);
                }
                if (control == null) return false;
                control.SelectedDateTime = startTime;
                PosDialogWindow.ShowPosDialogWindow(window);
                if (control.KeepChanges)
                {
                    if ((originalDateTime != null) && (control.SelectedDateTime == null))
                    {
                        details.SelectedTicket.SetStartTime(DateTime.Now);
                        details.SelectedTicket.Update();
                        UpdateFutureOrders();
                        AdjustOtherPartyTicketStartTimes(details.SelectedTicket);
                    }
                    else
                    {
                        details.SelectedTicket.SetStartTime(control.SelectedDateTime);
                        details.SelectedTicket.Update();
                        AdjustOtherPartyTicketStartTimes(details.SelectedTicket);
                    }
                    OrderEntryControl.UpdateSelectedTicketDetails();
                }
                return true;
            }
        }
        #endregion

        #region ManagePartyCommand
        private class ManagePartyCommand : CommandBase
        {
            public ManagePartyCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&

                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed) ||

                    ((SessionManager.ActiveTicket == null) && 
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed));
            }

            protected override bool Execute(ref object parameter)
            {
                if (SessionManager.ActiveTicket != null)
                {
                    if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                        (MainWindow.Singleton.orderEntryControl.orderCommandControl == null)) return false;
                    OrderEntryControl.EditPartyCommand(SessionManager.ActiveTicket);
                    MainWindow.Singleton.orderEntryControl.orderCommandControl.UpdatePartyButton();
                }
                else
                {
                    if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                        (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                    OrderEntryTicketDetailsControl details =
                        MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                    OrderEntryControl.EditPartyCommand(details.SelectedTicket);
                    details.UpdatePartyButton();
                }
                return true;
            }
        }
        #endregion

        #region TicketCommentCommand
        private class TicketCommentCommand : CommandBase
        {
            public TicketCommentCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed;
            }

            protected override bool Execute(ref object parameter)
            {
                string results = PosDialogWindow.PromptKeyboard(
                    Types.Strings.OrderEntryTicketComment,
                    SessionManager.ActiveTicket.ManagerNote);
                if (results != null)
                {
                    if ((SessionManager.ActiveTicket.ManagerNote == null) && results.Equals(""))
                        SessionManager.ActiveTicket.SetManagerNote(null);
                    else
                        SessionManager.ActiveTicket.SetManagerNote(results);
                }
                return true;
            }
        }
        #endregion

        #region TaxExemptionCommand
        private class TaxExemptionCommand : CommandBase
        {
            public TaxExemptionCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&

                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed) ||

                    ((SessionManager.ActiveTicket == null) &&
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed));
            }

            protected override bool Execute(ref object parameter)
            {
                return (SessionManager.ActiveTicket != null) ? ExecuteInOrder() : ExecuteNonOrder();
            }

            private static bool ExecuteInOrder()
            {
                string result =
                    PosDialogWindow.PromptKeyboard(Types.Strings.OrderEntryEnterTaxExemptionId,
                                                   SessionManager.ActiveTicket.TaxExemptId);
                if (result == null)
                    return false;
                if (String.IsNullOrEmpty(result))
                    result = null;
                SessionManager.ActiveTicket.SetTaxExemptId(result);
                SessionManager.ActiveTicket.SetIsBeingModified();
                return true;
            }

            private static bool ExecuteNonOrder()
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl.SelectedTicket == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;

                string result =
                    PosDialogWindow.PromptKeyboard(Types.Strings.EnterTaxExemptionId,
                    details.SelectedTicket.TaxExemptId);
                if (result == null)
                    return false;
                if (String.IsNullOrEmpty(result))
                    result = null;
                details.SelectedTicket.SetTaxExemptId(result);
                details.SelectedTicket.Update();
                return true;
            }
        }
        #endregion

        #region CashOutCommand
        private class CashOutCommand : CommandBase
        {
            public CashOutCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    SessionManager.ActiveEmployee.HasPermission(Permissions.Cashout) &&
                    (RegisterManager.ActiveRegisterDrawer != null) &&
                    (DayOfOperation.Today != null) &&

                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&
                    TicketItem.GetAll(SessionManager.ActiveTicket.PrimaryKey, false).Any()) ||

                    ((SessionManager.ActiveTicket == null) &&
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed));
            }

            protected override bool Execute(ref object parameter)
            {
                return (SessionManager.ActiveTicket != null) ? ExecuteInOrder() : ExecuteNonOrder();
            }

            private bool ExecuteInOrder()
            {
                OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
                oec.ticketDetailsControl.SelectedTicket = SessionManager.ActiveTicket;
                if (CanUseCashout(SessionManager.ActiveTicket))
                {
                    Ticket activeTicket = SessionManager.ActiveTicket;
                    bool canPlaceOrder = PlaceOrder.CanExecute(this);
                    if (canPlaceOrder || CloseTicket.CanExecute(this))
                    {
                        if (canPlaceOrder)
                            CloseActiveTicket();
                        else
                            CloseTicket.Execute(this);
                        if (PosHelper.CashOutTicket(activeTicket))
                            oec.ticketDetailsControl.SelectedTicket = null;
                        oec.ticketSelectionControl.UpdateTickets();
                    }
                }
                return true;
            }

            private bool ExecuteNonOrder()
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl.SelectedTicket == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;

                if (!CanUseCashout(details.SelectedTicket)) return false;

                // Cash out ticket
                if (PosHelper.CashOutTicket(details.SelectedTicket))
                    details.SelectedTicket = null;
                return true;
            }

            private bool CanUseCashout(Ticket selectedTicket)
            {
                if (RegisterManager.ActiveRegisterDrawer == null)
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.ThereIsNoRegisterDrawerStarted,
                        Types.Strings.TicketCashout);
                    return false;
                }

                if ((RegisterManager.ActiveRegisterDrawer.EmployeeId !=
                    SessionManager.ActiveEmployee.Id) &&
                    !SessionManager.ActiveEmployee.HasPermission(
                    Permissions.UseAnyRegisterDrawer))
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.YouDoNotHavePermissionToUseTheCurrentRegisterDrawer,
                        Types.Strings.TicketCashout);
                    return false;
                }
                if (!TicketItem.GetAll(selectedTicket.PrimaryKey, false).Any())
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.ThereAreNoItemsOnThisTicket, Types.Strings.TicketCashout);
                    return false;
                }
                if (RegisterManager.ActiveRegisterDrawer == null)
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.ThereIsNoRegisterDrawerStarted, Types.Strings.TicketCashout);
                    return false;
                }
                if (PosHelper.IsLocked(TableName.Ticket, selectedTicket.PrimaryKey.Id))
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.ThisTicketYouAreTryingToCashoutIsCurrentlyOpenOnAnotherTerminal,
                        Types.Strings.TicketLocked);
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region CloseTicketCommand
        private class CloseTicketCommand : CommandBase
        {
            public CloseTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    (SessionManager.ActiveTicket.IsCanceled ||
                    SessionManager.ActiveTicket.IsClosed ||
                    (DayOfOperation.Today == null) ||
                    (!SessionManager.ActiveTicket.HasChangedTicketItem() &&
                    TicketItem.GetAll(SessionManager.ActiveTicket.PrimaryKey, false).Any()));
            }

            protected override bool Execute(ref object parameter)
            {
                CloseActiveTicket();
                return true;
            }
        }
        #endregion

        #region EarlyCancelTicketCommand
        private class EarlyCancelTicketCommand : CommandBase
        {
            public EarlyCancelTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&
                    (SessionManager.ActiveTicket.PrepareTime == null) &&
                    (SessionManager.ActiveTicket.StartTime == null);
            }

            protected override bool Execute(ref object parameter)
            {
                if (!PosHelper.ConfirmEarlyCancelPrompt()) return false;
                PosHelper.ClearTicketCoupons(SessionManager.ActiveTicket.PrimaryKey);
                PosHelper.ClearTicketDiscounts(SessionManager.ActiveTicket.PrimaryKey);
                foreach (TicketItem ticketItem in TicketItem.GetAll(SessionManager.ActiveTicket.PrimaryKey))
                {
                    ticketItem.Delete();
                }
                TicketManager.Delete(SessionManager.ActiveTicket.PrimaryKey);
                SessionManager.ClearTicket();
                return true;
            }
        }
        #endregion

        #region VoidTicketCommand
        private class VoidTicketCommand : CommandBase
        {
            public VoidTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       SessionManager.ActiveEmployee.HasPermission(Permissions.Void) &&

                       (((SessionManager.ActiveTicket != null) &&
                         IsVoidable(SessionManager.ActiveTicket)) ||

                        ((SessionManager.ActiveTicket == null) &&
                         (details.SelectedTicket != null) &&
                         IsVoidable(details.SelectedTicket)));
            }

            private bool IsVoidable(Ticket ticket)
            {
                return !((ticket.PrepareTime == null) && (ticket.StartTime == null));
            }

            protected override bool Execute(ref object parameter)
            {
                if (SessionManager.ActiveTicket != null)
                {
                    if (PosHelper.VoidTicketCommand(SessionManager.ActiveTicket))
                        SessionManager.ClearTicket();
                }
                else
                {
                    if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                        (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                    OrderEntryTicketDetailsControl details =
                        MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                    if (PosHelper.VoidTicketCommand(details.SelectedTicket))
                    {
                        OrderEntryControl.InitializeTicketSelection(null);
                        details.SelectedTicket = null;
                    }
                }
                return true;
            }
        }
        #endregion

        #region CancelTicketCommand
        private class CancelTicketCommand : CommandBase
        {
            public CancelTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel) &&

                    (((SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&

                    ((SessionManager.ActiveTicket.PrepareTime != null) ||
                    ((SessionManager.ActiveTicket.PrepareTime == null) && (SessionManager.ActiveTicket.StartTime != null)))
                    
                    ) ||

                    ((SessionManager.ActiveTicket == null) &&
                    (details.SelectedTicket != null) &&
                    !details.SelectedTicket.IsCanceled &&
                    !details.SelectedTicket.IsClosed &&
                    
                    ((details.SelectedTicket.PrepareTime != null) ||
                    ((details.SelectedTicket.PrepareTime == null) && (details.SelectedTicket.StartTime != null)))
                    
                    ));
            }

            protected override bool Execute(ref object parameter)
            {
                if (SessionManager.ActiveTicket != null)
                {
                    if (PosHelper.CancelTicketCommand(SessionManager.ActiveTicket))
                        SessionManager.ClearTicket();
                }
                else
                {
                    if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                        (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                    OrderEntryTicketDetailsControl details =
                        MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                    if (PosHelper.CancelTicketCommand(details.SelectedTicket))
                    {
                        OrderEntryControl.InitializeTicketSelection(null);
                        details.SelectedTicket = null;
                    }
                }
                return true;
            }
        }
        #endregion

        #region ProcessReturnsCommand
        private class ProcessReturnsCommand : CommandBase
        {
            public ProcessReturnsCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    SessionManager.ActiveTicket.IsClosed &&
                    SessionManager.ActiveTicket.HasReturnPendingTicketItem() &&
                    (RegisterManager.ActiveRegisterDrawer != null) &&
                    SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterReturn);
            }

            protected override bool Execute(ref object parameter)
            {
                // Confirm the return
                if (!PosHelper.ConfirmReturnPrompt())
                    return false;

                OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
                List<TicketItem> returnedTicketItems =
                    (from TicketItemTemplate item in oec.receiptTape1.Items
                     where item.TicketItem.IsPendingReturn
                     select item.TicketItem).ToList();

                PrinterManager.Print(SessionManager.ActiveTicket,
                    TicketItemPrintOptions.TicketItemReturn,
                    returnedTicketItems.ToArray());

                // If a coupon or discount was applied to this item,
                // then be sure to give back the proper amount. Also
                // return any taxes paid on the ticket items. Open cash
                // drawer and tell user the amount to refund the customer
                // for the returned items.

                // Process the returns
                double returnTotal = 0;
                double taxTotal = 0;
                foreach (TicketItem ticketItem in returnedTicketItems)
                {
                    double returnAmount = ticketItem.GetTotalCost(ticketItem.QuantityPendingReturn);
                    double couponAmount = ticketItem.GetTotalCoupon(returnAmount);
                    double discountAmount = ticketItem.GetTotalDiscount(returnAmount);
                    returnAmount = (returnAmount - couponAmount - discountAmount);
                    double taxAmount = ticketItem.GetTax(returnAmount);

                    returnTotal += returnAmount;
                    taxTotal += taxAmount;

                    PosHelper.ReturnTicketItem(ticketItem, returnAmount + taxTotal);

                    if (ticketItem.Quantity == ticketItem.QuantityPendingReturn)
                    {
                        // Void the item
                        PosHelper.VoidTicketItem(ticketItem);
                    }
                    else
                    {
                        // Update ticket item
                        ticketItem.SetQuantity(ticketItem.Quantity -
                            ticketItem.QuantityPendingReturn);
                        ticketItem.Update();
                    }

                    // ToDo: Inventory updates
                }
                returnTotal += taxTotal;

                // Adjust the register drawer amount, and open the drawer
                RegisterManager.ActiveRegisterDrawer.RemoveFromCurrentAmount(returnTotal);
                RegisterManager.OpenCashDrawer();

                PosDialogWindow.ShowDialog(Types.Strings.OrderEntryReturnTotalIs +
                    returnTotal.ToString("C2"), Types.Strings.OrderEntryReturnTotal, DialogButtons.Ok);

                DoCancelTicketChangesCommand(false);
                return true;
            }
        }
        #endregion

        #region CancelTicketChangesNowCommand
        private class CancelTicketChangesNowCommand : CommandBase
        {
            public CancelTicketChangesNowCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    (SessionManager.ActiveTicket.IsCanceled ||
                    SessionManager.ActiveTicket.IsClosed);
            }

            protected override bool Execute(ref object parameter)
            {
                DoCancelTicketChangesCommand(false);
                return true;
            }
        }
        #endregion

        #region CancelTicketChangesCommand
        private class CancelTicketChangesCommand : CommandBase
        {
            public CancelTicketChangesCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&
                    (SessionManager.ActiveTicket.PrepareTime != null) &&
                    SessionManager.ActiveTicket.HasChangedTicketItem();
            }

            protected override bool Execute(ref object parameter)
            {
                DoCancelTicketChangesCommand(true);
                return true;
            }
        }
        #endregion

        #region PlaceOrderCommand
        private class PlaceOrderCommand : CommandBase
        {
            public PlaceOrderCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                    (DayOfOperation.Today != null) &&
                    (SessionManager.ActiveTicket != null) &&
                    !SessionManager.ActiveTicket.IsCanceled &&
                    !SessionManager.ActiveTicket.IsClosed &&
                    SessionManager.ActiveTicket.HasChangedTicketItem();
            }

            protected override bool Execute(ref object parameter)
            {
                CloseActiveTicket();
                return true;
            }
        }
        #endregion

        #region OpenTicketCommand
        private class OpenTicketCommand : CommandBase
        {
            public OpenTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                       (SessionManager.ActiveTicket == null) &&
                       (details.SelectedTicket != null);
            }

            protected override bool Execute(ref object parameter)
            {
                OrderEntryControl.OpenSelectedTicket();
                return true;
            }
        }
        #endregion

        #region FireTicketCommand
        private class FireTicketCommand : CommandBase
        {
            public FireTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                if (details.SelectedTicket == null) return false;

                bool isReadOnly = (details.SelectedTicket.IsClosed || details.SelectedTicket.IsCanceled);
                bool isCashedOut = (details.SelectedTicket.IsClosed && !details.SelectedTicket.IsCanceled);
                return (SessionManager.ActiveEmployee != null) &&
                       (SessionManager.ActiveTicket == null) &&
                       (DayOfOperation.Today != null) &&
                       !isReadOnly && !isCashedOut && SelectedTicketHasUnfiredItems();
            }

            protected override bool Execute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                PosHelper.FireEntrees(details.SelectedTicket);
                details.SetStatusLabel();
                details.buttonFireEntree.Visibility = Visibility.Collapsed;
                return true;
            }
        }
        #endregion

        #region PrintTicketCommand
        private class PrintTicketCommand : CommandBase
        {
            public PrintTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                       (SessionManager.ActiveTicket == null) &&
                       (details.SelectedTicket != null);
            }

            protected override bool Execute(ref object parameter)
            {
#if !DEMO
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                PrinterManager.Print(PrintDestination.Receipt,
                    DeviceManager.ActivePosPrinterLocal, details.SelectedTicket,
                    ((!details.SelectedTicket.IsCanceled) ?
                    TicketItemPrintOptions.AllNonCanceled :
                    TicketItemPrintOptions.All));
#else
                PosDialogWindow.ShowDialog(
                    Types.Strings.PrintingIsDisabledInDemoVersion, Types.Strings.Disabled);
#endif                
                return true;
            }
        }
        #endregion

        #region UnCancelTicketCommand
        private class UnCancelTicketCommand : CommandBase
        {
            public UnCancelTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                    (SessionManager.ActiveTicket == null) &&
                    (DayOfOperation.Today != null) &&
                    SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel) &&
                    (details.SelectedTicket != null) &&
                    details.SelectedTicket.IsCanceled;
            }

            protected override bool Execute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;

                if (PosHelper.UnCancelTicketCommand(details.SelectedTicket))
                {
                    OrderEntryControl.InitializeTicketSelection(null);
                    details.SelectedTicket = null;
                }
                
                return true;
            }
        }
        #endregion

        #region ChangeTicketOwnerCommand
        private class ChangeTicketOwnerCommand : CommandBase
        {
            public ChangeTicketOwnerCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       SessionManager.ActiveEmployee.HasPermission(Permissions.ChangeTicketOwner) &&
                       (details.SelectedTicket != null) &&
                       !details.SelectedTicket.IsCanceled &&
                       !details.SelectedTicket.IsClosed;
            }

            protected override bool Execute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;

                var control = new OrderEntryChangeEmployeeControl();
                control.InitializeListBox(details.SelectedTicket);
                var window = new PosDialogWindow(control, Types.Strings.ChangeEmployee, 280, 475);

                // Show the dialog            
                PosDialogWindow.ShowPosDialogWindow(window);

                MainWindow.Singleton.orderEntryControl.
                    ticketSelectionControl.UpdateTickets();
                return true;
            }
        }
        #endregion

        #region RefundTicketCommand
        private class RefundTicketCommand : CommandBase
        {
            public RefundTicketCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
                OrderEntryTicketDetailsControl.SelectedTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (RegisterManager.ActiveRegisterDrawer != null) &&
                       SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterRefund) &&
                       (details.SelectedTicket != null) &&
                       !details.SelectedTicket.IsCanceled &&
                       details.SelectedTicket.IsClosed &&
                       (TicketPayment.Count(details.SelectedTicket.PrimaryKey) > 0);
            }

            protected override bool Execute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
                OrderEntryTicketDetailsControl details =
                    MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
                TicketRefundType? refundType = PosHelper.ConfirmRefundPrompt();
                if (refundType != null)
                {
                    Ticket refundTicket = details.SelectedTicket;

                    // Print the ticket return receipt
                    PrinterManager.Print(refundTicket, TicketItemPrintOptions.TicketRefund);

                    // Refund ticket
                    double refundAmount = PosHelper.RefundTicketCommand(refundTicket, refundType.Value);

                    OrderEntryControl.InitializeTicketSelection(null);
                    details.SelectedTicket = null;

                    RegisterManager.OpenCashDrawer();

                    PosDialogWindow.ShowDialog(Types.Strings.RefundedTotalIs +
                        refundAmount.ToString("C2"), Types.Strings.RefundTotal, DialogButtons.Ok);
                }
                return true;
            }
        }
        #endregion

        #region IncreaseQuantityCommand
        private class IncreaseQuantityCommand : CommandBase
        {
            public IncreaseQuantityCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (parameter is TextBlockButton) &&
                       (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       (CanExecuteNormal || CanExecuteReturn);
            }

            private bool CanExecuteNormal
            {
                get
                {
                    return !SessionManager.ActiveTicket.IsClosed &&
                           !SessionManager.ActiveTicket.IsCanceled;
                }
            }

            private bool CanExecuteReturn
            {
                get
                {
                    return SessionManager.ActiveTicketItem.IsPendingReturn &&
                           (SessionManager.ActiveTicketItem.QuantityPendingReturn <
                            SessionManager.ActiveTicketItem.Quantity);
                }
            }

            protected override bool Execute(ref object parameter)
            {
                if (!SessionManager.ActiveTicketItem.IsPendingReturn)
                {
                    int quantity = (SessionManager.ActiveTicketItem.QuantityPending != null ?
                        SessionManager.ActiveTicketItem.QuantityPending.Value :
                        SessionManager.ActiveTicketItem.Quantity);
                    SessionManager.ActiveTicketItem.SetQuantity(quantity + 1);
                    OrderEntryControl.UpdateSelectedTicketItemTemplate();
                    UpdateChildTicketItems(SessionManager.ActiveTicketItem, quantity + 1);
                    DecreaseQuantity.Update();
                }
                else
                {
                    SessionManager.ActiveTicketItem.SetQuantityPendingReturn(
                        SessionManager.ActiveTicketItem.QuantityPendingReturn + 1);
                    var item = MainWindow.Singleton.orderEntryControl
                        .receiptTape1.listBoxTransactions.SelectedItem as TicketItemTemplate;
                    if (item != null) item.Update();
                    UpdateTicketItemButtons();
                }
                OrderEntryControl.SetOrderAmountText(MainWindow.Singleton.orderEntryControl
                    .receiptTape1.TicketSubTotal.ToString("C2"));
                return true;
            }

        }
        #endregion

        #region DecreaseQuantityCommand
        private class DecreaseQuantityCommand : CommandBase
        {
            public DecreaseQuantityCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       (CanExecuteNormal || CanExecuteReturn);
            }

            private bool CanExecuteNormal
            {
                get
                {
                    int quantity = SessionManager.ActiveTicketItem.QuantityPending.HasValue
                                        ? SessionManager.ActiveTicketItem.QuantityPending.Value
                                        : SessionManager.ActiveTicketItem.Quantity;
                    return !SessionManager.ActiveTicketItem.IsPendingReturn &&
                           !SessionManager.ActiveTicket.IsClosed &&
                           !SessionManager.ActiveTicket.IsCanceled &&
                           (quantity > 1);
                }
            }

            private bool CanExecuteReturn
            {
                get
                {
                    return SessionManager.ActiveTicketItem.IsPendingReturn &&
                           SessionManager.ActiveTicket.IsClosed &&
                           !SessionManager.ActiveTicket.IsCanceled &&
                           (SessionManager.ActiveTicketItem.QuantityPendingReturn > 1) &&
                           SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterReturn);
                }
            }

            protected override bool Execute(ref object parameter)
            {
                if (!SessionManager.ActiveTicketItem.IsPendingReturn)
                {
                    bool isLate = ((SessionManager.ActiveTicket.PrepareTime != null) &&
                        (SessionManager.ActiveTicketItem.OrderTime != null));
                    int originalQuantity = SessionManager.ActiveTicketItem.GetCurrentQuantity();
                    int quantity = (SessionManager.ActiveTicketItem.QuantityPending != null ?
                        SessionManager.ActiveTicketItem.QuantityPending.Value :
                        SessionManager.ActiveTicketItem.Quantity);

                    if (!isLate || ((quantity - 1) >= originalQuantity) ||
                        (SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel)) ||
                        (PosHelper.GetPermission(Permissions.LateCancel) != null))
                    {
                        SessionManager.ActiveTicketItem.SetQuantity(quantity - 1);
                        DecreaseQuantity.Update();
                        OrderEntryControl.UpdateSelectedTicketItemTemplate();
                        UpdateChildTicketItems(SessionManager.ActiveTicketItem, quantity - 1);
                    }
                    else
                    {
                        PosDialogWindow.ShowDialog(
                            Types.Strings.OrderEntryNoDecreasePermission,
                            Types.Strings.PermissionDenied);
                    }
                }
                else
                {
                    SessionManager.ActiveTicketItem.SetQuantityPendingReturn(
                        SessionManager.ActiveTicketItem.QuantityPendingReturn - 1);
                    var item = MainWindow.Singleton.orderEntryControl
                        .receiptTape1.listBoxTransactions.SelectedItem as TicketItemTemplate;
                    if (item != null) item.Update();
                    UpdateTicketItemButtons();
                }
                OrderEntryControl.SetOrderAmountText(
                    MainWindow.Singleton.orderEntryControl
                    .receiptTape1.TicketSubTotal.ToString("C2"));
                return true;
            }
        }
        #endregion

        #region SetQuantityCommand
        private class SetQuantityCommand : CommandBase
        {
            public SetQuantityCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.receiptTape1 == null)) return false;
                return (parameter is TextBlockButton) &&
                       (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       !SessionManager.ActiveTicket.IsClosed &&
                       !SessionManager.ActiveTicket.IsCanceled &&
                       (SessionManager.ActiveTicketItem != null);
            }

            protected override bool Execute(ref object parameter)
            {
                if (parameter is TextBlockButton)
                {
                    var button = (parameter as TextBlockButton);
                    ContextMenu contextMenu = button.ContextMenu;
                    NumberEntryControl numPad = GetNumberPadControl(contextMenu.Template.LoadContent());
                    //var numPad = contextMenu.Template.FindName(Strings.Numpad, contextMenu) as NumberEntryControl;
                    if (numPad != null)
                    {
                        numPad.UseDecimalPoint = false;
                        numPad.DisplayAsPercentage = false;
                        numPad.DisplayAsCurrency = false;
                        numPad.OriginalText = "";
                    }
                    contextMenu.PlacementTarget = button;
                    contextMenu.Placement = PlacementMode.Top;
                    contextMenu.IsOpen = true;
                    return true;
                }
                return false;
            }

            private NumberEntryControl GetNumberPadControl(DependencyObject parentDependencyObject)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
                {
                    DependencyObject depObject = VisualTreeHelper.GetChild(parentDependencyObject, i);
                    if (depObject is NumberEntryControl)
                        return depObject as NumberEntryControl;
                    if (VisualTreeHelper.GetChildrenCount(depObject) > 0)
                    {
                        NumberEntryControl childFound = GetNumberPadControl(depObject);
                        if (childFound != null)
                            return childFound;
                    }
                }
                return null;
            }
        }
        #endregion

        #region SpecialInstructionsCommand
        private class SpecialInstructionsCommand : CommandBase
        {
            public SpecialInstructionsCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicket.PrepareTime != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       !SessionManager.ActiveTicket.IsClosed &&
                       !SessionManager.ActiveTicket.IsCanceled;
            }

            protected override bool Execute(ref object parameter)
            {
                Item item = Item.Get(SessionManager.ActiveTicketItem.ItemId);
                string instructions = PosDialogWindow.PromptKeyboard(Types.Strings.OrderEntrySpecialInstructionsFor +
                    item.FullName + "\"", SessionManager.ActiveTicketItem.SpecialInstructions);
                if (instructions != null)
                {
                    if (instructions == "")
                        instructions = null;
                    SessionManager.ActiveTicketItem.SetSpecialInstructions(instructions);
                    UpdateInOrderCommands();
                    OrderEntryControl.UpdateSelectedTicketItemTemplate();
                }
                return true;
            }
        }
        #endregion

        #region CancelTicketItemCommand
        private class CancelTicketItemCommand : CommandBase
        {
            public CancelTicketItemCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.receiptTape1 == null)) return false;
                return (SessionManager.ActiveEmployee != null) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       !SessionManager.ActiveTicket.IsClosed &&
                       !SessionManager.ActiveTicket.IsCanceled;
            }

            protected override bool Execute(ref object parameter)
            {
                bool isLate = ((SessionManager.ActiveTicket.PrepareTime != null) &&
                    (SessionManager.ActiveTicketItem.OrderTime != null));
                if (isLate)
                    CancelItemCommand();
                else
                    EarlyCancelItemCommand();
                return true;
            }

            private void EarlyCancelItemCommand()
            {
                if (!PosHelper.ConfirmEarlyCancelPrompt(SessionManager.ActiveTicketItem)) return;
                PosHelper.VoidTicketItem(SessionManager.ActiveTicketItem);
                MainWindow.Singleton.orderEntryControl.receiptTape1.RemoveSelectedItemFromReceiptTape();
            }

            private void CancelItemCommand()
            {
                bool isLate = ((SessionManager.ActiveTicket.PrepareTime != null) &&
                    (SessionManager.ActiveTicketItem.OrderTime != null));

                if (!isLate ||
                    (SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel)) ||
                    (PosHelper.GetPermission(Permissions.LateCancel) != null))
                {
                    bool? cancelMade = PosHelper.ConfirmCancelPrompt(SessionManager.ActiveTicketItem);
                    if (cancelMade != null)
                    {
                        PosHelper.CancelTicketItem(SessionManager.ActiveTicketItem, cancelMade.Value);
                        MainWindow.Singleton.orderEntryControl.receiptTape1.RemoveSelectedItemFromReceiptTape();
                    }
                }
                else
                {
                    PosDialogWindow.ShowDialog(
                        Types.Strings.OrderEntryNoCancelPermission,
                        Types.Strings.PermissionDenied);
                }
            }
        }
        #endregion

        #region VoidTicketItemCommand
        private class VoidTicketItemCommand : CommandBase
        {
            public VoidTicketItemCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.receiptTape1 == null)) return false;
                return (SessionManager.ActiveEmployee != null) &&
                       SessionManager.ActiveEmployee.HasPermission(Permissions.Void) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       (!SessionManager.ActiveTicketItem.ParentTicketItemId.HasValue) && // Is not child ticket item
                       (SessionManager.ActiveTicketItem.OrderTime != null) &&
                       (SessionManager.ActiveTicket.PrepareTime != null) &&
                       !SessionManager.ActiveTicket.IsClosed &&
                       !SessionManager.ActiveTicket.IsCanceled;
            }

            protected override bool Execute(ref object parameter)
            {
                if (PosHelper.ConfirmVoidPrompt(SessionManager.ActiveTicketItem))
                {
                    PrinterManager.Print(SessionManager.ActiveTicket,
                        TicketItemPrintOptions.TicketItemVoid,
                        SessionManager.ActiveTicketItem);
                    TicketVoid.Add(SessionManager.ActiveEmployee.Id,
                        SessionManager.ActiveTicket.PrimaryKey,
                        SessionManager.ActiveTicketItem.PrimaryKey.Id,
                        SessionManager.ActiveTicketItem.GetTotalCost());
                    PosHelper.VoidTicketItem(SessionManager.ActiveTicketItem);
                    MainWindow.Singleton.orderEntryControl.
                        receiptTape1.RemoveSelectedItemFromReceiptTape();
                }
                return true;
            }
        }
        #endregion

        #region ReturnTicketItemCommand
        private class ReturnTicketItemCommand : CommandBase
        {
            public ReturnTicketItemCommand()
            {
                SessionManager.ActiveEmployeeChanged += (sender, args) => OnCanExecuteChanged();
                SessionManager.ActiveTicketChanged += (sender, args) => OnCanExecuteChanged();
            }

            protected override bool CanExecute(ref object parameter)
            {
                if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                    (MainWindow.Singleton.orderEntryControl.receiptTape1 == null)) return false;
                return (SessionManager.ActiveEmployee != null) &&
                       SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterReturn) &&
                       (DayOfOperation.Today != null) &&
                       (SessionManager.ActiveTicket != null) &&
                       (SessionManager.ActiveTicketItem != null) &&
                       (!SessionManager.ActiveTicketItem.ParentTicketItemId.HasValue) && // Is not child ticket item
                       (RegisterManager.ActiveRegisterDrawer != null) &&
                       SessionManager.ActiveTicket.IsClosed &&
                       !SessionManager.ActiveTicket.IsCanceled &&
                       Item.GetIsReturnable(SessionManager.ActiveTicketItem.ItemId);
            }

            protected override bool Execute(ref object parameter)
            {
                SessionManager.ActiveTicketItem
                    .SetIsPendingReturn(!SessionManager.ActiveTicketItem.IsPendingReturn);
                SessionManager.ActiveTicketItem
                    .SetQuantityPendingReturn(SessionManager.ActiveTicketItem.Quantity);
                var item = MainWindow.Singleton.orderEntryControl
                    .receiptTape1.listBoxTransactions.SelectedItem as TicketItemTemplate;
                if (item != null) item.Update();
                UpdateInOrderCommands();
                return true;
            }
        }
        #endregion
        #endregion

        #region Fields
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ReportsMenu = new ReportsMenuCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ChangePassword = new ChangePasswordCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase Logout = new LogoutCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase Exit = new ExitCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase PersonalSettings = new PersonalSettingsCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ClockOut = new ClockOutCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase SetupMenu = new SetupMenuCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase Console = new ConsoleCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase RegisterMenu = new RegisterMenuCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase SystemMenu = new SystemMenuCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ChangeTicketSeating = new ChangeTicketSeatingCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase Coupon = new CouponCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase Discount = new DiscountCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase FutureTime = new FutureTimeCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ManageParty = new ManagePartyCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase TicketComment = new TicketCommentCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase TaxExemption = new TaxExemptionCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CloseTicket = new CloseTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase EarlyCancelTicket = new EarlyCancelTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase VoidTicket = new VoidTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CancelTicket = new CancelTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ProcessReturns = new ProcessReturnsCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CancelTicketChangesNow = new CancelTicketChangesNowCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CancelTicketChanges = new CancelTicketChangesCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CashOut = new CashOutCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase PlaceOrder = new PlaceOrderCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase OpenTicket = new OpenTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase FireTicket = new FireTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase PrintTicket = new PrintTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase UnCancelTicket = new UnCancelTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ChangeTicketOwner = new ChangeTicketOwnerCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase RefundTicket = new RefundTicketCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase IncreaseQuantity = new IncreaseQuantityCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase DecreaseQuantity = new DecreaseQuantityCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase SetQuantity = new SetQuantityCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase SpecialInstructions = new SpecialInstructionsCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase CancelTicketItem = new CancelTicketItemCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase VoidTicketItem = new VoidTicketItemCommand();
        [Obfuscation(Exclude = true)]
        public static readonly CommandBase ReturnTicketItem = new ReturnTicketItemCommand();
        #endregion

        #region Static Methods
        public static void SetupInOrderCommands()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null))
                return;
            OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
            oec.buttonTouchInputCategory.Show();
            oec.buttonTouchInputCategory.SelectCategoryButton(-1);
            oec.SetOrderEntryCommands(true);
            UpdateInOrderCommands();
        }

        public static void UpdateInOrderCommands()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null))
                return;

            PlaceOrder.Update();
            CashOut.Update();
            CancelTicketChangesNow.Update();
            CancelTicketChanges.Update();
            ProcessReturns.Update();
            CancelTicket.Update();
            EarlyCancelTicket.Update();
            VoidTicket.Update();
            CloseTicket.Update();
            TaxExemption.Update();
            ChangeTicketSeating.Update();
            ProcessReturns.Update();
            TaxExemption.Update();
            TicketComment.Update();
            ManageParty.Update();
            FutureTime.Update();
            Coupon.Update();
            Discount.Update();
            SetQuantity.Update();
            IncreaseQuantity.Update();
            DecreaseQuantity.Update();
            CancelTicketItem.Update();
            VoidTicketItem.Update();
            ReturnTicketItem.Update();

            MainWindow.Singleton.orderEntryControl.orderCommandControl.UpdatePartyButton();
        }

        public static void UpdateTicketItemButtons()
        {
            SetQuantity.Update();
            IncreaseQuantity.Update();
            DecreaseQuantity.Update();
            CancelTicketItem.Update();
            VoidTicketItem.Update();
            ReturnTicketItem.Update();
        }

        public static void UpdateTicketDetailCommands()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null))
                return;
            OrderEntryTicketDetailsControl details = MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
            if ((details.SelectedTicket == null) || (SessionManager.ActiveEmployee == null))
                return;
            bool isReadOnly = (details.SelectedTicket.IsClosed || details.SelectedTicket.IsCanceled);
            details.buttonOpen.Text = (isReadOnly ? Types.Strings.View : Types.Strings.Open);

            // Update Commands
            OpenTicket.Update();
            FireTicket.Update();
            PrintTicket.Update();
            UnCancelTicket.Update();
            ManageParty.Update();
            ChangeTicketSeating.Update();
            ChangeTicketOwner.Update();
            FutureTime.Update();
            TaxExemption.Update();
            CashOut.Update();
            CancelTicket.Update();
            VoidTicket.Update();
            RefundTicket.Update();

            details.UpdatePartyButton();
        }

        public static void SetupNoOrderCommands()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null))
                return;
            OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
            oec.SetOrderEntryCommands(false);
            
            SystemMenu.Update();
            RegisterMenu.Update();
            ReportsMenu.Update();
            ChangePassword.Update();
            Logout.Update();
            Exit.Update();
            PersonalSettings.Update();
            ClockOut.Update();
            SetupMenu.Update();
            Console.Update();

            OrderEntryControl.SetDisplayedTicketTypeToStatusBar();
        }

        /// <summary>
        /// Verifing if future time is at least 1 minute in the future
        /// </summary>
        private static void VerifyFutureTime(Ticket ticket)
        {
            if ((ticket.StartTime == null) || (ticket.PrepareTime != null))
                return;
            int offsetMinutes = -1;
            DateTime nowTime = DateTime.Now;
            if (nowTime.Second > 55)
                offsetMinutes = 0;
            int? intValue = GetPrepTime().IntValue;
            if (intValue == null) return;
            DateTime earliestTime = nowTime +
                new TimeSpan(0, intValue.Value + offsetMinutes, 60 - nowTime.Second);
            if (earliestTime > ticket.StartTime)
                ticket.SetStartTime(earliestTime);
        }

        public static StoreSetting GetPrepTime()
        {
            StoreSetting result = SettingManager.GetStoreSetting("CookPrepTime");
            if (result.IntValue == null)
            {
                SettingManager.SetStoreSetting("CookPrepTime", 15);
                result = SettingManager.GetStoreSetting("CookPrepTime");
            }
            return result;
        }

        public static void UpdateFutureOrders()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null))
                return;
            OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
            DateTime now = DateTime.Now;
            IEnumerable<Ticket> tickets = TicketManager.GetOpenTickets();
            StoreSetting prepTime = GetPrepTime();
            foreach (Ticket ticket in tickets)
            {
                if ((ticket.StartTime == null) || ticket.IsBeingModified)
                    continue;
                if (ticket.PrepareTime != null) continue;
                var startDateTime = new DateTime(ticket.StartTime.Value.Year,
                    ticket.StartTime.Value.Month, ticket.StartTime.Value.Day,
                    ticket.StartTime.Value.Hour, ticket.StartTime.Value.Minute, 0);
                var nowDateTime = new DateTime(now.Year, now.Month, now.Day,
                    now.Hour, now.Minute, 0);
                if (prepTime.IntValue != null)
                {
                    TimeSpan remainingTime = startDateTime - nowDateTime -
                        new TimeSpan(0, prepTime.IntValue.Value, 0);
                    if (Math.Abs(remainingTime.TotalSeconds - 0) > double.Epsilon) continue;
                }
                ticket.SetPrepareTime(now);
                ticket.Update();
                PrinterManager.Print(ticket, TicketItemPrintOptions.AllNonCanceled);
                PosHelper.UpdateTicketItems(ticket, true);
            }
            // Update selected ticket details, encase that changed
            if (oec.ticketSelectionControl.SelectedTicket != null)
                oec.ticketDetailsControl.SelectedTicket =
                    oec.ticketSelectionControl.SelectedTicket;
        }

        private static void CloseActiveTicket()
        {
            Ticket selected = SessionManager.ActiveTicket;

            VerifyFutureTime(SessionManager.ActiveTicket);

            if (SessionManager.ActiveTicket.PartyId == 0)
            {
                CloseSingleTicket(SessionManager.ActiveTicket);
                SessionManager.CloseTicket();
            }
            else
            {
                ClosePartyTickets(SessionManager.ActiveTicket);
            }

            FinishCloseTicket(selected);
        }

        private static void ClosePartyTickets(Ticket selectedTicket)
        {
            var partyTickets = new List<Ticket>(
                TicketManager.GetPartyTickets(selectedTicket.PartyId));

            AdjustOtherPartyTicketStartTimes(partyTickets, selectedTicket);

            foreach (Ticket ticket in partyTickets)
            {
                CloseSingleTicket(ticket);
            }
            SessionManager.CloseTickets(partyTickets);
        }

        private static void AdjustOtherPartyTicketStartTimes(List<Ticket> partyTickets,
            Ticket selectedTicket)
        {
            bool checkPassed = partyTickets.Any(
                ticket => (ticket != selectedTicket) && (ticket.StartTime != selectedTicket.StartTime));
            if (!checkPassed)
                return;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.OrderEntryFutureTimeUpdateMessage,
                Types.Strings.OrderEntryUpdatePartyTickets, DialogButtons.YesNo) != DialogButton.Yes) return;
            foreach (Ticket ticket in partyTickets)
            {
                ticket.SetStartTime(selectedTicket.StartTime);
            }
        }

        private static void AdjustOtherPartyTicketStartTimes(Ticket selectedTicket)
        {
            if ((selectedTicket == null) || (selectedTicket.PartyId <= 0))
                return;
            var partyTickets = new List<Ticket>(
                TicketManager.GetPartyTickets(selectedTicket.PartyId));
            bool checkPassed = partyTickets.Where(ticket => ticket != selectedTicket).Any(
                ticket => ticket.StartTime != selectedTicket.StartTime);
            if (!checkPassed)
                return;
            if (PosDialogWindow.ShowDialog(
                Types.Strings.YouHaveChangedTheFutureTimeForThisTicketWouldYou +
                Types.Strings.LikeToUpdateTheOtherTicketsInThisPartyToTheSameFutureTime,
                Types.Strings.UpdatePartyTickets, DialogButtons.YesNo) == DialogButton.Yes)
            {
                foreach (Ticket ticket in partyTickets)
                {
                    ticket.SetStartTime(selectedTicket.StartTime);
                    ticket.Update();
                }
            }
        }


        private static void CloseSingleTicket(Ticket ticket)
        {
            PrinterManager.Print(ticket, TicketItemPrintOptions.OnlyChanged);
            PosHelper.UpdateTicketItems(ticket, (ticket.StartTime == null));
        }

        public static void FinishCloseTicket(Ticket ticket)
        {
            if (ticket.IsBeingModified)
                ticket.Update();
            SessionManager.ClearTicket();
            OrderEntryControl oec = MainWindow.Singleton.orderEntryControl;
            oec.ticketSelectionControl.UpdateTickets();
            oec.ticketSelectionControl.SelectedTicket = ticket;
            oec.ticketDetailsControl.SelectedTicket = ticket;
            OrderEntryControl.UpdateSelectedTicketDetails();
        }

        public static bool SelectedTicketHasUnfiredItems()
        {
            if ((MainWindow.Singleton == null) || (MainWindow.Singleton.orderEntryControl == null) ||
                (MainWindow.Singleton.orderEntryControl.ticketDetailsControl == null)) return false;
            OrderEntryTicketDetailsControl details =
                MainWindow.Singleton.orderEntryControl.ticketDetailsControl;
            return (details.SelectedTicket.Type == TicketType.DineIn) &&
                TicketItem.GetAllUnfired(details.SelectedTicket.PrimaryKey).Any();
        }

        private static void DoCancelTicketChangesCommand(bool prompt)
        {
            if (prompt &&
                PosDialogWindow.ShowDialog(
                    Types.Strings.OrderEntryConfirmCancelChanges, Types.Strings.Confirmation,
                     DialogButtons.YesNo) != DialogButton.Yes) return;
            TicketItem.DumpCachedChanges();
            FinishCloseTicket(SessionManager.ActiveTicket);
        }

        public static void ExecuteLogoutCommand()
        {
            if (Logout.CanExecute(MainWindow.Singleton))
                Logout.Execute(MainWindow.Singleton);
        }

        private static void UpdateChildTicketItems(TicketItem parentTicketItem, int newQuantity)
        {
            foreach (TicketItem ticketItem in TicketItem.GetAllChildTicketItems(parentTicketItem.PrimaryKey))
            {
                // 1. Update Quantities
                int? baseQuantity = ItemGroup.GetTargetItemQuantity(parentTicketItem.ItemId, ticketItem.ItemId);
                if (baseQuantity == null) continue;
                ticketItem.SetQuantity(baseQuantity.Value * newQuantity);
                ticketItem.Update();

                // 2. Find these items in the list and update them
                foreach (TicketItemTemplate item in MainWindow.Singleton.orderEntryControl
                    .receiptTape1.listBoxTransactions.Items)
                {
                    if (item.TicketItem.PrimaryKey.Id == ticketItem.PrimaryKey.Id)
                        item.Update();
                }
            }
        }

        // ReSharper disable InconsistentNaming
        public static void NumberEntryControl_EnterPressed(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            var receiptTape = MainWindow.Singleton.orderEntryControl.receiptTape1;
            var numberEntryControl = sender as NumberEntryControl;
            receiptTape.buttonQuantitySet.ContextMenu.IsOpen = false;
            if (numberEntryControl == null) return;
            int? intValue = numberEntryControl.IntValue;
            if (intValue == null) return;
            bool isLate = ((SessionManager.ActiveTicket.PrepareTime != null) &&
                           (SessionManager.ActiveTicketItem.OrderTime != null));
            int originalQuantity = SessionManager.ActiveTicketItem.GetCurrentQuantity();
            int quantity = (SessionManager.ActiveTicketItem.QuantityPending != null ?
                SessionManager.ActiveTicketItem.QuantityPending.Value :
                SessionManager.ActiveTicketItem.Quantity);

            int value = intValue.Value;
            if ((quantity <= value) || !isLate || (value >= originalQuantity) ||
                (SessionManager.ActiveEmployee.HasPermission(Permissions.LateCancel)) ||
                (PosHelper.GetPermission(Permissions.LateCancel) != null))
            {
                SessionManager.ActiveTicketItem.SetQuantity(value);
                receiptTape.buttonQuantityDecrease.IsEnabled = (value > 1);
                OrderEntryControl.UpdateSelectedTicketItemTemplate();
                UpdateChildTicketItems(SessionManager.ActiveTicketItem, value);
                OrderEntryControl.SetOrderAmountText(receiptTape.TicketSubTotal.ToString("C2"));
            }
            else
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.OrderEntryNoDecreasePermission,
                    Types.Strings.PermissionDenied);
            }
        }
        #endregion
    }
}
