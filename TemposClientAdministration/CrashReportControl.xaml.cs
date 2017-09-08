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
using TemposUpdateServiceModels;
using PosControls;
using System.Windows.Threading;
using TemposClientAdministration.Helpers;

namespace TemposClientAdministration
{
    /// <summary>
    /// Interaction logic for CrashReportControl.xaml
    /// </summary>
    public partial class CrashReportControl : UserControl
    {
        public CrashReportControl()
        {
            InitializeComponent();
            InitializeListbox(false);
            flowDocumentScroll.IsEnabled = false;
            flowDocumentScroll.Document = new FlowDocument();
            UpdateServer.NewCrashIncident += new EventHandler(UpdateServer_NewCrashIncident);
        }

        [Obfuscation(Exclude = true)]
        void UpdateServer_NewCrashIncident(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                InitializeListbox(true);
            }));
        }

        private void InitializeListbox(bool retainSelection)
        {
            CrashIncident[] incidents = CrashIncident.GetAll();
            FormattedListBoxItem selectedItem = (retainSelection ?
                listBox.SelectedItem : null) as FormattedListBoxItem;
            CrashIncident selectedIncident = ((selectedItem != null) ?
                selectedItem.ReferenceObject : null) as CrashIncident;
            selectedItem = null;
            listBox.Items.Clear();
            foreach (CrashIncident incident in incidents)
            {
                Customer customer = Customer.Get(incident.CustomerId);
                FormattedListBoxItem listItem =
                    new FormattedListBoxItem(incident,
                    customer.BusinessName + Environment.NewLine +
                    incident.When, true);
                if ((selectedIncident != null) && (selectedIncident.Id == incident.Id))
                    selectedItem = listItem;
                listBox.Items.Add(listItem);
            }
            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
                listBox.SelectedItem = selectedItem;
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormattedListBoxItem selectedItem = listBox.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null)
            {
                buttonDeleteIncident.IsEnabled = false;
                return;
            }
            buttonDeleteIncident.IsEnabled = true;
            ClearException();
            CrashIncident crashIncident = selectedItem.ReferenceObject as CrashIncident;
            CrashReport crashReport = CrashReport.Get(crashIncident.TopLevelCrashReportId);
            while (crashReport != null)
            {
                PrintCrashReport(crashReport);
                PrintLine("");
                if (crashReport.InnerExceptionCrashReportId == null)
                    break;
                crashReport = CrashReport.Get(crashReport.InnerExceptionCrashReportId.Value);
            }
        }

        private void ClearException()
        {
            flowDocumentScroll.Document = new FlowDocument();
            flowDocumentScroll.Document.PageWidth = flowDocumentScroll.ActualWidth - 5;
        }

        private void PrintCrashReport(CrashReport crashReport)
        {
            PrintLine("Exception: " + crashReport.ExceptionName);
            PrintLine("Message: " + crashReport.ExceptionMessage);
            PrintLine("StackTrace:");
            PrintLine(crashReport.StackTrace, new Thickness(10, 0, 10, 0));
        }

        private void PrintLine(string text, Thickness? margin = null)
        {
            if (flowDocumentScroll.Document.Blocks.Count > 0)
            {
                Paragraph p = flowDocumentScroll.Document.Blocks.FirstBlock as Paragraph;
                if (margin != null)
                    p.Margin = margin.Value;
                Run r = p.Inlines.FirstInline as Run;
                text = r.Text + Environment.NewLine + text;
                flowDocumentScroll.Document.Blocks.Clear();
            }

            flowDocumentScroll.Document.Blocks.Add(new Paragraph(new Run(text)));
            flowDocumentScroll.UpdateLayout();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDeleteIncident_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem = listBox.SelectedItem as FormattedListBoxItem;
            buttonDeleteIncident.IsEnabled = false;
            if (selectedItem == null)
                return;
            CrashIncident crashIncident = selectedItem.ReferenceObject as CrashIncident;
            crashIncident.Delete();
            listBox.Items.Remove(selectedItem);
            listBox.SelectedItem = null;
            ClearException();
        }

    }
}
