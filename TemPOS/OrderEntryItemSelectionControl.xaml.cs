using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Helpers;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ButtonTouchInput.xaml
    /// </summary>
    public partial class OrderEntryItemSelectionControl : UserControl
    {
        public TextBlockButton[] Buttons
        {
            get
            {
                return new [] {
                    Row0_Col0_Button, Row0_Col1_Button, Row0_Col2_Button, Row0_Col3_Button, Row0_Col4_Button, Row0_Col5_Button,
                    Row0_Col6_Button, Row0_Col7_Button, Row0_Col8_Button, Row0_Col9_Button, Row0_Col10_Button, Row0_Col11_Button,
                    Row1_Col0_Button, Row1_Col1_Button, Row1_Col2_Button, Row1_Col3_Button, Row1_Col4_Button, Row1_Col5_Button,
                    Row1_Col6_Button, Row1_Col7_Button, Row1_Col8_Button, Row1_Col9_Button, Row1_Col10_Button, Row1_Col11_Button
                };
            }
        }

        public OrderEntryItemSelectionControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.Text = "";
                button.Tag = "0";
                button.IsEnabled = true;
                button.Visibility = Visibility.Hidden;
                button.FontSize = 12;
            }
        }

        public void Show(int categoryId)
        {
            Category category = Category.Get(categoryId);
            int index = 0;
            if (category != null)
            {
                foreach (Item item in Item.GetAllForCategory(category.Id))
                {
                    if (!item.IsActive)
                        continue;
                    TextBlockButton button = Buttons[index];
                    button.Tag = item.Id.ToString(CultureInfo.InvariantCulture);
                    button.IsChecked = false;
                    button.IsEnabled = !item.IsOutOfStock;
                    string buttonName = item.ShortName;
                    if (String.IsNullOrEmpty(buttonName))
                        buttonName = item.FullName;
                    button.Text = StringHelper.ThinString(buttonName);
                    button.Visibility = Visibility.Visible;
                    index++;
                }
            }
            for (int i = index; i < Buttons.Length; i++)
            {
                TextBlockButton button = Buttons[i];
                button.Tag = "0";
                button.IsChecked = false;
                button.Text = "";
                button.Visibility = Visibility.Hidden;
            }
        }

        public void Hide()
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.Visibility = Visibility.Hidden;
            }
        }

        private void NotifyParent(bool sameButton)
        {
            Item item = SessionManager.ActiveItem;
            if ((SessionManager.ActiveCategory == null) || (item == null))
                return;

            // Add a new item
            OrderEntryControl.ClearItemOptions();
            SessionManager.ActiveItem = item;
            OrderEntryControl.AddItemToOrder(item);
            SessionManager.ActiveItem = item;
            OrderEntryControl.UpdateDisplayedOrderAmount();
            OrderEntryControl.SetItemOptions();
        }

        public void SelectItemButton(int itemId)
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.IsChecked = (GetId(button) == itemId);
            }
        }

        public int GetIndex(TextBlockButton button)
        {
            for (int i = 0; i < Buttons.Length; i++)
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
#if DEMO
            if (OrderEntryControl.CurrentTicketItemCount >= 3)
            {
                ((TextBlockButton) sender).IsChecked = false;
                PosDialogWindow.ShowDialog(
                    Strings.YouCanNotAddMoreThan3TicketItemsToATicketInTheDemoVersion,
                    Strings.DemoRestriction);
                return;

            }
#endif
            int index = GetIndex((TextBlockButton)sender);

            // Set the ActiveItem property
            Item foundItem = null;
            for (int i = 0; i < Buttons.Length; i++)
            {
                TextBlockButton button = Buttons[i];
                if (i == index)
                {
                    button.IsChecked = true;
                    foundItem = Item.Get(GetId(button));
                }
                else
                {
                    button.IsChecked = false;
                }
            }
            if ((SessionManager.ActiveItem != null) && (foundItem != null) &&
                (SessionManager.ActiveItem.Id == foundItem.Id))
                NotifyParent(true);
            else
            {
                SessionManager.ActiveItem = foundItem;
                NotifyParent(false);
            }
        }

    }
}
