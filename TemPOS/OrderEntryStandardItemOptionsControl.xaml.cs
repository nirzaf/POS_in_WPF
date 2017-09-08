using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Commands;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryStandardItemOptionsControl.xaml
    /// </summary>
    public partial class OrderEntryStandardItemOptionsControl : UserControl
    {
        public bool IsBeingUsed
        {
            get
            {
                return buttonTouchInputOptions1.IsBeingUsed;
            }
        }

        public OrderEntryStandardItemOptionsControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            buttonTouchInputOptions1.Click += buttonTouchInputOptions1_Click;
            buttonTouchInputOptions2.Click += buttonTouchInputOptions2_Click;
            buttonTouchInputOptions3.Click += buttonTouchInputOptions3_Click;
        }
        
        [Obfuscation(Exclude = true)]
        void buttonTouchInputOptions1_Click(object sender, EventHandlers.ButtonTouchInputClickArgs args)
        {
            TextBlockButton button = buttonTouchInputOptions1.Buttons[args.ButtonIndex];
            UpdateTicketOptions(button);
            UpdateReceiptTapeItem();
            OrderEntryCommands.UpdateInOrderCommands();
        }

        [Obfuscation(Exclude = true)]
        void buttonTouchInputOptions2_Click(object sender, EventHandlers.ButtonTouchInputClickArgs args)
        {
            TextBlockButton button = buttonTouchInputOptions2.Buttons[args.ButtonIndex];
            UpdateTicketOptions(button);
            UpdateReceiptTapeItem();
            OrderEntryCommands.UpdateInOrderCommands();
        }

        [Obfuscation(Exclude = true)]
        void buttonTouchInputOptions3_Click(object sender, EventHandlers.ButtonTouchInputClickArgs args)
        {
            TextBlockButton button = buttonTouchInputOptions3.Buttons[args.ButtonIndex];
            UpdateTicketOptions(button);
            UpdateReceiptTapeItem();
            OrderEntryCommands.UpdateInOrderCommands();
        }

        public void SetupTicketItemOptions(TicketItem ticketItem)
        {
            foreach (TicketItemOption ticketItemOption in
                TicketItemOption.GetAll(ticketItem.PrimaryKey))
            {
                foreach (TextBlockButton button in buttonTouchInputOptions1.Buttons)
                {
                    if (button.Visibility == Visibility.Hidden)
                        continue;
                    if (ticketItemOption.ItemOptionId == GetId(button))
                        button.IsChecked = true;
                }
                foreach (TextBlockButton button in buttonTouchInputOptions2.Buttons)
                {
                    if (button.Visibility == Visibility.Hidden)
                        continue;
                    if (ticketItemOption.ItemOptionId == GetId(button))
                        button.IsChecked = true;
                }
                foreach (TextBlockButton button in buttonTouchInputOptions3.Buttons)
                {
                    if (button.Visibility == Visibility.Hidden)
                        continue;
                    if (ticketItemOption.ItemOptionId == GetId(button))
                        button.IsChecked = true;
                }
            }
        }

        public void SetItemOptions(Item item)
        {
            if (item == null)
            {
                buttonTouchInputOptions1.Clear();
                buttonTouchInputOptions2.Clear();
                buttonTouchInputOptions3.Clear();
                return;
            }
            if ((item.ItemOptionSetIds[0] == 0) &&
                (item.ItemOptionSetIds[1] == 0) &&
                (item.ItemOptionSetIds[2] == 0))
            {
                // ToDo: Pizza Entry will need to disable visability
                //Visibility = Visibility.Collapsed;
                Clear();
                HideButtons();
                return;
            }
            if (item.ItemOptionSetIds[0] != 0)
            {
                buttonTouchInputOptions1.SetOptions(item.ItemOptionSetIds[0]);
            }
            if (item.ItemOptionSetIds[1] != 0)
            {
                buttonTouchInputOptions2.SetOptions(item.ItemOptionSetIds[1]);
            }
            if (item.ItemOptionSetIds[2] != 0)
            {
                buttonTouchInputOptions3.SetOptions(item.ItemOptionSetIds[2]);
            }
            // ToDo: Pizza Entry will need to disable visability
            //Visibility = (IsBeingUsed ? Visibility.Visible : Visibility.Collapsed);
        }

        public void HideButtons()
        {
            buttonTouchInputOptions1.Visibility = Visibility.Hidden;
            buttonTouchInputOptions2.Visibility = Visibility.Hidden;
            buttonTouchInputOptions3.Visibility = Visibility.Hidden;
        }

        private void UpdateReceiptTapeItem()
        {
            if (SessionManager.ActiveTicketItem == null)
                return;
            foreach (TicketItemTemplate item in OrderEntryControl.ReceiptTapeItems)
            {
                if (!Equals(item.TicketItem.PrimaryKey,
                    SessionManager.ActiveTicketItem.PrimaryKey)) continue;
                item.Update();
                break;
            }
        }

        private void UpdateTicketOptions(TextBlockButton button)
        {
            if ((SessionManager.ActiveTicketItem == null) || (button == null))
                return;
            // DEBUG: This isn't always going to be 'this' simple... If the ingredient
            // is already on, then it's might be getting removed (not added)... It
            // might also indicate the addition of extra. Change count (the constant '1',
            // in the Add() below) is always positive, even on deletions. It's a factor
            // that allows support for "extra-extra" type options.
            if (button.IsChecked == true)
            {
                if (!TicketItemOption.HasOption(SessionManager.ActiveTicketItem.PrimaryKey,
                    GetId(button)))
                {
                    TicketItemOption option =
                        TicketItemOption.Add(SessionManager.ActiveTicketItem.PrimaryKey.Id,
                        GetId(button), PosModels.Types.TicketItemOptionType.None, 1);
                }
            }
            else
            {
                TicketItemOption.Delete(SessionManager.ActiveTicketItem,
                    GetId(button), true);
            }
            OrderEntryControl.UpdateDisplayedOrderAmount();
        }

        public void Clear()
        {
            buttonTouchInputOptions1.Clear();
            buttonTouchInputOptions2.Clear();
            buttonTouchInputOptions3.Clear();
        }

        public int GetId(TextBlockButton button)
        {
            return Convert.ToInt32(button.Tag as string);
        }

    }
}
