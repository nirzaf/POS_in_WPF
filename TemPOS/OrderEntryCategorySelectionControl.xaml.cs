using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosControls;
using PosModels;
using TemPOS.Helpers;
using TemPOS.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryCategorySelectionControl.xaml
    /// </summary>
    public partial class OrderEntryCategorySelectionControl : UserControl
    {
        private Category _activeCategory;

        [Obfuscation(Exclude = true)]
        public event EventHandler ActiveCategoryChanged;

        public Category ActiveCategory
        {
            get { return _activeCategory; }
            set
            {
                _activeCategory = value;
                if (ActiveCategoryChanged != null)
                    ActiveCategoryChanged.Invoke(this, new EventArgs());
            }
        }

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

        public OrderEntryCategorySelectionControl()
        {
            InitializeComponent();
            if (!ConfigurationManager.IsInDesignMode)
                Clear();
            // Todo: wtf, refactor this... Place font size in xaml
            foreach (TextBlockButton button in Buttons)
            {
                button.FontSize = 12;
            }
        }

        public void Clear()
        {
            int count = 0;
            IEnumerable<Category> categories = Category.GetAll();
            foreach (Category category in categories)
            {
                if ((category.DisplayIndex < 0) ||
                    (category.DisplayIndex >= Buttons.Length))
                    continue;
                TextBlockButton button = Buttons[category.DisplayIndex];
                button.Tag = category.Id.ToString(CultureInfo.InvariantCulture);
                button.Text = StringHelper.ThinString(category.NameValue);
                button.Visibility = Visibility.Hidden;
                count++;
            }
            for (int i = count; i < Buttons.Length; i++)
            {
                TextBlockButton button = Buttons[i];
                button.Tag = "0";
                button.IsChecked = false;
                button.Text = "";
                button.Visibility = Visibility.Hidden;
            }
        }

        public void Show()
        {
            Clear();
            foreach (TextBlockButton button in Buttons)
            {
                if (!String.IsNullOrEmpty(button.Text))
                {
                    button.Visibility = Visibility.Visible;
                }

            }
        }

        public void Hide()
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateItems()
        {
            // Clear the Item Options
            OrderEntryControl.ClearItemOptions();

            if (SessionManager.ActiveItem == null)
                OrderEntryControl.ClearReceiptTape();

            SessionManager.ActiveCategory = ActiveCategory;

            // Move back to OrderEnteryControl, instead, call an event handler here
            if (SessionManager.ActiveCategory != null)
                OrderEntryControl.SetCategory();
        }

        public void SelectCategoryButton(int categoryId)
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.IsChecked = (GetId(button) == categoryId);
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
            int index = GetIndex((TextBlockButton)sender);
            // Set the ActiveCategory property
            for (int i = 0; i < Buttons.Length; i++)
            {
                TextBlockButton button = Buttons[i];
                if (i == index)
                {
                    ActiveCategory = Category.Get(GetId(button));
                    button.IsChecked = true;
                }
                else
                {
                    button.IsChecked = false;
                }
            }

            UpdateItems();
        }
    }
}
