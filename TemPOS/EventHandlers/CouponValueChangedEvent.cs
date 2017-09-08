using System;
using System.Reflection;
using PosModels;
using TemPOS.Types;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void CouponValueChangedHandler(object sender, CouponValueChangedArgs args);

    [Obfuscation(Exclude = true)]
    public class CouponValueChangedArgs : EventArgs
    {
        public Coupon ChangedCoupon
        {
            get;
            private set;
        }

        public CouponFieldName FieldName
        {
            get;
            private set;
        }

        public CouponValueChangedArgs(Coupon coupon, CouponFieldName field)
        {
            ChangedCoupon = coupon;
            FieldName = field;
        }
    }
}
