using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CouponEditorControl.xaml
    /// </summary>
    public partial class CouponEditorControl : UserControl
    {
        private bool _haltEvents = false;

        public Coupon ActiveCoupon
        {
            get
            {
                return couponEditorDetailsControl.ActiveCoupon;
            }
            set
            {
                _haltEvents = true;
                try
                {
                    couponEditorDetailsControl.ActiveCoupon = value;
                    couponCategorySelectionControl.ActiveCoupon = value;
                    couponItemSelectionControl.ActiveCoupon = value;
                    if (value == null)
                    {
                        couponCategorySelectionControl.SelectedCategoryIds = new int[0];
                        couponItemSelectionControl.SelectedItemIds = new int[0];
                    }
                    else
                    {
                        couponCategorySelectionControl.SelectedCategoryIds = GetCategoryIds(value);
                        couponItemSelectionControl.SelectedItemIds = GetItemIds(value);
                    }
                }
                catch (Exception ex)
                {
                    PosDialogWindow.ShowDialog(ex.Message, Strings.Exception);
                    _haltEvents = false;
                    throw ex;
                }
                _haltEvents = false;
            }
        }

        internal CouponEditorDetailsControl couponEditorDetailsControl
        {
            get { return tabControl.Tab1.DockedControl as CouponEditorDetailsControl; }
        }

        internal CouponCategorySelectionControl couponCategorySelectionControl
        {
            get { return tabControl.Tab2.DockedControl as CouponCategorySelectionControl; }
        }

        internal CouponItemSelectionControl couponItemSelectionControl
        {
            get { return tabControl.Tab3.DockedControl as CouponItemSelectionControl; }
        }

        private int[] GetItemIds(Coupon value)
        {
            return CouponItem.GetAll(value.Id).Select(item => item.ItemId).ToArray();
        }

        private int[] GetCategoryIds(Coupon value)
        {
            return CouponCategory.GetAll(value.Id).Select(item => item.CategoryId).ToArray();
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

        public CouponEditorControl()
        {
            InitializeComponent();
            couponEditorDetailsControl.ValueChanged += couponEditorDetailsControl_ValueChanged;
            couponCategorySelectionControl.ValueChanged += couponEditorDetailsControl_ValueChanged;
            couponItemSelectionControl.ValueChanged += couponEditorDetailsControl_ValueChanged;
        }

        [Obfuscation(Exclude = true)]
        void couponEditorDetailsControl_ValueChanged(object sender, CouponValueChangedArgs args)
        {
            DoChangedValueEvent(args.FieldName);
        }

        /*
[Obfuscation(Exclude = true)]
         * private void button_Selected(object sender, EventArgs e)
        {
            if (sender == buttonItemSelect)
                SelectItems();
            if (sender == buttonCategorySelect)
                SelectCategories();
        }
        */

        /*
        private void SelectCategories()
        {
            CouponCategorySelectionControl control = new CouponCategorySelectionControl(pendingCategoryIds);
            PosDialogWindow window = new PosDialogWindow(control, Strings.SelectCategories, 630, 630);
            PosDialogWindow currentWindow = (PosDialogWindow)Window.GetWindow(this);
            currentWindow.ShowShadingOverlay = true;
            window.ShowDialog();
            currentWindow.ShowShadingOverlay = false;
            if (!CouponEditorDetailsControl.EqualIdCollections(pendingCategoryIds, control.SelectedCategoryIds))
            {
                pendingCategoryIds = control.SelectedCategoryIds;
                DoChangedValueEvent(CouponFieldName.Categories);
            }
        }

        private void SelectItems()
        {
            CouponItemSelectionControl control = new CouponItemSelectionControl(pendingItemIds);
            PosDialogWindow window = new PosDialogWindow(control, Strings.SelectItems, 630, 630);
            PosDialogWindow currentWindow = (PosDialogWindow)Window.GetWindow(this);
            currentWindow.ShowShadingOverlay = true;
            window.ShowDialog();
            currentWindow.ShowShadingOverlay = false;
            if (!CouponEditorDetailsControl.EqualIdCollections(pendingItemIds, control.SelectedItemIds))
            {
                pendingItemIds = control.SelectedItemIds;
                DoChangedValueEvent(CouponFieldName.Items);
            }

        }
        */

        internal bool UpdateCoupon()
        {
            return couponEditorDetailsControl.UpdateCoupon();
        }
    }
}
