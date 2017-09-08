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
    /// Interaction logic for SolidColorBrushEditorControl.xaml
    /// </summary>
    public partial class SolidColorBrushEditorControl : UserControl
    {
        #region Licensed Access Only
        static SolidColorBrushEditorControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(SolidColorBrushEditorControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private bool haltEvents = false;
        private SolidColorBrush selectedBrush = Brushes.Transparent;

        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedBrushChanged;

        public SolidColorBrush SelectedBrush
        {
            get { return selectedBrush; }
            set
            {
                if (value != null)
                    selectedBrush = value;
                else
                    selectedBrush = Brushes.Transparent;

                haltEvents = true;
                string brushName = GetBrushName(selectedBrush);
                if (brushName != null)
                {
                    radioBoxNamedBrushes.IsSelected = true;
                    radioBoxSpecifiedColor.IsSelected = false;
                    comboBoxNamedBrushes.SelectedItem = brushName;
                }
                else
                {
                    radioBoxNamedBrushes.IsSelected = false;
                    radioBoxSpecifiedColor.IsSelected = true;
                    comboBoxNamedBrushes.SelectedItem = null;
                }
                textBoxAlpha.Text = selectedBrush.Color.A.ToString();
                textBoxBlue.Text = selectedBrush.Color.B.ToString();
                textBoxGreen.Text = selectedBrush.Color.G.ToString();
                textBoxRed.Text = selectedBrush.Color.R.ToString();
                borderSwatch.Background = selectedBrush;
                SetEnabled(brushName != null);
                haltEvents = false;
            }
        }

        public bool ShowSaveButton
        {
            get
            {
                return !(lastColumn.Width.Value == 0);
            }
            set
            {
                if (value)
                    lastColumn.Width = new GridLength(80, GridUnitType.Pixel);
                else
                    lastColumn.Width = new GridLength(0, GridUnitType.Pixel);
            }
        }

        public SolidColorBrushEditorControl()
        {
            InitializeComponent();
            InitializeNamedBrushes();
            SelectedBrush = new SolidColorBrush(Color.FromArgb(255, 40, 60, 120));
        }

        public static string GetBrushName(SolidColorBrush selectedBrush)
        {
            Type objectType = typeof(Brushes);
            PropertyInfo[] properties =
                objectType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in properties)
            {
                SolidColorBrush brush = (SolidColorBrush)propertyInfo.GetValue(null, null);
                if (selectedBrush.Color == brush.Color)
                {
                    return propertyInfo.Name;
                }
            }
            return null;
        }

        public static SolidColorBrush GetBrushByName(string brushName)
        {
            try
            {
                Type objectType = typeof(Brushes);
                PropertyInfo propertyInfo =
                    objectType.GetProperty(brushName, BindingFlags.Public | BindingFlags.Static);
                return (SolidColorBrush)propertyInfo.GetValue(null, null);
            }
            catch
            {
                return null;
            }
        }

        private void InitializeNamedBrushes()
        {
            Type objectType = typeof(Brushes);
            PropertyInfo[] properties =
                objectType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in properties)
            {
                comboBoxNamedBrushes.Items.Add(propertyInfo.Name);
            }
        }

        private void SetEnabled(bool isNamedBrush)
        {
            comboBoxNamedBrushes.IsEnabled = isNamedBrush;
            textBoxAlpha.IsEnabled =
                textBoxBlue.IsEnabled =
                textBoxGreen.IsEnabled =
                textBoxRed.IsEnabled = !isNamedBrush;
        }
        
        [Obfuscation(Exclude = true)]
        private void radioBoxNamedBrushes_SelectionGained(object sender, EventArgs e)
        {
            if (!haltEvents)
            {
                radioBoxSpecifiedColor.IsSelected = false;
                SetEnabled(true);
            }
        }

        [Obfuscation(Exclude = true)]
        private void radioBoxSpecifiedColor_SelectionGained(object sender, EventArgs e)
        {
            if (!haltEvents)
            {
                radioBoxNamedBrushes.IsSelected = false;
                SetEnabled(false);
            }
        }

        [Obfuscation(Exclude = true)]
        private void comboBoxNamedBrushes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!haltEvents && (comboBoxNamedBrushes.SelectedItem != null))
            {
                SelectedBrush = GetBrushByName(comboBoxNamedBrushes.SelectedItem);
                OnBrushChanged();
            }
        }

        [Obfuscation(Exclude = true)]
        private void textBoxRed_TextChanged(object sender, RoutedEventArgs e)
        {
            ProcessTextChanges();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxGreen_TextChanged(object sender, RoutedEventArgs e)
        {
            ProcessTextChanges();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxBlue_TextChanged(object sender, RoutedEventArgs e)
        {
            ProcessTextChanges();
        }

        [Obfuscation(Exclude = true)]
        private void textBoxAlpha_TextChanged(object sender, RoutedEventArgs e)
        {
            ProcessTextChanges();
        }

        private void ProcessTextChanges()
        {
            if (haltEvents)
                return;
            byte? red = GetRed();
            if (red == null)
            {
                PromptBadValue("red");
                return;
            }
            byte? green = GetGreen();
            if (green == null)
            {
                PromptBadValue("green");
                return;
            }
            byte? blue = GetBlue();
            if (blue == null)
            {
                PromptBadValue("blue");
                return;
            }
            byte? alpha = GetAlpha();
            if (alpha == null)
            {
                PromptBadValue("alpha");
                return;
            }
            SelectedBrush =                
                new SolidColorBrush(Color.FromArgb(alpha.Value, red.Value, green.Value, blue.Value));
            OnBrushChanged();
        }

        private void PromptBadValue(string colorName)
        {
            //PosDialogWindow.ShowDialog(Window.GetWindow(this),
            //    "You have specified an invalid " + colorName + " value. The valid range is from 0 to 255.", "Invalid Value");
        }

        private byte? GetRed()
        {
            try
            {
                return (byte)Convert.ToInt32(textBoxRed.Text);
            }
            catch
            {
                return null;
            }
        }

        private byte? GetGreen()
        {
            try
            {
                return (byte)Convert.ToInt32(textBoxGreen.Text);
            }
            catch
            {
                return null;
            }
        }

        private byte? GetBlue()
        {
            try
            {
                return (byte)Convert.ToInt32(textBoxBlue.Text);
            }
            catch
            {
                return null;
            }
        }

        private byte? GetAlpha()
        {
            try
            {
                return (byte)Convert.ToInt32(textBoxAlpha.Text);
            }
            catch
            {
                return null;
            }
        }

        private void OnBrushChanged()
        {
            buttonSave.IsEnabled = true;
            if (SelectedBrushChanged != null)
                SelectedBrushChanged.Invoke(this, new EventArgs());
        }

        [Obfuscation(Exclude = true)]
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            // Close the dialog
            Window.GetWindow(this).Close();
        }

    }
}
