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
using System.ComponentModel;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for PushCheckBox.xaml
    /// </summary>
    public partial class PushCheckBox : UserControl
    {
        #region Licensed Access Only
        static PushCheckBox()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PushCheckBox).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public static event EventHandler Changed;

        private bool isSelected = false;
        [Obfuscation(Exclude = true)]

        [Obfuscation(Exclude = true)]
        public event EventHandler SelectionChanged;

        public static readonly DependencyProperty EnabledBackgroundProperty =
            DependencyProperty.Register(
            "EnabledBackground", typeof(Brush), typeof(PushCheckBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledBackground
        {
            get { return (Brush)GetValue(EnabledBackgroundProperty); }
            set { SetValue(EnabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty EnabledSelectedBackgroundProperty =
            DependencyProperty.Register(
            "SelectedBackground", typeof(Brush), typeof(PushCheckBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledSelectedBackground
        {
            get { return (Brush)GetValue(EnabledSelectedBackgroundProperty); }
            set { SetValue(EnabledSelectedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(
            "DisabledBackground", typeof(Brush), typeof(PushCheckBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledSelectedBackgroundProperty =
            DependencyProperty.Register(
            "DisabledSelectedBackground", typeof(Brush), typeof(PushCheckBox),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledSelectedBackground
        {
            get { return (Brush)GetValue(DisabledSelectedBackgroundProperty); }
            set { SetValue(DisabledSelectedBackgroundProperty, value); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                bool was = isSelected;
                isSelected = value;
                UpdateBrushes();
                if ((was && !value) || (!was && value))
                {
                    if (SelectionChanged != null)
                        SelectionChanged.Invoke(this, new EventArgs());
                    if (Changed != null)
                        Changed.Invoke(this, new EventArgs());
                }
            }
        }

        private void UpdateBrushes()
        {
            if (IsEnabled)
            {
                gridControl.Background = EnabledBackground;
                canvasControl.Background =
                    (isSelected ? EnabledSelectedBackground : EnabledBackground);
            }
            else
            {
                gridControl.Background = DisabledBackground;
                canvasControl.Background =
                    (isSelected ? DisabledSelectedBackground : DisabledBackground);
            }
        }

        public PushCheckBox()
        {
            InitializeComponent();

            // Bindings
            Binding binding = new Binding();
            binding.Source = ConfigurationManager.EnabledCheckBoxBrush;
            BindingOperations.SetBinding(this, PushCheckBox.EnabledBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.EnabledSelectedCheckBoxBrush;
            BindingOperations.SetBinding(this, PushCheckBox.EnabledSelectedBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledCheckBoxBrush;
            BindingOperations.SetBinding(this, PushCheckBox.DisabledBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledSelectedCheckBoxBrush;
            BindingOperations.SetBinding(this, PushCheckBox.DisabledSelectedBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.BorderBrush;
            BindingOperations.SetBinding(borderControl, Border.BorderBrushProperty, binding);

            UpdateBrushes();
        }

        [Obfuscation(Exclude = true)]
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }

        [Obfuscation(Exclude = true)]
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateBrushes();
        }
    }
}
