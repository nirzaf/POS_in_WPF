using System.Collections.Generic;
using System.Windows.Controls;
using PosModels;
using PosControls;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for PrintOptionEditorControl.xaml
    /// </summary>
    public partial class PrintOptionEditorControl : UserControl
    {
        #region Field
        private bool _haltEvents;
        private PrintOptionSet _activePrintOptionSet;
        #endregion

        #region Properties
        public PrintOptionSet ActivePrintOptionSet
        {
            get
            {
                return _activePrintOptionSet;
            }
            set
            {
                _haltEvents = true;
                _activePrintOptionSet = value;
                InitializeFields();
                _haltEvents = false;
            }
        }
        #endregion

        #region Constructor and Initialization
        public PrintOptionEditorControl()
        {
            InitializeComponent();
            InitializeFields();
        }

        private void InitializeFields()
        {
            textBoxName.Text = "";
            listBoxOptions.Items.Clear();

            if (ActivePrintOptionSet != null)
            {
                textBoxName.Text = ActivePrintOptionSet.OptionSetName;
                IEnumerable<PrintOption> optionList = PrintOption.GetInSet(ActivePrintOptionSet.Id);
                foreach (PrintOption option in optionList)
                {
                    Printer printer = Printer.Get(option.PrinterId);
                    listBoxOptions.Items.Add(new FormattedListBoxItem(option.Id, printer.PrinterName));
                }
            }
        }
        #endregion
    }
}
