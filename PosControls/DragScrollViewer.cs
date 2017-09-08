using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Scrolls the stringValue of ScrollContent using droid/iPhone-like scrolling
    /// </summary>
    public class DragScrollViewer : ContentControl
    {
        #region Licensed Access Only
        static DragScrollViewer()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DragScrollViewer).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        #region Events
        /// <summary>
        /// This event is for the subclass DragScrollListBox
        /// </summary>
        protected event EventHandler ProcessSelection;
        #endregion

        #region Fields
        // Mouse motion variables
        private double initialVertialOffset;
        private double stoppedVertialOffset;
        protected bool isButtonDown = false;
        protected bool isAnimating = false;

        // Movement profile variables
        protected bool isDeclining;
        private double strokeLength;
        private DateTime? timeStart = null;
        private DateTime? timeApex = null;
        private Point? MouseMoveApogy = null;
        private Point? MouseMoveParogy = null;
        private Point? MouseMoveStart = null;
        private Point? MouseMovePrevious = null;

        // Controls
        protected Grid grid = new Grid();
        protected ScrollViewer scrollViewer = new ScrollViewer();
        protected Border borderControl = new Border();
        protected Grid borderGridControl = new Grid();
        protected Border maskBorder = new Border();
        protected Canvas canvasControl = new Canvas();
        #endregion

        #region Properties
        /// <summary>
        /// The content to be scrolled
        /// </summary>
        public object ScrollContent
        {
            get { return scrollViewer.Content; }
            set { scrollViewer.Content = value; }
        }

        protected ScrollViewer ScrollViewer
        {
            get { return scrollViewer; }
            private set { scrollViewer = value; }
        }

        public double ScrollableHeight
        {
            get { return scrollViewer.ScrollableHeight; }
        }

        public void ScrollToEnd()
        {
            ScrollOffset = ScrollableHeight;
        }

        #region ScrollOffset DependencyProperty
        public static readonly DependencyProperty DragScrollViewerScrollOffsetProperty =
            DependencyProperty.Register(
            "DragScrollViewerScrollOffsetProperty", typeof(double), typeof(DragScrollViewer),
            new UIPropertyMetadata(0.0, DragScrollViewer.ScrollOffsetValueChanged));

        /// <summary>
        /// The scrollViewer's 
        /// </summary>
        public virtual double ScrollOffset
        {
            get { return (double)GetValue(DragScrollViewerScrollOffsetProperty); }
            set { SetValue(DragScrollViewerScrollOffsetProperty, value); }
        }

        private double[] previousValues = { 0, 0, 0 };
        private int previousValuesIndex = 0;
        protected static object ScrollOffsetCoerceValue(DependencyObject d, object obj)
        {
            DragScrollViewer myClass = (DragScrollViewer)d;
            double newOffset = (double)obj;
            
            // Sampling Index, Loop
            if (myClass.previousValuesIndex == 3)
                myClass.previousValuesIndex = 0;

            // ((oldOffset != myClass.scrollViewer.VerticalOffset)

            // Restore Previous Value
            if (Double.IsInfinity(newOffset) || Double.IsNaN(newOffset) ||
                !PassesFixTest(myClass.previousValues, myClass.previousValuesIndex, newOffset))
            {
                if (myClass.isAnimating)
                    myClass.EndAnimation();
                return myClass.previousValues[myClass.previousValuesIndex++];
            }

            // Limit Minimum
            if (newOffset < 0)
            {
                if (myClass.isAnimating)
                    myClass.EndAnimation();
                return (myClass.previousValues[myClass.previousValuesIndex++] = 0);
            }
            
            // Limit Maximum
            if (newOffset > myClass.scrollViewer.ScrollableHeight)
            {
                if (myClass.isAnimating)
                    myClass.EndAnimation();
                return (myClass.previousValues[myClass.previousValuesIndex++] = myClass.scrollViewer.ScrollableHeight);
            }

            // Unchanged
            return (myClass.previousValues[myClass.previousValuesIndex++] = (double)obj);
        }

        private static bool PassesFixTest(double[] previousValues, int samplingIndex, double newOffset)
        {
            if (newOffset != 0)
                return true;

            double lastSample = previousValues[samplingIndex];
            if (lastSample > 15)
                return false;

            return true;
        }

        protected static void ScrollOffsetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragScrollViewer myClass = (DragScrollViewer)d;
            double newOffset = (double)e.NewValue;
            double oldOffset = (double)e.OldValue;
            myClass.scrollViewer.ScrollToVerticalOffset(newOffset);
        }
        #endregion

        #endregion

        #region Constructor
        public DragScrollViewer()
        {
            // Initialize this control's content
            InitializeGrid();
            InitializeScrollViewer();
            InitializeScrollIndicator();
            base.Content = grid;

            // Initialize initialVertialOffset
            initialVertialOffset = ScrollOffset;
            //scrollViewer.ScrollToVerticalOffset(ScrollOffset);

            // Update the marker
            UpdateViewPositionMarker();

        }
        #endregion

        #region Initialization
        private void InitializeGrid()
        {
            ColumnDefinition content = new ColumnDefinition();
            ColumnDefinition scroll = new ColumnDefinition();
            content.Width = new GridLength(280, GridUnitType.Star);
            scroll.Width = new GridLength(12, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(content);
            grid.ColumnDefinitions.Add(scroll);
        }

        private void InitializeScrollViewer()
        {
            scrollViewer.Width = Double.NaN;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            scrollViewer.BorderThickness = new Thickness(0, 0, 0, 0);
            scrollViewer.PreviewMouseDown +=
                new MouseButtonEventHandler(scrollViewer_PreviewMouseDown);
            scrollViewer.PreviewMouseUp +=
                new MouseButtonEventHandler(scrollViewer_PreviewMouseUp);
            scrollViewer.MouseMove +=
                new MouseEventHandler(scrollViewer_MouseMove);
            scrollViewer.MouseLeave +=
                new MouseEventHandler(scrollViewer_MouseLeave);
            scrollViewer.ScrollChanged +=
                new ScrollChangedEventHandler(scrollViewer_ScrollChanged);

            grid.Children.Add(scrollViewer);
        }

        private void InitializeScrollIndicator()
        {
            // borderControl
            borderControl.Name = "borderControl";
            borderControl.CornerRadius = new CornerRadius(3);
            borderControl.BorderThickness = new Thickness(1);
            borderControl.Margin = new Thickness(3);
            borderControl.BorderBrush = Brushes.Gray;
            borderControl.Background = Brushes.Transparent;

            // borderGridControl
            borderGridControl.Name = "borderGridControl";
            borderGridControl.Height = 0;
            borderGridControl.VerticalAlignment = VerticalAlignment.Top;
            borderControl.Child = borderGridControl;

            // maskBorder
            maskBorder.Name = "maskBorder";
            maskBorder.Background = Brushes.White;
            maskBorder.CornerRadius = new CornerRadius(2);
            borderGridControl.Children.Add(maskBorder);

            // canvasControl
            canvasControl.Background = Brushes.Transparent;
            canvasControl.OpacityMask = new VisualBrush(maskBorder);
            borderGridControl.Children.Add(canvasControl);

            grid.Children.Add(borderControl);
            Grid.SetColumn(borderControl, 1);
        }
        #endregion

        #region Marker Modification
        private void UpdateViewPositionMarker()
        {
            if (scrollViewer.ExtentHeight == 0)
            {
                SetMarker(0, 0);
                return;
            }

            double availableSize = borderControl.ActualHeight - 2;
            double visiblePercentage = scrollViewer.ViewportHeight / scrollViewer.ExtentHeight;
            double topRatio = ScrollOffset / scrollViewer.ExtentHeight;
            double top = availableSize * topRatio;
            double length = availableSize * visiblePercentage;
            if (visiblePercentage >= 1.0)
                SetMarker(0, availableSize);
            else
                SetMarker(top, length);
        }

        private void SetMarker(double top, double size)
        {
            double availableSize = borderControl.ActualHeight - 2;
            top = ClampDouble(top, 0, availableSize - 2);
            size = ClampDouble(size, 0, availableSize - top);
            borderGridControl.Margin = new Thickness(0, top, 0, 0);
            borderGridControl.Height = size;
        }

        public static double ClampDouble(double value, double min, double max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }
        #endregion

        #region ScrollViewer Event Handling
        private void scrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Non-interference with DragScrollListBox
            if ((ProcessSelection == null) && 
                (PointHitsDragScrollListBox(Mouse.GetPosition(this))))
            {
                isButtonDown = false;
                return;
            }
            bool wasAnimating = isAnimating;
            EndAnimation();
            isDeclining = false;
            timeStart = timeApex = DateTime.Now;
            initialVertialOffset = ScrollOffset;
            //scrollViewer.ScrollToVerticalOffset(ScrollOffset);
            MouseMovePrevious = MouseMoveApogy = MouseMoveParogy = MouseMoveStart = Mouse.GetPosition(this);
            isButtonDown = true;
        }

        private void scrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isButtonDown)
            {
                Point pt = e.GetPosition(scrollViewer);
                double offset = MouseMoveStart.Value.Y - pt.Y;

                if (MouseMovePrevious.Value.Y != pt.Y)
                {
                    bool isDecliningNow = (MouseMovePrevious.Value.Y < pt.Y);
                    if (isDeclining != isDecliningNow)
                    {
                        // a shift in brush-stroke direction, resets the apex time
                        isDeclining = isDecliningNow;
                        timeApex = DateTime.Now;

                        if (isDeclining)
                        {
                            MouseMoveParogy = pt;
                            MouseMoveApogy = MouseMovePrevious;
                        }
                        else
                        {
                            MouseMoveParogy = MouseMovePrevious;
                            MouseMoveApogy = pt;
                        }
                    }
                    else
                    {
                        if (pt.Y > MouseMoveApogy.Value.Y)
                            MouseMoveApogy = pt;
                        if (pt.Y < MouseMoveParogy.Value.Y)
                            MouseMoveParogy = pt;
                    }

                    //scrollViewer.ScrollToVerticalOffset(this.initialVertialOffset + offset);
                    ScrollOffset = this.initialVertialOffset + offset;
                }
                MouseMovePrevious = pt;
            }
        }

        private void scrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isButtonDown)
                DoButtonUp();
        }

        private void scrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isButtonDown)
                DoButtonUp();
        }

        private void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollOffset = scrollViewer.VerticalOffset;
            UpdateViewPositionMarker();
        }

        private void DoButtonUp()
        {
            isButtonDown = false;
            stoppedVertialOffset = ScrollOffset;
            //scrollViewer.ScrollToVerticalOffset(ScrollOffset);

            // No down-event, before the up-event
            if (((timeStart == null) || !timeStart.HasValue) ||
                ((timeApex == null) || !timeApex.HasValue))
                return;

            // This was a tap or delibrate scroll, not a drag-scroll
            TimeSpan determiningLength = DateTime.Now - timeStart.Value;

            // Process Selection
            if (ProcessSelection != null)
            {
                if (determiningLength.TotalMilliseconds > 1200)
                {
                    EndAnimation();
                    return;
                }
                if (initialVertialOffset == stoppedVertialOffset)
                {
                    ProcessSelection.Invoke(this, new EventArgs());
                    return;
                }
            }
            else if ((initialVertialOffset == stoppedVertialOffset) ||
                (determiningLength.TotalMilliseconds > 1200))
            {
                EndAnimation();
                return;
            }

            TimeSpan brushStrokeDurration = (DateTime.Now - timeApex.Value);
            int ms = (int)(brushStrokeDurration.TotalMilliseconds / 10);
            if (!isDeclining)
                strokeLength = (MouseMoveApogy.Value.Y - MouseMoveParogy.Value.Y) / 10;
            else
                strokeLength = (MouseMoveParogy.Value.Y - MouseMoveApogy.Value.Y) / 10;

            if ((strokeLength > -1) && (strokeLength < 0))
                strokeLength = -1;
            if ((strokeLength < 1) && (strokeLength > 0))
                strokeLength = 1;

            // Start the scrolling animation
            int durationMilliseconds = (int)Math.Abs(GetScrollDistantance() / strokeLength * ms);
            if (durationMilliseconds > 0)
            {
                Duration spanDuration =
                    new Duration(new TimeSpan(0, 0, 0, 0, durationMilliseconds));
                StartAnimation(spanDuration);
            }
        }

        private double GetScrollDistantance()
        {
            if (isDeclining)
                return ScrollOffset;
            else
                return scrollViewer.ScrollableHeight - ScrollOffset;
        }

        private bool PointHitsDragScrollListBox(Point point)
        {
            HitTestResult elementobj = VisualTreeHelper.HitTest(scrollViewer, point);
            if (elementobj == null)
                return false;

            UIElement element = elementobj.VisualHit as UIElement;
            while (element != null)
            {
                /*
                if (element is DragScrollViewer)
                {
                    // False for this instance, true for other instances
                    return (element != this);
                }
                */
                // Note: This seems it be the best conditional check, only time will tell
                if (element is DragScrollListBox)
                    return true;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return false;
        }
        #endregion

        #region Animation Handling
        protected virtual void StartAnimation(Duration spanDuration)
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
            this.BeginAnimation(DragScrollViewerScrollOffsetProperty, scrollAnimation);
            isAnimating = true;
        }

        protected virtual void EndAnimation()
        {
            if (isAnimating)
            {
                this.BeginAnimation(DragScrollViewerScrollOffsetProperty, null);
                isAnimating = false;
            }
        }
        #endregion
    }
}
