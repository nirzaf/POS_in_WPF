using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PosControls
{
    [Serializable]
    internal class BrushWrapper : ISerializable
    {
        #region Licensed Access Only
        static BrushWrapper()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BrushWrapper).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        string brushType = null;

        // SolidColorBrush
        private byte r, g, b, a;

        // DrawingBrush
        private Drawing drawingBrushDrawing = null;

        // ImageBrush
        private Uri sourceUri = null;

        // GradientBrushes
        private GradientStopCollection gradientStopCollection = null;
        private GradientSpreadMethod spreadMethod =
            GradientSpreadMethod.Pad;
        private ColorInterpolationMode interpolationMode =
            ColorInterpolationMode.SRgbLinearInterpolation;
        private double opacity = 1.0;

        // LinearGradientBrushes
        private Point startPoint = new Point(0, 0);
        private Point endPoint = new Point(1, 1);

        // RadialGradientBrushes
        private Point centerPoint = new Point(0.5, 0.5);
        private Point originPoint = new Point(0.5, 0.5);
        private double radiusX = 0.5;
        private double radiusY = 0.5;

        public BrushWrapper(Brush brush)
        {
            if (brush is SolidColorBrush)
                InitializeSolidColorBrush(brush as SolidColorBrush);
            else if (brush is LinearGradientBrush)
                InitializeLinearGradientBrush(brush as LinearGradientBrush);
            else if (brush is RadialGradientBrush)
                InitializeRadialGradientBrush(brush as RadialGradientBrush);
            else if (brush is DrawingBrush)
                InitializeDrawingBrush(brush as DrawingBrush);
            else if (brush is ImageBrush)
                InitializeImageBrush(brush as ImageBrush);
            else if (brush is VisualBrush)
                InitializeVisualBrush(brush as VisualBrush);
        }

        public BrushWrapper(RadialGradientBrush brush)
        {
            InitializeRadialGradientBrush(brush);
        }

        public BrushWrapper(DrawingBrush brush)
        {
            InitializeDrawingBrush(brush);
        }

        public BrushWrapper(LinearGradientBrush linearGradientBrush)
        {
            InitializeLinearGradientBrush(linearGradientBrush);
        }

        public BrushWrapper(SolidColorBrush solidBrush)
        {
            InitializeSolidColorBrush(solidBrush);
        }

        public BrushWrapper(ImageBrush imageBrush)
        {
            InitializeImageBrush(imageBrush);
        }

        public BrushWrapper(VisualBrush visualBrush)
        {
            InitializeVisualBrush(visualBrush);
        }

        public BrushWrapper(byte a, byte r, byte g, byte b)
        {
            brushType = "SolidColorBrush";
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        private void InitializeVisualBrush(VisualBrush visualBrush)
        {
            brushType = "VisualBrush";
            this.sourceUri = new Uri(
                (visualBrush.Visual as MediaElement).Source.AbsolutePath,
                UriKind.Absolute);
        }

        private void InitializeImageBrush(ImageBrush imageBrush)
        {
            brushType = "ImageBrush";
            this.sourceUri = (imageBrush.ImageSource as BitmapImage).UriSource;
        }

        private void InitializeDrawingBrush(DrawingBrush brush)
        {
            brushType = "DrawingBrush";
            drawingBrushDrawing = brush.Drawing.Clone();
        }

        private void InitializeRadialGradientBrush(RadialGradientBrush radialGradientBrush)
        {
            brushType = "RadialGradientBrush";
            gradientStopCollection = radialGradientBrush.GradientStops.Clone();
            InitializeGradientBrush(radialGradientBrush);
            centerPoint = radialGradientBrush.Center;
            originPoint = radialGradientBrush.GradientOrigin;
            radiusX = radialGradientBrush.RadiusX;
            radiusY = radialGradientBrush.RadiusY;
        }

        private void InitializeLinearGradientBrush(LinearGradientBrush linearGradientBrush)
        {
            brushType = "LinearGradientBrush";
            gradientStopCollection = linearGradientBrush.GradientStops.Clone();
            InitializeGradientBrush(linearGradientBrush);
            startPoint = linearGradientBrush.StartPoint;
            endPoint = linearGradientBrush.EndPoint;
        }

        private void InitializeGradientBrush(GradientBrush gradientBrush)
        {
            spreadMethod = gradientBrush.SpreadMethod;
            interpolationMode = gradientBrush.ColorInterpolationMode;
            opacity = gradientBrush.Opacity;
        }

        public void InitializeSolidColorBrush(SolidColorBrush solidBrush)
        {
            brushType = "SolidColorBrush";
            r = solidBrush.Color.R;
            g = solidBrush.Color.G;
            b = solidBrush.Color.B;
            a = solidBrush.Color.A;
        }

        protected BrushWrapper(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                //throw new System.ArgumentNullException("info");
                return;
            brushType = (string)info.GetValue("Type", typeof(string));
            if (brushType == "SolidColorBrush")
            {
                r = (byte)info.GetValue("R", typeof(byte));
                g = (byte)info.GetValue("G", typeof(byte));
                b = (byte)info.GetValue("B", typeof(byte));
                a = (byte)info.GetValue("A", typeof(byte));
            }
            else if (brushType == "LinearGradientBrush")
            {
                InitializeGradientStopCollection(info);
                InitializeGradientBrushProperties(info);
                InitializeLinearGradientBrush(info);
            }
            else if (brushType == "RadialGradientBrush")
            {
                InitializeGradientStopCollection(info);
                InitializeGradientBrushProperties(info);
                InitializeRadialGradientBrush(info);
            }
            else if (brushType == "DrawingBrush")
            {
                InitializeDrawingBrush(info);
            }
            else if (brushType == "ImageBrush")
            {
                InitializeImageBrush(info);
            }
            else if (brushType == "VisualBrush")
            {
                InitializeVisualBrush(info);
            }
        }

        private void InitializeVisualBrush(SerializationInfo info)
        {
            object obj = info.GetValue("VideoSourceUri", typeof(Uri));
            if (obj != null)
                sourceUri = (Uri)obj;
        }

        private void InitializeImageBrush(SerializationInfo info)
        {
            object obj = info.GetValue("ImageSourceUri", typeof(Uri));
            if (obj != null)
                sourceUri = (Uri)obj;
        }

        private void InitializeDrawingBrush(SerializationInfo info)
        {
            throw new NotImplementedException();
        }

        private void InitializeRadialGradientBrush(SerializationInfo info)
        {
            object obj = info.GetValue("CenterPoint", typeof(Point));
            if (obj != null)
                centerPoint = (Point)obj;
            obj = info.GetValue("OriginPoint", typeof(Point));
            if (obj != null)
                originPoint = (Point)obj;
            obj = info.GetValue("RadiusX", typeof(double));
            if (obj != null)
                radiusX = (double)obj;
            obj = info.GetValue("RadiusY", typeof(double));
            if (obj != null)
                radiusY = (double)obj;
        }

        private void InitializeLinearGradientBrush(SerializationInfo info)
        {
            object obj = info.GetValue("StartPoint", typeof(Point));
            if (obj != null)
                startPoint = (Point)obj;
            obj = info.GetValue("EndPoint", typeof(Point));
            if (obj != null)
                endPoint = (Point)obj;
        }

        private void InitializeGradientBrushProperties(SerializationInfo info)
        {
            object obj = info.GetValue("SpreadMethod", typeof(int));
            if (obj != null)
                spreadMethod = (GradientSpreadMethod)obj;
            obj = info.GetValue("InterpolationMode", typeof(int));
            if (obj != null)
                interpolationMode = (ColorInterpolationMode)obj;
            obj = info.GetValue("Opacity", typeof(double));
            if (obj != null)
                opacity = (double)obj;
        }

        private void InitializeGradientStopCollection(SerializationInfo info)
        {
            gradientStopCollection = new GradientStopCollection();
            int gradientCount = (int)info.GetValue("GradientCount", typeof(int));
            for (int gradientIndex = 0; gradientIndex < gradientCount; gradientIndex++)
            {
                byte r = (byte)info.GetValue("R" + gradientIndex, typeof(byte));
                byte g = (byte)info.GetValue("G" + gradientIndex, typeof(byte));
                byte b = (byte)info.GetValue("B" + gradientIndex, typeof(byte));
                byte a = (byte)info.GetValue("A" + gradientIndex, typeof(byte));
                Color color = Color.FromArgb(a, r, g, b);
                double offset = (double)info.GetValue("Offset" + gradientIndex, typeof(double));
                gradientStopCollection.Add(new GradientStop(color, offset));
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            info.AddValue("Type", brushType);
            if (brushType == "SolidColorBrush")
            {
                info.AddValue("R", r);
                info.AddValue("G", g);
                info.AddValue("B", b);
                info.AddValue("A", a);
            }
            else if ((brushType == "LinearGradientBrush") ||
                (brushType == "RadialGradientBrush"))
            {
                info.AddValue("SpreadMethod", spreadMethod);
                info.AddValue("InterpolationMode", interpolationMode);
                info.AddValue("Opacity", opacity);
                info.AddValue("GradientCount", gradientStopCollection.Count);
                for (int i = 0; i < gradientStopCollection.Count; i++)
                {
                    info.AddValue("R" + i, gradientStopCollection[i].Color.R);
                    info.AddValue("G" + i, gradientStopCollection[i].Color.G);
                    info.AddValue("B" + i, gradientStopCollection[i].Color.B);
                    info.AddValue("A" + i, gradientStopCollection[i].Color.A);
                    info.AddValue("Offset" + i, gradientStopCollection[i].Offset);
                }
                if (brushType == "LinearGradientBrush")
                {
                    info.AddValue("StartPoint", startPoint);
                    info.AddValue("EndPoint", endPoint);
                }
                else if (brushType == "RadialGradientBrush")
                {
                    info.AddValue("CenterPoint", centerPoint);
                    info.AddValue("OriginPoint", originPoint);
                    info.AddValue("RadiusX", radiusX);
                    info.AddValue("RadiusY", radiusY);
                }
            }
            else if (brushType == "DrawingBrush")
            {
                throw new NotImplementedException();
            }
            else if (brushType == "ImageBrush")
            {
                info.AddValue("ImageSourceUri", sourceUri);
            }
            else if (brushType == "VisualBrush")
            {
                info.AddValue("VideoSourceUri", sourceUri);
            }
        }

        public Brush CreateBrush()
        {
            if (brushType == "SolidColorBrush")
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));

            if (brushType == "LinearGradientBrush")
            {
                LinearGradientBrush brush =
                    new LinearGradientBrush(gradientStopCollection);
                brush.EndPoint = endPoint;
                brush.StartPoint = startPoint;
                brush.Opacity = opacity;
                brush.ColorInterpolationMode = interpolationMode;
                brush.SpreadMethod = spreadMethod;
                return brush;
            }

            if (brushType == "RadialGradientBrush")
            {
                RadialGradientBrush brush =
                    new RadialGradientBrush(gradientStopCollection);
                brush.Center = centerPoint;
                brush.GradientOrigin = originPoint;
                brush.RadiusX = radiusX;
                brush.RadiusY = radiusY;
                brush.Opacity = opacity;
                brush.ColorInterpolationMode = interpolationMode;
                brush.SpreadMethod = spreadMethod;
                return brush;
            }

            if (brushType == "DrawingBrush")
                throw new NotImplementedException();

            if (brushType == "ImageBrush")
                return new ImageBrush(new BitmapImage(sourceUri));

            if (brushType == "VisualBrush")
            {
                MediaElement media = new MediaElement();
                media.Source = sourceUri;
                return new VisualBrush(media);
            }

            throw new NotImplementedException();
        }
    }
}
