using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemOptionMaintenanceControl.xaml
    /// </summary>
    public partial class ItemOptionMaintenanceControl : UserControl
    {
        private ItemOptionSet _activeItemOptionSet;

        public ItemOptionSet ActiveItemOptionSet
        {
            get { return _activeItemOptionSet; }
            set
            {
                _activeItemOptionSet = value;
                InitializeItemOptionsListBox();
            }
        }

        public ItemOptionMaintenanceControl()
        {
            InitializeComponent();
        }

        private void InitializeItemOptionsListBox()
        {
            IEnumerable<ItemOption> options = ItemOption.GetInSet(_activeItemOptionSet.Id);
            FormattedListBoxItem first = null;
            foreach (ItemOption option in options)
            {
                FormattedListBoxItem item = new FormattedListBoxItem(option,
                    option.Name, true);
                if (first == null)
                    first = item;
                listBoxOptions.Items.Add(item);
            }
            listBoxOptions.UpdateLayout();
            listBoxOptions.ScrollIntoView(first);
        }

        [Obfuscation(Exclude = true)]
        private void listBoxOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            FormattedListBoxItem item = e.AddedItems[0] as FormattedListBoxItem;
            if (item == null) return;
            ItemOption option = item.ReferenceObject as ItemOption;

            editorControl.ActiveItemOptionSet = ActiveItemOptionSet;
            editorControl.ActiveItemOption = option;
            editorControl.IsEnabled = true;
            buttonDelete.IsEnabled = true;
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void editorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void SetEditMode(bool inEditMode)
        {
            listBoxOptions.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonDelete.IsEnabled = (!inEditMode && (listBoxOptions.SelectedItem != null));
            buttonCancel.IsEnabled = inEditMode;
            buttonUpdate.IsEnabled = inEditMode;
        }

        private void AddStart()
        {
            listBoxOptions.SelectedItem = null;
            editorControl.IsEnabled = true;
            editorControl.ActiveItemOptionSet = ActiveItemOptionSet;
            editorControl.ActiveItemOption = null;
            SetEditMode(true);
        }

        private void DeleteOption()
        {
            FormattedListBoxItem item = listBoxOptions.SelectedItem as FormattedListBoxItem;
            if (item == null || (item.ReferenceObject == null)) return;
            if (PosDialogWindow.ShowDialog(
                Strings.ItemSetupConfirmDeleteItemOption, Strings.Confirmation,
                DialogButtons.YesNo) != DialogButton.Yes) return;
            ItemOption option = item.ReferenceObject as ItemOption;
            if (option == null) return;
            option.Discontinue();
            listBoxOptions.Items.Remove(item);
            buttonDelete.IsEnabled = false;
            editorControl.IsEnabled = false;
            editorControl.ActiveItemOption = null;
        }

        private void CancelChanges()
        {
            FormattedListBoxItem item = listBoxOptions.SelectedItem as FormattedListBoxItem;
            if (item != null)
            {
                editorControl.ActiveItemOption = item.ReferenceObject as ItemOption;
                //ItemOption currentItemOption = item.ReferenceObject as ItemOption;
                //editorControl.ActiveItemOption = ItemOption.Get(currentItemOption.Id);
                //item.Set(editorControl.ActiveItemOption, item.Text);
            }
            else
            {
                editorControl.IsEnabled = false;
                editorControl.ActiveItemOption = null;
            }
            SetEditMode(false);
        }

        private void UpdateOption()
        {
            bool isNew = (listBoxOptions.SelectedItem == null);
            if (!editorControl.HasValidationError())
            {
                editorControl.Update();
                if (isNew)
                {
                    FormattedListBoxItem item = new FormattedListBoxItem(
                        editorControl.ActiveItemOption,
                        editorControl.ActiveItemOption.Name,
                        true);
                    listBoxOptions.Items.Add(item);
                    listBoxOptions.UpdateLayout();
                    listBoxOptions.SelectedItem = item;
                    listBoxOptions.ScrollIntoView(item);
                }
                else
                {
                    FormattedListBoxItem listItem = listBoxOptions.SelectedItem as FormattedListBoxItem;
                    if (listItem != null)
                    {
                        listItem.Set(editorControl.ActiveItemOption,
                        editorControl.ActiveItemOption.Name);
                    }
                }
                SetEditMode(false);
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStart();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteOption();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateOption();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

        public static PosDialogWindow CreateInDefaultWindow(string itemOptionSetName)
        {
            ItemOptionMaintenanceControl control = new ItemOptionMaintenanceControl();
            return new PosDialogWindow(control, Strings.ItemSetupOptionsEdit + itemOptionSetName,
                915, 480);
        }
    }
}
