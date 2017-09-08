using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemEditorOptionSetControl.xaml
    /// </summary>
    public partial class ItemEditorOptionSetControl : UserControl
    {
        private bool _haltEvents = true;
        private readonly List<ItemOptionSet> _cachedOptionSets = new List<ItemOptionSet>();
        private Item _activeItem;

        #region Events
        [Obfuscation(Exclude = true)]
        public event ItemValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(ItemFieldName field)
        {
            if ((ValueChanged != null) && (ActiveItem != null) && !_haltEvents)
                ValueChanged.Invoke(this, new ItemValueChangedArgs(ActiveItem, field));
        }
        #endregion        

        public Item ActiveItem
        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;
                if (value != null)
                {
                    OptionSetId1 = value.ItemOptionSetIds[0];
                    OptionSetId2 = value.ItemOptionSetIds[1];
                    OptionSetId3 = value.ItemOptionSetIds[2];
                }
                else
                {
                    OptionSetId1 = OptionSetId2 = OptionSetId3 = 0;
                }
                InitializeOptionSets();
            }
        }

        public int OptionSetId1
        {
            get;
            private set;
        }

        public int OptionSetId2
        {
            get;
            private set;
        }

        public int OptionSetId3
        {
            get;
            private set;
        }

        public bool KeepChanges
        {
            get;
            set;
        }

        public ItemEditorOptionSetControl()
        {
            KeepChanges = false;
            _cachedOptionSets.AddRange(ItemOptionSet.GetAll());
            InitializeComponent();
            InitializeOptionSets();
            InitializeReadOnly();
        }

        private void InitializeReadOnly()
        {
            if (DayOfOperation.Today == null) return;
            gridControl.IsEnabled = false;
            labelReadOnly.Visibility = Visibility.Visible;
        }

        private void InitializeOptionSets()
        {
            _haltEvents = true;
            comboBoxOptionSet1.Items.Clear();
            comboBoxOptionSet2.Items.Clear();
            comboBoxOptionSet3.Items.Clear();

            // Remove an option set
            comboBoxOptionSet1.Items.Add("");
            comboBoxOptionSet2.Items.Add("");
            comboBoxOptionSet3.Items.Add("");

            // Fill the comboboxes
            foreach (ItemOptionSet set in
                _cachedOptionSets.Where(set => (ActiveItem == null) || !set.ContainsItemOptionUsingItem(ActiveItem.Id)))
            {
                comboBoxOptionSet1.Items.Add(set.Name);
                comboBoxOptionSet2.Items.Add(set.Name);
                comboBoxOptionSet3.Items.Add(set.Name);
            }

            // Deal with selecting the correct option
            ItemOptionSet set1 = null;
            if (OptionSetId1 >= 0)
                set1 = GetOptionSet(_cachedOptionSets, OptionSetId1);
            ItemOptionSet set2 = null;
            if (OptionSetId2 >= 0)
                set2 = GetOptionSet(_cachedOptionSets, OptionSetId2);
            ItemOptionSet set3 = null;
            if (OptionSetId3 >= 0)
                set3 = GetOptionSet(_cachedOptionSets, OptionSetId3);

            comboBoxOptionSet1.SelectedIndex = ((set1 != null) ?
                GetComboBoxIndex(comboBoxOptionSet1, set1.Name) : 0);
            comboBoxOptionSet2.SelectedIndex = ((set2 != null) ?
                GetComboBoxIndex(comboBoxOptionSet2, set2.Name) : 0);
            comboBoxOptionSet3.SelectedIndex = ((set3 != null) ?
                GetComboBoxIndex(comboBoxOptionSet3, set3.Name) : 0);
            
            _haltEvents = false;
        }

        private ItemOptionSet GetOptionSet(IEnumerable<ItemOptionSet> sets, int p)
        {
            return sets.FirstOrDefault(set => set.Id == p);
        }

        private int GetComboBoxIndex(PushComboBox comboBox, string p)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i].Equals(p))
                    return i;
            }
            return -1;
        }

        private int GetItemOptionSetIdByName(string text)
        {
            return (from set in _cachedOptionSets where set.Name.Equals(text) select set.Id).FirstOrDefault();
        }

        public void UpdateOptionSets()
        {
            _cachedOptionSets.Clear();
            _cachedOptionSets.AddRange(ItemOptionSet.GetAll());
            InitializeOptionSets();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxOptionSet1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents) return;
            string text = comboBoxOptionSet1.SelectedItem;
            OptionSetId1 = GetItemOptionSetIdByName(text);
            DoChangedValueEvent(ItemFieldName.OptionSet1);
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxOptionSet2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents) return;
            string text = comboBoxOptionSet2.SelectedItem;
            OptionSetId2 = GetItemOptionSetIdByName(text);
            DoChangedValueEvent(ItemFieldName.OptionSet2);
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxOptionSet3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_haltEvents) return;
            string text = comboBoxOptionSet3.SelectedItem;
            OptionSetId3 = GetItemOptionSetIdByName(text);
            DoChangedValueEvent(ItemFieldName.OptionSet3);
        }
    }
}
