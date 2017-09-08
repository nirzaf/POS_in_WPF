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
    /// Interaction logic for GradientStopEditorControl.xaml
    /// </summary>
    public partial class GradientStopEditorControl : UserControl
    {
        #region Licensed Access Only
        static GradientStopEditorControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(GradientStopEditorControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private GradientStop gradientStop;

        public GradientStop GradientStop
        {
            get { return gradientStop; }
            set
            {
                gradientStop = value;
                solidColorBrushEditor.SelectedBrush =
                    new SolidColorBrush(gradientStop.Color);
                textGradientOffset.Text =
                    gradientStop.Offset.ToString();
            }
        }

        public GradientStopEditorControl()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        private void textGradientOffset_TextChanged(object sender, RoutedEventArgs e)
        {
            SetEnabledSaveButton();
            ProcessGradientStopChange();
        }
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        private void solidColorBrushEditor_SelectedBrushChanged(object sender, EventArgs e)
        {
            SetEnabledSaveButton();
            ProcessGradientStopChange();
        }

        private void ProcessGradientStopChange()
        {
            double? offset = GetGradientOffset();
            if (offset != null)
                gradientStop =
                    new GradientStop(
                        solidColorBrushEditor.SelectedBrush.Color,
                        offset.Value);
        }

        private double? GetGradientOffset()
        {
            string text = textGradientOffset.Text;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.StartsWith("."))
                    text = "0" + text;
                if (text.EndsWith("."))
                    text = text + "0";
                return Convert.ToDouble(text);
            }
            return null;
        }

        private void SetEnabledSaveButton()
        {
            if (IsLoaded)
            {
                double? offset = GetGradientOffset();
                buttonSave.IsEnabled = (offset != null);
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
