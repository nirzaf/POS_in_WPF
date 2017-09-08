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
using System.ComponentModel;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for PushRadioButton.xaml
    /// </summary>
    public partial class PushRadioButton : UserControl
    {
        #region Licensed Access Only
        static PushRadioButton()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(PushRadioButton).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public static event EventHandler Changed;

        private bool controlIsChecked = false;
        
        [Obfuscation(Exclude = true)]
        public event EventHandler SelectionGained;
        [Obfuscation(Exclude = true)]
        public event EventHandler Reselected;

        public static readonly DependencyProperty EnabledBackgroundProperty =
            DependencyProperty.Register(
            "EnabledBackground", typeof(Brush), typeof(PushRadioButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledBackground
        {
            get { return (Brush)GetValue(EnabledBackgroundProperty); }
            set { SetValue(EnabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty EnabledSelectedBackgroundProperty =
            DependencyProperty.Register(
            "SelectedBackground", typeof(Brush), typeof(PushRadioButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush EnabledSelectedBackground
        {
            get { return (Brush)GetValue(EnabledSelectedBackgroundProperty); }
            set { SetValue(EnabledSelectedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(
            "DisabledBackground", typeof(Brush), typeof(PushRadioButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DisabledSelectedBackgroundProperty =
            DependencyProperty.Register(
            "DisabledSelectedBackground", typeof(Brush), typeof(PushRadioButton),
            new FrameworkPropertyMetadata(Brushes.Transparent));

        [Bindable(true)]
        public Brush DisabledSelectedBackground
        {
            get { return (Brush)GetValue(DisabledSelectedBackgroundProperty); }
            set { SetValue(DisabledSelectedBackgroundProperty, value); }
        }

        public bool IsSelected
        {
            get
            {
                return controlIsChecked;
            }
            set
            {
                controlIsChecked = value;
                UpdateBrushes();
            }
        }

        public string Text
        {
            get
            {
                //return labelControl.Content as string;
                return textBlockControl.Text;
            }
            set
            {
                //labelControl.Content = value;
                textBlockControl.Text = value;
            }
        }

        public PushRadioButton()
        {
            InitializeComponent();

            // Bindings
            Binding binding = new Binding();
            binding.Source = ConfigurationManager.EnabledRadioButtonBackgroundBrush;
            BindingOperations.SetBinding(this, PushRadioButton.EnabledBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.EnabledSelectedRadioButtonBackgroundBrush;
            BindingOperations.SetBinding(this, PushRadioButton.EnabledSelectedBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledRadioButtonBackgroundBrush;
            BindingOperations.SetBinding(this, PushRadioButton.DisabledBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.DisabledSelectedRadioButtonBackgroundBrush;
            BindingOperations.SetBinding(this, PushRadioButton.DisabledSelectedBackgroundProperty, binding);

            binding = new Binding();
            binding.Source = ConfigurationManager.BorderBrush;
            BindingOperations.SetBinding(borderEllipseControl, Ellipse.FillProperty, binding);

            UpdateBrushes();
            IsEnabledChanged += 
                new DependencyPropertyChangedEventHandler(PushRadioButton_IsEnabledChanged);
        }

        [Obfuscation(Exclude = true)]
        void PushRadioButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateBrushes();
        }

        private void UpdateBrushes()
        {
            labelControl.Foreground = ConfigurationManager.LabelForegroundBrush;
            textBlockControl.Foreground = ConfigurationManager.LabelForegroundBrush;
            if (IsEnabled)
            {
                if (controlIsChecked)
                    ellipseControl.Fill = EnabledSelectedBackground;
                else
                    ellipseControl.Fill = EnabledBackground;
            }
            else
            {
                if (controlIsChecked)
                    ellipseControl.Fill = DisabledSelectedBackground;
                else
                    ellipseControl.Fill = DisabledBackground;
            }
        }

        [Obfuscation(Exclude = true)]
        private void ellipseControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            bool handled = true;
            if (!IsSelected)
            {
                IsSelected = !IsSelected;
                if (SelectionGained != null)
                    SelectionGained.Invoke(this, new EventArgs());
                if (Changed != null)
                    Changed.Invoke(this, new EventArgs());
            }
            else if (Reselected != null)
                Reselected.Invoke(this, new EventArgs());
            else
                handled = false;
            e.Handled = handled;            
        }

        [Obfuscation(Exclude = true)]
        private void Ellipse_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Ellipse borderEllipse = sender as Ellipse;
            if (borderEllipse.ActualHeight >= 2)
                ellipseControl.Height = borderEllipse.ActualHeight - 2;
        }

    }
}
