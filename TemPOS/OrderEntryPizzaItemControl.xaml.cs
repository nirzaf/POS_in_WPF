using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TemPOS.EventHandlers;
using PosModels;
using System.ComponentModel;
using PosControls;
using TemPOS.Helpers;
using TemPOS.Managers;

namespace TemPOS
{
    // TODO: Needs to change to the dynamic flow design

    /// <summary>
    /// Interaction logic for OrderEntryPizzaItemControl.xaml
    /// </summary>
    public partial class OrderEntryPizzaItemControl : UserControl
    {
        public BindingList<TextBlockButton> Buttons;   
        public bool IsBeingUsed
        {
            get;
            set;
        }

        public OrderEntryPizzaItemControl()
        {
            IsBeingUsed = false;
            Buttons = new BindingList<TextBlockButton>();
            InitializeComponent();
                /*
                new TextBlockButton[] {
                Row0_Col0_Button, Row0_Col1_Button, Row0_Col2_Button, Row0_Col3_Button,
                Row0_Col4_Button, Row0_Col5_Button, Row0_Col6_Button, Row0_Col7_Button,
                Row0_Col8_Button, Row0_Col9_Button, Row0_Col10_Button, Row0_Col11_Button,

                Row1_Col0_Button, Row1_Col1_Button, Row1_Col2_Button, Row1_Col3_Button,
                Row1_Col4_Button, Row1_Col5_Button, Row1_Col6_Button, Row1_Col7_Button,
                Row1_Col8_Button, Row1_Col9_Button, Row1_Col10_Button, Row1_Col11_Button,

                Row2_Col0_Button, Row2_Col1_Button, Row2_Col2_Button, Row2_Col3_Button,
                Row2_Col4_Button, Row2_Col5_Button, Row2_Col6_Button, Row2_Col7_Button,
                Row2_Col8_Button, Row2_Col9_Button, Row2_Col10_Button, Row2_Col11_Button };
                */
        }

        private void UpdateReceiptTapeItem()
        {
            foreach (TicketItemTemplate item in OrderEntryControl.ReceiptTapeItems)
            {
                if (item.TicketItem.PrimaryKey.Equals(SessionManager.ActiveTicketItem.PrimaryKey))
                {
                    item.Update();
                    break;
                }
            }
        }

        private void UpdateTicketOptions(TextBlockButton button)
        {
            if (button.IsChecked == true)
            {
                if (!TicketItemOption.HasOption(SessionManager.ActiveTicketItem.PrimaryKey,
                    GetId(button)))
                {
                    TicketItemOption option =
                        TicketItemOption.Add(SessionManager.ActiveTicketItem.PrimaryKey.Id,
                        GetId(button), PosModels.Types.TicketItemOptionType.Add, 1);
                }
            }
            else
            {
                TicketItemOption.Delete(
                    SessionManager.ActiveTicketItem,
                    GetId(button), true);
            }
            OrderEntryControl.UpdateDisplayedOrderAmount();
        }

        internal void Clear()
        {
            if (Buttons == null)
                return;
            foreach (TextBlockButton button in Buttons)
            {
                button.Tag = "0";
                button.IsChecked = false;
                button.Text = "";
                button.Visibility = Visibility.Hidden;
            }
            IsBeingUsed = false;
        }

        internal void SetItemOptions(Item item)
        {
            if ((item.ItemOptionSetIds[0] == 0) &&
                (item.ItemOptionSetIds[1] == 0) &&
                (item.ItemOptionSetIds[2] == 0))
            {
                Visibility = Visibility.Collapsed;
                Clear();
                return;
            }

            int index = 0;
            ItemOptionSet set = ItemOptionSet.Get(item.ItemOptionSetIds[0]);
            IsBeingUsed = set.IsPizzaStyle;
            Visibility = (IsBeingUsed ? Visibility.Visible : Visibility.Collapsed);
            if (IsBeingUsed)
            {
                IEnumerable<ItemOption> options = ItemOption.GetInSet(item.ItemOptionSetIds[0]);
                foreach (ItemOption option in options)
                {
                    TextBlockButton button = Buttons[index];
                    button.Tag = option.Id.ToString(CultureInfo.InvariantCulture);
                    button.IsChecked = false;
                    button.Text = StringHelper.ThinString(option.Name);
                    button.Visibility = Visibility.Visible;
                    index++;
                }
                //SelectedMaximum = set.SelectedMaximum;
                //SelectedMinimum = set.SelectedMinimum;
            }
            for (int i = index; i < Buttons.Count; i++)
            {
                TextBlockButton button = Buttons[i];
                button.Tag = "0";
                button.IsChecked = false;
                button.Text = "";
                button.Visibility = Visibility.Hidden;
            }
        }

        public void SetupTicketItemOptions(TicketItem ticketItem)
        {
            IEnumerable<TicketItemOption> ticketItemOptions =
                TicketItemOption.GetAll(ticketItem.PrimaryKey);
            //SessionManager.ActiveTicketItem = ticketItem;

            foreach (TicketItemOption ticketItemOption in ticketItemOptions)
            {
                foreach (TextBlockButton button in Buttons)
                {
                    if (button.Visibility == Visibility.Hidden)
                        continue;
                    if (ticketItemOption.ItemOptionId == GetId(button))
                        button.IsChecked = true;
                }
            }
        }

        public int GetIndex(TextBlockButton button)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Equals(button, Buttons[i]))
                    return i;
            }
            return -1;
        }

        public int GetId(TextBlockButton button)
        {
            return Convert.ToInt32(button.Tag as string);
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton button = (TextBlockButton)sender;
            UpdateTicketOptions(button);
            UpdateReceiptTapeItem();
        }

    }
}
