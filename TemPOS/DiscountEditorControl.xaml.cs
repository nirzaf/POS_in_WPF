using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for DiscountEditorControl.xaml
    /// </summary>
    public partial class DiscountEditorControl : UserControl
    {
        private bool _haltEvents;

        public string Description
        {
            get
            {
                if (textBoxName == null)
                    return null;
                return textBoxName.Text;
            }
        }

        public double? Amount
        {
            get
            {
                try
                {
                    if (textBoxAmount.Text.Contains("%"))
                        return Convert.ToDouble(textBoxAmount.Text.Replace("%", "")) / 100;
                    return Convert.ToDouble(textBoxAmount.Text);
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool RequiresPermission
        {
            get
            {
                if (radioButtonRequiresManager == null)
                    return false;
                return radioButtonRequiresManager.IsSelected;
            }
        }

        public bool IsPercentage
        {
            get
            {
                if (radioButtonIsPercentage == null)
                    return false;
                return radioButtonIsPercentage.IsSelected;
            }
        }

        public bool ApplyToTicketItem
        {
            get
            {
                if (radioButtonOnTicketItem == null)
                    return false;
                return radioButtonOnTicketItem.IsSelected;
            }
        }

        public bool IsActive
        {
            get
            {
                if (radioButtonIsActive == null)
                    return false;
                return radioButtonIsActive.IsSelected;
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler DetailsChanged;
        private void DoDetailsChangedEvent()
        {
            if ((DetailsChanged != null) && !_haltEvents)
                DetailsChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public DiscountEditorControl()
        {
            InitializeComponent();
        }

        public void InitializeNew()
        {
            textBoxName.Text = "";
            checkBoxPrompt.IsSelected = false;
            textBoxAmount.Text = "";
            textBoxAmount.IsEnabled = true;
            radioButtonIsPercentage.IsSelected = true;
            radioButtonIsNotPercentage.IsSelected = false;
            radioButtonRequiresManager.IsSelected = false;
            radioButtonNoManagerRequired.IsSelected = false;
            radioButtonEntireTicket.IsSelected = true;
            radioButtonOnTicketItem.IsSelected = false;
            radioButtonIsActive.IsSelected = true;
            radioButtonIsNotActive.IsSelected = false;
            textBoxAmount.PromptType = CustomTextBoxType.Percentage;
        }

        public void InitializeDiscount(Discount discount)
        {
            _haltEvents = true;
            textBoxName.Text = discount.Description;
            if (discount.Amount != null)
            {
                textBoxAmount.PromptType = discount.AmountIsPercentage ?
                    CustomTextBoxType.Percentage :
                    CustomTextBoxType.Currency;
                checkBoxPrompt.IsSelected = false;
                if (discount.AmountIsPercentage)
                    textBoxAmount.Text = "" + (discount.Amount.Value * 100).ToString("F1") + "%";
                else
                    textBoxAmount.Text = "" + discount.Amount.Value;
                textBoxAmount.IsEnabled = true;
            }
            else
            {
                checkBoxPrompt.IsSelected = true;
                textBoxAmount.Text = "";
                textBoxAmount.IsEnabled = false;
            }
            radioButtonIsPercentage.IsSelected = discount.AmountIsPercentage;
            radioButtonIsNotPercentage.IsSelected = !discount.AmountIsPercentage;
            radioButtonRequiresManager.IsSelected = discount.RequiresPermission;
            radioButtonNoManagerRequired.IsSelected = !discount.RequiresPermission;
            radioButtonOnTicketItem.IsSelected = discount.ApplyToTicketItem;
            radioButtonEntireTicket.IsSelected = !discount.ApplyToTicketItem;
            radioButtonIsActive.IsSelected = discount.IsActive;
            radioButtonIsNotActive.IsSelected = !discount.IsActive;
            _haltEvents = false;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoDetailsChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrompt_SelectionChanged(object sender, EventArgs e)
        {
            textBoxAmount.IsEnabled = !checkBoxPrompt.IsSelected;
            if (!textBoxAmount.IsEnabled)
                textBoxAmount.Text = "";
            DoDetailsChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAmount_TextChanged(object sender, RoutedEventArgs e)
        {
            DoDetailsChangedEvent();
        }

        private void SetAmountPromptType()
        {
            textBoxAmount.PromptType = IsPercentage ?                
                CustomTextBoxType.Percentage :
                CustomTextBoxType.Currency;
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsPercentage_SelectionGained(object sender, EventArgs e)
        {
            radioButtonIsPercentage.IsSelected = Equals(sender, radioButtonIsPercentage);
            radioButtonIsNotPercentage.IsSelected = Equals(sender, radioButtonIsNotPercentage);
            SetAmountPromptType();
            DoDetailsChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonApplyTo_SelectionGained(object sender, EventArgs e)
        {
            radioButtonEntireTicket.IsSelected = Equals(sender, radioButtonEntireTicket);
            radioButtonOnTicketItem.IsSelected = Equals(sender, radioButtonOnTicketItem);
            DoDetailsChangedEvent();            
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonRequiresManager_SelectionGained(object sender, EventArgs e)
        {
            radioButtonNoManagerRequired.IsSelected = Equals(sender, radioButtonNoManagerRequired);
            radioButtonRequiresManager.IsSelected = Equals(sender, radioButtonRequiresManager);
            DoDetailsChangedEvent();
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsActive_SelectionGained(object sender, EventArgs e)
        {
            radioButtonIsActive.IsSelected = Equals(sender, radioButtonIsActive);
            radioButtonIsNotActive.IsSelected = Equals(sender, radioButtonIsNotActive);
            DoDetailsChangedEvent();
        }

    }
}
