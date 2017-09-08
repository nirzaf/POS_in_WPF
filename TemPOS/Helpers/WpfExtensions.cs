using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using PosControls;

namespace TemPOS.Helpers
{
    public static class WpfExtensions
    {
        #region For ButtonTouchCommandInput
        public static void Set(this Button button, string text, bool isShown)
        {
            if (isShown)
                Show(button, text);
            else
                Collapse(button);
        }

        public static void Collapse(this Button button)
        {
            button.Content = "";
            button.Visibility = Visibility.Collapsed;
        }

        public static void Hide(this Button button)
        {
            button.Content = "";
            button.Visibility = Visibility.Hidden;
        }

        public static void Show(this Button button, string text)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
            button.Content = textBlock;
            button.Visibility = Visibility.Visible;
        }

        public static void Set(this TextBlockButton button, string text, bool isShown)
        {
            if (isShown)
                Show(button, text);
            else
                Collapse(button);
        }

        public static void Collapse(this TextBlockButton button)
        {
            button.Text = "";
            button.Visibility = Visibility.Collapsed;
        }

        public static void Hide(this TextBlockButton button)
        {
            button.Text = "";
            button.Visibility = Visibility.Hidden;
        }

        public static void Show(this TextBlockButton button, string text)
        {
            button.Text = text;
            button.Visibility = Visibility.Visible;
        }

        public static void Set(this ToggleButton button, string text, bool isShown)
        {
            if (isShown)
                Show(button, text);
            else
                Collapse(button);
        }

        public static void Collapse(this ToggleButton button)
        {
            button.Content = "";
            button.Visibility = Visibility.Collapsed;
        }

        public static void Hide(this ToggleButton button)
        {
            button.Content = "";
            button.Visibility = Visibility.Hidden;
        }

        public static void Show(this ToggleButton button, string text)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
            button.Content = textBlock;
            button.Visibility = Visibility.Visible;
            button.IsChecked = false;
        }
        #endregion


    }
}
