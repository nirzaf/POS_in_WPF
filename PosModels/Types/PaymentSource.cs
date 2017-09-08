using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    public enum PaymentSource : byte
    {
        Unknown,
        Cash,
        Credit,
        Check,
        GiftCard
    }

    public static class PaymentSourceExtensions
    {
        public static PaymentSource GetPaymentSource(this byte paymentSource)
        {
            try
            {
                return (PaymentSource)paymentSource;
            }
            catch
            {
                return PaymentSource.Unknown;
            }
        }
    }

}
