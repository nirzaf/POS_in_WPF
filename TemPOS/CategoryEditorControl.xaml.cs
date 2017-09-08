using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PosControls;
using PosModels;
using TemPOS.EventHandlers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CategoryEditorControl.xaml
    /// </summary>
    public partial class CategoryEditorControl : UserControl
    {
        private bool _haltEvents;
        private int _originalDisplayIndex = -1;
        private Category _activeCategory;

        public IEnumerable<Category> Categories
        {
            get
            {
                // Find the ItemMaintenanceControl
                Window window = Window.GetWindow(this);
                if (window != null)
                {
                    ItemMaintenanceControl control = FindItemMaintenanceControl(window);
                    if (control != null)
                    {
                        foreach (var category in (
                                                     from FormattedListBoxItem item in control.listBoxCategories.Items
                                                     select item.ReferenceObject).OfType<Category>())
                        {
                            yield return category;
                        }
                    }
                }
            }
        }

        private ItemMaintenanceControl FindItemMaintenanceControl(DependencyObject depObject)
        {
            if (depObject is ItemMaintenanceControl)
                return depObject as ItemMaintenanceControl;
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(depObject); index++)
            {
                DependencyObject childDepObject = VisualTreeHelper.GetChild(depObject, index);
                if (childDepObject is ItemMaintenanceControl)
                    return childDepObject as ItemMaintenanceControl;
                ItemMaintenanceControl control = FindItemMaintenanceControl(childDepObject);
                if (control != null)
                    return control;
            }
            return null;
        }


        public Category ActiveCategory
        {
            get
            {
                return _activeCategory;
            }
            set
            {
                _haltEvents = true;
                _activeCategory = value;
                if (_activeCategory != null)
                    _originalDisplayIndex = _activeCategory.DisplayIndex;
                else
                    _originalDisplayIndex = -1;
                InitializeFields();
                _haltEvents = false;
            }
        }

        #region Events
        [Obfuscation(Exclude = true)]
        public event CategoryValueChangedHandler ValueChanged;
        private void DoChangedValueEvent(CategoryFieldName field)
        {
            if ((ValueChanged != null) && (ActiveCategory != null) && !_haltEvents)
                ValueChanged.Invoke(this, new CategoryValueChangedArgs(ActiveCategory, field));
        }
        #endregion

        public CategoryEditorControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            comboBoxDisplyIndex.Items.Clear();
            comboBoxDisplyIndex.Items.Add(Strings.CategoryEditorDoNotDisplay);
            int highestIndex = 0;
            int countOfDisplayIndices = 0;
            foreach (Category category in Categories)
            {
                if (category.IsDiscontinued)
                    continue;
                if (category.DisplayIndex >= 0)
                    countOfDisplayIndices++;
                if (category.DisplayIndex > highestIndex)
                    highestIndex = category.DisplayIndex;
            }
            for (int i = 1; i <= (countOfDisplayIndices + ((ActiveCategory == null) ? 1 : 0)); i++)
            {
                comboBoxDisplyIndex.Items.Add("" + i);
            }
            if (ActiveCategory != null)
            {
                textBoxName.Text = ActiveCategory.NameValue;
                comboBoxDisplyIndex.SelectedIndex = ActiveCategory.DisplayIndex + 1;
            }
            else
            {
                textBoxName.Text = "";
                comboBoxDisplyIndex.SelectedItem =
                    comboBoxDisplyIndex.Items[comboBoxDisplyIndex.Items.Count - 1];
            }
        }

        public bool UpdateCategory()
        {
            string name;
            short displayIndex;
            try
            {
                name = GetName();
                displayIndex = GetDisplayIndex();
            }
            catch (Exception ex)
            {
                // Message the user regarding the first invalid field
                MessageBox.Show(ex.Message + Environment.NewLine +
                    Environment.NewLine + ex.InnerException.Message,
                    Strings.Exception);
                return false;
            }

            // Is there an ActiveCategory?
            if (ActiveCategory == null)
            {
                int originalIndex = _originalDisplayIndex;
                ActiveCategory = Category.Add(name, displayIndex);
                ReindexDisplayIndices(ActiveCategory, originalIndex);
            }
            else
            {
                // Update the category values for the ActiveCategory
                ActiveCategory.SetName(name);
                ActiveCategory.SetDisplayIndex(displayIndex);

                // Update the database
                ActiveCategory.Update();
                ReindexDisplayIndices(ActiveCategory, _originalDisplayIndex);
            }

            return true;
        }

        public void ReindexDisplayIndices(Category selectedCategory, int originalIndex)
        {
            // Conditions
            // 1. DisplayIndex != -1 && NewIndex == -1 (delete)
            //      * reindex all higher values, by -1
            // 2. DisplayIndex == -1 && NewIndex != -1 (insert)
            //      * reindex all higher values, by +1
            // 3. NewIndex > DisplayIndex, (move)
            //      * reindex higher stringValue from removal to insertion by -1
            // 4. NewIndex < DisplayIndex, (move)
            //      * reindex lower values from insertion to removal by +1
            if ((originalIndex == -1) && (selectedCategory.DisplayIndex != -1)) // insert
            {
                foreach (Category category in Categories)
                {
                    if (category.Id == selectedCategory.Id)
                        continue;
                    if (category.DisplayIndex >= selectedCategory.DisplayIndex)
                    {
                        category.SetDisplayIndex((short)(category.DisplayIndex + 1));
                        category.Update();
                    }
                }
            }
            else if ((originalIndex != -1) && (selectedCategory.DisplayIndex == -1)) // delete
            {
                foreach (Category category in Categories)
                {
                    if (category.Id == selectedCategory.Id)
                        continue;
                    if (category.DisplayIndex >= originalIndex)
                    {
                        category.SetDisplayIndex((short)(category.DisplayIndex - 1));
                        category.Update();
                    }
                }
            }
            else if (originalIndex > selectedCategory.DisplayIndex) // Move
            {
                foreach (Category category in Categories)
                {
                    if (category.Id == selectedCategory.Id)
                        continue;
                    if ((category.DisplayIndex < originalIndex) &&
                        (category.DisplayIndex >= selectedCategory.DisplayIndex))
                    {
                        category.SetDisplayIndex((short)(category.DisplayIndex + 1));
                        category.Update();
                    }
                }

            }
            else if (originalIndex < selectedCategory.DisplayIndex)  // Move
            {
                foreach (Category category in Categories)
                {
                    if (category.Id == selectedCategory.Id)
                        continue;
                    if ((category.DisplayIndex > originalIndex) &&
                        (category.DisplayIndex <= selectedCategory.DisplayIndex))
                    {
                        category.SetDisplayIndex((short)(category.DisplayIndex - 1));
                        category.Update();
                    }
                }
            }
        }

        private string GetName()
        {
            try
            {
                return textBoxName.Text;
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CategoryEditorNameInvalid, innerException);
            }
        }

        private short GetDisplayIndex()
        {
            try
            {
                if (comboBoxDisplyIndex.Text.Equals(Strings.CategoryEditorDoNotDisplay))
                    return -1;
                return (short)(Convert.ToInt16(comboBoxDisplyIndex.Text) - 1);
            }
            catch (Exception innerException)
            {
                throw new Exception(Strings.CategoryEditorDisplayIndexInvalid, innerException);
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxName_TextChanged(object sender, RoutedEventArgs e)
        {
            DoChangedValueEvent(CategoryFieldName.Name);
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxDisplyIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoChangedValueEvent(CategoryFieldName.DisplayIndex);
        }
    }
}
