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
using PosControls.Helpers;
using PosControls.Interfaces;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for ActionNotificationControl.xaml
    /// </summary>
    public partial class ActionNotificationControl : UserControl
    {
        #region Licensed Access Only
        static ActionNotificationControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ActionNotificationControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion
        
        public PosDialogWindow ParentWindow
        {
            get;
            private set;
        }

        public IShadeable OwnerWindow
        {
            get;
            private set;
        }

        private ActionNotificationControl()
        {
            InitializeComponent();
        }

        public void Show()
        {
            if (OwnerWindow != null)
            {
                OwnerWindow.ShowShadingOverlay = true;
                ParentWindow.Closed += window_Closed;
            }
            ParentWindow.Show();
        }

        public void Close()
        {
            ParentWindow.Close();
        }

        [Obfuscation(Exclude = true)]
        void window_Closed(object sender, EventArgs e)
        {
            OwnerWindow.ShowShadingOverlay = false;
        }

        public static ActionNotificationControl Create(IShadeable ownerWindow,
            string message, string titlebar)
        {
            ActionNotificationControl notification = new ActionNotificationControl();

            double width;
            if (!notification.MeasureText(message, out width))
                throw new Exception("Text Measurement Exception");
            width = MathHelper.Clamp(width + 10, 200, 1000);

            notification.labelMessage.Content = message;
            notification.OwnerWindow = ownerWindow;
            notification.ParentWindow = new PosDialogWindow(notification,
                titlebar, width, 85);
            notification.ParentWindow.IsClosable = false;
            notification.ParentWindow.Topmost = true;

            return notification;
        }

        private bool MeasureText(string text, out double width)
        {
            width = 0;
            Typeface typeface = new Typeface(labelMessage.FontFamily, labelMessage.FontStyle,
                    labelMessage.FontWeight, labelMessage.FontStretch);
            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                return false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                ushort glyph = glyphTypeface.CharacterToGlyphMap[ch];
                width += glyphTypeface.AdvanceWidths[glyph];
            }
            width *= labelMessage.FontSize;
            return true;
        }

    }
}
