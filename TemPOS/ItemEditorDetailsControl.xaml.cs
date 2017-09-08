using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Types;
using PosControls;
using System.Collections.Generic;
using TemPOS.EventHandlers;
using TemPOS.Types;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorDetailsControl.xaml
    /// </summary>
    public partial class ItemEditorDetailsControl : UserControl
    {
        #region Fields
        private bool _haltEvents;
        #endregion

        #region Properties
        public Item ActiveItem
        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;
                if (value != null)
                {
                    CategoryId = _activeItem.CategoryId;
                    FullName = _activeItem.FullName;
                    ShortName = _activeItem.ShortName;
                    Price = _activeItem.Price;
                    PrintDestinations = _activeItem.PrintDestinations;
                    TaxId = _activeItem.TaxId;
                    IsActive = _activeItem.IsActive;
                    IsReturnable = _activeItem.IsReturnable;
                    IsTaxExemptable = _activeItem.IsTaxExemptable;
                    IsFired = _activeItem.IsFired;
                    IsOutOfStock = _activeItem.IsOutOfStock;
                    IsGrouping = _activeItem.IsGrouping;
                }
                else
                {
                    comboBoxCategory.SelectedIndex = 0;
                    string categoryName = comboBoxCategory.SelectedItem;
                    Category category = Category.Get(categoryName);
                    //IEnumerable<Category> categories = Category.GetAll();
                    //Category matchingCategory = Category.FindByName(categories, categoryName);
                    CategoryId = category.Id;
                    FullName = "";
                    ShortName = "";
                    Price = 0;
                    PrintDestinations = PrintDestination.None;
                    TaxId = 0;
                    IsReturnable = false;
                    IsFired = false;
                    IsTaxExemptable = true;
                    IsActive = true;
                    IsOutOfStock = false;
                    IsGrouping = false;
                }
                _haltEvents = true;
                InitializeCategories();
                InitializeTextFields();
                InitializeTaxField();
                InitializePrintOptionSet();
                InitializeIsReturnable(false, IsReturnable);
                InitializeIsFired(false, IsFired);
                InitializeIsTaxExemptable(false, IsTaxExemptable);
                InitializeIsEnabled(false, IsActive);
                InitializeIsOutOfStock(false, IsOutOfStock);
                InitializeIsGrouping(false, IsGrouping);
                _haltEvents = false;
            }
        }

        private Item _activeItem;

        public int CategoryId
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

        public string ShortName
        {
            get;
            private set;
        }

        public double Price
        {
            get;
            private set;
        }

        public PrintDestination PrintDestinations
        {
            get;
            private set;
        }

        public int TaxId
        {
            get;
            private set;
        }

        public bool IsActive
        {
            get;
            private set;
        }

        public bool IsReturnable
        {
            get;
            private set;
        }

        public bool IsTaxExemptable
        {
            get;
            private set;
        }

        public bool IsFired
        {
            get;
            private set;
        }

        public bool IsOutOfStock
        {
            get;
            private set;
        }

        public bool IsGrouping
        {
            get;
            private set;
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event ItemValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(ItemFieldName field)
        {
            if ((ValueChanged != null) && !_haltEvents)
                ValueChanged.Invoke(this, new ItemValueChangedArgs(ActiveItem, field));
        }
        #endregion

        #region Initialize
        public ItemEditorDetailsControl()
        {
            InitializeComponent();
            InitializeCategories();
            InitializeTextFields();
            InitializeIsEnabled(true);
            InitializePrintOptionSet();
            InitializeTaxField();
            InitializeIsReturnable(true);
            InitializeIsTaxExemptable(true);
            InitializeIsFired(true);
            InitializeIsOutOfStock(true);
            InitializeIsGrouping(true);
        }

        private void InitializeIsGrouping(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(radioButtonIsGrouping, radioButtonIsNotGrouping,
                (ActiveItem == null ? defaultValue : ActiveItem.IsGrouping), init);
        }

        private void InitializeIsOutOfStock(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(pushButtonIsOutOfStockYes, pushButtonIsOutOfStockNo,
                (ActiveItem == null ? defaultValue : ActiveItem.IsOutOfStock), init);
        }

        private void InitializeIsFired(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(pushButtonIsFiredYes, pushButtonIsFiredNo,
                (ActiveItem == null ? defaultValue : ActiveItem.IsFired), init);
        }

        private void InitializeIsTaxExemptable(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(pushButtonIsTaxExemptableYes, pushButtonIsTaxExemptableNo,
                (ActiveItem == null ? defaultValue : ActiveItem.IsTaxExemptable), init);
        }

        private void InitializeIsReturnable(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(pushButtonIsReturnableYes, pushButtonIsReturnableNo,
                (ActiveItem == null ? defaultValue : ActiveItem.IsReturnable), init);
        }

        private void InitializeIsEnabled(bool init, bool? defaultValue = null)
        {
            InitializeRadioButton(pushButtonIsForSaleYes, pushButtonIsForSaleNo,
                (ActiveItem == null ? defaultValue : ActiveItem.IsActive), init);
        }

        private void InitializeRadioButton(PushRadioButton yes, PushRadioButton no, bool? isTrue, bool init)
        {
            // Deal with selecting the correct option
            if (isTrue != null)
            {
                yes.IsSelected = isTrue.Value;
                no.IsSelected = !isTrue.Value;
            }
            else if (!init)
            {
                yes.IsSelected = false;
                no.IsSelected = false;
            }
        }

        private void InitializePrintOptionSet()
        {
            if (ActiveItem == null)
            {
                checkBoxPrintDestinationJournal.IsSelected =
                    checkBoxPrintDestinationReceipt.IsSelected =
                    checkBoxPrintDestinationBar1.IsSelected =
                    checkBoxPrintDestinationBar2.IsSelected =
                    checkBoxPrintDestinationBar3.IsSelected =
                    checkBoxPrintDestinationKitchen1.IsSelected =
                    checkBoxPrintDestinationKitchen2.IsSelected =
                    checkBoxPrintDestinationKitchen3.IsSelected = false;
                PrintDestinations = PrintDestination.None;
                return;
            }

            checkBoxPrintDestinationJournal.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Journal) != 0);
            checkBoxPrintDestinationReceipt.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Receipt) != 0);
            checkBoxPrintDestinationKitchen1.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Kitchen1) != 0);
            checkBoxPrintDestinationKitchen2.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Kitchen2) != 0);
            checkBoxPrintDestinationKitchen3.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Kitchen3) != 0);
            checkBoxPrintDestinationBar1.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Bar1) != 0);
            checkBoxPrintDestinationBar2.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Bar2) != 0);
            checkBoxPrintDestinationBar3.IsSelected =
                ((ActiveItem.PrintDestinations & PrintDestination.Bar3) != 0);

            PrintDestinations = ActiveItem.PrintDestinations;
        }

        private void InitializeCategories()
        {
            int selectionIndex = 0;
            int index = 0;
            IEnumerable<Category> categories = Category.GetAll();
            comboBoxCategory.Items.Clear();

            foreach (Category category in categories)
            {
                comboBoxCategory.Items.Add(category.NameValue);
                if ((ActiveItem != null) && (category.Id == ActiveItem.CategoryId))
                    selectionIndex = index;
                index++;
            }

            //if (selectionIndex >= 0)
                comboBoxCategory.SelectedIndex = selectionIndex;
        }

        private void InitializeTextFields()
        {
            if (ActiveItem != null)
            {
                textBoxFullName.Text = ActiveItem.FullName;
                textBoxPosName.Text = ActiveItem.ShortName;
                textBoxPrice.Text = ActiveItem.Price.ToString("C2");
            }
            else
            {
                textBoxFullName.Text = "";
                textBoxPosName.Text = "";
                textBoxPrice.Text = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + "0.00";
            }
        }

        private void InitializeTaxField()
        {
            int selectionIndex = 0;
            IEnumerable<Tax> taxes = Tax.GetAll();
            comboBoxTaxId.Items.Clear();
            comboBoxTaxId.Items.Add(""); // No stringValue

            int index = 0;
            foreach (Tax tax in taxes)
            {
                comboBoxTaxId.Items.Add(tax.TaxName);
                if ((ActiveItem != null) && (ActiveItem.TaxId == tax.Id))
                    selectionIndex = index + 1;
                index++;
            }

            // Deal with selecting the correct option
            comboBoxTaxId.SelectedIndex = selectionIndex;
        }
        #endregion

        #region Event Handlers
        [Obfuscation(Exclude = true)]
        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            PushComboBox pushComboBox = sender as PushComboBox;
            if (pushComboBox == null) return;
            string categoryName = pushComboBox.SelectedItem;
            Category matchingCategory = Category.Get(categoryName);
            //Category matchingCategory =
            //    Category.FindByName(Category.GetAll(), categoryName);
            if (matchingCategory == null) return;
            CategoryId = matchingCategory.Id;
            DoChangedValueEvent(ItemFieldName.Category);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxFullName_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents)
                return;
            FullName = textBoxFullName.Text;
            DoChangedValueEvent(ItemFieldName.FullName);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPosName_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents)
                return;
            ShortName = textBoxPosName.Text;
            DoChangedValueEvent(ItemFieldName.PosName);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPrice_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_haltEvents)
                return;
            try
            {
                Price = Convert.ToDouble(textBoxPrice.Text.Replace(
                    CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, ""));
            }
            catch (Exception ex)
            {
                PosDialogWindow.ShowDialog(
                    ex.Message, Strings.ItemEditorInvalidPrice);
                return;
            }
            DoChangedValueEvent(ItemFieldName.Price);
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxTaxId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            try
            {
                PushComboBox pushComboBox = sender as PushComboBox;
                if (pushComboBox != null)
                {
                    string taxName = pushComboBox.SelectedItem;
                    Tax tax = Tax.Get(taxName);
                    TaxId = ((tax != null) ? tax.Id : 0);
                }
            }
            catch (Exception ex)
            {
                PosDialogWindow.ShowDialog(
                    ex.Message, Strings.ItemEditorInvalidTaxSetting);
                return;
            }
            DoChangedValueEvent(ItemFieldName.Tax);
        }

        [Obfuscation(Exclude = true)]
        private void pushButtonIsForSale_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsActive = Equals(sender, pushButtonIsForSaleYes);
            pushButtonIsForSaleYes.IsSelected = IsActive;
            pushButtonIsForSaleNo.IsSelected = !IsActive;
            DoChangedValueEvent(ItemFieldName.IsEnabled);
        }

        [Obfuscation(Exclude = true)]
        private void pushButtonIsTaxExemptable_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsTaxExemptable = Equals(sender, pushButtonIsTaxExemptableYes);
            pushButtonIsTaxExemptableYes.IsSelected = IsTaxExemptable;
            pushButtonIsTaxExemptableNo.IsSelected = !IsTaxExemptable;
            DoChangedValueEvent(ItemFieldName.IsTaxExemptable);
        }

        [Obfuscation(Exclude = true)]
        private void pushButtonIsReturnable_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsReturnable = Equals(sender, pushButtonIsReturnableYes);
            pushButtonIsReturnableYes.IsSelected = IsReturnable;
            pushButtonIsReturnableNo.IsSelected = !IsReturnable;
            DoChangedValueEvent(ItemFieldName.IsReturnable);
        }

        [Obfuscation(Exclude = true)]
        private void pushButtonIsFired_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsFired = Equals(sender, pushButtonIsFiredYes);
            pushButtonIsFiredYes.IsSelected = IsFired;
            pushButtonIsFiredNo.IsSelected = !IsFired;
            DoChangedValueEvent(ItemFieldName.IsFired);
        }

        [Obfuscation(Exclude = true)]
        private void pushButtonIsOutOfStock_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsOutOfStock = Equals(sender, pushButtonIsOutOfStockYes);
            pushButtonIsOutOfStockYes.IsSelected = IsOutOfStock;
            pushButtonIsOutOfStockNo.IsSelected = !IsOutOfStock;
            DoChangedValueEvent(ItemFieldName.IsOutOfStock);
        }

        [Obfuscation(Exclude = true)]
        private void radioButtonIsGrouping_SelectionGained(object sender, EventArgs e)
        {
            if (_haltEvents)
                return;
            IsGrouping = Equals(sender, radioButtonIsGrouping);
            radioButtonIsGrouping.IsSelected = IsGrouping;
            radioButtonIsNotGrouping.IsSelected = !IsGrouping;
            DoChangedValueEvent(ItemFieldName.IsGrouping);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationJournal_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Journal);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationReceipt_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Receipt);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationKitchen1_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Kitchen1);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationKitchen2_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Kitchen2);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationKitchen3_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Kitchen3);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationBar1_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Bar1);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationBar2_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Bar2);
        }

        [Obfuscation(Exclude = true)]
        private void checkBoxPrintDestinationBar3_SelectionChanged(object sender, EventArgs e)
        {
            TogglePrintDestination(PrintDestination.Bar3);
        }

        private void TogglePrintDestination(PrintDestination destination)
        {
            if (_haltEvents)
                return;
            PrintDestinations =
                ((checkBoxPrintDestinationJournal.IsSelected ? PrintDestination.Journal : PrintDestination.None) |
                (checkBoxPrintDestinationReceipt.IsSelected ? PrintDestination.Receipt : PrintDestination.None) |
                (checkBoxPrintDestinationKitchen1.IsSelected ? PrintDestination.Kitchen1 : PrintDestination.None) |
                (checkBoxPrintDestinationKitchen2.IsSelected ? PrintDestination.Kitchen2 : PrintDestination.None) |
                (checkBoxPrintDestinationKitchen3.IsSelected ? PrintDestination.Kitchen3 : PrintDestination.None) |
                (checkBoxPrintDestinationBar1.IsSelected ? PrintDestination.Bar1 : PrintDestination.None) |
                (checkBoxPrintDestinationBar2.IsSelected ? PrintDestination.Bar2 : PrintDestination.None) |
                (checkBoxPrintDestinationBar3.IsSelected ? PrintDestination.Bar3 : PrintDestination.None));
            DoChangedValueEvent(ItemFieldName.PrintDestination);
        }
        #endregion
    }
}
