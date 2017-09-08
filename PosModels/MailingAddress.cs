using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels
{
    public class MailingAddress
    {
        public string Address1
        {
            get;
            private set;
        }

        public string Address2
        {
            get;
            private set;
        }

        public string City
        {
            get;
            private set;
        }

        public string State
        {
            get;
            private set;
        }

        public int ZipCode
        {
            get;
            private set;
        }

        public MailingAddress(string address1, string address2, string city,
            string state, int zipCode)
        {
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}
