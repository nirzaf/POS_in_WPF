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
using PosControls;
using TemPOS.Commands;
using TemPOS.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for OrderEntryOrderCommandsControl.xaml
    /// </summary>
    public partial class OrderEntryOrderCommandsControl : UserControl
    {
        public OrderEntryOrderCommandsControl()
        {
            InitializeComponent();
            OrderEntryCommands.PlaceOrder.Executed += (sender, args) =>
            {
                if (ConfigurationManager.LogoutOnPlaceOrder)
                    OrderEntryCommands.Logout.Execute(this);
            };
        }

        public void UpdatePartyButton()
        {
            if (SessionManager.ActiveTicket == null)
                return;
            buttonManageParty.Text = (SessionManager.ActiveTicket.PartyId == 0 ?
                Strings.ConvertToParty : Strings.ManageParty);
        }

    }
}
