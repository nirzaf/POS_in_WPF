using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using PosModels.Types;

namespace PosModels
{
    [Serializable]
    public class CreditCardInfo
    {
        #region Licensed Access Only
        static CreditCardInfo()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(CreditCardInfo).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public string CardNumber
        {
            get;
            private set;
        }

        [ModeledData()]
        public int ExpirationMonth
        {
            get;
            private set;
        }

        [ModeledData()]
        public int ExpirationYear
        {
            get;
            private set;
        }

        [ModeledData()]
        public int CardSecurityCode
        {
            get;
            private set;
        }

        [ModeledData()]
        public string OwnersName
        {
            get;
            private set;
        }

        public MailingAddress BillingAddress
        {
            get;
            private set;
        }

        public CreditCardInfo(string cardNumber, int expirationMonth, int expirationYear,
            int cardSecurityCode, string ownersName, MailingAddress billingAddress)
        {
            CardNumber = cardNumber;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            CardSecurityCode = cardSecurityCode;
            OwnersName = ownersName;
            BillingAddress = billingAddress;
        }
    }
}
