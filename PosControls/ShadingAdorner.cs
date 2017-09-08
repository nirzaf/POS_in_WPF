using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Threading;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Adorner that disables all controls that fall under it
    /// </summary>
    public class ShadingAdorner : Adorner
    {
        #region Licensed Access Only
        static ShadingAdorner()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ShadingAdorner).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color to paint
        /// </summary>
        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color to paint
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(ShadingAdorner),
            new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#7F303030")));


        /// <summary>
        /// Gets or sets the border 
        /// </summary>
        public Pen Border
        {
            get { return (Pen)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border 
        /// </summary>
        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register("Border", typeof(Pen), typeof(ShadingAdorner),
            new UIPropertyMetadata(new Pen(Brushes.Gray, 1)));

        //the start point where to start drawing
        private static readonly Point startPoint =
            new Point(0, 0);

        /// <summary>
        /// Gets or sets the foreground to use for the text
        /// </summary>
        public Brush ForeGround
        {
            get { return (Brush)GetValue(ForeGroundProperty); }
            set { SetValue(ForeGroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground to use for the text
        /// </summary>
        public static readonly DependencyProperty ForeGroundProperty =
            DependencyProperty.Register("ForeGround", typeof(Brush), typeof(ShadingAdorner),
            new UIPropertyMetadata(Brushes.Black));

        #endregion

        /// <summary>
        /// Constructor for the adorner
        /// </summary>
        /// <param name="adornerElement">The element to be adorned</param>
        public ShadingAdorner(UIElement adornerElement)
            : base(adornerElement)
        {
        }


        /// <summary>
        /// Called to draw on screen
        /// </summary>
        /// <param name="drawingContext">The drawind context in which we can draw</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Size size = new Size(
                (DesiredSize.Width / ConfigurationManager.ProgramScale),
                (DesiredSize.Height / ConfigurationManager.ProgramScale));
            drawingContext.DrawRectangle(Color, Border, new Rect(startPoint, size));
            base.OnRender(drawingContext);
        }
    }
}
