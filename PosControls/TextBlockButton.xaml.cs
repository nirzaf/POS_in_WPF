using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for TextBlockButton.xaml
    /// </summary>
    public partial class TextBlockButton : ICommandSource
    {
        #region Licensed Access Only
        static TextBlockButton()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TextBlockButton).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBlockButton));

        // Summary:
        //     Occurs when a TexkBlockButton is clicked.
        [Category("Behavior")]
        [Obfuscation(Exclude = true)]
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBlockButton),
            new UIPropertyMetadata(null));

        public new double FontSize
        {
            get { return base.FontSize; }
            set
            {
                base.FontSize = value;
                buttonControl.FontSize = value;
                toggleButtonControl.FontSize = value;
            }
        }

        public bool IsCheckable
        {
            get { return (buttonControl.Visibility != Visibility.Visible); }
            set
            {
                buttonControl.Visibility = (value ? Visibility.Collapsed : Visibility.Visible);
                toggleButtonControl.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public bool? IsChecked
        {
            get { return toggleButtonControl.IsChecked; }
            set { toggleButtonControl.IsChecked = value; }
        }

        public bool CollapseNonExecutableCommands
        {
            get;
            set;
        }

        public ICommand Command
        {
            get { return toggleButtonControl.Command; }
            set
            {
                ICommand originalCommand = toggleButtonControl.Command;
                if (originalCommand != null)
                    originalCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                toggleButtonControl.Command = value;
                value.CanExecuteChanged += Command_CanExecuteChanged;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            IsEnabled = Command.CanExecute(this);
            if (CollapseNonExecutableCommands)
                Visibility = (IsEnabled ? Visibility.Visible : Visibility.Collapsed);
        }

        public object CommandParameter
        {
            get { return toggleButtonControl.CommandParameter; }
            set { toggleButtonControl.CommandParameter = value; }
        }

        public IInputElement CommandTarget
        {
            get { return toggleButtonControl.CommandTarget; }
            set { toggleButtonControl.CommandTarget = value; }
        }

        public TextBlockButton()
        {
            InitializeComponent();
            CollapseNonExecutableCommands = true;
            buttonControl.Click += ClickEventHandler;
            toggleButtonControl.Click += ClickEventHandler;
        }
        
        [Obfuscation(Exclude = true)]
        void ClickEventHandler(object sender, RoutedEventArgs e)
        {
            if ((Command != null) && Command.CanExecute(this))
                Command.Execute(this);
            else if ((Command != null) && CollapseNonExecutableCommands)            
                Visibility = Visibility.Hidden;
            var newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
