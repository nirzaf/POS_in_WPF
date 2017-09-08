using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TemPOS.Managers;
using TemPOS.Types;
using PosModels;
using PosControls;
using TemPOS.Helpers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ReportViewerControl.xaml
    /// </summary>
    public partial class ReportViewerControl : UserControl
    {
        #region Fields
        private bool _showEmployeeList;
        #endregion

        #region Events
        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedEmployeeChanged;
        #endregion

        #region Properties
        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public bool ShowEmployeeList
        {
            get { return _showEmployeeList; }
            set
            {
                _showEmployeeList = value;

                // Show or hide the first grid column
                gridControl.ColumnDefinitions[0].Width = !ShowEmployeeList ? new GridLength(0) : new GridLength(75, GridUnitType.Star);
                gridControl.UpdateLayout();
                
                // Add employees to list
                listBox1.Items.Clear();
                IEnumerable<Employee> employees = Employee.GetAll();
                FormattedListBoxItem first = null;
                foreach (Employee employee in employees)
                {
                    Person person = Person.Get(Employee.GetPersonId(employee.Id));
                    FormattedListBoxItem listboxItem = new FormattedListBoxItem(employee, person.LastName +
                        ", " + person.FirstName, true);
                    if (first == null)
                        first = listboxItem;
                    listBox1.Items.Add(listboxItem);
                }
                listBox1.SelectedItem = first;
            }

        }

        public Employee SelectedEmployee
        {
            get
            {
                if (listBox1.SelectedItem == null)
                    return null;
                FormattedListBoxItem formattedListBoxItem =
                    listBox1.SelectedItem as FormattedListBoxItem;
                if (formattedListBoxItem != null)
                    return formattedListBoxItem.ReferenceObject as Employee;
                return null;
            }
        }

        public List<string> Lines
        {
            get;
            private set;
        }
        #endregion

        public ReportViewerControl()
        {
            Lines = new List<string>();
            InitializeComponent();
            if (!ShowEmployeeList)
                gridControl.ColumnDefinitions[0].Width = new GridLength(0);
            flowDocumentScroll.IsEnabled = false;
            ClearDocument();
            Loaded += CommandShellControl_Loaded;
        }

        public void ClearDocument()
        {
            flowDocumentScroll.Document = new FlowDocument();
        }
        
        [Obfuscation(Exclude = true)]
        void CommandShellControl_Loaded(object sender, RoutedEventArgs e)
        {
            flowDocumentScroll.Document.PageWidth = flowDocumentScroll.ActualWidth - 5;
        }

        public void DefineTable(int columnCount)
        {
            Table newTable = new Table();
            TableColumn[] columns = new TableColumn[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columns[i] = new TableColumn();
                newTable.Columns.Add(columns[i]);
            }
            flowDocumentScroll.Document.Blocks.Add(newTable);
        }

        public void DefineTable(GridLength[] widths)
        {
            Table newTable = new Table();
            TableColumn[] columns = new TableColumn[widths.Length];
            for (int i = 0; i < widths.Length; i++)
            {
                columns[i] = new TableColumn
                {
                    Width = widths[i]
                };
                newTable.Columns.Add(columns[i]);
            }
            flowDocumentScroll.Document.Blocks.Add(newTable);
        }

        public void PrintTableLine(string text, int padding = 0, bool useBorder = false)
        {
            Lines.Add(text);
            string[] tokens = StringHelper.SplitMultiSpaceLine(text, padding);

            Table t = flowDocumentScroll.Document.Blocks.LastBlock as Table;
            TableRowGroup rowGroup = new TableRowGroup();
            TableRow row = new TableRow();
            if (t == null) return;
            t.Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
            if (tokens.Length > 0)
            {
                for (int i = 0; i < tokens.Length; i++)
                {
                    TableCell cell = new TableCell();
                    if (useBorder)
                    {
                        cell.BorderBrush = ConfigurationManager.BorderBrush;
                        cell.BorderThickness = new Thickness(1, 1, 1, 1);
                    }
                    Paragraph p = new Paragraph(new Run(tokens[i]))
                    {
                        TextAlignment = TextAlignment.Left
                    };
                    cell.Blocks.Add(p);
                    row.Cells.Add(cell);
                }
            }
            else
            {
                TableCell cell = new TableCell();
                Paragraph p = new Paragraph(new Run(""))
                {
                    TextAlignment = TextAlignment.Left
                };
                cell.Blocks.Add(p);
                row.Cells.Add(cell);
            }
            rowGroup.Rows.Add(row);
            t.RowGroups.Add(rowGroup);
        }

        public void PrintLine(string text)
        {
            Lines.Add(text);
            if (flowDocumentScroll.Document.Blocks.Count > 0)
            {
                Paragraph p = flowDocumentScroll.Document.Blocks.FirstBlock as Paragraph;
                if (p != null)
                {
                    Run r = p.Inlines.FirstInline as Run;
                    if (r != null) text = r.Text + Environment.NewLine + text;
                }
                flowDocumentScroll.Document.Blocks.Clear();
            }

            flowDocumentScroll.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public void PrintToReceipt()
        {
            for (int i=0; i < Lines.Count; i++)
            {
                string line = Lines[i];
#if !DEBUG
                // Change formatting for the PosPrinter
                while (line.Contains("   "))
                {
                    line = line.Replace("   ", "  ");
                }
                line = line.Replace("  ", PrinterEscapeCodes.SetRight + PrinterEscapeCodes.SetSize(8));
#endif
#if !DEMO
                PrinterManager.PrintLineToReceipt(DeviceManager.ActivePosPrinterLocal, line);
#endif
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
                return;
            if (SelectedEmployeeChanged != null)
                SelectedEmployeeChanged.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        private void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintToReceipt();
        }

        public static PosDialogWindow CreateInDefaultWindow(string title)
        {
            ReportViewerControl control = new ReportViewerControl();
            return new PosDialogWindow(control, title, 800, 500);
        }
    }
}
