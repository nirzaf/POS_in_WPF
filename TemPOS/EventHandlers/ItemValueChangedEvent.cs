using System;
using System.Reflection;
using PosModels;
using TemPOS.Types;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void ItemValueChangedHandler(object sender, ItemValueChangedArgs args);

    [Obfuscation(Exclude = true)]
    public class ItemValueChangedArgs : EventArgs
    {
        public Item ChangedItem
        {
            get;
            private set;
        }

        public ItemFieldName FieldName
        {
            get;
            private set;
        }

        public ItemValueChangedArgs(Item item, ItemFieldName field)
        {
            ChangedItem = item;
            FieldName = field;
        }
    }
}
