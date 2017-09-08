using System;
using System.Reflection;
using PosModels;
using TemPOS.Types;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void TaxValueChangedHandler(object sender, TaxValueChangedArgs args);

    [Obfuscation(Exclude = true)]
    public class TaxValueChangedArgs : EventArgs
    {
        public Tax ChangedTax
        {
            get;
            private set;
        }

        public TaxFieldName FieldName
        {
            get;
            private set;
        }

        public TaxValueChangedArgs(Tax tax, TaxFieldName field)
        {
            ChangedTax = tax;
            FieldName = field;
        }
    }
}
