using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Generic;

namespace PosControls
{
    public class GifImage : Image
    {
        #region Licensed Access Only
        static GifImage()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(GifImage).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private GifBitmapDecoder gf;
        private Int32Animation anim;
        private bool animationIsWorking = false;
        private Uri uriSource = null;
        private Stream streamSource = null;
        private List<ImageBrush> opacityMasks = new List<ImageBrush>();


        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage ob = obj as GifImage;
            Window window = Window.GetWindow(ob);
            ob.OpacityMask = ob.opacityMasks[(int)ev.NewValue];
            ob.Source = ob.gf.Frames[(int)ev.NewValue];
            ob.InvalidateVisual();
        }

        public Stream StreamSource
        {
            get
            {
                return streamSource;
            }
            set
            {
                streamSource = value;
                gf = new GifBitmapDecoder(streamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                anim = new Int32Animation(0, gf.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, gf.Frames.Count / 10, (int)((gf.Frames.Count / 10.0 - gf.Frames.Count / 10) * 1000))));
                anim.RepeatBehavior = RepeatBehavior.Forever;
                Source = gf.Frames[0];
                SetOpacityMasksForSource();
            }
        }

        public Uri UriSource
        {
            get
            {
                return uriSource;
            }
            set
            {
                uriSource = value;
                gf = new GifBitmapDecoder(uriSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                anim = new Int32Animation(0, gf.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, gf.Frames.Count / 10, (int)((gf.Frames.Count / 10.0 - gf.Frames.Count / 10) * 1000))));
                anim.RepeatBehavior = RepeatBehavior.Forever;
                Source = gf.Frames[0];
                SetOpacityMasksForSource();
            }
        }

        public GifImage()
        {            
            //this.CacheMode = new BitmapCache();
            this.Loaded += new RoutedEventHandler(GifImage_Loaded);
        }

        private void SetOpacityMasksForSource()
        {
            opacityMasks.Clear();
            if (Source != null)
            {
                foreach (BitmapFrame frame in gf.Frames)
                {
                    BitmapSource bitmapSource = new FormatConvertedBitmap(frame, PixelFormats.Pbgra32, null, 0);
                    WriteableBitmap modifiedImage = new WriteableBitmap(bitmapSource);

                    int h = modifiedImage.PixelHeight;
                    int w = modifiedImage.PixelWidth;
                    int[] pixelData = new int[w * h];
                    int widthInByte = 4 * w;

                    modifiedImage.CopyPixels(pixelData, widthInByte, 0);

                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        if ((pixelData[i] & 0x00ffffff) == 0)
                            pixelData[i] = 0;
                    }

                    modifiedImage.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInByte, 0);
                    opacityMasks.Add(new ImageBrush(modifiedImage));
                }
                if (opacityMasks.Count > 0)
                    OpacityMask = opacityMasks[0];
            }
        }

        void GifImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible)
                StartAnimation();
            if (!ConfigurationManager.IsInDesignMode)
            {
                this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(GifImage_IsVisibleChanged);
                Window.GetWindow(this).Deactivated += new EventHandler(GifImage_Deactivated);
                Window.GetWindow(this).Activated += new EventHandler(GifImage_Activated);
            }
        }

        void GifImage_Activated(object sender, EventArgs e)
        {
            StartAnimation();
        }

        void GifImage_Deactivated(object sender, EventArgs e)
        {
            // Note: This prevents a display bug on WinXP
            StopAnimation();
        }

        void GifImage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
                StartAnimation();
            else
                StopAnimation();

        }

        private void StartAnimation()
        {
            if (!animationIsWorking)
            {
                animationIsWorking = true;
                BeginAnimation(FrameIndexProperty, anim);
            }
        }

        private void StopAnimation()
        {
            if (animationIsWorking)
            {
                animationIsWorking = false;
                BeginAnimation(FrameIndexProperty, null);
            }
        }
    }
}
