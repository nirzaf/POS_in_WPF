using System;
using System.Collections.Generic;
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
using PosModels;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TaxEditorControl.xaml
    /// </summary>
    public partial class TaxEditorControl : UserControl
    {
        #region Fields
        private bool _haltEvents;
        private Tax _activeTax;
        #endregion

        #region Properties
        public Tax ActiveTax
        {
            get
            {
                return _activeTax;
            }
            set
            {
                _haltEvents = true;
                _activeTax = value;
                InitializeFields();
                _haltEvents = false;
            }
        }
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event TaxValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(TaxFieldName field)
        {
            if ((ValueChanged != null) && (ActiveTax != null) && !_haltEvents)
                ValueChanged.Invoke(this, new TaxValueChangedArgs(ActiveTax, field));
        }
        #endregion

        public TaxEditorControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            if (ActiveTax != null)
            {
                textBoxDescription.Text = ActiveTax.TaxName;
                double percentage = ActiveTax.Percentage * 100;
                textBoxPercentage.Text = percentage.ToString("F1") + "%";
            }
            else
            {
                textBoxDescription.Text = "";
                textBoxPercentage.Text = "0%";
            }
        }

        public bool UpdateTax()
        {
            string description;
            double percentage;
            try
            {
                description = GetDescription();
                percentage = GetPercentage();
            }
            catch (Exception ex)
            {
                // Message the user regarding the first invalid field
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Strings.Exception);
                return false;
            }

            // Is there an ActiveCategory?
            if (ActiveTax == null)
            {
                ActiveTax = Tax.Add(description, percentage);
            }
            else
            {
                // Update the category values for the ActiveCategory
                ActiveTax.SetTaxName(description);
                ActiveTax.SetPercentage(percentage);

                // Update the database
                ActiveTax.Update();
            }

            return true;
        }

        private double GetPercentage()
        {
            try
            {
                return Convert.ToDouble(textBoxPercentage.Text.Replace("%", "")) / 100;
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.PleaseEnterAValidPercentageValue, innerException);
            }
        }

        private string GetDescription()
        {
            try
            {
                if ((textBoxDescription.Text == null) || (textBoxDescription.Text.Equals("")))
                    throw new Exception(Strings.EmptyString);
                return textBoxDescription.Text;
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.PleaseEnterAValidDescription, innerException);
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxDescription_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(TaxFieldName.Description);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxPercentage_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(TaxFieldName.Percentage);
        }
    }
}
