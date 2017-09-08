using System;
using System.Reflection;
using PosModels;
using TemPOS.Types;

namespace TemPOS.EventHandlers
{
    [Obfuscation(Exclude = true)]
    public delegate void CategoryValueChangedHandler(object sender, CategoryValueChangedArgs args);

    [Obfuscation(Exclude = true)]
    public class CategoryValueChangedArgs : EventArgs
    {
        public Category ChangedCategory
        {
            get;
            private set;
        }

        public CategoryFieldName FieldName
        {
            get;
            private set;
        }

        public CategoryValueChangedArgs(Category category, CategoryFieldName field)
        {
            ChangedCategory = category;
            FieldName = field;
        }
    }
}
