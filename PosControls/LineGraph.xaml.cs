using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for LineGraph.xaml
    /// </summary>
    public partial class LineGraph : UserControl
    {
        private Ellipse previousEllipse = null;

        public PointCollection Points
        {
            get;
            private set;
        }

        public double? MinimumX
        {
            get;
            private set;
        }

        public double? MinimumY
        {
            get;
            private set;
        }

        public double? MaximumX
        {
            get;
            private set;
        }

        public double? MaximumY
        {
            get;
            private set;
        }

        public string AxisXTitle
        {
            get { return textBlockLabelX.Text; }
            set { textBlockLabelX.Text = value; }
        }

        public string AxisYTitle
        {
            get { return textBlockLabelY.Text; }
            set { textBlockLabelY.Text = value; }
        }

        public LineGraph()
        {
            Points = new PointCollection();
            Points.Changed += Points_Changed;
            InitializeComponent();
            Loaded += LineGraph_Loaded;
            SizeChanged += LineGraph_SizeChanged;
        }

        [Obfuscation(Exclude = true)]
        void LineGraph_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasControl.LayoutTransform = new RotateTransform(270,
                canvasControl.ActualWidth / 2, canvasControl.ActualHeight / 2);
        }
        
        [Obfuscation(Exclude = true)]
        void LineGraph_Loaded(object sender, RoutedEventArgs e)
        {
            AxisXTitle = "X Axis";
            AxisYTitle = "Y Axis";
            Points.Add(new Point(1, 1));
            Points.Add(new Point(2, 2));
            Points.Add(new Point(3, 3));
        }

        [Obfuscation(Exclude = true)]
        void Points_Changed(object sender, EventArgs e)
        {
            SetLimits();
            Refresh();
        }

        private void SetLimits()
        {
            MinimumY = MinimumX = MinimumX = MinimumY = null;
            foreach (Point point in Points)
            {
                if (MinimumX == null)
                {
                    MinimumX = point.X;
                    MaximumX = point.X;
                }
                else if (point.X > MaximumX)
                    MaximumX = point.X;
                else if (point.X < MinimumX)
                    MinimumX = point.X;

                if (MinimumY == null)
                {
                    MinimumY = point.Y;
                    MaximumY = point.Y;
                }
                else if (point.Y > MaximumY)
                    MaximumY = point.Y;
                else if (point.Y < MinimumY)
                    MinimumY = point.Y;
            }
        }

        public void Refresh()
        {
            DrawVerticalTextBlocks();
            DrawHorizontalTextBlocks();
            PlotPoints();
        }

        private void DrawVerticalTextBlocks()
        {
            int lines = 7;
            verticalStackPanel.Children.Clear();
            for (int i = lines - 1; i >= 0; i--)
            {
                double range = MaximumY.Value - MinimumY.Value;
                double value = MinimumY.Value + (range * (i / (double)(lines - 1)));
                verticalStackPanel.Children.Add(
                    CreateVerticalTextBlock(value.ToString("F2")));
            }
        }

        private void DrawHorizontalTextBlocks()
        {
            int lines = 7;
            horizontalStackPanel.Children.Clear();
            for (int i = 0; i < lines; i++)
            {
                double range = MaximumX.Value - MinimumX.Value;
                double value = MinimumX.Value + (range * (i / (double)(lines - 1)));
                horizontalStackPanel.Children.Add(
                    CreateHorizontalTextBlock(value.ToString("F2")));
            }
        }

        private TextBlock CreateVerticalTextBlock(string text)
        {
            TextBlock result = new TextBlock();
            result.HorizontalAlignment = HorizontalAlignment.Right;
            result.Margin = new Thickness(4);
            result.Text = text;
            return result;
        }

        private TextBlock CreateHorizontalTextBlock(string text)
        {
            TextBlock result = CreateVerticalTextBlock(text);
            result.LayoutTransform = new RotateTransform(90);
            return result;
        }


        private void PlotPoints()
        {
            canvasControl.Children.Clear();
            foreach (Point point in Points)
            {
                canvasControl.Children.Add(CreateEllipse(point));
            }
        }

        private Ellipse CreateEllipse(Point point)
        {
            Ellipse result = new Ellipse();
            Canvas.SetLeft(result, ((point.X - 1) * 70) + 8);
            Canvas.SetRight(result, ((point.X + 1) * 70) + 8);
            Canvas.SetTop(result, ((point.Y - 1) * 70) + 6);
            Canvas.SetBottom(result, ((point.Y + 1) * 70) + 6);
            result.Fill = Brushes.White;
            result.Height = 2;
            result.Width = 2;
            result.ToolTip = new ToolTip();
            ToolTip toolTip = (result.ToolTip as ToolTip);
            toolTip.Content = new TextBlock();
            (toolTip.Content as TextBlock).Text = 
                "(" + point.X + ", " + point.Y + ")";
            toolTip.PlacementTarget = result;
            result.MouseEnter += new MouseEventHandler(result_MouseEnter);
            return result;
        }

        [Obfuscation(Exclude = true)]
        void result_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            if (previousEllipse != null)
                CloseToolTip(previousEllipse);
            previousEllipse = ellipse;
            OpenToolTip(ellipse);
        }

        private void CloseToolTip(Ellipse ellipse)
        {
            ToolTip toolTip = (ellipse.ToolTip as ToolTip);
            if (toolTip.IsOpen)
            {
                toolTip.IsOpen = false;
                DispatcherTimer timer = toolTip.Tag as DispatcherTimer;
                timer.IsEnabled = false;
                toolTip.Tag = null;
                ellipse.Tag = null;
                timer.Tag = null;
            }
        }

        private void OpenToolTip(Ellipse ellipse)
        {
            ToolTip toolTip = (ellipse.ToolTip as ToolTip);
            if (!toolTip.IsOpen)
            {
                toolTip.IsOpen = true;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tag = ellipse;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = new TimeSpan(0, 0, 7);
                timer.IsEnabled = true;
                toolTip.Tag = timer;
            }
        }

        [Obfuscation(Exclude = true)]
        void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (sender as DispatcherTimer);
            CloseToolTip(timer.Tag as Ellipse);
        }
    }
}
