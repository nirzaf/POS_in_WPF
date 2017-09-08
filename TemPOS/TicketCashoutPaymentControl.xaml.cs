using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PosModels;
using PosModels.Types;
using PosControls;
using TemPOS.Types;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;
#if !DEMO
using Microsoft.PointOfService;
#endif

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketCashoutPaymentControl.xaml
    /// </summary>
    public partial class TicketCashoutPaymentControl : UserControl
    {
        #region Fields
        private Ticket _selectedTicket;
        #endregion

        #region Properties
        public bool IsTicketPayed
        {
            get;
            private set;
        }

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                InitializeTicket();
            }
        }
        #endregion

        public TicketCashoutPaymentControl()
        {
            InitializeComponent();
            numberEntryControl.FloatValue = null;
            Loaded += TicketCashoutPaymentControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void TicketCashoutPaymentControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigurationManager.IsInDesignMode) return;
#if !DEMO
            if (DeviceManager.ActiveCashDrawer1 != null)
                DeviceManager.ActiveCashDrawer1.StatusUpdateEvent +=
                    ActiveCashDrawer_StatusUpdateEvent;
            if (DeviceManager.ActiveCashDrawer2 != null)
                DeviceManager.ActiveCashDrawer2.StatusUpdateEvent +=
                    ActiveCashDrawer_StatusUpdateEvent;
#endif
        }
        
        [Obfuscation(Exclude = true)]
        public event EventHandler TicketPayed;

        private void InitializeTicket()
        {
            double subTotal = SelectedTicket.GetSubTotal();
            double couponTotal = SelectedTicket.GetCouponTotal();
            double discountTotal = SelectedTicket.GetDiscountTotal();
            double tax = SelectedTicket.GetTax();
            double total = subTotal - couponTotal - discountTotal + tax;
            double amountPayed = SelectedTicket.GetPaymentTotal();
            double amountDue = total - amountPayed;
            labelCouponAmount.Content = String.Format("{0:C}", couponTotal);
            labelDiscountAmount.Content = String.Format("{0:C}", discountTotal);
            labelSubTotal.Content = String.Format("{0:C}", subTotal);
            labelTaxAmount.Content = String.Format("{0:C}", tax);
            labelTotalAmount.Content = String.Format("{0:C}", total);
            labelAmountPayed.Content = String.Format("{0:C}", amountPayed);
            labelAmountDue.Content = String.Format("{0:C}", amountDue);
            bool wasPayed = IsTicketPayed;
            IsTicketPayed = (amountDue < 0.01);
            if (!wasPayed && IsTicketPayed && (TicketPayed != null))
                TicketPayed.Invoke(this, new EventArgs());
        }

        private double GetTicketTotal()
        {
            double subTotal = SelectedTicket.GetSubTotal();
            double couponTotal = SelectedTicket.GetCouponTotal();
            double discountTotal = SelectedTicket.GetDiscountTotal();
            double tax = SelectedTicket.GetTax();
            return (subTotal - couponTotal - discountTotal + tax);
        }

        private double GetTotalPayments(IEnumerable<TicketPayment> payments)
        {
            if (payments == null)
                return 0;
            return payments.Sum(ticketPayment => ticketPayment.Amount);
        }

#if !DEMO
        [Obfuscation(Exclude = true)]
        void ActiveCashDrawer_StatusUpdateEvent(object sender, StatusUpdateEventArgs e)
        {
            CashDrawer drawer = (sender as CashDrawer);
            if (drawer == null) return;
            if (!drawer.DrawerOpened && IsTicketPayed)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(OnDrawerClose));
            }                
        }
#endif
        private void OnDrawerClose()
        {
            Window.GetWindow(this).Close();
        }

        private PaymentSource GetPaymentType(object sender)
        {
            if (Equals(sender, buttonCash))
                return PaymentSource.Cash;
            if (Equals(sender, buttonCheck))
                return PaymentSource.Check;
            if (Equals(sender, buttonCreditCard))
                return PaymentSource.Credit;
            // Placing a conditinal for the last value in the enum will just
            // confuse the compiler. else if (sender == buttonGiftCard)
            return PaymentSource.GiftCard;

        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (numberEntryControl.FloatValue != null)
            {
                double amount = numberEntryControl.FloatValue.Value;
                if (amount > 0)
                {
                    PaymentSource type = GetPaymentType(sender);
                    double sum = GetTotalPayments(TicketPayment.GetAll(SelectedTicket.PrimaryKey));
                    double total = GetTicketTotal();
                    double due = total - sum;
                    double actualAmount = amount;
                    if (amount > due)
                        amount = due;
                    TicketPayment.Add(SelectedTicket.PrimaryKey,
                        RegisterManager.ActiveRegisterDrawer.Id,
                        SessionManager.ActiveEmployee.Id, amount, type);
                    InitializeTicket();
                    if (IsTicketPayed)
                    {
                        double amountPaid = sum + actualAmount;
                        double change = actualAmount - amount;
                        labelAmountPayed.Content = amountPaid.ToString("C2");
                        labelAmountDueLabel.Content = Types.Strings.ChangeDue;
                        labelAmountDue.Content = change.ToString("C2");
                        RegisterManager.ActiveRegisterDrawer.AddToCurrentAmount(total);
                        SelectedTicket.SetCloseTime(DateTime.Now);
                        SelectedTicket.Update();
                        RegisterManager.OpenCashDrawer();
                    }
                }
                numberEntryControl.FloatValue = null;
            }
        }

    }
}
