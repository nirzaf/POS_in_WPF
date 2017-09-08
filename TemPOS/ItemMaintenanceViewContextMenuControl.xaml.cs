using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ItemMaintenanceViewContextMenuControl.xaml
    /// </summary>
    public partial class ItemMaintenanceViewContextMenuControl : UserControl
    {
        public enum ItemViewMode
        {
            Categories,
            ItemsInCategory,
            AllItems,
            ItemOptionSets,
            Ingredients
        }

        private static ItemViewMode _viewMode = ItemViewMode.AllItems;
        
        [Obfuscation(Exclude = true)]
        public static event EventHandler ViewModeChanged;

        public ContextMenu ViewContextMenu
        {
            get
            {
                DependencyObject depObject = VisualTreeHelper.GetParent(this);
                while (depObject != null)
                {
                    if (depObject is ContextMenu)
                        return depObject as ContextMenu;
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
                return null;
            }
        }
        public static ItemViewMode ViewMode
        {
            get
            {
                return _viewMode;
            }
            set
            {
                if (_viewMode != value)
                {
                    _viewMode = value;
                    if (ViewModeChanged != null)
                        ViewModeChanged.Invoke(null, new EventArgs());
                }
            }
        }

        public ItemMaintenanceViewContextMenuControl()
        {
            InitializeComponent();
            buttonAllCategories.IsSelected = (ViewMode == ItemViewMode.Categories);
            buttonAllItemsInCategory.IsSelected = (ViewMode == ItemViewMode.ItemsInCategory);
            buttonAllItems.IsSelected = (ViewMode == ItemViewMode.AllItems);
            buttonAllItemOptionsSets.IsSelected = (ViewMode == ItemViewMode.ItemOptionSets);
            buttonAllIngredients.IsSelected = (ViewMode == ItemViewMode.Ingredients);
        }

        [Obfuscation(Exclude = true)]
        private void SetRadioChecked(object sender)
        {
            buttonAllCategories.IsSelected = Equals(sender, buttonAllCategories);
            buttonAllItemsInCategory.IsSelected = Equals(sender, buttonAllItemsInCategory);
            buttonAllIngredients.IsSelected = Equals(sender, buttonAllIngredients);
            buttonAllItemOptionsSets.IsSelected = Equals(sender, buttonAllItemOptionsSets);
            buttonAllItems.IsSelected = Equals(sender, buttonAllItems);
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllItems_SelectionGained(object sender, EventArgs e)
        {
            ViewContextMenu.IsOpen = false;
            SetRadioChecked(sender);
            ViewMode = ItemViewMode.AllItems;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllItemOptionsSets_SelectionGained(object sender, EventArgs e)
        {
            ViewContextMenu.IsOpen = false;
            SetRadioChecked(sender);
            ViewMode = ItemViewMode.ItemOptionSets;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllIngredients_SelectionGained(object sender, EventArgs e)
        {
            ViewContextMenu.IsOpen = false;
            SetRadioChecked(sender);
            ViewMode = ItemViewMode.Ingredients;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllCategories_SelectionGained(object sender, EventArgs e)
        {
            ViewContextMenu.IsOpen = false;
            SetRadioChecked(sender);
            ViewMode = ItemViewMode.Categories;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAllItemsInCategory_SelectionGained(object sender, EventArgs e)
        {
            ViewContextMenu.IsOpen = false;
            SetRadioChecked(sender);
            ViewMode = ItemViewMode.ItemsInCategory;
        }
    }
}
