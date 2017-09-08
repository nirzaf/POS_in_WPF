using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PosControls.Helpers;

namespace PosControls
{
    public class TimeEntryClockControl : Canvas
    {
        #region Licensed Access Only
        static TimeEntryClockControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TimeEntryClockControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private TimeEntryInternalClockControl clockControl =
            new TimeEntryInternalClockControl();

        public event EventHandler TimeChanged
        {
            add { clockControl.TimeChanged += value; }
            remove { clockControl.TimeChanged -= value; }
        }

        public bool UseMilitaryFormat
        {
            get { return clockControl.UseMilitaryFormat; }
            set { clockControl.UseMilitaryFormat = value; }
        }

        public TimeSpan Time
        {
            get { return clockControl.Time; }
            set { clockControl.Time = value; }
        }

        public bool IsAM
        {
            get { return clockControl.IsAM; }
            set { clockControl.IsAM = value; }
        }

        public bool IsPM
        {
            get { return clockControl.IsPM; }
            set { clockControl.IsPM = value; }
        }

        public int Hour
        {
            get { return Time.Hours; }
        }

        public int Minute
        {
            get { return Time.Minutes; }
        }

        public TimeEntryClockControl()
            : base()
        {
            this.Height = 200;
            this.Width = 200;

            this.VerticalAlignment = VerticalAlignment.Center;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            InitializeClock();
        }

        private void InitializeClock()
        {
            clockControl.Time = TimeSpan.Zero;
            Canvas.SetTop(clockControl, 0);
            Canvas.SetLeft(clockControl, 0);
            Canvas.SetBottom(clockControl, 200);
            Canvas.SetRight(clockControl, 200);
            this.Children.Add(clockControl);
        }

        private enum ClockControlFormat
        {
            Hours12,
            Hours24,
            Minutes,
        }

        private class TimeEntryInternalClockControl : Canvas
        {
            #region Variables
            private bool haltEventHandling = true;
            private bool useMilitaryFormat = false;
            private ClockControlFormat format;
            private bool modifyingMinutes = false;
            private TimeSpan time;
            private double minuteHandDegrees = 270;
            private double hourHandDegrees = 270;
            private MouseEventArgs lastMouseEvent = null;
            private MouseButtonEventArgs lastMouseButtonEvent = null;
            private Point center = new Point(100, 100);

            private Ellipse clockBorder = new Ellipse();
            private Label[] ticks = new Label[12];
            // the hand
            private Ellipse centerPoint = new Ellipse();
            private Line hourHandLine = new Line();
            private Polygon hourHandPoly = new Polygon();
            private Line minuteHandLine = new Line();
            private Polygon minuteHandPoly = new Polygon();
            private Label backGrid = new Label();
            #endregion

            #region Events
            public event EventHandler TimeChanged = null;
            #endregion

            #region Properties
            public bool IsAM
            {
                get { return (Time.Hours < 12); }
                set
                {
                    if (value && Time.Hours >= 12)
                        Time = new TimeSpan(Time.Hours - 12, Time.Minutes, Time.Seconds);
                    else if (!value && Time.Hours < 12)
                        Time = new TimeSpan(Time.Hours + 12, Time.Minutes, Time.Seconds);
                }
            }

            public bool IsPM
            {
                get { return (Time.Hours >= 12); }
                set
                {
                    if (value && Time.Hours < 12)
                        Time = new TimeSpan(Time.Hours + 12, Time.Minutes, Time.Seconds);
                    else if (!value && Time.Hours >= 12)
                        Time = new TimeSpan(Time.Hours - 12, Time.Minutes, Time.Seconds);
                }
            }

            public bool UseMilitaryFormat
            {
                get { return useMilitaryFormat; }
                set
                {
                    useMilitaryFormat = value;
                    FormatHands();
                }

            }

            public TimeSpan Time
            {
                get { return time; }
                set
                {
                    time = value;
                    int hourTickSize = (UseMilitaryFormat ? 15 : 30);
                    hourHandDegrees = MathHelper.ToStandardAngle(
                        270 + (time.Hours * hourTickSize));
                    minuteHandDegrees = MathHelper.ToStandardAngle(
                        270 + (time.Minutes * 6));
                    int offset = (int)(hourTickSize * value.Minutes / 60f);
                    InitializeMinuteHand(minuteHandDegrees);
                    InitializeHourHand(hourHandDegrees + offset);
                    FormatHands();
                    if (TimeChanged != null)
                        TimeChanged.Invoke(this, new EventArgs());
                }
            }

            public ClockControlFormat Format
            {
                get { return format; }
                set
                {
                    format = value;
                    if (value == ClockControlFormat.Hours12)
                    {
                        for (int i = 0; i < ticks.Length; i++)
                        {
                            ticks[i].Content = "" + ((i == 0) ? 12 : i);
                        }
                    }
                    else if (value == ClockControlFormat.Hours24)
                    {
                        for (int i = 0; i < ticks.Length; i++)
                        {
                            ticks[i].Content = "" + (i * 2);
                        }
                    }
                    else if (value == ClockControlFormat.Minutes)
                    {
                        for (int i = 0; i < ticks.Length; i++)
                        {
                            ticks[i].Content = "" + (i * 5);
                        }
                    }
                }
            }
            #endregion

            #region Constructor
            public TimeEntryInternalClockControl()
                : base()
            {
                this.Width = 200;
                this.Height = 200;
                // Create the labelDisplayTime array objects
                for (int i = 0; i < ticks.Length; i++)
                {
                    ticks[i] = new Label();
                }
                InitializeGrid();
                InitializeClock();
                InitializeLabels();
                InitializeMinuteHand(minuteHandDegrees);
                InitializeHourHand(hourHandDegrees);
                InitializeCenterPoint();
                FormatHands();
                if (Mouse.LeftButton == MouseButtonState.Released)
                    haltEventHandling = false;
            }
            #endregion

            #region Initialization
            private void InitializeGrid()
            {
                backGrid.Width = 200;
                backGrid.Height = 200;
                if (!this.Children.Contains(backGrid))
                    this.Children.Add(backGrid);
                backGrid.PreviewMouseUp +=
                    new MouseButtonEventHandler(backGrid_PreviewMouseUp);
                backGrid.PreviewMouseDown +=
                    new MouseButtonEventHandler(backGrid_PreviewMouseDown);
                backGrid.PreviewMouseMove +=
                    new MouseEventHandler(backGrid_PreviewMouseMove);
                backGrid.MouseEnter += new MouseEventHandler(backGrid_MouseEnter);
                backGrid.MouseLeave += new MouseEventHandler(backGrid_MouseLeave);
            }

            private void InitializeCenterPoint()
            {
                // Base                
                if (!this.Children.Contains(centerPoint))
                {
                    centerPoint.Stroke = Brushes.White;
                    centerPoint.Fill = Brushes.White;
                    centerPoint.Width = 8;
                    centerPoint.Height = 8;
                    this.Children.Add(centerPoint);
                    Canvas.SetLeft(centerPoint, 96);
                    Canvas.SetRight(centerPoint, 104);
                    Canvas.SetTop(centerPoint, 96);
                    Canvas.SetBottom(centerPoint, 104);
                }
            }

            private void InitializeMinuteHand(double degrees)
            {
                double diameter = 70;
                Point pt = MathHelper.FindPointOnCircle(center, diameter, degrees);
                //minuteHandLine.Stroke = Brushes.White;
                minuteHandLine.StrokeThickness = 1;
                minuteHandLine.X1 = pt.X;
                minuteHandLine.Y1 = pt.Y;
                minuteHandLine.X2 = 100;
                minuteHandLine.Y2 = 100;
                if (!this.Children.Contains(minuteHandLine))
                    this.Children.Add(minuteHandLine);

                // Pointer
                diameter = 60;
                Point baseCenterPt = MathHelper.FindPointOnCircle(center, diameter, degrees);
                Point polyPt1 = MathHelper.FindPointOnCircle(baseCenterPt, 4, degrees + 90);
                Point polyPt2 = MathHelper.FindPointOnCircle(baseCenterPt, 4, degrees - 90);
                //minuteHandPoly.Stroke = Brushes.White;
                //minuteHandPoly.Fill = Brushes.White;
                minuteHandPoly.Points.Clear();
                minuteHandPoly.Points.Add(pt);
                minuteHandPoly.Points.Add(polyPt1);
                minuteHandPoly.Points.Add(polyPt2);
                if (!this.Children.Contains(minuteHandPoly))
                    this.Children.Add(minuteHandPoly);
            }

            private void InitializeHourHand(double degrees)
            {
                // Line
                double diameter = 60;
                Point center = new Point(100, 100);
                Point pt = MathHelper.FindPointOnCircle(center, diameter, degrees);
                //hourHandLine.Stroke = Brushes.White;
                hourHandLine.StrokeThickness = 2;
                hourHandLine.X1 = pt.X;
                hourHandLine.Y1 = pt.Y;
                hourHandLine.X2 = 100;
                hourHandLine.Y2 = 100;
                if (!this.Children.Contains(hourHandLine))
                    this.Children.Add(hourHandLine);

                // Pointer
                diameter = 50;
                Point baseCenterPt = MathHelper.FindPointOnCircle(center, diameter, degrees);
                Point polyPt1 = MathHelper.FindPointOnCircle(baseCenterPt, 6, degrees + 90);
                Point polyPt2 = MathHelper.FindPointOnCircle(baseCenterPt, 6, degrees - 90);
                //hourHandPoly.Stroke = Brushes.White;
                //hourHandPoly.Fill = Brushes.White;
                hourHandPoly.Points.Clear();
                hourHandPoly.Points.Add(pt);
                hourHandPoly.Points.Add(polyPt1);
                hourHandPoly.Points.Add(polyPt2);
                if (!this.Children.Contains(hourHandPoly))
                    this.Children.Add(hourHandPoly);
            }

            private void InitializeLabels()
            {
                double degrees = 270;
                double diameter = 85;
                Point center = new Point(100, 100);
                Color defaultForegroundColor = ((SolidColorBrush)ConfigurationManager.LabelForegroundBrush).Color;
                byte colorByte = (byte) (((int)defaultForegroundColor.R + defaultForegroundColor.G +
                    defaultForegroundColor.B) / 3 * 0.87);
                for (int i = 0; i < ticks.Length; i++)
                {

                    ticks[i].FontWeight = FontWeights.Normal;
                    ticks[i].FontSize = 14;
                    ticks[i].Content = "" + i;
                    ticks[i].Height = 32;
                    ticks[i].Width = 32;
                    ticks[i].VerticalAlignment = VerticalAlignment.Center;
                    ticks[i].HorizontalAlignment = HorizontalAlignment.Center;
                    ticks[i].VerticalContentAlignment = VerticalAlignment.Center;
                    ticks[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                    //ticks[i].Foreground = ConfigurationManager.LabelForegroundBrush;
                    ticks[i].Foreground = new SolidColorBrush(Color.FromArgb(0xff, colorByte, colorByte, colorByte));
                    Point pt = MathHelper.FindPointOnCircle(center, diameter, degrees);
                    Canvas.SetTop(ticks[i], pt.Y - 16);
                    Canvas.SetLeft(ticks[i], pt.X - 16);
                    degrees += 30;
                    this.Children.Add(ticks[i]);
                    degrees = MathHelper.ToStandardAngle(degrees);
                }
            }

            private void InitializeClock()
            {
                clockBorder.Stroke = Brushes.White;
                clockBorder.StrokeThickness = 2;
                clockBorder.Width = 200;
                clockBorder.Height = 200;
                this.Children.Add(clockBorder);
            }
            #endregion

            #region MouseMove Handling
            protected override void OnPreviewMouseMove(MouseEventArgs e)
            {
                base.OnPreviewMouseMove(e);
                // One-shot Check
                if (lastMouseEvent == e)
                    return;
                lastMouseEvent = e;
                if (haltEventHandling)
                    return;
                HandleMouseMove(e);
            }

            void backGrid_PreviewMouseMove(object sender, MouseEventArgs e)
            {
                // One-shot Check
                if (lastMouseEvent == e)
                    return;
                lastMouseEvent = e;
                if (haltEventHandling)
                    return;
                HandleMouseMove(e);
            }

            private void HandleMouseMove(MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    bool isPM = IsPM;
                    double clickAngle = MathHelper.ToStandardAngle(
                        MathHelper.RadiansToDegrees(
                        MathHelper.FindAngleToPoint(center,
                        e.GetPosition(this))) + 90);
                    if (!modifyingMinutes)
                    {
                        if (UseMilitaryFormat)
                        {
                            double angle = MathHelper.ToStandardAngle(clickAngle + 7.5);
                            int selectedHour = (int)(angle / 15);
                            if (Time.Hours != selectedHour)
                                Time = new TimeSpan(selectedHour, Time.Minutes,
                                    Time.Seconds);
                        }
                        else
                        {
                            double angle = MathHelper.ToStandardAngle(clickAngle + 15);
                            int selectedHour = (int)(angle / 30);
                            if (Time.Hours != selectedHour)
                                Time = new TimeSpan(selectedHour + (isPM ? 12 : 0),
                                    Time.Minutes, Time.Seconds);
                        }
                    }
                    else
                    {
                        double angle = MathHelper.ToStandardAngle(clickAngle + 3);
                        int selectedMinute = (int)(angle / 6);
                        if (Time.Minutes != selectedMinute)
                            Time = new TimeSpan(Time.Hours, selectedMinute, Time.Seconds);
                    }
                }
            }
            #endregion

            #region MouseDown Handling
            protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
            {
                base.OnPreviewMouseDown(e);
                // One-shot Check
                if (lastMouseButtonEvent == e)
                    return;
                lastMouseButtonEvent = e;
                if (haltEventHandling)
                    return;
                HandleMouseDown(e);
            }

            void backGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
                // One-shot Check
                if (lastMouseButtonEvent == e)
                    return;
                lastMouseButtonEvent = e;
                if (haltEventHandling)
                    return;
                HandleMouseDown(e);
            }

            private void HandleMouseDown(MouseButtonEventArgs e)
            {
                Point clickPoint = e.GetPosition(this);
                if (MathHelper.FindDistanceBetweenPoints(center, clickPoint) < 25)
                {
                    // Toggle
                    modifyingMinutes = !modifyingMinutes;

                    // Alter how the hands look and the labels
                    FormatHands();

                    // Do not change the Time
                    return;
                }
                
                // Update the time for the new click
                HandleMouseMove(e);
            }

            private void FormatHands()
            {
                if (modifyingMinutes)
                {
                    Format = ClockControlFormat.Minutes;
                    hourHandLine.Stroke = hourHandLine.Fill =
                        hourHandPoly.Stroke = hourHandPoly.Fill = Brushes.DarkGray;
                    minuteHandLine.Stroke = minuteHandLine.Fill =
                        minuteHandPoly.Stroke = minuteHandPoly.Fill = Brushes.White;
                }
                else
                {
                    Format = (UseMilitaryFormat ?
                        ClockControlFormat.Hours24 :
                        ClockControlFormat.Hours12);
                    hourHandLine.Stroke = hourHandLine.Fill =
                        hourHandPoly.Stroke = hourHandPoly.Fill = Brushes.White;
                    minuteHandLine.Stroke = minuteHandLine.Fill =
                        minuteHandPoly.Stroke = minuteHandPoly.Fill = Brushes.DarkGray;
                }
            }
            #endregion

            #region MouseUp Handling
            void backGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
            {
                haltEventHandling = false;
            }
            #endregion

            #region MouseLeave Handling
            void backGrid_MouseLeave(object sender, MouseEventArgs e)
            {
                haltEventHandling = false;
            }
            #endregion

            #region MouseEnter Handling
            void backGrid_MouseEnter(object sender, MouseEventArgs e)
            {
                //haltEventHandling = (Mouse.LeftButton == MouseButtonState.Pressed);
            }
            #endregion

        }
    }
}
