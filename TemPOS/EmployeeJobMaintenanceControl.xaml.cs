using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for EmployeeJobMaintenanceControl.xaml
    /// </summary>
    public partial class EmployeeJobMaintenanceControl : UserControl
    {
        FormattedListBoxItem lastKnownSelection = null;

        public EmployeeJobMaintenanceControl()
        {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeJobsList();
        }

        private void InitializeJobsList()
        {
            foreach (FormattedListBoxItem item in EmployeeJob.GetAll()
                .Select(job => new FormattedListBoxItem(job, job.Description, true)))
            {
                listBox1.Items.Add(item);
            }
        }

        [Obfuscation(Exclude = true)]
        private void InitializeEventHandlers()
        {
            editorControl.ValueChanged += editorControl_ValueChanged;
        }

        [Obfuscation(Exclude = true)]
        void editorControl_ValueChanged(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void SetEditMode(bool inEditMode)
        {
            if (inEditMode || (listBox1.SelectedItem == null))
                groupBoxProperties.IsEnabled = inEditMode;
            groupBoxList.IsEnabled = !inEditMode;
            buttonAdd.IsEnabled = !inEditMode;
            buttonUpdate.IsEnabled = inEditMode;
            buttonCancel.IsEnabled = inEditMode;
            var parentWindow = Window.GetWindow(this) as PosDialogWindow;
            if (parentWindow != null) parentWindow.SetButtonsEnabled(!inEditMode);
        }

        private void AddEmployeeJob()
        {
            lastKnownSelection = listBox1.SelectedItem as FormattedListBoxItem;
            if (lastKnownSelection != null) lastKnownSelection.IsSelected = false;
            listBox1.SelectedItem = null;
            SetEditMode(true);
            editorControl.Add();
        }

        private void UpdateChanges()
        {
            bool isNew = editorControl.IsNew;
            editorControl.Update();
            if (isNew)
                AddEmployeeJobToListBox(editorControl.ActiveJob);
            else if (listBox1.SelectedItem != null)
            {
                FormattedListBoxItem formattedListBoxItem = listBox1.SelectedItem as FormattedListBoxItem;
                if (formattedListBoxItem != null)
                    formattedListBoxItem.Text =
                        editorControl.ActiveJob.Description;
            }
            SetEditMode(false);
        }

        private void AddEmployeeJobToListBox(EmployeeJob job)
        {
            FormattedListBoxItem newItem =
                new FormattedListBoxItem(job, job.Description, true);
            listBox1.Items.Add(newItem);
            listBox1.ScrollIntoView(newItem);
            listBox1.SelectedItem = newItem;
        }

        private void CancelChanges()
        {
            if (editorControl.IsNew)
            {
                listBox1.SelectedItem = null;
                editorControl.ActiveJob = null;
            }
            else
                editorControl.ActiveJob = editorControl.ActiveJob;
            SetEditMode(false);
        }

        private void UpdateSelection()
        {
            if (lastKnownSelection == null)
            {
                listBox1.SelectedItem = listBox1.Items[0];
                listBox1.ScrollIntoView(listBox1.SelectedItem);
                return;
            }
            if (listBox1.Items.Contains(lastKnownSelection))
            {
                listBox1.SelectedItem = lastKnownSelection;
                lastKnownSelection.IsSelected = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            var item = e.AddedItems[0] as FormattedListBoxItem;
            if (item != null)
            {
                var job = item.ReferenceObject as EmployeeJob;
                editorControl.ActiveJob = job;
            }
            groupBoxProperties.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeJob();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateChanges();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelChanges();
        }

    }
}
