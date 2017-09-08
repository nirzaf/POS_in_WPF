using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.EventHandlers;
using TemPOS.Helpers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryItemOptionsLineControl.xaml
    /// </summary>
    public partial class OrderEntryItemOptionsLineControl : UserControl
    {
        [Obfuscation(Exclude = true)]
        public event ButtonTouchInputClickEventHandler Click;

        public bool IsBeingUsed
        {
            get;
            private set;
        }

        public int SelectedMaximum
        {
            get;
            set;
        }

        public int SelectedMinimum
        {
            get;
            set;
        }

        public int PreSelectedCount
        {
            get
            {
                int count = Buttons.Where(button => button != null)
                    .Count(button => button.IsChecked == true);
                if (count == 0)
                    return count;
                return count - 1;
            }
        }

        public TextBlockButton[] Buttons
        {
            get
            {
                return new [] {
                    Row0_Col0_Button, Row0_Col1_Button, Row0_Col2_Button, Row0_Col3_Button, Row0_Col4_Button, Row0_Col5_Button,
                    Row0_Col6_Button, Row0_Col7_Button, Row0_Col8_Button, Row0_Col9_Button, Row0_Col10_Button, Row0_Col11_Button
                };
            }
        }

        public OrderEntryItemOptionsLineControl()
        {
            InitializeComponent();
            SelectedMaximum = 12;
            IsBeingUsed = false;
            foreach (TextBlockButton button in Buttons)
            {
                if (button == null)
                    continue;
                button.FontSize = 12;
            }
        }

        private void DoClick(TextBlockButton button, int index)
        {
            if (Click != null)
                Click.Invoke(this, new ButtonTouchInputClickArgs(index));
        }

        public void SetOptions(int itemOptionSetId)
        {
            int index = 0;
            Visibility = Visibility.Visible;
            ItemOptionSet set = ItemOptionSet.Get(itemOptionSetId);
            IsBeingUsed = !set.IsPizzaStyle;
            if (IsBeingUsed)
            {
                foreach (ItemOption option in ItemOption.GetInSet(itemOptionSetId))
                {
                    TextBlockButton button = Buttons[index];
                    button.Tag = option.Id.ToString(CultureInfo.InvariantCulture);
                    button.IsChecked = false;
                    button.Text = StringHelper.ThinString(option.Name);
                    button.Visibility = Visibility.Visible;
                    index++;
                }
                SelectedMaximum = set.SelectedMaximum;
                SelectedMinimum = set.SelectedMinimum;
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

        public void Clear()
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.Tag = "0";
                button.IsChecked = false;
                button.Text = "";
                button.Visibility = Visibility.Hidden;
            }
            IsBeingUsed = false;
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
            try
            {
                return Convert.ToInt32(button.Tag as string);
            }
            catch
            {
                return 0;
            }
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton source = (TextBlockButton)sender;
            int index = GetIndex(source);
            if (source.IsChecked != true)
            {
                if (PreSelectedCount + 1 > SelectedMinimum)
                {
                    // Unselection allowed
                    DoClick(source, index);
                }
                else
                    source.IsChecked = true;
            }
            if ((SelectedMaximum == 1) && (PreSelectedCount == 1))
            {
                for (int i = 0; i < Buttons.Length; i++)                            
                {
                    if ((Equals(Buttons[i], sender)) || (Buttons[i].IsChecked != true)) continue;
                    Buttons[i].IsChecked = false;
                    if (Click != null)
                        Click.Invoke(this, new ButtonTouchInputClickArgs(i));
                }
                // Selection is only the sender
                DoClick(source, index);
            }
            else if (PreSelectedCount < SelectedMaximum)
                // Selection is allowed
                DoClick(source, index);
            else
                source.IsChecked = false;
        }
    }
}
