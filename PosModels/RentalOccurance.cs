using System;
using System.Linq;
using System.Reflection;
using PosModels.Types;

namespace PosModels
{
    [ModeledDataClass()]
    public class RentalOccurance
    {
        #region Licensed Access Only
        static RentalOccurance()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RentalOccurance).Assembly.GetName().GetPublicKeyToken(),
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
        public int RentalId
        {
            get;
            private set;
        }

        [ModeledData()]
        public int CustomerId
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime RentalDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime ReturnDate
        {
            get;
            private set;
        }

        [ModeledData()]
        public DateTime? DateReturned
        {
            get;
            private set;
        }
    }
}
