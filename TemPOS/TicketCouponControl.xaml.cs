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
using PosModels;
using PosControls;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketCouponControl.xaml
    /// </summary>
    public partial class TicketCouponControl : UserControl
    {
        #region Fields
        private Ticket _selectedTicket;
        private TicketItem _selectedTicketItem;
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
                    InitializeAppliedCoupons();
                else
                    listBoxApplied.Items.Clear();
                InitializeAvailableCoupons();
            }
        }

        public TicketItem SelectedTicketItem
        {
            get { return _selectedTicketItem; }
            set {
                _selectedTicketItem = value;
                SelectedItem = value != null ? Item.Get(value.ItemId) : null;
                InitializeAvailableCoupons();
            }
        }

        public Item SelectedItem
        {
            get;
            set;
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler CouponsChanged;
        private void DoCouponsChangedEvent()
        {
            if ((CouponsChanged != null) && (SelectedTicket != null))
                CouponsChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public TicketCouponControl()
        {
            InitializeComponent();
        }

        private void InitializeAppliedCoupons()
        {
            List<int> addedIds = new List<int>();
            listBoxApplied.Items.Clear();
            IEnumerable<TicketCoupon> allTicketCoupons = TicketCoupon.GetAll(SelectedTicket.PrimaryKey);
            foreach (TicketCoupon ticketCoupon in allTicketCoupons)
            {
                Coupon coupon = Coupon.Get(ticketCoupon.CouponId);
                if (!addedIds.Contains(coupon.Id))
                {
                    addedIds.Add(coupon.Id);
                    AddApplied(coupon);
                }
                else if (!coupon.ApplyToAll)
                {
                    AddApplied(coupon);
                }
            }
        }

        private void InitializeAvailableCoupons()
        {
            List<Coupon> allCoupons = new List<Coupon>(Coupon.GetAll());
            List<CouponCategory> allCategories = new List<CouponCategory>(CouponCategory.GetAll());
            List<CouponItem> allItems = new List<CouponItem>(CouponItem.GetAll());

            listBoxAvailable.Items.Clear();
            foreach (Coupon coupon in allCoupons)
            {
                if (!coupon.IsActive)
                    continue;
                List<CouponCategory> couponCategories =
                    new List<CouponCategory>(CouponCategory.FindAllByCouponId(allCategories, coupon.Id));
                List<CouponItem> couponItems = 
                    new List<CouponItem>(CouponItem.FindAllByCouponId(allItems, coupon.Id));

                // Check for applied coupons and exclude them
                if ((from FormattedListBoxItem item in listBoxApplied.Items
                     select (Coupon)item.ReferenceObject)
                     .Any(appliedCoupon => appliedCoupon.Id == coupon.Id))
                    continue;

                // Add applicable coupons
                bool add = false;
                if (coupon.ApplyToAll)
                {
                    add = true;
                }
                else if ((SelectedTicketItem == null) && (couponItems.Count == 0) && (couponCategories.Count == 0))
                {
                    add = true;
                }
                else if (SelectedTicketItem != null)
                {
                    Category category = Category.Get(SelectedItem.CategoryId);
                    if (coupon.IsExclusive)
                    {
                        HasRestrictions(coupon, SelectedTicketItem);

                        if (couponItems.Any(couponItem => SelectedTicketItem.ItemId == couponItem.ItemId))
                        {
                            add = true;
                        }
                        if (couponCategories.Any(couponCategory => category.Id == couponCategory.CategoryId))
                        {
                            add = true;
                        }
                    }
                    else
                    {
                        bool found = couponItems.Any(couponItem => SelectedTicketItem.ItemId == couponItem.ItemId) ||
                            couponCategories.Any(couponCategory => category.Id == couponCategory.CategoryId);
                        if (!found)
                            add = true;
                    }
                }
                if (add)
                    AddAvailable(coupon);
            }
        }

        private FormattedListBoxItem AddApplied(Coupon coupon)
        {
            FormattedListBoxItem result =
                new FormattedListBoxItem(coupon,
                    coupon.Description /* + Environment.NewLine +
                    String.Format("{0:C}", ticketCoupon.GetCouponValue()) */, true);
            listBoxApplied.Items.Add(result);
            return result;
        }

        private FormattedListBoxItem AddAvailable(Coupon coupon)
        {
            FormattedListBoxItem result =
                new FormattedListBoxItem(coupon, coupon.Description, true);
            listBoxAvailable.Items.Add(result);
            return result;
        }

        private void ApplyCoupon()
        {
            TicketCoupon result = null;

            FormattedListBoxItem selectedItem =
                (FormattedListBoxItem)listBoxAvailable.SelectedItem;

            Coupon coupon = (Coupon)selectedItem.ReferenceObject;
            if (coupon.ApplyToAll)
            {
                int count = 0;
                foreach (TicketItemTemplate ticketItem in Items)
                {
                    if (HasRestrictions(coupon, ticketItem.TicketItem))
                        continue;
                    // Create TicketCoupon
                    result = TicketCoupon.AddForTicketItem(coupon.Id,
                        SelectedTicket.PrimaryKey, ticketItem.TicketItem.PrimaryKey.Id);
                    count++;
                    if ((coupon.LimitPerTicket != null) &&
                        (count >= coupon.LimitPerTicket.Value))
                    {
                        break;
                    }
                }
            }
            else if (coupon.ApplyToTicket())
            {
                result = TicketCoupon.AddForTicket(coupon.Id, SelectedTicket.PrimaryKey);
            }
            else
            {
                if (SelectedTicketItem != null)
                {
                    if (!HasRestrictions(coupon, SelectedTicketItem))
                        result = TicketCoupon.AddForTicketItem(coupon.Id, SelectedTicket.PrimaryKey,
                            _selectedTicketItem.PrimaryKey.Id);
                }
                else
                {
                    // this shouldn't happen
                    PosDialogWindow.ShowDialog(
                        Strings.YouNeedToSelectATicketItemToUseThisCouponOn,
                        Strings.Error);
                    return;
                }
            }
            if (result == null)
                PosDialogWindow.ShowDialog(
                    Strings.ThatCouponCanNotBeAppliedToAnyItemsOnThisTicket, Strings.Error);
            else
            {
                listBoxAvailable.SelectedItem = null;
                listBoxAvailable.Items.Remove(selectedItem);
                listBoxAvailable.SelectedItem = AddApplied(coupon);
            }
        }

        private bool HasRestrictions(Coupon coupon, TicketItem ticketItem)
        {
            Item item = Item.Get(ticketItem.ItemId);
            // Coupon Category Restrictions
            List<CouponCategory> cats = new List<CouponCategory>(CouponCategory.GetAll(coupon.Id));
            if ((cats.Count > 0) && coupon.IsExclusive &&
                CouponCategory.FindByCategoryId(cats, item.CategoryId) == null)
                return true;
            if ((cats.Count > 0) && !coupon.IsExclusive &&
                CouponCategory.FindByCategoryId(cats, item.CategoryId) != null)
                return true;
            // Coupon Item Restrictions
            List<CouponItem> items = new List<CouponItem>(CouponItem.GetAll(coupon.Id));
            if ((items.Count > 0) && coupon.IsExclusive &&
                CouponItem.FindByItemId(items, item.Id) == null)
                return true;
            if ((items.Count > 0) && !coupon.IsExclusive &&
                CouponItem.FindByItemId(items, item.Id) != null)
                return true;
            return false;
        }

        private void RemoveCoupon()
        {
            FormattedListBoxItem selectedItem =
                (FormattedListBoxItem)listBoxApplied.SelectedItem;
            listBoxApplied.SelectedItem = null;
            listBoxApplied.Items.Remove(selectedItem);

            Coupon coupon =
                (Coupon)selectedItem.ReferenceObject;

            IEnumerable<TicketCoupon> allCoupons = TicketCoupon.GetAll(SelectedTicket.PrimaryKey);
            foreach (TicketCoupon ticketCoupon in allCoupons)
            {
                if (ticketCoupon.CouponId == coupon.Id)
                {
                    TicketCoupon.Delete(ticketCoupon.PrimaryKey);
                }
            }

            InitializeAvailableCoupons();
        }
        
        [Obfuscation(Exclude = true)]
        private void listBoxAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxApplied.SelectedItem = null;
            buttonApplyCoupon.IsEnabled = true;
            buttonClearSelectedCoupon.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxApplied_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            listBoxAvailable.SelectedItem = null;
            buttonApplyCoupon.IsEnabled = false;
            buttonClearSelectedCoupon.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonApplyCoupon_Click(object sender, RoutedEventArgs e)
        {
            ApplyCoupon();
            buttonApplyCoupon.IsEnabled = false;
            buttonClearSelectedCoupon.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonClearSelectedCoupon_Click(object sender, RoutedEventArgs e)
        {
            RemoveCoupon();
            buttonApplyCoupon.IsEnabled = false;
            buttonClearSelectedCoupon.IsEnabled = false;
        }

    }
}
