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
using PosControls.Interfaces;
using PosControls.Helpers;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for GradientBrushEditorControl.xaml
    /// </summary>
    public partial class GradientBrushEditorControl : UserControl
    {
        #region Licensed Access Only
        static GradientBrushEditorControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(GradientBrushEditorControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool haltEvents = false;
        private GradientBrush selectedBrush;
        private GradientStopCollection gradientStopCollection =
            new GradientStopCollection();

        public GradientBrush SelectedBrush
        {
            get { return selectedBrush; }
            set
            {
                haltEvents = true;
                selectedBrush = value;
                gradientStopCollection = value.GradientStops.Clone();
                if (selectedBrush is LinearGradientBrush)
                {
                    radioButtonLinearGradient.IsSelected = true;
                    radioButtonRadialGradient.IsSelected = false;

                    LinearGradientBrush linearGradientBrush = selectedBrush as LinearGradientBrush;
                    textBoxLinearStartPointX.Text = String.Format("{0:0.000}", linearGradientBrush.StartPoint.X);
                    textBoxLinearStartPointY.Text = String.Format("{0:0.000}", linearGradientBrush.StartPoint.Y);
                    textBoxLinearEndPointX.Text = String.Format("{0:0.000}", linearGradientBrush.EndPoint.X);
                    textBoxLinearEndPointY.Text = String.Format("{0:0.000}", linearGradientBrush.EndPoint.Y);
                }
                else if (selectedBrush is RadialGradientBrush)
                {
                    radioButtonRadialGradient.IsSelected = true;
                    radioButtonLinearGradient.IsSelected = false;

                    RadialGradientBrush radialGradientBrush = selectedBrush as RadialGradientBrush;
                    textBoxOriginX.Text = String.Format("{0:0.000}", radialGradientBrush.GradientOrigin.X);
                    textBoxOriginY.Text = String.Format("{0:0.000}", radialGradientBrush.GradientOrigin.Y);
                    textBoxRadiusX.Text = String.Format("{0:0.000}", radialGradientBrush.RadiusX);
                    textBoxRadiusY.Text = String.Format("{0:0.000}", radialGradientBrush.RadiusY);
                    textBoxCenterX.Text = String.Format("{0:0.000}", radialGradientBrush.Center.X);
                    textBoxCenterY.Text = String.Format("{0:0.000}", radialGradientBrush.Center.Y);
                }

                if (selectedBrush.SpreadMethod == GradientSpreadMethod.Pad)
                    comboBoxSpreadMethod.SelectedIndex = 0;
                else if (selectedBrush.SpreadMethod == GradientSpreadMethod.Reflect)
                    comboBoxSpreadMethod.SelectedIndex = 1;
                else if (selectedBrush.SpreadMethod == GradientSpreadMethod.Repeat)
                    comboBoxSpreadMethod.SelectedIndex = 2;

                if (selectedBrush.ColorInterpolationMode == ColorInterpolationMode.SRgbLinearInterpolation)
                    comboBoxColorInterpolationMode.SelectedIndex = 0;
                else if (selectedBrush.ColorInterpolationMode == ColorInterpolationMode.ScRgbLinearInterpolation)
                    comboBoxColorInterpolationMode.SelectedIndex = 1;

                // Gradient Stops
                listBoxGradientStopCollection.Items.Clear();
                listBoxGradientStopCollection.SelectedItem = null;
                if (SelectedBrush != null)
                {
                    SelectedBrush.GradientStops = gradientStopCollection;
                    foreach (GradientStop gradientStop in gradientStopCollection)
                    {
                        listBoxGradientStopCollection.Items.Add(
                            new FormattedListBoxItem(gradientStop, true));
                    }
                }

                SetOpacity(selectedBrush.Opacity);
                SetPreviewSwatch();
                haltEvents = false;
            }

        }

        public GradientBrushEditorControl()
        {
            InitializeComponent();            
        }

        private void SetPreviewSwatch()
        {
            GradientBrush brush;
            if (radioButtonLinearGradient.IsSelected)
            {
                LinearGradientBrush linearGradientBrush =
                    new LinearGradientBrush(gradientStopCollection);
                linearGradientBrush.StartPoint = GetStartPoint();
                linearGradientBrush.EndPoint = GetEndPoint();
                brush = linearGradientBrush;
            }
            else
            {
                RadialGradientBrush radialGradientBrush =
                    new RadialGradientBrush(gradientStopCollection);
                radialGradientBrush.RadiusX = GetRadiusX();
                radialGradientBrush.RadiusY = GetRadiusY();
                radialGradientBrush.GradientOrigin = GetOrigin();
                radialGradientBrush.Center = GetCenter();
                brush = radialGradientBrush;
            }

            if (comboBoxSpreadMethod.SelectedIndex == 0)
                brush.SpreadMethod = GradientSpreadMethod.Pad;
            else if (comboBoxSpreadMethod.SelectedIndex == 1)
                brush.SpreadMethod = GradientSpreadMethod.Reflect;
            else if (comboBoxSpreadMethod.SelectedIndex == 2)
                brush.SpreadMethod = GradientSpreadMethod.Repeat;

            if (comboBoxColorInterpolationMode.SelectedIndex == 0)
                brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
            else if (comboBoxColorInterpolationMode.SelectedIndex == 1)                
                brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;

            brush.Opacity = GetOpacity();
            borderSwatch.Background = 
                selectedBrush = brush;
        }

        private IEnumerable<GradientStop> GetGradientStops()
        {
            foreach (FormattedListBoxItem item in listBoxGradientStopCollection.Items)
            {
                yield return item.GradientStop;
            }
        }

        private double? ToDouble(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (text.StartsWith("."))
                    text = "0" + text;
                if (text.EndsWith("."))
                    text = text + "0";
                try
                {
                    return Convert.ToDouble(text);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        private double GetOpacity()
        {
            double? opacity = ToDouble(textBoxRadientOpacity.Text);
            if (opacity != null)
            {
                return MathHelper.Clamp(opacity.Value, 0, 1);
            }
            return 1;
        }

        private void SetOpacity(double value)
        {
            textBoxLinearOpacity.Text =
                textBoxRadientOpacity.Text =
                String.Format("{0:0.000}", value);
        }

        private Point GetOrigin()
        {
            double? x = ToDouble(textBoxOriginX.Text);
            double? y = ToDouble(textBoxOriginY.Text);
            if ((x != null) && (y != null))
                return new Point(x.Value, y.Value);
            return new Point(0.5, 0.5); // Default
        }

        private Point GetCenter()
        {
            double? x = ToDouble(textBoxCenterX.Text);
            double? y = ToDouble(textBoxCenterY.Text);
            if ((x != null) && (y != null))
                return new Point(x.Value, y.Value);
            return new Point(0.5, 0.5); // Default
        }

        private double GetRadiusY()
        {
            double? radius = ToDouble(textBoxRadiusX.Text);
            if (radius != null)
                return radius.Value;
            return 0.5; // Default
        }

        private double GetRadiusX()
        {
            double? radius = ToDouble(textBoxRadiusX.Text);
            if (radius != null)
                return radius.Value;
            return 0.5; // Default
        }

        private Point GetStartPoint()
        {
            double? x = ToDouble(textBoxLinearStartPointX.Text);
            double? y = ToDouble(textBoxLinearStartPointY.Text);
            if ((x != null) && (y != null))
                return new Point(x.Value, y.Value);
            return new Point(0, 0); // Default
        }

        private Point GetEndPoint()
        {
            double? x = ToDouble(textBoxLinearEndPointX.Text);
            double? y = ToDouble(textBoxLinearEndPointY.Text);
            if ((x != null) && (y != null))
                return new Point(x.Value, y.Value);
            return new Point(1, 1); // Default
        }

        private void SetEnabledButtons()
        {
            FormattedListBoxItem selectedItem =
                listBoxGradientStopCollection.SelectedItem as FormattedListBoxItem;
            buttonEdit.IsEnabled =
                buttonRemove.IsEnabled =
                (selectedItem != null);
            buttonMoveUp.IsEnabled =
                ((selectedItem != null) && (listBoxGradientStopCollection.SelectedIndex > 0));
            buttonMoveDown.IsEnabled =
                ((selectedItem != null) && (listBoxGradientStopCollection.SelectedIndex <=
                listBoxGradientStopCollection.Items.Count - 2));
        }

        #region Event Handling
        [Obfuscation(Exclude = true)]
        private void radioButtonLinearGradient_SelectionGained(object sender, EventArgs e)
        {
            radioButtonRadialGradient.IsSelected = false;
            stackPanelRadialParameters.Visibility = Visibility.Collapsed;
            stackPanelLinearParameters.Visibility = Visibility.Visible;
            SetPreviewSwatch();
            buttonSave.IsEnabled = true;
        }
        
        [Obfuscation(Exclude = true)]
        private void radioButtonRadialGradient_SelectionGained(object sender, EventArgs e)
        {
            radioButtonLinearGradient.IsSelected = false;
            stackPanelLinearParameters.Visibility = Visibility.Collapsed;
            stackPanelRadialParameters.Visibility = Visibility.Visible;
            SetPreviewSwatch();
            buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void listBoxGradientStopCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEnabledButtons();
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxSpreadMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!haltEvents && IsInitialized)
            {
                SetPreviewSwatch();
                buttonSave.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxColorInterpolationMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!haltEvents && IsInitialized)
            {
                SetPreviewSwatch();
                buttonSave.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!haltEvents && IsInitialized)
            {
                SetPreviewSwatch();
                buttonSave.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxLinearOpacity_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!haltEvents && IsInitialized)
            {
                double? value = ToDouble(textBoxLinearOpacity.Text);
                if (value != null)
                {
                    double modifiedValue = MathHelper.Clamp(value.Value, 0, 1);
                    if (modifiedValue != value.Value)
                        SilentlyChangeText(textBoxLinearOpacity, textBoxLinearOpacity_TextChanged,
                            String.Format("{0:0.000}", modifiedValue));
                    buttonSave.IsEnabled = true;
                }
                SilentlyChangeText(textBoxRadientOpacity, textBoxRadientOpacity_TextChanged,
                    textBoxLinearOpacity.Text);
                SetPreviewSwatch();
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxRadientOpacity_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!haltEvents && IsInitialized)
            {
                double? value = ToDouble(textBoxRadientOpacity.Text);
                if (value != null)
                {
                    double modifiedValue = MathHelper.Clamp(value.Value, 0, 1);
                    if (modifiedValue != value.Value)
                        SilentlyChangeText(textBoxRadientOpacity, textBoxRadientOpacity_TextChanged,
                            String.Format("{0:0.000}", modifiedValue));
                    buttonSave.IsEnabled = true;
                }
                SilentlyChangeText(textBoxLinearOpacity, textBoxLinearOpacity_TextChanged,
                    textBoxRadientOpacity.Text);
                SetPreviewSwatch();
            }
        }

        [Obfuscation(Exclude = true)]
        private void SilentlyChangeText(CustomTextBox textBox, RoutedEventHandler handler, string text)
        {
            textBox.TextChanged -= handler;
            textBox.Text = text;
            textBox.TextChanged += handler;
        }

        [Obfuscation(Exclude = true)]
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            GradientStopEditorControl control = new GradientStopEditorControl();
            PosDialogWindow window = new PosDialogWindow(control, "Gradient Stop Editor", 450, 500);
            if ((PosDialogWindow.ShowPosDialogWindow(this, window) != null) &&
                (control.GradientStop != null))
            {
                gradientStopCollection.Add(control.GradientStop);
                listBoxGradientStopCollection.Items.Add(
                    new FormattedListBoxItem(control.GradientStop, true));
                SetPreviewSwatch();
                buttonSave.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxGradientStopCollection.SelectedItem == null)
                return;
            FormattedListBoxItem selectedItem =
                listBoxGradientStopCollection.SelectedItem as FormattedListBoxItem;
            GradientStop gradientStop = selectedItem.GradientStop;
            GradientStopEditorControl control = new GradientStopEditorControl();
            PosDialogWindow window = new PosDialogWindow(control, "Gradient Stop Editor", 450, 500);

            control.GradientStop = gradientStop;
            if (PosDialogWindow.ShowPosDialogWindow(this, window) != null)
            {
                selectedItem.SetGradientStop(control.GradientStop);
                gradientStopCollection[listBoxGradientStopCollection.SelectedIndex] =
                    control.GradientStop;

                // Update UI
                SetPreviewSwatch();
                buttonSave.IsEnabled = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            FormattedListBoxItem selectedItem =
                listBoxGradientStopCollection.SelectedItem as FormattedListBoxItem;
            if (selectedItem == null)
                return;
            int index = listBoxGradientStopCollection.SelectedIndex;

            // Remove GradientStop
            listBoxGradientStopCollection.Items.Remove(selectedItem);
            listBoxGradientStopCollection.SelectedItem = null;
            gradientStopCollection.RemoveAt(index);

            // Update UI
            SetPreviewSwatch();
            SetEnabledButtons();
            buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = listBoxGradientStopCollection.SelectedIndex;
            if (index < 1)
                return;

            FormattedListBoxItem selectedItem =
                listBoxGradientStopCollection.Items[index] as FormattedListBoxItem;
            FormattedListBoxItem oneUpItem =
                listBoxGradientStopCollection.Items[index - 1] as FormattedListBoxItem;

            // Update GradientStops
            GradientStop gs1 = selectedItem.GradientStop;
            GradientStop gs2 = oneUpItem.GradientStop;
            selectedItem.SetGradientStop(new GradientStop(gs2.Color, gs1.Offset));
            oneUpItem.SetGradientStop(new GradientStop(gs1.Color, gs2.Offset));
            gradientStopCollection[index] = selectedItem.GradientStop;
            gradientStopCollection[index - 1] = oneUpItem.GradientStop;

            // Update UI
            listBoxGradientStopCollection.SelectedItem =
                listBoxGradientStopCollection.Items[listBoxGradientStopCollection.SelectedIndex - 1];
            SetEnabledButtons();
            SetPreviewSwatch();
            buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = listBoxGradientStopCollection.SelectedIndex;
            if (index > listBoxGradientStopCollection.Items.Count - 2)
                return;
            FormattedListBoxItem selectedItem =
                listBoxGradientStopCollection.Items[index] as FormattedListBoxItem;
            FormattedListBoxItem oneDownItem =
                listBoxGradientStopCollection.Items[index + 1] as FormattedListBoxItem;

            // Update GradientStops
            GradientStop gs1 = selectedItem.GradientStop;
            GradientStop gs2 = oneDownItem.GradientStop;
            selectedItem.SetGradientStop(new GradientStop(gs2.Color, gs1.Offset));
            oneDownItem.SetGradientStop(new GradientStop(gs1.Color, gs2.Offset));
            gradientStopCollection[index] = selectedItem.GradientStop;
            gradientStopCollection[index + 1] = oneDownItem.GradientStop;

            // Update UI
            listBoxGradientStopCollection.SelectedItem =
                listBoxGradientStopCollection.Items[listBoxGradientStopCollection.SelectedIndex + 1];
            SetEnabledButtons();
            SetPreviewSwatch();
            buttonSave.IsEnabled = true;
        }

        [Obfuscation(Exclude = true)]
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            // Verify that gradient stops are not empty
            if (listBoxGradientStopCollection.Items.Count < 2)
            {
                PosDialogWindow.ShowDialog(
                    "You must enter at least two gradient stops.", "Error");
                return;
            }

            // Close Window
            Window.GetWindow(this).Close();
        }
        #endregion

    }
}
