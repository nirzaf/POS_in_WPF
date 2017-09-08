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
using TemposClientAdministration.Helpers;
using TemposLibrary;
using System.Threading;
using System.Windows.Threading;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for UpdateServiceControl.xaml
    /// </summary>
    public partial class UpdateServiceControl : UserControl
    {
        private bool restartWhenStopped = false;

        public UpdateServiceControl()
        {
            InitializeComponent();
            flowDocumentScroll.Document = new FlowDocument();
            UpdateServer.Started += new EventHandler(UpdateServer_Started);
            UpdateServer.Stopped += new EventHandler(updateServer_Stopped);
            UpdateServer.Debug += new TextEventHandler(updateServer_Debug);
        }
        
        [Obfuscation(Exclude = true)]
        void UpdateServer_Started(object sender, EventArgs e)
        {
            SetButtons(false);
        }

        [Obfuscation(Exclude = true)]
        void updateServer_Stopped(object sender, EventArgs e)
        {
            SetButtons(true);
            if (restartWhenStopped)
            {
                restartWhenStopped = false;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    timer.Start();
                }));
            }
        }

        [Obfuscation(Exclude = true)]
        void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            if (!((UpdateServer.ServerThread != null) &&
                UpdateServer.ServerThread.IsAlive))
            {
                timer.Stop();
                UpdateServer.Start();
            }

        }

        private void SetButtons(bool isStopped)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                buttonStart.IsEnabled = isStopped;
                buttonStop.IsEnabled = !isStopped;
                buttonRestart.IsEnabled = !isStopped;
            }));
        }

        [Obfuscation(Exclude = true)]
        void updateServer_Debug(object sender, TextEventArgs args)
        {
            PrintLine(args.Text);
        }

        private void PrintLine(string text)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (flowDocumentScroll.Document.Blocks.Count > 0)
                {
                    Paragraph p = flowDocumentScroll.Document.Blocks.FirstBlock as Paragraph;
                    Run r = p.Inlines.FirstInline as Run;
                    text = r.Text + Environment.NewLine + text;
                    flowDocumentScroll.Document.Blocks.Clear();
                }

                flowDocumentScroll.Document.Blocks.Add(new Paragraph(new Run(text)));
                flowDocumentScroll.UpdateLayout();
                dragScrollViewer.ScrollToEnd();
                //flowDocumentScroll.Document.PageWidth = this.ActualWidth - 5;
                this.UpdateLayout();
            }));
        }

        [Obfuscation(Exclude = true)]
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            UpdateServer.Start();
        }

        [Obfuscation(Exclude = true)]
        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            restartWhenStopped = true;
            UpdateServer.Stop();
        }

        [Obfuscation(Exclude = true)]
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            UpdateServer.Stop();
        }
    }
}
