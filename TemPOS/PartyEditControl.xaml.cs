using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for PartyEditControl.xaml
    /// </summary>
    public partial class PartyEditControl : UserControl
    {
        public Party ActiveParty
        {
            get;
            private set;
        }

        public int? PartySize
        {
            get
            {
                try
                {
                    return Convert.ToInt32(textBoxPartySize.Text);
                }
                catch
                {
                    return null;
                }
            }
        }

        public PartyEditControl()
        {
            InitializeComponent();
            if (!ConfigurationManager.UseOnScreenTextEntry)
                buttonAdd.IsEnabled = false;
        }

        public void Initialize(int partyId)
        {
            ActiveParty = Party.Get(partyId);
            InitializeFields();
            InitializeInvites();
        }

        private void InitializeInvites()
        {
            IEnumerable<PartyInvite> invites = PartyInvite.GetAll(ActiveParty.Id);
            foreach (PartyInvite invite in invites)
            {
                FormattedListBoxItem item = new FormattedListBoxItem(invite.Id, invite.GuestName, true);
                item.SetHeight(35);
                listBoxGuests.Items.Add(item);
            }
        }

        private void InitializeFields()
        {
            textBoxHostName.Text = ActiveParty.CustomerName;
            textBoxPartySize.Text = "" + ActiveParty.Size;
            textBoxNotes.Text = ActiveParty.Note;
        }

        private void AddInvite()
        {
            if (ConfigurationManager.UseOnScreenTextEntry)
            {
                PosDialogWindow parent = ((PosDialogWindow)Window.GetWindow(this));
                parent.ShowShadingOverlay = true;
                string inviteName = PosDialogWindow.PromptKeyboard(Strings.InvitesName, "");
                parent.ShowShadingOverlay = false;
                if (!String.IsNullOrEmpty(inviteName) && !String.IsNullOrWhiteSpace(inviteName))
                {
                    listBoxGuests.SelectedItem = null;
                    AddInvite(inviteName);
                    textBoxGuestName.Text = inviteName;
                }
            }
            else
            {
                AddInvite(textBoxGuestName.Text);
            }

        }

        private void AddInvite(string inviteName)
        {
            PartyInvite invite = PartyInvite.Add(ActiveParty.Id, inviteName);
            foreach (FormattedListBoxItem existingItem in listBoxGuests.Items)
            {
                existingItem.IsSelected = false;
            }
            FormattedListBoxItem item = new FormattedListBoxItem(invite.Id, inviteName, true);
            item.SetHeight(35);
            listBoxGuests.Items.Add(item);
            listBoxGuests.SelectedItem = item;
            listBoxGuests.ScrollIntoView(item);
            buttonDelete.IsEnabled = true;
        }

        private void DeleteInvite()
        {
            if (listBoxGuests.SelectedItem == null)
                return;
            FormattedListBoxItem item =
                (FormattedListBoxItem)listBoxGuests.SelectedItem;
            PartyInvite.Delete(item.Id);
            listBoxGuests.Items.Remove(item);
            textBoxGuestName.Text = "";
            buttonDelete.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxHostName_TextChanged(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                ActiveParty.SetCustomerName(textBoxHostName.Text);
                ActiveParty.Update();
            }
        }
        
        [Obfuscation(Exclude = true)]
        private void textBoxPartySize_TextChanged(object sender, RoutedEventArgs e)
        {
            int? partySize = PartySize;
            if (partySize != null)
            {
                ActiveParty.SetSize(partySize.Value);
                ActiveParty.Update();
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxNotes_TextChanged(object sender, RoutedEventArgs e)
        {
            if (textBoxNotes.Text != null)
            {
                ActiveParty.SetNote(textBoxNotes.Text);
                ActiveParty.Update();
            }
        }

        [Obfuscation(Exclude = true)]
        private void listBoxGuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
                textBoxGuestName.Text = ((FormattedListBoxItem)e.AddedItems[0]).Text;
            buttonDelete.IsEnabled = (listBoxGuests.SelectedItem != null);
        }

        [Obfuscation(Exclude = true)]
        private void textBoxGuestName_TextChanged(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem = (FormattedListBoxItem)listBoxGuests.SelectedItem;
            if (selectedItem != null)
            {

                selectedItem.Text = textBoxGuestName.Text;
                PartyInvite invite = PartyInvite.Get(selectedItem.Id);
                if (invite != null)
                {
                    invite.SetGuestName(textBoxGuestName.Text);
                    invite.Update();
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddInvite();
        }

        [Obfuscation(Exclude = true)]
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteInvite();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            PartyEditControl control = new PartyEditControl();
            return new PosDialogWindow(control, Strings.EditParty, 600, 475);
        }
    }
}
