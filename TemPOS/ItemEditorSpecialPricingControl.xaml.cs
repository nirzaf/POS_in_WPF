using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using System.Globalization;
using PosModels.Types;
using PosControls;
using TemPOS.EventHandlers;
using TemPOS.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorSpecialPricingControl.xaml
    /// </summary>
    public partial class ItemEditorSpecialPricingControl : UserControl
    {
        private Item _activeItem;
        private bool _haltEvents = true;

        public class ItemPricingModel
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public Days DayOfWeek { get; set; }
            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public double Price { get; set; }
            
            public ItemPricingModel(ItemPricing itemPricing)
            {
                Id = itemPricing.Id;
                ItemId = itemPricing.ItemId;
                DayOfWeek = itemPricing.DayOfWeek;
                StartTime = itemPricing.StartTime;
                EndTime = itemPricing.EndTime;
                Price = itemPricing.Price;
            }

            public ItemPricingModel(int itemId, Days dayOfWeek,
                TimeSpan? startTime, TimeSpan? endTime, double price)
            {
                Id = 0;
                ItemId = itemId;
                DayOfWeek = dayOfWeek;
                StartTime = startTime;
                EndTime = endTime;
                Price = price;
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event ItemValueChangedHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if ((ValueChanged != null) && (ActiveItem != null) && !_haltEvents)
                ValueChanged.Invoke(this, new ItemValueChangedArgs(ActiveItem, ItemFieldName.SpecialPricing));
        }
        #endregion

        public Item ActiveItem
        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;
                if (value != null)
                {
                    ItemId = value.Id;
                    ExistingItems = new List<ItemPricingModel>();
                    foreach (ItemPricing specialPrice in
                        ItemPricing.GetAll(_activeItem.Id, false, false))
                    {
                        ExistingItems.Add(new ItemPricingModel(specialPrice));
                    }
                    NewItems = new List<ItemPricingModel>();
                    RemovedItems = new List<ItemPricingModel>();
                    ChangedItems = new List<ItemPricingModel>();
                    InitializeListBox();
                }
                else
                {
                    ItemId = 0;
                    ExistingItems = new List<ItemPricingModel>();
                    NewItems = new List<ItemPricingModel>();
                    RemovedItems = new List<ItemPricingModel>();
                    ChangedItems = new List<ItemPricingModel>();
                    InitializeListBox();
                }
            }
        }

        public List<ItemPricingModel> ExistingItems
        {
            get;
            private set;
        }

        public List<ItemPricingModel> NewItems
        {
            get;
            private set;
        }

        public List<ItemPricingModel> RemovedItems
        {
            get;
            private set;
        }

        public List<ItemPricingModel> ChangedItems
        {
            get;
            private set;
        }

        public int ItemId
        {
            get;
            private set;
        }

        public ItemEditorSpecialPricingControl()
        {
            ExistingItems = new List<ItemPricingModel>();
            NewItems = new List<ItemPricingModel>();
            RemovedItems = new List<ItemPricingModel>();
            ChangedItems = new List<ItemPricingModel>();
            InitializeComponent();
            InitializeDaysOfTheWeek();
            InitializeListBox();
            SetReadOnly(true);
            listBox1.SelectedItem = null;
            _haltEvents = false;
        }

        private void InitializeDaysOfTheWeek()
        {
            comboBoxDay.Items.Add(Days.Any.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Sunday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Monday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Tuesday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Wednesday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Thursday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Friday.ToLanguageSpecificString());
            comboBoxDay.Items.Add(Days.Saturday.ToLanguageSpecificString());
            comboBoxDay.SelectedIndex = 0;        
        }

        private void InitializeListBox(int? index = null)
        {
            _haltEvents = true;
            listBox1.Items.Clear();
            foreach (ItemPricingModel price in ExistingItems)
            {
                if (RemovedItems.Contains(price))
                    continue;
                AddItemPricingToListBox(price);
            }

            foreach (ItemPricingModel price in NewItems)
            {
                AddItemPricingToListBox(price);
            }
            if (listBox1.Items.Count > 0)
            {
                FormattedListBoxItem item = listBox1.Items[0] as FormattedListBoxItem;
                if (index.HasValue)
                {
                    if (index.Value >= listBox1.Items.Count)
                        item = (FormattedListBoxItem)listBox1.Items[listBox1.Items.Count - 1];
                    else
                        item = (FormattedListBoxItem)listBox1.Items[index.Value];
                }
                listBox1.SelectedItem = item;
                if (item != null)
                {
                    item.IsSelected = true;
                    InitializeFields((ItemPricingModel)item.ReferenceObject);
                }
                buttonRemove.IsEnabled = true;
                SetReadOnly(false);
            }
            else
            {
                ClearFields();
                buttonRemove.IsEnabled = false;
                SetReadOnly(true);
            }
            _haltEvents = false;
        }

        private void SetReadOnly(bool isReadOnly)
        {
            labelDay.IsEnabled =
                labelEndTime.IsEnabled =
                labelPrice.IsEnabled =
                labelStartTime.IsEnabled =
                comboBoxDay.IsEnabled =
                textBoxPrice.IsEnabled =
                timePickerEnd.IsEnabled =
                timePickerStart.IsEnabled = !isReadOnly;
        }

        private static string GenerateListBoxDescription(ItemPricingModel price)
        {
            string description = price.DayOfWeek.ToString();
            if (price.StartTime.HasValue && price.EndTime.HasValue)
			{
                description += Strings.ItemEditorListboxText1 +
					price.StartTime.Value.Hours + ":" +
                    price.StartTime.Value.Minutes.ToString("D2") +
                    Strings.ItemEditorListboxText2 +
					price.EndTime.Value.Hours + ":" +
					price.EndTime.Value.Minutes.ToString("D2");
			}
            description += " (" + String.Format("{0:C}", price.Price) + ")";
            return description;
        }

        private void AddItemPricingToListBox(ItemPricingModel price)
        {
            if (price == null)
                return;
            string description = GenerateListBoxDescription(price);
            FormattedListBoxItem item =
                new FormattedListBoxItem(price, description, true);
            listBox1.Items.Add(item);
            listBox1.SelectedItem = item;
            listBox1.ScrollIntoView(listBox1.SelectedItem);
        }

        private void InsertItemPricingInListBox(int index, ItemPricingModel price)
        {
            string description = GenerateListBoxDescription(price);
            if (price.Id > 0)
                listBox1.Items.Insert(index,
                    new FormattedListBoxItem(price, description, true));
            else
                listBox1.Items.Insert(index,
                    new FormattedListBoxItem(price, description, true));
            listBox1.SelectedItem = listBox1.Items[index];
            listBox1.ScrollIntoView(listBox1.SelectedItem);
        }

        private void AddItemPricing()
        {
            //ItemPricing dummy = new ItemPricing(0, ItemId, Types.Days.Any, 0f, null, null, null, false);
            //ItemPricing newItem = ItemPricing.Add(ItemId, Types.Days.Any, 0f, null, null, null, false);
            ItemPricingModel newItem = new ItemPricingModel(ItemId, Days.Any, null, null, 0f);
            AddItemPricingToListBox(newItem);
            NewItems.Add(newItem);
            foreach (FormattedListBoxItem item in listBox1.Items)
            {
                item.IsSelected = Equals(item, listBox1.SelectedItem);
            }
            listBox1.ScrollIntoView(listBox1.SelectedItem);
            InitializeFields(newItem);
            SetReadOnly(false);
            buttonRemove.IsEnabled = true;
            DoChangedValueEvent();
            //InitializeListBox();
        }

        private void RemoveItemPricing()
        {
            if (listBox1.SelectedItem != null)
            {
                int index = listBox1.SelectedIndex;
                FormattedListBoxItem pricing = (FormattedListBoxItem)listBox1.SelectedItem;
                ItemPricingModel model = (ItemPricingModel)pricing.ReferenceObject;
                if (model.Id == 0)
                    NewItems.Remove(model);
                else
                    RemovedItems.Add(model);
                listBox1.Items.Remove(pricing);
                ClearFields();
                DoChangedValueEvent();
                InitializeListBox(index);
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0)) return;
            FormattedListBoxItem item = (FormattedListBoxItem) e.AddedItems[0];
            ItemPricingModel model = (ItemPricingModel) item.ReferenceObject;
            InitializeFields(model);
            buttonRemove.IsEnabled = true;
            //SetReadOnly(false);
        }

        private void InitializeFields(ItemPricingModel item)
        {
            _haltEvents = true;
            textBoxPrice.Text = item.Price.ToString("C2");
            comboBoxDay.SelectedIndex = (int)item.DayOfWeek;
            if (item.StartTime.HasValue)
            {
                timePickerStart.Hour = item.StartTime.Value.Hours;
                timePickerStart.Minute = item.StartTime.Value.Minutes;
            }
            else
            {
                timePickerStart.Hour = 0;
                timePickerStart.Minute = 0;
            }
            timePickerStart.UpdateLayout();

            if (item.EndTime.HasValue)
            {
                timePickerEnd.Hour = item.EndTime.Value.Hours;
                timePickerEnd.Minute = item.EndTime.Value.Minutes;
            }
            else
            {
                timePickerEnd.Hour = 23;
                timePickerEnd.Minute = 59;
                timePickerEnd.UpdateLayout();
            }
            //textBoxDiscountMin.Text = "" + (item.AdditionalDiscountMin.HasValue ? "" + item.AdditionalDiscountMin.Value : "");
            //textBoxDiscountMax.Text = "" + (item.AdditionalDiscountMax.HasValue ? "" + item.AdditionalDiscountMax.Value : "");
            //textBoxDiscountPrice.Text = "" + (item.AdditionalDiscountPrice.HasValue ? "" + item.AdditionalDiscountPrice.Value : "");
            _haltEvents = false;
        }

        private void ClearFields()
        {
            _haltEvents = true;
            textBoxDiscountMax.Text = "";
            textBoxDiscountMin.Text = "";
            textBoxPrice.Text = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "0.00";
            textBoxDiscountPrice.Text = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "0.00";
            comboBoxDay.SelectedIndex = 0;
            timePickerStart.HourString = "00";
            timePickerStart.MinuteString = "00";
            timePickerEnd.HourString = "23";
            timePickerEnd.MinuteString = "59";
            _haltEvents = false;
        }

        private ItemPricingModel GetExistingItemPricing(int id)
        {
            return ExistingItems.FirstOrDefault(price => price.Id == id);
        }

        private ItemPricingModel GetChangedItem(ItemPricingModel changedItem)
        {
            return (ChangedItems == null) ? null :
                ChangedItems.FirstOrDefault(pricing => pricing.Id == changedItem.Id);
        }

        private double GetPrice()
        {
            try
            {
                return Convert.ToDouble(textBoxPrice.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch
            {
                return 0;
            }
        }

        private Days GetDayOfWeek()
        {
            return (Days)comboBoxDay.SelectedIndex;
        }

        public TimeSpan? GetStartTime()
        {
            return new TimeSpan(timePickerStart.Hour, timePickerStart.Minute, 00);
            /*
            if ((Convert.ToInt32(timePickerStart.HourString) == 12) && (Convert.ToInt32(timePickerStart.MinuteString) == 0) && (timePickerStart.IsAM) &&
                (Convert.ToInt32(timePickerEnd.HourString) == 11) && (Convert.ToInt32(timePickerEnd.MinuteString) == 59) && (timePickerEnd.IsPM))
                return null;
            return new TimeSpan(Convert.ToInt32(timePickerStart.HourString), Convert.ToInt32(timePickerStart.MinuteString), 0);
            */
        }

        public TimeSpan? GetEndTime()
        {
            return new TimeSpan(timePickerEnd.Hour, timePickerEnd.Minute, 59);
            /*
            if ((Convert.ToInt32(timePickerStart.HourString) == 12) && (Convert.ToInt32(timePickerStart.MinuteString) == 0) && (timePickerStart.IsAM) &&
                (Convert.ToInt32(timePickerEnd.HourString) == 11) && (Convert.ToInt32(timePickerEnd.MinuteString) == 59) && (timePickerEnd.IsPM))
                return null;
            return new TimeSpan(Convert.ToInt32(timePickerEnd.HourString), Convert.ToInt32(timePickerEnd.MinuteString), 0);
            */
        }

        public int? GetDiscountMax()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxDiscountMax.Text))
                    return null;
                return Convert.ToInt32(textBoxDiscountMax.Text);
            }
            catch
            {
                return null;
            }
        }

        public int? GetDiscountMin()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxDiscountMin.Text))
                    return null;
                return Convert.ToInt32(textBoxDiscountMin.Text);
            }
            catch
            {
                return null;
            }
        }

        public double? GetDiscountPrice()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxDiscountPrice.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "")))
                    return null;
                return Convert.ToDouble(textBoxDiscountPrice.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch
            {
                return null;
            }
        }

        [Obfuscation(Exclude = true)]
        private void pushComboBoxDayOfWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricingModel pricing = ((ItemPricingModel)itemPricing.ReferenceObject);
            pricing.DayOfWeek = GetDayOfWeek();
            if ((itemPricing.Id > 0) && (GetChangedItem(pricing) == null) && (pricing != null))
                ChangedItems.Add(pricing);
            itemPricing.Text = GenerateListBoxDescription(pricing);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPrice_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricingModel pricing = ((ItemPricingModel)itemPricing.ReferenceObject);
            pricing.Price = GetPrice();
            if ((itemPricing.Id > 0) && (GetChangedItem(pricing) == null) && (pricing != null))
                ChangedItems.Add(pricing);            
            itemPricing.Text = GenerateListBoxDescription(pricing);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void timePickerEnd_TimeChanged(object sender, EventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricingModel pricing = ((ItemPricingModel)itemPricing.ReferenceObject);
            pricing.EndTime = GetEndTime();
            if ((itemPricing.Id > 0) && (GetChangedItem(pricing) == null) && (pricing != null))
                ChangedItems.Add(pricing);
            itemPricing.Text = GenerateListBoxDescription(pricing);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void timePickerStart_TimeChanged(object sender, EventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricingModel pricing = ((ItemPricingModel)itemPricing.ReferenceObject);
            pricing.StartTime = GetStartTime();
            if ((itemPricing.Id > 0) && (GetChangedItem(pricing) == null) && (pricing != null))
                ChangedItems.Add(pricing);
            itemPricing.Text = GenerateListBoxDescription(pricing);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxDiscountMin_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            /*
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricing pricing = null;
            pricing = GetExistingItemPricing(itemPricing.Id);
            if (pricing != null)
            {
                pricing.SetAdditionalDiscountMin(GetDiscountMin());
                pricing.Update();
                if (GetChangedItem(pricing) == null)
            [Obfuscation(Exclude = true)]
                    ChangedItems.Add(pricing);
            [Obfuscation(Exclude = true)]
            }
            itemPricing.Text = GenerateListBoxDescription(pricing);
             */
        }

        [Obfuscation(Exclude = true)]
        private void textBoxDiscountMax_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents || (listBox1.SelectedItem == null))
                return;
            /*
            FormattedListBoxItem itemPricing = (FormattedListBoxItem)listBox1.SelectedItem;
            ItemPricing pricing = null;
            if (itemPricing.Id > 0)
            {
                (pricing = GetExistingItemPricing(itemPricing.Id)).SetAdditionalDiscountMax(GetDiscountMax());
                if ((GetChangedItem(pricing) == null) && (pricing != null))
                    ChangedItems.Add(pricing);
            }
                [Obfuscation(Exclude = true)]
            else
                (pricing = ((ItemPricing)itemPricing.ReferenceObject)).SetAdditionalDiscountMax(GetDiscountMax());
            [Obfuscation(Exclude = true)]

            itemPricing.Text = GenerateListBoxDescription(pricing);
        [Obfuscation(Exclude = true)]
             */
        }

        [Obfuscation(Exclude = true)]
        private void textBoxDiscountPrice_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItemPricing();
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveItemPricing();
        }

    }
}
