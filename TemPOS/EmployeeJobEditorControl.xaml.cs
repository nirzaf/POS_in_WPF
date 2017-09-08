using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EmployeeJobEditorControl.xaml
    /// </summary>
    public partial class EmployeeJobEditorControl : UserControl
    {
        private bool _haltEvents; 
        private EmployeeJob _activeJob;

        public EmployeeJob ActiveJob {
            get { return _activeJob; }
            set
            {
                _activeJob = value;
                _haltEvents = true;
                if (value != null)
                {
                    textBoxName.Text = value.Description;
                    buttonDeliveriesNo.IsSelected = !value.AllowedDispatch;
                    buttonDeliveriesYes.IsSelected = value.AllowedDispatch;
                    buttonTipsNo.IsSelected = !value.HasTips;
                    buttonTipsYes.IsSelected = value.HasTips;
                    buttonIsEnabledNo.IsSelected = !value.IsEnabled;
                    buttonIsEnabledYes.IsSelected = value.IsEnabled;
                }
                else
                {
                    textBoxName.Text = "";
                    buttonDeliveriesNo.IsSelected = true;
                    buttonDeliveriesYes.IsSelected = false;
                    buttonTipsNo.IsSelected = true;
                    buttonTipsYes.IsSelected = false;
                    buttonIsEnabledNo.IsSelected = false;
                    buttonIsEnabledYes.IsSelected = true;
                }
                _haltEvents = false;
            }
        }

        public bool IsNew
        {
            get;
            private set;
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if ((ValueChanged != null) && (ActiveJob != null) && !_haltEvents)
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public EmployeeJobEditorControl()
        {
            IsNew = false;
            InitializeComponent();
        }

        public bool Update()
        {
            if (ActiveJob != null)
            {
                ActiveJob.SetAllowedDispatch(buttonDeliveriesYes.IsSelected);
                ActiveJob.SetDescription(textBoxName.Text);
                ActiveJob.SetHasTips(buttonTipsYes.IsSelected);
                ActiveJob.SetIsEnabled(buttonIsEnabledYes.IsSelected);
                return ActiveJob.Update();
            }
            IsNew = false;
            ActiveJob = EmployeeJob.Add(
                textBoxName.Text,
                buttonTipsYes.IsSelected,
                buttonDeliveriesYes.IsSelected);
            if (buttonIsEnabledNo.IsSelected)
            {
                ActiveJob.SetIsEnabled(false);
                ActiveJob.Update();
            }
            return true;
        }

        internal void Add()
        {
            IsNew = true;
            ActiveJob = null;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonTips_SelectionGained(object sender, EventArgs e)
        {
            if ((!buttonTipsYes.IsSelected || !Equals(sender, buttonTipsNo)) &&
                (!buttonTipsNo.IsSelected || !Equals(sender, buttonTipsYes))) return;
            buttonTipsNo.IsSelected = Equals(sender, buttonTipsNo);
            buttonTipsYes.IsSelected = Equals(sender, buttonTipsYes);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDeliveries_SelectionGained(object sender, EventArgs e)
        {
            if ((!buttonDeliveriesYes.IsSelected || !Equals(sender, buttonDeliveriesNo)) &&
                (!buttonDeliveriesNo.IsSelected || !Equals(sender, buttonDeliveriesYes))) return;
            buttonDeliveriesNo.IsSelected = Equals(sender, buttonDeliveriesNo);
            buttonDeliveriesYes.IsSelected = Equals(sender, buttonDeliveriesYes);
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void buttonIsEnabled_SelectionGained(object sender, EventArgs e)
        {
            if ((!buttonIsEnabledYes.IsSelected || !Equals(sender, buttonIsEnabledNo)) &&
                (!buttonIsEnabledNo.IsSelected || !Equals(sender, buttonIsEnabledYes))) return;
            buttonIsEnabledNo.IsSelected = Equals(sender, buttonIsEnabledNo);
            buttonIsEnabledYes.IsSelected = Equals(sender, buttonIsEnabledYes);
            DoChangedValueEvent();
        }
    }
}
