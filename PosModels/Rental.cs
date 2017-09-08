using System;
using System.Linq;
using System.Reflection;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class Rental
    {
        #region Licensed Access Only
        static Rental()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Rental).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        [ModeledData()]
        public int Id
        {
            get;
            private set;
        }

        [ModeledData()]
        public string Description
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? ReleaseDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime PurchaseDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public short Quantity
        {
            get;
            private set;
        }

        [ModeledData()]
        public double Cost
        {
            get;
            private set;
        }

        [ModeledData("DetailDescription")]
        [ModeledDataNullable()]
        public string DetailedDescription
        {
            get;
            private set;
        }
    }
}
