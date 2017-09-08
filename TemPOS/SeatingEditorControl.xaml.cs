using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosModels.Managers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingEditorControl.xaml
    /// </summary>
    public partial class SeatingEditorControl : UserControl
    {
        private bool _haltEvent;
        private Seating _selectedSeating;

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;
        private void DoChangedValueEvent()
        {
            if (!_haltEvent && (ValueChanged != null))
                ValueChanged.Invoke(this, new EventArgs());
        }
        #endregion

        public Room SelectedRoom
        {
            get;
            set;
        }

        public Seating SelectedSeating
        {
            get { return _selectedSeating; }
            set
            {
                _selectedSeating = value;
                _haltEvent = true;
                // Initialize fields
                if (_selectedSeating != null)
                {
                    textBoxName.Text = _selectedSeating.Description;
                    textBoxCapacity.Text = "" + _selectedSeating.Capacity;
                }
                else
                {
                    textBoxName.Text = "";
                    textBoxCapacity.Text = "0";
                }                
                _haltEvent = false;
            }
        }

        public SeatingEditorControl()
        {
            InitializeComponent();
        }

        public bool UpdateSeating()
        {
            // Add Seating
            if (SelectedSeating == null)
            {
                SelectedSeating = SeatingManager.AddSeating(SelectedRoom.Id,
                    GetName(), GetCapacity());
                return (SelectedSeating != null);
            }
            
            // Update Seating
            SelectedSeating.SetDescription(GetName());
            SelectedSeating.SetCapacity(GetCapacity());
            return SelectedSeating.Update();
        }

        private string GetName()
        {
            if (textBoxName.Text == null)
                return "";
            return textBoxName.Text;
        }

        private int GetCapacity()
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(textBoxCapacity.Text);
            }
            catch
            {
            }
            return result;
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxCapacity_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent();
        }
    }
}
