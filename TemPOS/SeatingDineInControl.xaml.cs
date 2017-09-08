using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using PosModels;
using PosControls;
using PosModels.Managers;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for SeatingDineInControl.xaml
    /// </summary>
    public partial class SeatingDineInControl : UserControl
    {
        private readonly Dictionary<TextBlockButton, int> _seatingIds =
            new Dictionary<TextBlockButton, int>();

        public TextBlockButton[] Buttons
        {
            get
            {
                return new [] {
                    Row0_Col0_Button, Row0_Col1_Button, Row0_Col2_Button, Row0_Col3_Button, Row0_Col4_Button, Row0_Col5_Button,                    
                    Row1_Col0_Button, Row1_Col1_Button, Row1_Col2_Button, Row1_Col3_Button, Row1_Col4_Button, Row1_Col5_Button,
                    Row2_Col0_Button, Row2_Col1_Button, Row2_Col2_Button, Row2_Col3_Button, Row2_Col4_Button, Row2_Col5_Button,
                    Row3_Col0_Button, Row3_Col1_Button, Row3_Col2_Button, Row3_Col3_Button, Row3_Col4_Button, Row3_Col5_Button
                };
            }
        }

        public int SelectedIndex
        {
            get;
            private set;
        }

        public string SelectedTableName
        {
            get;
            private set;
        }

        public int SelectedSeatingId
        {
            get;
            private set;
        }

        public SeatingDineInControl()
        {
            SelectedIndex = -1;
            InitializeComponent();
        }

        public void HideRemainingButtons(int startingAt)
        {
            for (int i = startingAt; i < Buttons.Length; i++)
            {
                Buttons[i].Visibility = Visibility.Hidden;
            }
        }

        private void AssignSeatingIdToButton(TextBlockButton button, int id)
        {
            if (_seatingIds.ContainsKey(button))
                _seatingIds[button] = id;
            else
                _seatingIds.Add(button, id);
        }

        public void SetupSeatingButtons(Room room, Ticket activeTicket)
        {
            int index = 0;
            Seating[] seatings = SeatingManager.GetAllSeating();
            if (room != null)
            {
                labelSelectSeating.Content = Strings.SelectSeating + ": " + room.Description;
                foreach (Seating seat in seatings)
                {
                    if (index < Buttons.Length)
                    {
                        if (seat.RoomId == room.Id)
                        {
                            AssignSeatingIdToButton(Buttons[index], seat.Id);
                            Buttons[index].Text = Helpers.StringHelper.ThinString(seat.Description);
                            Buttons[index].Visibility = Visibility.Visible;
                            Buttons[index].IsChecked = ((activeTicket != null) && (activeTicket.SeatingId == seat.Id));
                            index++;
                        }
                    }
                }
            }
            else
            {
                labelSelectSeating.Content = Strings.SEATINGISNOTSETUP;
            }
            HideRemainingButtons(index);
        }

        public int GetIndex(TextBlockButton button)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Equals(button, Buttons[i]))
                    return i;
            }
            return -1;
        }

        public int GetId(TextBlockButton button)
        {
            return Convert.ToInt32(button.Tag as string);
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlockButton button = (TextBlockButton)sender;
            SelectedSeatingId = _seatingIds[button];
            SelectedIndex = GetIndex(button);
            SelectedTableName = Buttons[SelectedIndex].Text.Replace(Environment.NewLine, " ");
            Window.GetWindow(this).Close();
        }

    }
}
