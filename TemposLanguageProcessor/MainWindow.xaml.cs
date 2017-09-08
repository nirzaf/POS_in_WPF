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
using TemposLanguageProcessor.Extractor;
using System.Windows.Threading;
using System.Threading;

namespace TemposLanguageProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread ExtractorThread;
        public static MainWindow Singleton;

        public static event EventHandler Yes;
        public static event EventHandler YesBind;
        public static event EventHandler No;
        public static event EventHandler Cancel;

        public MainWindow()
        {
            Singleton = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ExtractorThread = new Thread(CsExtractor.Start);
            //ExtractorThread.Start();
            ExtractorThread = new Thread(WpfExtractor.Start);
            ExtractorThread.Start();
            textBox.Focus();
        }

        public static void ClearEventHandlers()
        {
            Singleton.ClearEventHandlersInternal();
        }

        private void ClearEventHandlersInternal()
        {
            YesBind = null;
            Yes = null;
            No = null;
            Cancel = null;
        }

        public static void Write(string text)
        {
            Singleton.Dispatcher.Invoke(new Action(delegate
            {
                Singleton.textBox.AppendText(text);
                Singleton.textBox.ScrollToEnd();
                Singleton.textBox.Select(Singleton.textBox.Text.Length, 0);
            }));
        }

        public static void WriteLine(string text = "")
        {
            Singleton.Dispatcher.Invoke(new Action(delegate
            {
                Singleton.textBox.AppendText(text + Environment.NewLine);
                Singleton.textBox.ScrollToEnd();
                Singleton.textBox.Select(Singleton.textBox.Text.Length, 0);
            }));
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.B) && (YesBind != null))
                YesBind.Invoke(this, new EventArgs());
            else if ((e.Key == Key.Y) && (Yes != null))
                Yes.Invoke(this, new EventArgs());
            else if (((e.Key == Key.N) || (e.Key == Key.Enter) || (e.Key == Key.Space)) && (No != null))
                No.Invoke(this, new EventArgs());
            else if ((e.Key == Key.Escape) && (Cancel != null))
                Cancel.Invoke(this, new EventArgs());
            e.Handled = true;
        }

        private void textBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if ((ExtractorThread != null) && ExtractorThread.IsAlive)
                ExtractorThread.Abort();
        }
    }
}
