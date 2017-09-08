using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TemPOS.Helpers;
using PosControls;
using PosControls.Helpers;
using PosModels;
using PosModels.Managers;
using PosModels.Types;
using TemPOS.Commands;
using TemPOS.Managers;
using TemPOS.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryReceiptTape.xaml
    /// </summary>
    public partial class OrderEntryReceiptTape
    {
        [Obfuscation(Exclude = true)]
        public event SelectionChangedEventHandler SelectionChanged;

        private Ticket _selectedTicket;

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                SelectedItem = null;
                Items.Clear();
                listBoxTransactions.ScrollOffset = 0;
                if (value == null) return;
                AddTicketItems(value.PrimaryKey);
                Seating seat = SeatingManager.GetSeating(value.SeatingId);
                if (seat != null)
                    OrderEntryControl.SetSeating(seat.Description);
                else
                {
                    switch (value.Type)
                    {
                        case TicketType.DineIn:
                            // This case only happens with seating selection off
                            // so no need to call OrderEntryControl.SetSeating, set it anyways?
                            OrderEntryControl.SetSeating(Types.Strings.OrderEntryDineIn);
                            break;
                        case TicketType.Pickup:
                            OrderEntryControl.SetSeating(Types.Strings.OrderEntryCarryout);
                            break;
                        case TicketType.Catering:
                            OrderEntryControl.SetSeating(Types.Strings.OrderEntryCatering);
                            break;
                        case TicketType.Delivery:
                            OrderEntryControl.SetSeating(Types.Strings.OrderEntryDelivery);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                OrderEntryCommands.UpdateTicketItemButtons();
            }
        }

        public UIElementCollection Items
        {
            get { return listBoxTransactions.Items; }
        }

        public TicketItemTemplate SelectedItem
        {
            get
            {
                return (TicketItemTemplate)listBoxTransactions.SelectedItem;
            }
            set
            {
                listBoxTransactions.SelectedItem = value;
                if (value != null)
                    listBoxTransactions.ScrollIntoView(value);
            }
        }

        public TicketItemTemplate[] SelectedItems
        {
            get
            {
                var list = new List<TicketItemTemplate>();
                list.Clear();
                list.AddRange(Items.Cast<TicketItemTemplate>().Where(item => item.IsSelected));
                return list.ToArray();
            }
        }

        public int SelectedIndex
        {
            get
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Equals(Items[i], SelectedItem))
                        return i;
                }
                return -1;
            }
        }

        public double TicketSubTotal
        {
            get
            {
                if (SessionManager.ActiveTicket == null)
                    return 0;
                return SessionManager.ActiveTicket.GetSubTotal();
            }
        }

        public double TicketTotal
        {
            get
            {
                double result = 0.0f;
                var taxes = new List<Tax>(Tax.GetAll());
                foreach (TicketItemTemplate template in Items)
                {
                    TicketItem item = template.TicketItem;
                    double cost = item.GetTotalCost();
                    result += cost;
                    result += cost *
                        Tax.FindTax(taxes, Item.Get(item.ItemId).TaxId).Percentage;
                }
                return result;
            }
        }

        public OrderEntryReceiptTape()
        {
            InitializeComponent();
        }

        public void ScrollIntoView(object item)
        {
            listBoxTransactions.ScrollIntoView(item);
        }

        [Obfuscation(Exclude = true)]
        private void listBoxTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update buttons than can be effected by ticket selection changes
            OrderEntryCommands.UpdateTicketItemButtons();

            if (SelectionChanged != null)
                SelectionChanged.Invoke(this, e);
        }


        public void UpdateButtons()
        {
            bool basicEnable = ((SelectedItem != null) &&
                !SessionManager.ActiveTicket.IsClosed &&
                !SessionManager.ActiveTicket.IsCanceled);

            bool isChildItem = SelectedItem != null &&
                SelectedItem.TicketItem.ParentTicketItemId.HasValue;

            // Quantity Change buttons
            buttonQuantityDecrease.IsEnabled =
                (basicEnable && (SelectedItem.TicketItem.Quantity > 1) && !isChildItem);
            buttonQuantityIncrease.IsEnabled =
                buttonQuantitySet.IsEnabled = basicEnable && !isChildItem;
            
            if (IsSelectedItemPendingReturn() && (SelectedItem != null))
            {
                buttonQuantityDecrease.IsEnabled = !isChildItem &&
                    (SelectedItem.TicketItem.QuantityPendingReturn > 1);
                buttonQuantityIncrease.IsEnabled = !isChildItem &&
                    (SelectedItem.TicketItem.QuantityPendingReturn <
                        SelectedItem.TicketItem.Quantity);
            }

            buttonVoidItem.IsEnabled =
                buttonCancelItem.IsEnabled =
                buttonSpecialInstructions.IsEnabled = basicEnable && !isChildItem;
        }

        private bool IsSelectedItemPendingReturn()
        {
            return ((SelectedItem != null) && SelectedItem.TicketItem.IsPendingReturn);
        }

        public bool HasReturns()
        {
            return listBoxTransactions.Items.Cast<TicketItemTemplate>()
                .Any(item => item.TicketItem.IsPendingReturn);
        }

        public void ClearSelection()
        {
            listBoxTransactions.SelectedItem = null;
            OrderEntryCommands.UpdateTicketItemButtons();
        }

        public TicketItem AddItemToOrder(Item item)
        {
            double itemPrice = item.Price;

            // TODO: VIP! Check for special pricing
            foreach (ItemPricing price in ItemPricing.GetAll(item.Id, true, true))
            {
                itemPrice = price.Price;
            }

            TicketItem ticketItem =
                TicketItem.Add(SessionManager.ActiveTicket.PrimaryKey, item.Id, 1, itemPrice, null, null);
            
            if (item.IsGrouping)
            {
                var itemGroups = new List<ItemGroup>(ItemGroup.GetAll(item.Id));
                var ticketItems = new Stack<TicketItem>();
                foreach (TicketItem childTicketItem in itemGroups
                    .Select(itemGroup =>
                        TicketItem.Add(SessionManager.ActiveTicket.PrimaryKey, itemGroup.TargetItemId,
                        1, 0, null, null, ticketItem.PrimaryKey.Id)))
                {
                    ticketItems.Push(childTicketItem);
                }

                AddItemToOrder(ticketItems.Pop(), BranchType.Last);
                foreach (TicketItem childTicketItem in ticketItems)
                {
                    AddItemToOrder(childTicketItem, BranchType.Middle);
                }
            }
            return AddItemToOrder(ticketItem);
        }

        private TicketItem AddItemToOrder(TicketItem ticketItem,
            BranchType branching = BranchType.None, bool select = true)
        {
            SessionManager.ActiveTicketItem = ticketItem;

            var temp = new TicketItemTemplate(ticketItem, branching);
            Items.Insert(0, temp);
            if (select)
                SelectedItem = temp;

            return ticketItem;
        }


        public void AddTicketItems(YearId primaryKey)
        {
            var ticketItems = new List<TicketItem>(TicketItem.GetAll(primaryKey));
            var stackedTicketItems = new Stack<TicketItem>(ticketItems);
            foreach (TicketItem ticketItem in ticketItems)
            {
                if (ticketItem.IsCanceled &&
                    !(SessionManager.ActiveTicket.IsCanceled || SessionManager.ActiveTicket.IsClosed))
                    continue;
                if (ticketItem.ParentTicketItemId != null)
                    continue;
                int count = 0;
                int ticketItemId = ticketItem.PrimaryKey.Id;
                foreach (TicketItem childTicketItem in stackedTicketItems
                    .Where(childTicketItem => childTicketItem.ParentTicketItemId != null)
                    .Where(childTicketItem => childTicketItem.ParentTicketItemId == ticketItemId))
                {
                    count++;
                    AddItemToOrder(childTicketItem, ((count == 1) ?
                        BranchType.Last : BranchType.Middle), false);
                }
                Items.Insert(0, new TicketItemTemplate(ticketItem));
            }
        }

        public void RemoveSelectedItemFromReceiptTape()
        {
            // Remove all child items
            List<TicketItemTemplate> childTicketItems =
                (from TicketItemTemplate item in Items
                 where item.TicketItem.ParentTicketItemId == SelectedItem.TicketItem.PrimaryKey.Id
                 select item).ToList();
            foreach (TicketItemTemplate childTicketItem in childTicketItems)
            {
                Items.Remove(childTicketItem);
            }
            // Remove the item from the receipt tape
            Items.Remove(SelectedItem);
            OrderEntryControl.SetOrderAmountText(TicketSubTotal.ToString("C2"));
            SessionManager.ResetItemEntry();

            OrderEntryControl.InitializeTicket();
            OrderEntryCommands.UpdateInOrderCommands();
            ClearSelection();
        }

        [Obfuscation(Exclude = true)]
        private void ContextMenu_Initialized(object sender, EventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;
            border.CornerRadius = new CornerRadius(4);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = ConfigurationManager.BorderBrush;
            border.Background = ConfigurationManager.ApplicationBackgroundBrush;
            border.ClipToBounds = true;

            var numberEntryControl = border.Child as NumberEntryControl;
            if (numberEntryControl == null) return;
            numberEntryControl.FireEventOnEnter = true;
            numberEntryControl.EnterPressed += OrderEntryCommands.NumberEntryControl_EnterPressed;
        }
    }
}
