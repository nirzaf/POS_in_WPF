using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Input;
using PosControls.Interfaces;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Reflection;

namespace PosControls
{
    public class DragScrollListBox : DragScrollViewer
    {
        #region Licensed Access Only
        static DragScrollListBox()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DragScrollListBox).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        #region Fields
        private bool haltSelectionHandling = true;
        private UIElement selectedItem = null;
        private int selectedIndex = -1;
        private SelectionMode selectionMode = SelectionMode.Single;
        private IList selectedItems = new List<object>();
        private StackPanel stackPanel = new StackPanel();
        //private ItemsControl itemsControl = new ItemsControl();
        #endregion

        #region Properties
        #region ScrollOffset DependencyProperty
        public static readonly DependencyProperty DragScrollListBoxScrollOffsetProperty =
            DependencyProperty.Register(
            "DragScrollListBoxScrollOffsetProperty", typeof(double), typeof(DragScrollListBox),
            new UIPropertyMetadata(0.0,
                new PropertyChangedCallback(ScrollOffsetValueChanged)));

        /// <summary>
        /// The scrollViewer's 
        /// </summary>
        public override double ScrollOffset
        {
            get { return (double)GetValue(DragScrollListBoxScrollOffsetProperty); }
            set { SetValue(DragScrollListBoxScrollOffsetProperty, value); }
        }
        #endregion

        public UIElementCollection Items
        {
            get { return stackPanel.Children; }
        }

        public SelectionMode SelectionMode
        {
            get { return selectionMode; }
            set
            {
                selectionMode = value;
            }
        }

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent("DragScrollListBox.SelectionChanged",
            RoutingStrategy.Bubble,
            typeof(SelectionChangedEventHandler), typeof(DragScrollListBox));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public UIElement SelectedItem
        {
            get
            {
                if (SelectionMode == SelectionMode.Multiple)
                    throw new Exception("Use SelectedItems for multiple selection mode");
                return selectedItem;
            }
            set
            {
                if (SelectionMode == SelectionMode.Multiple)
                    throw new Exception("Use SelectedItems for multiple selection mode");
                selectedItem = value;
                foreach (ISelectable item in Items)
                {
                    item.IsSelected = (item == selectedItem);
                }
            }
        }

        public IList SelectedItems
        {
            get {
                if (SelectionMode != SelectionMode.Multiple)
                    throw new Exception("Use SelectedItem when not using multiple selection mode");
                return selectedItems;
            }
        }

        public int SelectedIndex
        {
            get
            {
                if (SelectedItem == null)
                    return -1;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (SelectedItem == Items[i])
                        return i;
                }
                return -1;
            }
        }
        #endregion

        #region Constructor
        public DragScrollListBox()
        {
            InitializeListBox();
            InitializeSelf();
        }
        #endregion

        #region Initialization
        private void InitializeSelf()
        {
            base.ProcessSelection += new EventHandler(baseClass_ProcessSelection);
            ScrollViewer.MouseLeave += new System.Windows.Input.MouseEventHandler(this.scrollViewer_MouseLeave);
            ScrollViewer.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.ScrollViewer_PreviewMouseUp);
            ScrollViewer.MouseEnter += new System.Windows.Input.MouseEventHandler(this.scrollViewer_MouseEnter);
            //stackPanel.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listBoxContent_SelectionChanged);
            //Items.CurrentChanged += new EventHandler(Items_CurrentChanged);
            //Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
            //Items.
            //stackPanel.
            scrollViewer.SizeChanged += new SizeChangedEventHandler(scrollViewer_SizeChanged);

            if (Mouse.LeftButton == MouseButtonState.Released)
                haltSelectionHandling = false;
        }

        private void InitializeListBox()
        {
            stackPanel.Margin = new Thickness(5);
            stackPanel.Width = Double.NaN;
            stackPanel.Height = Double.NaN;
            stackPanel.Background = Brushes.Transparent;
            stackPanel.Focusable = false;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
            ScrollViewer.SetHorizontalScrollBarVisibility(stackPanel, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(stackPanel, ScrollBarVisibility.Disabled);
            ScrollContent = stackPanel;
            Items.Clear();
        }
        #endregion

        public void ClearItems()
        {
            Items.Clear();
            EndAnimation();
            isButtonDown = false;
            ScrollOffset = 0;
        }

        #region Event Handling
        // DEBUG: Not being handled
        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Will occur if items is cleared
            if (Items.Count == 0)
                EndAnimation();
        }

        // If sized changed, items where added or removed
        void scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Only deal with removed items
            if (e.NewSize.Height >= e.PreviousSize.Height)
                return;

            haltSelectionHandling = true;
            IList addedItems = new ArrayList();
            IList removedItems = new ArrayList();

            // Update SelectedItem and SelectedItems
            if (SelectionMode == System.Windows.Controls.SelectionMode.Single)
            {
                if ((SelectedItem != null) && !stackPanel.Children.Contains(SelectedItem))
                {
                    UIElement selected = SelectedItem;
                    SelectedItem = null;
                    removedItems.Add(SelectedItem);
                }
            }
            else if (SelectionMode == System.Windows.Controls.SelectionMode.Multiple)
            {
                foreach (UIElement item in SelectedItems)
                {
                    if (!stackPanel.Children.Contains(item))
                    {
                        removedItems.Add(SelectedItem);                        
                    }
                }
                foreach (UIElement item in removedItems)
                {
                    SelectedItems.Remove(item);
                }
            }

            if (removedItems.Count > 0)
            {
                SelectionChangedEventArgs newEventArgs =
                    new SelectionChangedEventArgs(DragScrollListBox.SelectionChangedEvent,
                        removedItems, addedItems);
                RaiseEvent(newEventArgs);
            }
            haltSelectionHandling = false;
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            haltSelectionHandling = false;
        }

        private void scrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            haltSelectionHandling = false;
        }

        private void scrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                haltSelectionHandling = true;
        }

        private void baseClass_ProcessSelection(object sender, EventArgs e)
        {
            //if ((mouseLocation == null) || !mouseLocation.HasValue)
            Point mouseLocation = Mouse.GetPosition(scrollViewer);

            object element = GetElementFromPoint(mouseLocation);
            if (element != null)
                ProcessSelection(element as UIElement);

        }

        private new void ProcessSelection(UIElement element)
        {
            IList addedItems = new ArrayList();
            IList removedItems = new ArrayList();
            haltSelectionHandling = true;
            if (SelectionMode == SelectionMode.Multiple)
            {
                if ((SelectedItems != null) &&
                    (SelectedItems.Contains(element)))
                {
                    removedItems.Add(element);
                    SelectedItems.Remove(element);
                }
                else
                {
                    addedItems.Add(element);
                    SelectedItems.Add(element);
                }
            }
            else if (SelectionMode == SelectionMode.Single)
            {
                if (SelectedItem != null)
                    removedItems.Add(SelectedItem);
                addedItems.Add(element);
                SelectedItem = element;
            }
            if (element is ISelectable)
            {
                AlterSelection(element as ISelectable);
            }
            haltSelectionHandling = false;

            SelectionChangedEventArgs newEventArgs = new SelectionChangedEventArgs(DragScrollListBox.SelectionChangedEvent, removedItems, addedItems);
            RaiseEvent(newEventArgs);
        }

        private void AlterSelection(ISelectable entry)
        {
            if (SelectionMode == SelectionMode.Multiple)
            {
                if (entry.IsSelectable)
                    entry.IsSelected = !entry.IsSelected;
            }
            else if ((SelectionMode == SelectionMode.Single) &&
                     (Items != null))
            {
                SelectedItem = (UIElement)entry;
            }
        }

        private object GetElementFromPoint(Point point)
        {
            HitTestResult elementobj = VisualTreeHelper.HitTest(scrollViewer, point);
            if (elementobj == null)
                return null;

            UIElement element = elementobj.VisualHit as UIElement;
            if (element != null)
            {
                while (true)
                {
                    /*
                    if ((element == stackPanel) ||
                        (element == borderControl) ||
                        (element == borderGridControl) ||
                        (element == grid) ||
                        (element == canvasControl))
                    {
                        return null;
                    }
                    //object element = this.ItemContainerGenerator.ItemFromContainer(element);
                    */
                    object item = element;
                    if (item != null)
                    {
                        bool itemFound = !(item.Equals(DependencyProperty.UnsetValue));
                        if (itemFound)
                        {
                            // Debug
                            //if ((element is FormattedListBoxItem) || (element is TicketItemTemplate))
                            if (item is ISelectable)
                                return item;
                        }
                        element = (UIElement)VisualTreeHelper.GetParent(element);
                    }
                    else
                        return null;
                }
            }
            return null;
        }
        #endregion

        #region Animations
        protected override void StartAnimation(Duration spanDuration)
        {
            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = ScrollOffset;
            if (isDeclining)
                scrollAnimation.To = 0;
            else
                scrollAnimation.To = scrollViewer.ScrollableHeight;
            scrollAnimation.DecelerationRatio = .9;
            scrollAnimation.SpeedRatio = .5;
            scrollAnimation.Duration = spanDuration;
            this.BeginAnimation(DragScrollListBoxScrollOffsetProperty, scrollAnimation);
            isAnimating = true;
        }

        protected override void EndAnimation()
        {
            if (isAnimating)
            {
                this.BeginAnimation(DragScrollListBoxScrollOffsetProperty, null);
                isAnimating = false;
            }
        }
        #endregion

        #region Public Helper Methods
        public void ScrollIntoView(object element)
        {
            if ((element == null || !(element is Control)))
                return;
            Control control = element as Control;
            if (!stackPanel.Children.Contains(control))
                return;

            EndAnimation();
            stackPanel.UpdateLayout();

            Vector controlTop = VisualTreeHelper.GetOffset(control);
            double bottomOffset = controlTop.Y + control.ActualHeight;
            double offsetAtBottomOfView = ScrollOffset + scrollViewer.ViewportHeight;
            if (controlTop.Y < ScrollOffset)
                ScrollOffset = controlTop.Y;
            else if (bottomOffset > offsetAtBottomOfView)
                ScrollOffset = bottomOffset - scrollViewer.ViewportHeight;
        }

        public new void ScrollToEnd()
        {
            stackPanel.UpdateLayout();
            ScrollOffset = scrollViewer.ScrollableHeight;
        }        
        #endregion

    }
}
