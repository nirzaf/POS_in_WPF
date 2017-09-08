using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TemPOS.Helpers;
using PosControls;
using PosModels.Types;
using TemPOS.Managers;

namespace TemPOS
{
    public delegate void CommandHandler();

    /// <summary>
    /// Interaction logic for ButtonTouchCommandInput.xaml
    /// </summary>
    public partial class ButtonTouchCommandInput : UserControl
    {
        private readonly List<Command> _commands = new List<Command>();

        private TextBlockButton[] Buttons;

        public ButtonTouchCommandInput()
        {
            InitializeComponent();
            Buttons = new [] {
                button1, button2, button3, button4, button5, button6, button7, button8,
                button9, button10, button11
            };
        }

        public void SetButtonWidth(double width)
        {
            foreach (TextBlockButton button in Buttons)
            {
                button.Width = width;
            }
        }

        [Obfuscation(Exclude = true)]
        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlockButton button = (TextBlockButton)sender;
            if (button.ContextMenu != null)
            {
                button.ContextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32(((TextBlockButton)sender).Tag);
            _commands[index].Handler.Invoke();
        }

        public void AddCommand(
            string commandText,
            CommandHandler handler,
            Permissions requiredPermission,
            bool isEnabled = true)
        {
            if ((requiredPermission == Permissions.None) ||
                (SessionManager.ActiveEmployee.HasPermission(requiredPermission)))
            {
                AddCommand(commandText, handler, isEnabled);
            }
        }

        public void AddCommand(
            string commandText,
            CommandHandler handler,
            Permissions requiredPermission,
            ContextMenu menu)
        {
            if ((requiredPermission == Permissions.None) ||
                (SessionManager.ActiveEmployee.HasPermission(requiredPermission)))
            {
                AddCommand(commandText, handler);
                Buttons[_commands.Count - 1].ContextMenu = menu;
            }
        }

        public void AddCommand(
            string commandText,
            CommandHandler handler,
            Permissions[] requiredPermissions,
            ContextMenu menu, bool isEnabled = true)
        {
            if (SessionManager.ActiveEmployee.HasPermission(requiredPermissions))
            {
                AddCommand(commandText, handler, isEnabled);
                if (menu != null)
                    menu.PlacementTarget = Buttons[_commands.Count - 1];
                Buttons[_commands.Count - 1].ContextMenu = menu;
            }
        }

        public void AddCommand(
            string commandText,
            CommandHandler handler,
            Permissions[] requiredPermissions,
            bool isEnabled = true)
        {
            if (SessionManager.ActiveEmployee.HasPermission(requiredPermissions))
            {
                AddCommand(commandText, handler, isEnabled);
            }
        }

        public void AddCommand(string commandText, CommandHandler handler, ContextMenu menu)
        {
            AddCommand(commandText, handler);
            Buttons[_commands.Count - 1].ContextMenu = menu;
        }

        public void AddCommand(string commandText, CommandHandler handler, double width,
            ContextMenu myMenu)
        {
            AddCommand(commandText, handler, width);
            Buttons[_commands.Count - 1].ContextMenu = myMenu;
        }

        public void AddCommand(string commandText, CommandHandler handler, double width,
            bool isEnabled = true, bool isTogglable = false)
        {
            if (Buttons.Length == _commands.Count)
                return;
            AddCommand(commandText, handler, isEnabled, isTogglable);
            Buttons[_commands.Count - 1].Width = width;
        }

        public void AddCommand(string commandText, CommandHandler handler,
            bool isEnabled = true, bool isTogglable = false)
        {
            if (Buttons.Length == _commands.Count)
                return;
            Buttons[_commands.Count].Show(commandText);
            Buttons[_commands.Count].IsEnabled = isEnabled;
            Buttons[_commands.Count].IsCheckable = isTogglable;
            _commands.Add(new Command(commandText, handler));
        }

        public void ClearCommands()
        {
            _commands.Clear();
            foreach (TextBlockButton button in Buttons)
            {
                button.IsEnabled = true;
                button.ContextMenu = null;
                button.Hide();
            }
        }

        public TextBlockButton FindButton(string buttonText)
        {
            return Buttons
                .Where(button => button.Text != null)
                .FirstOrDefault(button => button.Text.Equals(buttonText));
        }

        public class Command
        {
            public string CommandText
            {
                get;
                private set;
            }

            public CommandHandler Handler
            {
                get;
                private set;
            }

            public Command(string commandText, CommandHandler handler)
            {
                CommandText = commandText;
                Handler = handler;
            }
        }
    }
}
