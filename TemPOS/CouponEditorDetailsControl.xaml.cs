using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using System.Globalization;
using PosControls.Types;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CouponEditorDetailsControl.xaml
    /// </summary>
    public partial class CouponEditorDetailsControl : UserControl
    {
        private bool _haltEvents;
        private int[] _pendingItemIds;
        private int[] _pendingCategoryIds;
        private Coupon _activeCoupon;

        public Coupon ActiveCoupon
        {
            get
            {
                return _activeCoupon;
            }
            set
            {
                _haltEvents = true;
                _activeCoupon = value;
                InitializeFields();
                _haltEvents = false;
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event CouponValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(CouponFieldName field)
        {
            if ((ValueChanged != null) && (ActiveCoupon != null) && !_haltEvents)
                ValueChanged.Invoke(this, new CouponValueChangedArgs(ActiveCoupon, field));
        }
        #endregion

        public CouponEditorDetailsControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            comboBoxCouponExclusionSelection.Items.Clear();
            comboBoxCouponExclusionSelection.Items.Add(Strings.CouponEditorExcludeAllExceptFor);
            comboBoxCouponExclusionSelection.Items.Add(Strings.CouponEditorIncludeAllExceptFor);

            if (ActiveCoupon != null)
            {
                radioButtonIsPercentage.IsSelected = ActiveCoupon.AmountIsPercentage;
                radioButtonIsNotPercentage.IsSelected = !ActiveCoupon.AmountIsPercentage;
                if (ActiveCoupon.AmountIsPercentage)
                {
                    textBoxAmount.PromptType = CustomTextBoxType.Percentage;
                    textBoxAmount.Text = (ActiveCoupon.Amount * 100).ToString("F1") + "%";
                }
                else
                {
                    textBoxAmount.PromptType = CustomTextBoxType.Currency;
                    textBoxAmount.Text = ActiveCoupon.Amount.ToString("C2");
                }
                textBoxName.Text = ActiveCoupon.Description;
                radioButtonIsActive.IsSelected = ActiveCoupon.IsActive;
                radioButtonIsNotActive.IsSelected = !ActiveCoupon.IsActive;
                comboBoxCouponExclusionSelection.SelectedIndex = ((!ActiveCoupon.IsExclusive) ? 1 : 0);
                radioButtonApplyToAll.IsSelected = ActiveCoupon.ApplyToAll;
                radioButtonIsNotApplyToAll.IsSelected = !ActiveCoupon.ApplyToAll;
                radioButtonIsThirdParty.IsSelected = ActiveCoupon.ThirdPartyCompensation;
                radioButtonIsNotThirdParty.IsSelected = !ActiveCoupon.ThirdPartyCompensation;
                // Initilize the existing collections
                _pendingItemIds = GetItemIds();
                _pendingCategoryIds = GetCategoryIds();
                if (ActiveCoupon.AmountLimit.HasValue)
                    textBoxAmountLimit.Text = "" + ActiveCoupon.AmountLimit.Value.ToString("C2");
                else
                    textBoxAmountLimit.Text = "";
                if (ActiveCoupon.LimitPerTicket.HasValue)
                    textBoxLimitPerTicket.Text = "" + ActiveCoupon.LimitPerTicket.Value;
                else
                    textBoxLimitPerTicket.Text = "";
            }
            else
            {
                textBoxAmountLimit.Text =
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "0.00";
                textBoxAmount.Text = "0%";
                textBoxName.Text = "";
                radioButtonIsPercentage.IsSelected = false;
                radioButtonIsNotPercentage.IsSelected = true;
                radioButtonIsNotThirdParty.IsSelected = true;
                radioButtonIsThirdParty.IsSelected = false;
                comboBoxCouponExclusionSelection.SelectedIndex = 0;
                radioButtonIsActive.IsSelected = true;
                radioButtonIsNotActive.IsSelected = false;
                radioButtonApplyToAll.IsSelected = false;
                radioButtonIsNotApplyToAll.IsSelected = true;
                _pendingItemIds = null;
                _pendingCategoryIds = null;
                textBoxAmount.PromptType = CustomTextBoxType.Currency;
            }
        }

        private int[] GetCategoryIds()
        {
            if (ActiveCoupon == null)
                return null;
            IEnumerable<CouponCategory> categories = CouponCategory.GetAll(ActiveCoupon.Id);
            List<int> selected = categories.Select(category => category.CategoryId).ToList();
            if (selected.Count == 0)
                return null;
            selected.Sort();
            return selected.ToArray();
        }

        private int[] GetItemIds()
        {
            if (ActiveCoupon == null)
                return null;
            List<int> selected =
                CouponItem.GetAll(ActiveCoupon.Id)
                .Select(item => item.ItemId).ToList();
            if (selected.Count == 0)
                return null;
            selected.Sort();
            return selected.ToArray();
        }

        public bool UpdateCoupon()
        {
            string description;
            double amount;
            bool isPercentage;
            bool isActive;
            bool isExclusive;
            bool applyToAll;
            bool isThirdParty;
            double? amountLimit;
            int? limitPerTicket;
            try
            {
                description = GetDescription();
                amount = GetAmount();
                isPercentage = GetPercentage();
                isActive = GetIsActive();
                isExclusive = GetIsExclusive();
                applyToAll = GetApplyToAll();
                isThirdParty = GetIsThirdParty();
                amountLimit = GetAmountLimit();
                limitPerTicket = GetLimitPerTicket();
            }
            catch (Exception ex)
            {
                // Message the user regarding the first invalid field
                string innerMessage = "";
                if (ex.InnerException != null)
                    innerMessage = ex.InnerException.Message;
                MessageBox.Show(
                    ex.Message + Environment.NewLine + Environment.NewLine + innerMessage,
                    Strings.Exception);
                return false;
            }

            // Is there an ActiveCategory?
            if (ActiveCoupon == null)
            {
                ActiveCoupon = Coupon.Add(description, amount, isPercentage, isActive,
                    isExclusive, applyToAll, isThirdParty, amountLimit, limitPerTicket);
            }
            else
            {
                // Update the category values for the ActiveCategory
                ActiveCoupon.SetDescription(description);
                ActiveCoupon.SetAmount(amount);
                ActiveCoupon.SetIsPercentage(isPercentage);
                ActiveCoupon.SetIsActive(isActive);
                ActiveCoupon.SetExclusive(isExclusive);
                ActiveCoupon.SetApplyToAll(applyToAll);
                ActiveCoupon.SetAmountLimit(amountLimit);
                ActiveCoupon.SetLimitPerTicket(limitPerTicket);
                ActiveCoupon.SetThirdPartyCompensation(isThirdParty);

                // Update the database
                ActiveCoupon.Update();
            }

            UpdateCouponItems();
            UpdateCouponCategories();
            return true;
        }

        private void UpdateCouponCategories()
        {
            int[] currentCategoryIds = GetCategoryIds();
            if (EqualIdCollections(currentCategoryIds, _pendingCategoryIds))
                return;
            if (currentCategoryIds != null)
            {
                List<CouponCategory> couponCategories = new List<CouponCategory>(CouponCategory.GetAll(ActiveCoupon.Id));

                // Remove any in current, that doesn't exist in pending
                foreach (CouponCategory existing in
                    from id in currentCategoryIds
                    where _pendingCategoryIds == null || !_pendingCategoryIds.Any(testId => testId == id)
                    select CouponCategory.FindByCategoryId(couponCategories, id))
                {
                    CouponCategory.Delete(existing.Id);
                }
            }

            // Add any missing pending to the current
            if (_pendingCategoryIds == null) return;
            foreach (int id in _pendingCategoryIds
                .Where(id => !(currentCategoryIds != null && currentCategoryIds.Any(testId => testId == id))))
            {
                // Add a new CouponCategory for this Coupon
                CouponCategory.Add(ActiveCoupon.Id, id);
            }
        }

        private void UpdateCouponItems()
        {
            int[] currentItemIds = GetItemIds();
            if (EqualIdCollections(currentItemIds, _pendingItemIds))
                return;

            if (currentItemIds != null)
            {
                List<CouponItem> couponItems = new List<CouponItem>(CouponItem.GetAll(ActiveCoupon.Id));

                // Remove any in current, that doesn't exist in pending
                foreach (CouponItem existing in
                    from id in currentItemIds
                    where _pendingItemIds == null || !_pendingItemIds.Any(testId => testId == id)
                    select CouponItem.FindByItemId(couponItems, id))
                {
                    CouponItem.Delete(existing.Id);
                }
            }

            // Add any missing pending to the current
            if (_pendingItemIds != null)
            {
                foreach (int id in _pendingItemIds
                    .Where(id => !(currentItemIds != null && currentItemIds.Any(testId => testId == id))))
                {
                    // Add a new CouponCategory for this Coupon
                    CouponItem.Add(ActiveCoupon.Id, id);
                }
            }

        }

        private bool GetApplyToAll()
        {
            return radioButtonApplyToAll.IsSelected;
        }

        private bool GetIsThirdParty()
        {
            return radioButtonIsThirdParty.IsSelected;
        }

        private int? GetLimitPerTicket()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxLimitPerTicket.Text))
                    return null;
                return Convert.ToInt32(textBoxLimitPerTicket.Text);
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CouponEditorInvlaidLimitPerTicket, innerException);
            }
        }

        private double? GetAmountLimit()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxAmountLimit.Text))
                    return null;
                return Convert.ToDouble(textBoxAmountLimit.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CouponEditorInvalidAmountLimit, innerException);
            }
        }

        private bool GetIsExclusive()
        {
            return (comboBoxCouponExclusionSelection.SelectedIndex == 0);
        }

        private string GetDescription()
        {
            try
            {
                return textBoxName.Text;
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CouponEditorInvalidName, innerException);
            }
        }

        private double GetAmount()
        {
            try
            {
                double amount = Convert.ToDouble(textBoxAmount.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "").
                    Replace("%", ""));
                if (radioButtonIsPercentage.IsSelected)
                    amount = amount / 100;
                return amount;
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CouponEditorInvalidAmount, innerException);
            }
        }

        private bool GetPercentage()
        {
            if ((radioButtonIsNotPercentage.IsSelected != true) && (radioButtonIsPercentage.IsSelected != true))
                throw new Exception(Strings.CouponEditorInvalidType);
            return radioButtonIsPercentage.IsSelected;
        }

        private bool GetIsActive()
        {
            return (radioButtonIsActive.IsSelected);
        }

        public static bool EqualIdCollections(int[] x, int[] y)
        {
            // Both null is, equal
            if ((x == null) && (y == null))
                return true;
            // One is null, one is not null, not equal
            if ((x == null) || (y == null))
                return false;
            // Different sizes, not equal
            return x.Length == y.Length && 
                x.Select(xValue => y.Any(yValue => xValue == yValue))
                .All(found => found);
        }

        private void SetAmountPromptType()
        {
            bool isPercentage = radioButtonIsPercentage.IsSelected;
            textBoxAmount.PromptType = isPercentage ?
                CustomTextBoxType.Percentage :
                CustomTextBoxType.Currency;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(CouponFieldName.Description);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(CouponFieldName.Amount);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLimitPerTicket_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(CouponFieldName.LimitPerTicket);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAmountLimit_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(CouponFieldName.AmountLimit);
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxCouponExclusionSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoChangedValueEvent(CouponFieldName.IsExclusive);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsPercentage_SelectionGained(object sender, EventArgs e)
        {
            radioButtonIsPercentage.IsSelected = Equals(sender, radioButtonIsPercentage);
            radioButtonIsNotPercentage.IsSelected = Equals(sender, radioButtonIsNotPercentage);
            SetAmountPromptType();
            DoChangedValueEvent(CouponFieldName.IsPercentage);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsActive_SelectionGained(object sender, EventArgs e)
        {
            radioButtonIsActive.IsSelected = Equals(sender, radioButtonIsActive);
            radioButtonIsNotActive.IsSelected = Equals(sender, radioButtonIsNotActive);
            DoChangedValueEvent(CouponFieldName.IsActive);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsThirdParty_SelectionGained(object sender, EventArgs e)
        {
            radioButtonIsThirdParty.IsSelected = Equals(sender, radioButtonIsThirdParty);
            radioButtonIsNotThirdParty.IsSelected = Equals(sender, radioButtonIsNotThirdParty);
            DoChangedValueEvent(CouponFieldName.ThirdParty);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonApplyToAll_SelectionGained(object sender, EventArgs e)
        {
            radioButtonApplyToAll.IsSelected = Equals(sender, radioButtonApplyToAll);
            radioButtonIsNotApplyToAll.IsSelected = Equals(sender, radioButtonIsNotApplyToAll);
            DoChangedValueEvent(CouponFieldName.ApplyToAll);
        }

    }
}
