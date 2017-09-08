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
using System.Threading;
using System.Windows.Threading;

namespace TemposClientAdministration
{
    public delegate void StringDelegate(string text);

    /// <summary>
    /// Interaction logic for UpdateBuilderControl.xaml
    /// </summary>
    public partial class UpdateBuilderControl : UserControl
    {
        public UpdateBuilderControl()
        {
            InitializeComponent();
            flowDocumentScroll.Document = new FlowDocument();
        }

        [Obfuscation(Exclude = true)]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateServer.Started += new EventHandler(UpdateServer_Started);
            UpdateServer.Start();
        }
        
        [Obfuscation(Exclude = true)]
        void UpdateServer_Started(object sender, EventArgs e)
        {
            UpdateServer.Started -= UpdateServer_Started;
            UpdateServer.Stopped += new EventHandler(UpdateServer_Stopped);
            UpdateServer.Stop();
        }

        [Obfuscation(Exclude = true)]
        void UpdateServer_Stopped(object sender, EventArgs e)
        {
            UpdateServer.Stopped -= UpdateServer_Stopped;
            UpdateServer.Start();
        }

        private void StartThreadedBuild(object clientObject)
        {
            UpdateBuilder.PrintLine = PrintLine;
            UpdateBuilder.Build();
            this.Dispatcher.Invoke((Action)(() =>
            {
                buttonBuildUpdate.IsEnabled = true;
            }));
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
                flowDocumentScroll.Document.PageWidth = this.ActualWidth - 5;
                this.UpdateLayout();
            }));
        }

        [Obfuscation(Exclude = true)]
        private void buttonBuildUpdate_Click(object sender, RoutedEventArgs e)
        {
            buttonBuildUpdate.IsEnabled = false;
            flowDocumentScroll.Document = new FlowDocument();
            flowDocumentScroll.Document.PageWidth = flowDocumentScroll.ActualWidth - 5;
            ThreadPool.QueueUserWorkItem(StartThreadedBuild, null);
        }
    }
}
