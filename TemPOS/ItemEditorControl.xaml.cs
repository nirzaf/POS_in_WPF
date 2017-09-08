using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using PosControls;
using System.Collections.Generic;
using TemPOS.EventHandlers;
using TemPOS.Managers;
using TemPOS.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorControl.xaml
    /// </summary>
    public partial class ItemEditorControl : UserControl
    {
        private bool _haltEvents;
        private Item _activeItem;

        public Item ActiveItem
        {
            get
            {
                return _activeItem;
            }
            set
            {
                _haltEvents = true;
                _activeItem = value;
                if (IsInitialized)
                {
                    // initialize
                    itemEditorGroupingControl.ActiveItem = value;
                    itemEditorIngredientsControl.ActiveItem = value;
                    itemEditorDetailsControl.ActiveItem = value;
                    itemEditorOptionControl.ActiveItem = value;
                    itemEditorSpecialPricing.ActiveItem = value;
                    tabControl.SetTabVisibility(2, itemEditorDetailsControl.IsGrouping ?
                        Visibility.Visible : Visibility.Collapsed);
                }
                _haltEvents = false;
            }
        }

        internal ItemEditorDetailsControl itemEditorDetailsControl
        {
            get
            {
                DragScrollViewer viewer = tabControl.Tab1.DockedControl as DragScrollViewer;
                return (viewer != null) ?
                    viewer.ScrollContent as ItemEditorDetailsControl : null;
            }
        }

        internal ItemEditorGroupingControl itemEditorGroupingControl
        {
            get
            {
                return tabControl.Tab2.DockedControl as ItemEditorGroupingControl;
            }
        }

        internal ItemEditorIngredientsControl itemEditorIngredientsControl
        {
            get
            {
                return tabControl.Tab3.DockedControl as ItemEditorIngredientsControl;
            }
        }

        internal ItemEditorOptionSetControl itemEditorOptionControl
        {
            get
            {
                return tabControl.Tab4.DockedControl as ItemEditorOptionSetControl;
            }
        }

        internal ItemEditorSpecialPricingControl itemEditorSpecialPricing
        {
            get
            {
                return tabControl.Tab5.DockedControl as ItemEditorSpecialPricingControl;
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event ItemValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(ItemFieldName field)
        {
            if ((ValueChanged != null) && (ActiveItem != null) && !_haltEvents)
                ValueChanged.Invoke(this, new ItemValueChangedArgs(ActiveItem, field));
        }
        #endregion

        public ItemEditorControl()
        {
            InitializeComponent();
            tabControl.SetTabVisibility(2, Visibility.Collapsed);
        }

        public bool UpdateItem()
        {
            int categoryId = itemEditorDetailsControl.CategoryId;
            string fullName = itemEditorDetailsControl.FullName;
            string shortName = itemEditorDetailsControl.ShortName;
            double price = itemEditorDetailsControl.Price;
            PrintDestination printOptionSet = itemEditorDetailsControl.PrintDestinations;
            bool isActive = itemEditorDetailsControl.IsActive;
            int taxId = itemEditorDetailsControl.TaxId;
            int itemOptionSet1 = itemEditorOptionControl.OptionSetId1;
            int itemOptionSet2 = itemEditorOptionControl.OptionSetId2;
            int itemOptionSet3 = itemEditorOptionControl.OptionSetId3;
            bool isReturnable = itemEditorDetailsControl.IsReturnable;
            bool isTaxExemptable = itemEditorDetailsControl.IsTaxExemptable;
            bool isFired = itemEditorDetailsControl.IsFired;
            bool isOutOfStock = itemEditorDetailsControl.IsOutOfStock;
            bool isGrouping = itemEditorDetailsControl.IsGrouping;
            
            // TODO: Implement for delivery and future times
            TimeSpan? prepareTime = null;

            // Verify Category is set
            if (categoryId <= 0)
            {
                PosDialogWindow.ShowDialog(
                    Strings.ItemEditorErrorNoCategory, Strings.ItemEditorInvalidCategory);
                return false;
            }

            // Verify name is valid and unique
            try
            {
                if (string.IsNullOrEmpty(fullName))
                    throw new Exception(Strings.ItemEditorErrorNoName);
                Item current = Item.Get(fullName);
                if (current != null)
                {
                    if ((ActiveItem == null) ||
                       ((ActiveItem != null) && (current.Id != ActiveItem.Id)))
                    throw new Exception(Strings.ItemEditorErrorExistingName);
                }
            }
            catch (Exception ex)
            {
                PosDialogWindow.ShowDialog(ex.Message, Strings.ItemEditorInvalidName);
                return false;
            }

            // Is there an ActiveItem?
            Item activeItem = ActiveItem;
            if (activeItem == null)
            {
                activeItem = Item.Add(categoryId, fullName, shortName, price, itemOptionSet1,
                    itemOptionSet2, itemOptionSet3, printOptionSet, isActive, taxId,
                    prepareTime, isReturnable, isTaxExemptable, isFired, isOutOfStock, isGrouping);
            }
            else
            {
                LogItemChanges(activeItem, itemOptionSet1, itemOptionSet2, itemOptionSet3);
                // Update the category values for the ActiveItem
                activeItem.SetCategory(categoryId);
                activeItem.SetFullName(fullName);
                activeItem.SetShortName(shortName);
                if (Math.Abs(price - activeItem.Price) > double.Epsilon)
                    LogPriceChange(activeItem.Id, activeItem.Price, price);
                activeItem.SetPrice(price);
                activeItem.SetOptionSet1(itemOptionSet1);
                activeItem.SetOptionSet2(itemOptionSet2);
                activeItem.SetOptionSet3(itemOptionSet3);
                activeItem.SetActive(isActive);
                activeItem.SetPrintOptionSet(printOptionSet);
                activeItem.SetTaxId(taxId);
                activeItem.SetPrepareTime(prepareTime);
                activeItem.SetIsReturnable(isReturnable);
                activeItem.SetIsTaxExemptable(isTaxExemptable);
                activeItem.SetIsFired(isFired);
                activeItem.SetIsOutOfStock(isOutOfStock);
                activeItem.SetIsGrouping(isGrouping);
                // Update the database
                activeItem.Update();
            }

            // Update ItemPricing
            List<int> removedIds = new List<int>();
            if (itemEditorSpecialPricing.RemovedItems != null)
            {
                foreach (ItemEditorSpecialPricingControl.ItemPricingModel removed in itemEditorSpecialPricing.RemovedItems)
                {
                    removedIds.Add(removed.Id);
                    ItemPricing.Delete(removed.Id);
                    LogPriceChange(removed.Id, removed.ItemId, removed.Price, null, removed.DayOfWeek, null,
                        removed.StartTime, null, removed.EndTime, null);
                }
            }
            if (itemEditorSpecialPricing.NewItems != null)
            {
                foreach (ItemEditorSpecialPricingControl.ItemPricingModel added in itemEditorSpecialPricing.NewItems)
                {
                    // ToDo: Placeholder Constants
                    ItemPricing itemPricing =
                        ItemPricing.Add(activeItem.Id, added.DayOfWeek, added.StartTime, added.EndTime, added.Price,
                        null, null, null, true);
                    LogPriceChange(itemPricing.Id, added.ItemId, null, added.Price, null, added.DayOfWeek,
                        null, added.StartTime, null, added.EndTime);
                }
            }
            if (itemEditorSpecialPricing.ExistingItems != null)
            {
                foreach (ItemEditorSpecialPricingControl.ItemPricingModel existing in itemEditorSpecialPricing.ExistingItems)
                {
                    if (removedIds.Contains(existing.Id))
                        continue;
                    double? oldPrice = null, newPrice = null;
                    Days? oldDayOfWeek = null, newDayOfWeek = null;
                    TimeSpan? oldStartTime = null, newStartTime = null, oldEndTime = null, newEndTime = null;
                    ItemPricing itemPricing = ItemPricing.Get(existing.Id);
                    if (Math.Abs(itemPricing.Price - existing.Price) > double.Epsilon)
                    {
                        oldPrice = itemPricing.Price;
                        newPrice = existing.Price;
                    }
                    if (itemPricing.DayOfWeek != existing.DayOfWeek)
                    {
                        oldDayOfWeek = itemPricing.DayOfWeek;
                        newDayOfWeek = existing.DayOfWeek;
                    }
                    if (itemPricing.StartTime != existing.StartTime)
                    {
                        oldStartTime = itemPricing.StartTime;
                        newStartTime = existing.StartTime;
                    }
                    if (itemPricing.EndTime != existing.EndTime)
                    {
                        oldEndTime = itemPricing.EndTime;
                        newEndTime = existing.EndTime;
                    }
                    itemPricing.SetItemId(existing.ItemId);
                    itemPricing.SetDayOfTheWeek(existing.DayOfWeek);
                    itemPricing.SetPrice(existing.Price);
                    itemPricing.SetStartTime(existing.StartTime);
                    itemPricing.SetEndTime(existing.EndTime);
                    itemPricing.Update();
                    LogPriceChange(itemPricing.Id, itemPricing.ItemId, oldPrice, newPrice, oldDayOfWeek,
                        newDayOfWeek, oldStartTime, newStartTime, oldEndTime, newEndTime);
                }
            }

            if (ActiveItem == null)
                ActiveItem = activeItem;
            itemEditorGroupingControl.Update(ActiveItem.Id);
            itemEditorIngredientsControl.Update(ActiveItem.Id);
            itemEditorSpecialPricing.ActiveItem = ActiveItem;

            return true;
        }

        private void LogItemChanges(Item activeItem,
            int itemOptionSet1, int itemOptionSet2, int itemOptionSet3)
        {
            List<int> currentItemOptionSets = new List<int>(activeItem.ItemOptionSetIds);
            List<int> newItemOptionSets = new List<int>(
                new [] { itemOptionSet1, itemOptionSet2, itemOptionSet3 });
            foreach (int itemOptionSetId in newItemOptionSets.Where(
                itemOptionSetId => !currentItemOptionSets.Contains(itemOptionSetId) && (itemOptionSetId > 0)))
            {
                ItemAdjustment.Add(SessionManager.ActiveEmployee.Id, ItemAdjustmentType.OptionSetAddition,
                                   activeItem.Id, itemOptionSetId);
            }
            foreach (int itemOptionSetId in currentItemOptionSets)
            {
                // Deletion
                if (!newItemOptionSets.Contains(itemOptionSetId) && (itemOptionSetId > 0))
                    ItemAdjustment.Add(SessionManager.ActiveEmployee.Id, ItemAdjustmentType.OptionSetDeletion,
                        activeItem.Id, itemOptionSetId);
            }
        }

        private void LogPriceChange(int itemPricingId, int itemId, double? oldPrice,
            double? newPrice, Days? oldDayOfWeek, Days? newDayOfWeek, TimeSpan? oldStartTime,
            TimeSpan? newStartTime, TimeSpan? oldEndTime, TimeSpan? newEndTime)
        {
            ItemPricingChangeLog.Add(itemId, SessionManager.ActiveEmployee.Id, itemPricingId,
                oldPrice, newPrice, oldDayOfWeek, newDayOfWeek, oldStartTime, newStartTime,
                oldEndTime, newEndTime);
        }

        private void LogPriceChange(int itemId, double oldPrice, double newPrice)
        {
            ItemPricingChangeLog.Add(itemId, SessionManager.ActiveEmployee.Id,
                null, oldPrice, newPrice);
        }

        [Obfuscation(Exclude = true)]
        private void itemEditorDetailsControl_ValueChanged(object sender, ItemValueChangedArgs args)
        {
            if (args.FieldName == ItemFieldName.IsGrouping)
                tabControl.SetTabVisibility(2, (itemEditorDetailsControl.IsGrouping ?
                    Visibility.Visible : Visibility.Collapsed));            
            DoChangedValueEvent(args.FieldName);
        }

        [Obfuscation(Exclude = true)]
        private void itemEditorOptionControl_ValueChanged(object sender, ItemValueChangedArgs args)
        {
            DoChangedValueEvent(args.FieldName);
        }

        [Obfuscation(Exclude = true)]
        private void itemEditorSpecialPricing_ValueChanged(object sender, ItemValueChangedArgs args)
        {
            DoChangedValueEvent(args.FieldName);
        }

        [Obfuscation(Exclude = true)]
        private void ItemEditorIngredientsControl_ValueChanged(object sender, EventArgs e)
        {
            DoChangedValueEvent(ItemFieldName.Recipe);
        }

        [Obfuscation(Exclude = true)]
        private void ItemEditorGroupingControl_ValueChanged(object sender, EventArgs args)
        {
            DoChangedValueEvent(ItemFieldName.Groupings);
        }
    }
}
