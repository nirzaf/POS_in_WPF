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
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for DialogBoxControl.xaml
    /// </summary>
    public partial class DialogBoxControl : UserControl
    {
        #region Licensed Access Only
        static DialogBoxControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DialogBoxControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public DialogButtons Buttons;

        public bool IsOK
        {
            get;
            private set;
        }

        public bool IsYes
        {
            get;
            private set;
        }

        public DialogBoxControl()
        {
            IsOK = false;
            IsYes = false;
            InitializeComponent();
        }

        public void InitilizeButtonChoices(DialogButtons buttons)
        {
            Buttons = buttons;
            if ((buttons == DialogButtons.Ok) || (buttons == DialogButtons.OkCancel))
            {
                TextBlockButton buttonOk =
                    SetButton(new TextBlockButton(), "buttonOk", "Ok");
                buttonOk.Click += buttonOk_Selected;
                stackPanel.Children.Add(buttonOk);
            }
            if (buttons == DialogButtons.OkCancel)
            {
                TextBlockButton buttonCancel =
                    SetButton(new TextBlockButton(), "buttonCancel", "Cancel");
                buttonCancel.Click += buttonCancel_Selected;
                stackPanel.Children.Add(buttonCancel);
            }
            if (buttons == DialogButtons.YesNo)
            {
                TextBlockButton buttonYes =
                    SetButton(new TextBlockButton(), "buttonYes", "Yes");
                buttonYes.Click+= buttonYes_Selected;
                stackPanel.Children.Add(buttonYes);
            }
            if (buttons == DialogButtons.YesNo)
            {
                TextBlockButton buttonNo =
                    SetButton(new TextBlockButton(), "buttonNo", "No");
                buttonNo.Click += buttonCancel_Selected;
                stackPanel.Children.Add(buttonNo);
            }
            stackPanel.UpdateLayout();
        }

        private TextBlockButton SetButton(TextBlockButton button, string name, string text)
        {
            button.Name = name;            
            button.Margin = new Thickness(5, 5, 5, 5);
            button.Width = 70;
            button.Height = 64;
            button.Text = text;
            button.Visibility = System.Windows.Visibility.Visible;
            button.IsEnabled = true;
            return button;
        }

        [Obfuscation(Exclude = true)]
        private void buttonOk_Selected(object sender, RoutedEventArgs e)
        {
            IsOK = true;
            Window.GetWindow(this).Close();
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonYes_Selected(object sender, RoutedEventArgs e)
        {
            IsYes = true;
            Window.GetWindow(this).Close();
        }

        [Obfuscation(Exclude = true)]
        private void buttonCancel_Selected(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }

    public enum DialogButton
    {
        Ok,
        Cancel,
        Yes,
        No
    }

    public enum DialogButtons
    {
        Ok,
        OkCancel,
        YesNo
    }

}
