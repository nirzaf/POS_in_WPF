using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;
using PosModels.Types;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketDiscountControl.xaml
    /// </summary>
    public partial class TicketDiscountControl : UserControl
    {
        #region Fields
        private Ticket _selectedTicket;
        private UIElementCollection _receiptTapeItems;
        #endregion

        #region Properties
        public UIElementCollection Items
        {
            get {
                if (_receiptTapeItems == null)
                    return OrderEntryControl.ReceiptTapeItems;
                return _receiptTapeItems;
            }
            set { _receiptTapeItems = value; }
        }

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set {
                _selectedTicket = value;
                if (value != null)
                    InitializeAppliedDiscounts();
                else
                    listBoxApplied.Items.Clear();
                InitializeAvailableDiscounts();
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler DiscountsChanged;
        private void DoDiscountsChangedEvent()
        {
            if ((DiscountsChanged != null) && (SelectedTicket != null))
                DiscountsChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public TicketDiscountControl()
        {
            InitializeComponent();
        }

        private void InitializeAppliedDiscounts()
        {
            List<int> addedIds = new List<int>();
            listBoxApplied.Items.Clear();
            foreach (Discount coupon in TicketDiscount.GetAll(SelectedTicket.PrimaryKey)
                .Select(ticketDiscount => Discount.Get(ticketDiscount.DiscountId))
                .Where(coupon => !addedIds.Contains(coupon.Id)))
            {
                addedIds.Add(coupon.Id);
                AddApplied(coupon);
            }
        }

        private void InitializeAvailableDiscounts()
        {
            listBoxAvailable.Items.Clear();
            foreach (Discount discount in Discount.GetAll(true))
            {
                if ((from FormattedListBoxItem item in listBoxApplied.Items
                     select (Discount) item.ReferenceObject)
                     .All(appliedDiscount => appliedDiscount.Id != discount.Id))
                {
                    AddAvailable(discount);
                }
            }
        }

        private FormattedListBoxItem AddApplied(Discount discount)
        {
            FormattedListBoxItem result =
                new FormattedListBoxItem(discount, discount.Description, true);
            listBoxApplied.Items.Add(result);
            return result;
        }

        private FormattedListBoxItem AddAvailable(Discount discount)
        {
            FormattedListBoxItem result =
                new FormattedListBoxItem(discount, discount.Description, true);
            listBoxAvailable.Items.Add(result);
            return result;
        }

        private void ApplyDiscount()
        {
            TicketDiscount result = null;

            FormattedListBoxItem selectedItem =
                (FormattedListBoxItem)listBoxAvailable.SelectedItem;
            Discount discount = (Discount)selectedItem.ReferenceObject;

            bool hasPermission = (!discount.RequiresPermission ||
                SessionManager.ActiveEmployee.HasPermission(Permissions.RegisterDiscounts));
            if (!hasPermission &&
                (PosHelper.GetPermission(Permissions.RegisterDiscounts) == null))
            {
                PosDialogWindow.ShowDialog(
                    Types.Strings.YouDoNotHavePermissionToApplyThisDiscount,
                    Types.Strings.PermissionDenied);
                return;
            }

            if (discount.Amount == null)
            {
                double? amount = discount.AmountIsPercentage ?
                    PosDialogWindow.PromptPercentage(this, Types.Strings.EnterDiscountPercentage, null) :
                    PosDialogWindow.PromptNumber(Types.Strings.EnterDiscountAmount, (double?)null);
                if (amount != null)
                    result = TicketDiscount.Add(discount.Id, SelectedTicket.PrimaryKey,
                        amount.Value, SessionManager.PseudoEmployeeId);
            }
            else
            {
                result = TicketDiscount.Add(discount.Id, SelectedTicket.PrimaryKey,
                    null, SessionManager.PseudoEmployeeId);
            }

            if (result != null)
            {
                listBoxAvailable.SelectedItem = null;
                listBoxAvailable.Items.Remove(selectedItem);

                listBoxApplied.Items.Add(selectedItem);
                listBoxApplied.SelectedItem = selectedItem;
            }
        }

        private void RemoveDiscount()
        {
            FormattedListBoxItem selectedItem =
                (FormattedListBoxItem)listBoxApplied.SelectedItem;
            listBoxApplied.SelectedItem = null;
            listBoxApplied.Items.Remove(selectedItem);

            Discount discount =
                (Discount)selectedItem.ReferenceObject;

            IEnumerable<TicketDiscount> allDiscounts = TicketDiscount.GetAll(SelectedTicket.PrimaryKey);
            foreach (TicketDiscount ticketDiscount in allDiscounts)
            {
                if (ticketDiscount.DiscountId == discount.Id)
                {
                    TicketDiscount.Delete(ticketDiscount.PrimaryKey);
                }
            }

            InitializeAvailableDiscounts();
        }
        
        [Obfuscation(Exclude = true)]
        private void listBoxAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxApplied.SelectedItem = null;
            buttonApplyDiscount.IsEnabled = true;
            buttonClearSelectedDiscount.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxApplied_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxAvailable.SelectedItem = null;
            buttonApplyDiscount.IsEnabled = false;
            buttonClearSelectedDiscount.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            ApplyDiscount();
            buttonApplyDiscount.IsEnabled = false;
            buttonClearSelectedDiscount.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonClearSelectedDiscount_Click(object sender, RoutedEventArgs e)
        {
            RemoveDiscount();
            buttonApplyDiscount.IsEnabled = false;
            buttonClearSelectedDiscount.IsEnabled = false;
        }

    }
}
