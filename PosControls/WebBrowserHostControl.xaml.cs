using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for WebBrowserHostControl.xaml
    /// </summary>
    public partial class WebBrowserHostControl : System.Windows.Controls.UserControl
    {
        #region Licensed Access Only
        static WebBrowserHostControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(WebBrowserHostControl).Assembly.GetName().GetPublicKeyToken(),
                Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private Form form;
        private System.Windows.Forms.WebBrowser webBrowser;
        public delegate void NavigateStringDelegate(string uriString);
        public delegate void NavigateUriDelegate(Uri uri);

        [Obfuscation(Exclude = true)]
        public event EventHandler NavigationReady = null;

        public WebBrowserHostControl()
        {
            InitializeComponent();
            // Initilize Events
            Initialized += WebBrowserHostControl_Initialized;
            SizeChanged += WebBrowserHostControl_SizeChanged;
            IsVisibleChanged += WebBrowserHostControl_IsVisibleChanged;
            // Initialize form
            form = new Form();
            form.FormBorderStyle = FormBorderStyle.None;
            form.ShowInTaskbar = false;
            form.TopMost = true;
            form.HandleCreated += form_HandleCreated;
            // Initialize WebBrowser
            webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.ScrollBarsEnabled = false;
            form.Controls.Add(webBrowser);
            //SetVisibility();
            //form.FormClosing += new FormClosingEventHandler(form_FormClosing);
            form.FormClosed += form_FormClosed;
        }

        [Obfuscation(Exclude = true)]
        void form_HandleCreated(object sender, EventArgs e)
        {
            if (NavigationReady != null)
                NavigationReady.Invoke(sender, new EventArgs());
        }
        
        [Obfuscation(Exclude = true)]
        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            PosDialogWindow window = (PosDialogWindow)Window.GetWindow(this);
            RemoveAllHandler();
            window.Close();
        }

        private void RemoveAllHandler()
        {
            PosDialogWindow window = (PosDialogWindow)Window.GetWindow(this);
            //form.FormClosing -= form_FormClosing;
            form.FormClosed -= form_FormClosed;
            window.Closing -= window_Closing;
            this.Initialized -= WebBrowserHostControl_Initialized;
            this.SizeChanged -= WebBrowserHostControl_SizeChanged;
            this.IsVisibleChanged -= WebBrowserHostControl_IsVisibleChanged;
        }

        [Obfuscation(Exclude = true)]
        void WebBrowserHostControl_Initialized(object sender, EventArgs e)
        {
            PosDialogWindow window = (PosDialogWindow)Window.GetWindow(this);
            window.LocationChanged += new EventHandler(window_LocationChanged);
            window.Closing += new System.ComponentModel.CancelEventHandler(window_Closing);
        }

        [Obfuscation(Exclude = true)]
        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveAllHandler();
            form.Close();
        }

        [Obfuscation(Exclude = true)]
        void window_LocationChanged(object sender, EventArgs e)
        {
            SetWindowLocation();
        }

        [Obfuscation(Exclude = true)]
        void WebBrowserHostControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetVisibility();
            SetWindowLocation();
        }

        [Obfuscation(Exclude = true)]
        void WebBrowserHostControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetWindowLocation();
        }

        private void SetVisibility()
        {
            if (IsVisible)
                form.Show();
            else
                form.Hide();
        }

        private void SetWindowLocation()
        {

            PosDialogWindow window = (PosDialogWindow)Window.GetWindow(this);
            if ((window != null) && !form.IsDisposed)
            {
                try
                {
                    Point topLeftPoint = GetControlPoint(gridControl);
                    form.Location = new System.Drawing.Point((int)topLeftPoint.X, (int)topLeftPoint.Y);
                    
                    webBrowser.Width = form.Width = (int)gridControl.ActualWidth;
                    webBrowser.Height = form.Height = (int)gridControl.ActualHeight;
                }
                catch
                {
                }
            }
        }

        public Point GetPhysicalTopLeftPoint(Window window)
        {
            Matrix transformToDevice;
            PresentationSource source = PresentationSource.FromVisual(window);
            if (source != null)
            {
                transformToDevice = source.CompositionTarget.TransformToDevice;
                return (Point)transformToDevice.Transform(new Vector(window.Left, window.Top));
            }
            return new Point();
        }

        public Point GetControlPoint(Grid control)
        {
            PosDialogWindow window = (PosDialogWindow)Window.GetWindow(this);
            if (window != null)
            {
                Point pt = GetPhysicalTopLeftPoint(window);
                GeneralTransform transform = control.TransformToAncestor(window);
                Point offset = transform.Transform(new Point(0, 0));
                return new Point(pt.X + offset.X, pt.Y + offset.Y);
            }
            return new Point();
        }

        private void SetLocation(int x, int y)
        {
            form.Location = new System.Drawing.Point(x, y);
        }

        public void Navigate(string uriString)
        {
            //webBrowser.DocumentText 
            //webBrowser.Navigate(uriString);
            try
            {
                webBrowser.BeginInvoke(new NavigateStringDelegate(webBrowser.Navigate), new object[] { uriString });
            }
            catch
            {
            }
        }

        public void Navigate(Uri uri)
        {
            //webBrowser.Navigate(uri);
            try
            {
                webBrowser.BeginInvoke(new NavigateUriDelegate(webBrowser.Navigate), new object[] { uri });
            }
            catch
            {
            }
        }

    }
}
