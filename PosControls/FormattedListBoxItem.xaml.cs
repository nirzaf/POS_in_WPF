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
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for FormattedListBoxItem.xaml
    /// </summary>
    public partial class FormattedListBoxItem : UserControl, ISelectable
    {
        #region Licensed Access Only
        static FormattedListBoxItem()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(FormattedListBoxItem).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
            BackgroundProperty.OverrideMetadata(typeof(Brush),
                new FrameworkPropertyMetadata(ConfigurationManager.ListItemBackgroundBrush,
                    new PropertyChangedCallback(BackgroundValueChanged)));
            ForegroundProperty.OverrideMetadata(typeof(Brush),
                new FrameworkPropertyMetadata(ConfigurationManager.ListItemBackgroundBrush,
                    new PropertyChangedCallback(ForegroundValueChanged)));
        }
        #endregion

        protected static void ForegroundValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormattedListBoxItem)
            {
                FormattedListBoxItem obj = (d as FormattedListBoxItem);
                //obj.textBoxItem.Foreground = ConfigurationManagerForegroundBrush;
                return;
            }
        }

        protected static void BackgroundValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormattedListBoxItem)
            {
                FormattedListBoxItem obj = (d as FormattedListBoxItem);
                //obj.textBoxItem.Foreground = ConfigurationManagerForegroundBrush;
                //obj.textBoxItem =
                return;
            }
        }

        public int Id
        {
            get;
            set;
        }

        public object ReferenceObject
        {
            get;
            private set;
        }

        public GradientStop GradientStop
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                return textBoxItem.Text;
            }
            set
            {
                textBoxItem.Text = value;
            }
        }

        public double AllHeights
        {
            get { return Height; }
            set
            {
                gridControlGradientStops.MinHeight = value;
                gridControl.MinHeight = value;
                textBoxItem.MinHeight = value;
                Height = value;
            }
        }

        #region ISelectable
        public bool IsSelectable
        {
            get;
            set;
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
            "IsSelectedProperty", typeof(bool), typeof(FormattedListBoxItem),
            new UIPropertyMetadata(false,
                new PropertyChangedCallback(IsSelectedValueChanged),
                new CoerceValueCallback(IsSelectedCoerceValue)));

        protected static object IsSelectedCoerceValue(DependencyObject depObject, object value)
        {
            FormattedListBoxItem myClass = (FormattedListBoxItem)depObject;
            bool newValue = (bool)value;
            if (newValue && !myClass.IsSelectable)
                return false;
            return value;
        }

        protected static void IsSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FormattedListBoxItem myClass = (FormattedListBoxItem)d;
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            // A tag value of null is the same as IsSelected == true
            if (newValue)
                myClass.Tag = null;
            else
                myClass.Tag = true;           
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        #endregion
        
        public FormattedListBoxItem()
        {
            Tag = true;
            InitializeComponent();
            IsSelectable = false;
        }

        public FormattedListBoxItem(int id, string text, bool isSelectable = false)
        {
            Tag = true;
            InitializeComponent();
            Id = id;
            Text = text;
            IsSelectable = isSelectable;
        }

        public FormattedListBoxItem(object referenceObject, string text, bool isSelectable = false)
            : this(0, text)
        {
            IsSelectable = isSelectable;
            ReferenceObject = referenceObject;
        }

        public FormattedListBoxItem(GradientStop gradientStop, bool isSelectable = false)
        {
            Tag = true;
            InitializeComponent();
            IsSelectable = isSelectable;
            gridControl.Visibility = Visibility.Collapsed;
            gridControlGradientStops.Visibility = Visibility.Visible;
            SetGradientStop(gradientStop);
        }

        public void SetGradientStop(GradientStop gradientStop)
        {
            GradientStop = gradientStop;
            borderSwatch.Background = new SolidColorBrush(gradientStop.Color);
            labelOffsetValue.Content = String.Format("{0:0.000}", gradientStop.Offset);
        }

        public void SetHeight(double height)
        {
            gridControl.MinHeight = height;
            gridControl.Height = height;
            textBoxItem.MinHeight = height;
            textBoxItem.Height = height;
        }

        [Obfuscation(Exclude = true)]
        private void textBoxItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        public void Set(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public void Set(object referenceObject, string text)
        {
            ReferenceObject = referenceObject;
            Text = text;
        }
    }
}
