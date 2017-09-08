using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using PosModels;
using TemPOS.Types;
using PosModels.Types;
using PosControls;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketCashoutControl.xaml
    /// </summary>
    public partial class TicketCashoutControl : UserControl
    {
        private Ticket _ticket;
        public Ticket SelectedTicket
        {
            get { return _ticket; }
            set
            {
                _ticket = value;
                couponEntryControl.Items = listBoxTicketItems.Items;
                discountControl.SelectedTicket = value;
                paymentControl.SelectedTicket = value;
                couponEntryControl.SelectedTicket = value;
                InitializeTicket();
            }
        }

        public TicketItem[] TicketItems
        {
            get
            {
                return (
                    from TicketItemTemplate listItem in listBoxTicketItems.Items
                    select listItem.TicketItem).ToArray();
            }
        }

        public bool IsPaymentComplete
        {
            get { return paymentControl.IsTicketPayed; }
        }

        public TicketCashoutControl()
        {
            InitializeComponent();
            if (!SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterRefund))
                buttonRefund.Visibility = Visibility.Collapsed;
            Loaded += TicketCashoutControl_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void TicketCashoutControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closed += TicketCashoutControl_Closed;
#if !DEMO
            if (!ConfigurationManager.IsInDesignMode && DeviceManager.ActiveScanner != null)
            {
                DeviceManager.ActiveScanner.DataEvent += ActiveScanner_DataEvent;
                DeviceManager.ActiveScanner.ErrorEvent += ActiveScanner_ErrorEvent;
            }
#endif
        }

        [Obfuscation(Exclude = true)]
        void TicketCashoutControl_Closed(object sender, EventArgs e)
        {
            if (!paymentControl.IsTicketPayed)
                TicketPayment.DeleteAll(SelectedTicket.PrimaryKey);
        }


#if !DEMO
        [Obfuscation(Exclude = true)]
        void ActiveScanner_ErrorEvent(object sender, Microsoft.PointOfService.DeviceErrorEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        void ActiveScanner_DataEvent(object sender, Microsoft.PointOfService.DataEventArgs e)
        {
            //MessageBox.Show(data, "Data");
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)(DoCreditCard));
        }
#endif

        private void DoCreditCard()
        {
#if !DEMO
            string data = Encoding.ASCII.GetString(DeviceManager.ActiveScanner.ScanData);
#endif
        }

        public void InitializeTicket()
        {
            listBoxTicketItems.Items.Clear();
            foreach (TicketItem ticketItem in
                from ticketItem in TicketItem.GetAll(SelectedTicket.PrimaryKey, false)
                let item = Item.Get(ticketItem.ItemId) select ticketItem)
            {
                listBoxTicketItems.Items.Add(new TicketItemTemplate(ticketItem));
            }
        }

        private void TaxExemption()
        {
            string result =
                PosDialogWindow.PromptKeyboard(Types.Strings.EnterTaxExemptionId,
                SelectedTicket.TaxExemptId);
            if (result == null)
                return;
            if (String.IsNullOrEmpty(result))
                result = null;
            SelectedTicket.SetTaxExemptId(result);
            SelectedTicket.Update();
            // Update the tax on the payment control
            paymentControl.SelectedTicket = SelectedTicket;
        }

        private void ModifyDiscounts()
        {
            if (buttonCoupon.IsChecked == true)
                ModifyCouponsNow();
            ModifyDiscountsNow();
            SelectedTicket = SelectedTicket;
        }

        private void ModifyDiscountsNow()
        {
            buttonDiscount.IsChecked = (buttonDiscount.IsChecked != true);
            discountControl.Visibility = (buttonDiscount.IsChecked == true ?
                Visibility.Visible : Visibility.Hidden);
            paymentControl.Visibility = (buttonDiscount.IsChecked == true ?
                Visibility.Hidden : Visibility.Visible);
            labelTopRight.Content = (buttonDiscount.IsChecked == true ?
                Types.Strings.TicketDiscounts : Types.Strings.TicketCashout);
        }

        private void ModifyCoupons()
        {
            if (buttonDiscount.IsChecked == true)
                ModifyDiscountsNow();
            ModifyCouponsNow();
            SelectedTicket = SelectedTicket;
        }

        private void ModifyCouponsNow()
        {
            buttonCoupon.IsChecked = (buttonCoupon.IsChecked != true);
            couponEntryControl.Visibility = (buttonCoupon.IsChecked == true ?
                Visibility.Visible : Visibility.Hidden);
            paymentControl.Visibility = (buttonCoupon.IsChecked == true ?
                Visibility.Hidden : Visibility.Visible);
            labelTopRight.Content = (buttonCoupon.IsChecked == true ?
                Types.Strings.TicketCoupons : Types.Strings.TicketCashout);
        }

        private void RefundTicket()
        {
            TicketRefundType? refundType = PosHelper.ConfirmRefundPrompt();
            if (refundType != null)
            {
                double refundAmount = PosHelper.RefundTicketCommand(SelectedTicket, refundType.Value);

                RegisterManager.OpenCashDrawer();

                PosDialogWindow.ShowDialog(Types.Strings.RefundedTotalIs +
                    refundAmount.ToString("C2"), Types.Strings.RefundTotal, DialogButtons.Ok);
            }
            Window window = Window.GetWindow(this);
            if (window != null) window.Close();
        }

        [Obfuscation(Exclude = true)]
        private void paymentControl_TicketPayed(object sender, EventArgs e)
        {
#if !DEMO
            if (DeviceManager.ActivePosPrinterLocal != null)
                buttonPrintReceipt.IsEnabled = true;
#else
            buttonPrintReceipt.IsEnabled = false;
#endif
            buttonRefund.IsEnabled = true;
            buttonCoupon.IsEnabled = false;
            buttonDiscount.IsEnabled = false;
            buttonTaxExemption.IsEnabled = false;
            paymentControl.IsEnabled = false;
            listBoxTicketItems.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxTicketItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            TicketItemTemplate item = e.AddedItems[0] as TicketItemTemplate;
            if (item != null) couponEntryControl.SelectedTicketItem = item.TicketItem;
        }

        [Obfuscation(Exclude = true)]
        private void buttonRefund_Click(object sender, RoutedEventArgs e)
        {
            RefundTicket();
        }

        [Obfuscation(Exclude = true)]
        private void buttonPrintReceipt_Click(object sender, RoutedEventArgs e)
        {
#if !DEMO
            PrinterManager.Print(PrintDestination.Receipt,
                DeviceManager.ActivePosPrinterLocal, SelectedTicket,
                TicketItemPrintOptions.AllNonCanceled);
#else
            PosDialogWindow.ShowDialog(Types.Strings.DisabledInDemoVersion, Types.Strings.Disabled);
#endif
        }

        [Obfuscation(Exclude = true)]
        private void buttonCoupon_Click(object sender, RoutedEventArgs e)
        {
            ModifyCoupons();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDiscount_Click(object sender, RoutedEventArgs e)
        {
            ModifyDiscounts();
        }

        [Obfuscation(Exclude = true)]
        private void buttonTaxExemption_Click(object sender, RoutedEventArgs e)
        {
            TaxExemption();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            TicketCashoutControl control = new TicketCashoutControl();
            return new PosDialogWindow(control, Types.Strings.CashOutTicket, 900, 610);
        }

    }
}
