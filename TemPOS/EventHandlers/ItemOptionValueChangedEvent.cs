using System;
using System.Reflection;
using PosModels;
using TemPOS.Types;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void ItemOptionValueChangedHandler(object sender, ItemOptionValueChangedArgs args);

    [Obfuscation(Exclude = true)]
    public class ItemOptionValueChangedArgs : EventArgs
    {
        public ItemOption ChangedItemOption
        {
            get;
            private set;
        }

        public ItemOptionFieldName FieldName
        {
            get;
            private set;
        }

        public ItemOptionValueChangedArgs(ItemOption changedItemOption, ItemOptionFieldName fieldName)
        {
            FieldName = fieldName;
            ChangedItemOption = changedItemOption;
        }
    }
}
