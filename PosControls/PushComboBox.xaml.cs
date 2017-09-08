using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for PushComboBox.xaml
    /// </summary>
    public partial class PushComboBox : UserControl
    {
        #region Licensed Access Only
        static PushComboBox()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PushComboBox).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private int selectedIndex = -1;
        private List<string> itemCollection = new List<string>();
        private List<FormattedListBoxItem> listItemCollection = 
            new List<FormattedListBoxItem>();

        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedIndexChanged;

        public List<string> Items
        {
            get { return itemCollection; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex)
                    return;
                selectedIndex = value;
                for (int i = 0; i < listItemCollection.Count; i++)
                {
                    listItemCollection[i].IsSelected = (i == selectedIndex);
                }
                if ((selectedIndex >= 0) && (selectedIndex < itemCollection.Count))
                    textBlockControl.Text = itemCollection[selectedIndex];
                else
                    textBlockControl.Text = null;
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged.Invoke(this, new EventArgs());
            }
        }

        public string SelectedItem
        {
            get
            {
                if (selectedIndex >= 0)
                    return itemCollection[selectedIndex];
                return null;
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = -1;
                }
                else
                {
                    for (int i = 0; i < itemCollection.Count; i++)
                    {
                        if (itemCollection[i].Equals(value))
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        public string Text
        {
            get { return textBlockControl.Text; }
            set { textBlockControl.Text = value; }
        }

        public static readonly DependencyProperty EnabledForegroundProperty =
            DependencyProperty.Register(
            "EnabledForeground", typeof(Brush), typeof(PushComboBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledForeground
        {
            get { return (Brush)GetValue(EnabledForegroundProperty); }
            set { SetValue(EnabledForegroundProperty, value); }
        }

        public static readonly DependencyProperty EnabledBackgroundProperty =
            DependencyProperty.Register(
            "EnabledBackground", typeof(Brush), typeof(PushComboBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledBackground
        {
            get { return (Brush)GetValue(EnabledBackgroundProperty); }
            set { SetValue(EnabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(
            "DisabledBackground", typeof(Brush), typeof(PushComboBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledForegroundProperty =
            DependencyProperty.Register(
            "DisabledForeground", typeof(Brush), typeof(PushComboBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledForeground
        {
            get { return (Brush)GetValue(DisabledForegroundProperty); }
            set { SetValue(DisabledForegroundProperty, value); }
        }

        public PushComboBox()
        {
            InitializeComponent();

            // Bindings
            Binding binding = new Binding();
            binding.Source = ConfigurationManager.EnabledComboBoxForegroundBrush;
            BindingOperations.SetBinding(this, PushComboBox.EnabledForegroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.EnabledComboBoxBackgroundBrush;
            BindingOperations.SetBinding(this, PushComboBox.EnabledBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledComboBoxForegroundBrush;
            BindingOperations.SetBinding(this, PushComboBox.DisabledForegroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledComboBoxBackgroundBrush;
            BindingOperations.SetBinding(this, PushComboBox.DisabledBackgroundProperty, binding);

            SetCurrentBrushes();
        }

        private void SetCurrentBrushes()
        {
            borderControl.BorderBrush = ConfigurationManager.BorderBrush;
            if (IsEnabled)
            {
                borderControl.Background = EnabledBackground;
                textBlockControl.Foreground = EnabledForeground;
            }
            else
            {
                borderControl.Background = DisabledBackground;
                textBlockControl.Foreground = DisabledForeground;
            }
        }

        private void SetContextMenu()
        {
            ControlTemplate controlTemplate = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)e.Current;
                string name = entry.Key as string;
                if (name == "pushButtonControlTemplate")
                {
                    controlTemplate = entry.Value as ControlTemplate;
                    break;
                }
            }
            if (controlTemplate != null)
            {
                selectedListBoxItem = null;
                currentListBox = null;
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Template = controlTemplate;
                contextMenu.Placement = PlacementMode.Bottom;
                contextMenu.PlacementTarget = borderControl;
                borderControl.ContextMenu = contextMenu;
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            FormattedListBoxItem listItem = e.AddedItems[0] as FormattedListBoxItem;
            borderControl.ContextMenu.IsOpen = false;
            SelectedIndex = listItem.Id;
        }

        private DragScrollListBox currentListBox = null;
        private FormattedListBoxItem selectedListBoxItem = null;

        [Obfuscation(Exclude = true)]
        private void Border_Initialized(object sender, EventArgs e)
        {
            Border border = (Border)sender;
            border.Background = ConfigurationManager.ApplicationBackgroundBrush;
            border.BorderBrush = ConfigurationManager.BorderBrush;
            currentListBox = border.Child as DragScrollListBox;
            AddFormattedListBoxItems(currentListBox);
        }
        
        private void AddFormattedListBoxItems(DragScrollListBox listBox)
        {
            listItemCollection.Clear();
            listBox.Items.Clear();
            for (int i = 0; i < Items.Count; i++)
            {
                FormattedListBoxItem listBoxItem =
                    new FormattedListBoxItem(i, Items[i], true);
                listBoxItem.IsSelected = (i == selectedIndex);
                if (listBoxItem.IsSelected)
                    selectedListBoxItem = listBoxItem;
                listBoxItem.AllHeights = 45;
                listItemCollection.Add(listBoxItem);
                listBox.Items.Add(listBoxItem);
            }
            if ((selectedListBoxItem != null) && (currentListBox != null))
                currentListBox.ScrollIntoView(selectedListBoxItem);
        }

        [Obfuscation(Exclude = true)]
        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if ((selectedListBoxItem != null) && (currentListBox != null))
                currentListBox.ScrollIntoView(selectedListBoxItem);
        }

        [Obfuscation(Exclude = true)]
        private void borderControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetContextMenu();
            ContextMenu contextMenu = borderControl.ContextMenu;
            contextMenu.IsOpen = true;
            contextMenu.Width = borderControl.ActualWidth;
            e.Handled = true;
        }

        [Obfuscation(Exclude = true)]
        private void borderControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetCurrentBrushes();
        }
    }
}
