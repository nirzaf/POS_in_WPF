using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for InternetMapViewerControl.xaml
    /// </summary>
    public partial class InternetMapViewerControl : UserControl
    {
        #region Licensed Access Only
        static InternetMapViewerControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(InternetMapViewerControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public InternetMapViewerControl()
        {            
            InitializeComponent();
            webBrowser.NavigationReady += webBrowser_NavigationReady;
        }

        [Obfuscation(Exclude = true)]
        private void webBrowser_NavigationReady(object sender, EventArgs args)
        {
            InitializeForGoogleMaps();
        }

        private void InitializeForGoogleMaps()
        {
            PosDialogWindow parent = (PosDialogWindow)Window.GetWindow(this);
            //webBrowser.Source = new Uri("http://www.google.com", UriKind.RelativeOrAbsolute);
            webBrowser.Navigate(new Uri("file:///D:/Viipe.com/PosControls/PosControls/bin/Debug/Maps.html"));
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            InitializeForGoogleMaps();
        }

        [Obfuscation(Exclude = true)]
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
