using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels.Types;
using PosControls;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for CancelMadeUnmadeControl.xaml
    /// </summary>
    public partial class CancelMadeUnmadeControl : UserControl
    {
        public bool? IsMade
        {
            get;
            private set;
        }

        public TicketRefundType? RefundMode
        {
            get;
            private set;
        }

        public CancelMadeUnmadeControl()
        {
            RefundMode = null;
            InitializeComponent();
        }

        public CancelMadeUnmadeControl(bool refundMode)
        {
            RefundMode = null;
            InitializeComponent();
            if (refundMode)
            {
                buttonDontCancel.Visibility = Visibility.Collapsed;
                buttonDontRefund.Visibility = Visibility.Visible;
                buttonReopen.Visibility = Visibility.Visible;
                buttonVoid.Visibility = Visibility.Visible;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonReopen_Click(object sender, RoutedEventArgs e)
        {
            RefundMode = TicketRefundType.Reopened;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonMade_Click(object sender, RoutedEventArgs e)
        {
            RefundMode = TicketRefundType.CancelMade;
            IsMade = true;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonUnmade_Click(object sender, RoutedEventArgs e)
        {
            RefundMode = TicketRefundType.CancelUnmade;
            IsMade = false;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonVoid_Click(object sender, RoutedEventArgs e)
        {
            RefundMode = TicketRefundType.Void;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDontCancel_Click(object sender, RoutedEventArgs e)
        {
            IsMade = null;
            Window.GetWindow(this).Close();
        }

        public static PosDialogWindow CreateInDefaultWindow(string title, bool refundMode = false)
        {
            CancelMadeUnmadeControl control = new CancelMadeUnmadeControl(refundMode);
            return refundMode ?
                new PosDialogWindow(control, title, 515, 115) :
                new PosDialogWindow(control, title, 310, 115);
        }
    }
}
