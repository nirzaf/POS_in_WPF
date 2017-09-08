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
    public class TabDetails : Control
    {
        #region Licensed Access Only
        static TabDetails()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(TabDetails).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        private string tabName = null;
        private double buttonWidth = 0;
        private double tabWidth = 0;
        private double tabHeight = 0;
        private FrameworkElement dockedControl;
        private HorizontalAlignment horizontalContentAlignment = HorizontalAlignment.Stretch;
        private VerticalAlignment verticalContentAlignment = VerticalAlignment.Stretch;

        [Obfuscation(Exclude = true)]
        public event EventHandler ValueChanged;

        public string TabName
        {
            get { return tabName; }
            set
            {
                if (tabName == value)
                    return;
                tabName = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }

        public double ButtonWidth
        {
            get { return buttonWidth; }
            set
            {
                if (buttonWidth == value)
                    return;
                buttonWidth = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }

        }

        public double TabWidth
        {
            get { return tabWidth; }
            set
            {
                if (tabWidth == value)
                    return;
                tabWidth = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }

        public double TabHeight
        {
            get { return tabHeight; }
            set
            {
                if (tabHeight == value)
                    return;
                tabHeight = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }

        public FrameworkElement DockedControl
        {
            get { return dockedControl; }
            set
            {
                if (dockedControl == value)
                    return;
                dockedControl = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return verticalContentAlignment; }
            set
            {
                if (verticalContentAlignment == value)
                    return;
                verticalContentAlignment = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return horizontalContentAlignment; }
            set
            {
                if (horizontalContentAlignment == value)
                    return;
                horizontalContentAlignment = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// Interaction logic for TabControl.xaml
    /// </summary>
    public partial class TabControl : UserControl
    {
        [Obfuscation(Exclude = true)]
        private event EventHandler IndexChanged;

        private TabDetails tab1, tab2, tab3, tab4, tab5;

        public int Index
        {
            get
            {
                if (buttonTab1.IsChecked == true)
                    return 1;
                if (buttonTab2.IsChecked == true)
                    return 2;
                if (buttonTab3.IsChecked == true)
                    return 3;
                if (buttonTab4.IsChecked == true)
                    return 4;
                if (buttonTab5.IsChecked == true)
                    return 5;
                return 0;
            }
            set
            {
                buttonTab1.IsChecked = (value == 1);
                buttonTab2.IsChecked = (value == 2);
                buttonTab3.IsChecked = (value == 3);
                buttonTab4.IsChecked = (value == 4);
                buttonTab5.IsChecked = (value == 5);
                if (value == 1)
                    ShowDockPanel(buttonTab1);
                else if (value == 2)
                    ShowDockPanel(buttonTab2);
                else if (value == 3)
                    ShowDockPanel(buttonTab3);
                else if (value == 4)
                    ShowDockPanel(buttonTab4);
                else if (value == 5)
                    ShowDockPanel(buttonTab5);
                else
                    throw new NotImplementedException("Valid values: 1-5");
            }
        }

        public TabDetails Tab1
        {
            get { return tab1; }
            set
            {
                tab1 = value;
                if (value != null)
                {
                    buttonTab1.Visibility = Visibility.Visible;
                    tab1.ValueChanged += new EventHandler(tab1_ValueChanged);
                }
                else
                {
                    buttonTab1.Visibility = Visibility.Collapsed;
                    //scrollViewerDockPanel1.Visibility = Visibility.Collapsed;
                    dockPanel1.Visibility = Visibility.Collapsed;
                }
                if (buttonTab1.IsChecked == true)
                    ShowDockPanel(buttonTab1);
            }
        }

        [Obfuscation(Exclude = true)]
        void tab1_ValueChanged(object sender, EventArgs e)
        {
            if (tab1.TabWidth > 0)
                dockPanel1.Width = tab1.TabWidth;
            if (tab1.TabHeight > 0)
                dockPanel1.Height = tab1.TabHeight;
            dockPanel1.HorizontalAlignment = tab1.HorizontalContentAlignment;
            dockPanel1.VerticalAlignment = tab1.VerticalContentAlignment;
            buttonTab1.Content = tab1.TabName;
            buttonTab1.Width = tab1.ButtonWidth;
            if (tab1.DockedControl != null)
            {
                dockPanel1.Children.Clear();
                dockPanel1.Children.Add(tab1.DockedControl);
            }
        }

        public TabDetails Tab2
        {
            get { return tab2; }
            set
            {
                tab2 = value;
                if (value != null)
                {
                    buttonTab2.Visibility = Visibility.Visible;
                    tab2.ValueChanged += new EventHandler(tab2_ValueChanged);
                }
                else
                {
                    buttonTab2.Visibility = Visibility.Collapsed;
                    //scrollViewerDockPanel2.Visibility = Visibility.Collapsed;
                    dockPanel2.Visibility = Visibility.Collapsed;
                }
                if (buttonTab2.IsChecked == true)
                    ShowDockPanel(buttonTab2);
            }
        }

        [Obfuscation(Exclude = true)]
        void tab2_ValueChanged(object sender, EventArgs e)
        {
            if (tab2.TabWidth > 0)
                dockPanel2.Width = tab2.TabWidth;
            if (tab2.TabHeight > 0)
                dockPanel2.Height = tab2.TabHeight;
            dockPanel2.HorizontalAlignment = tab2.HorizontalContentAlignment;
            dockPanel2.VerticalAlignment = tab2.VerticalContentAlignment;
            buttonTab2.Content = tab2.TabName;
            buttonTab2.Width = tab2.ButtonWidth;
            if (tab2.DockedControl != null)
            {
                dockPanel2.Children.Clear();
                dockPanel2.Children.Add(tab2.DockedControl);
            }
        }

        public TabDetails Tab3
        {
            get { return tab3; }
            set
            {
                tab3 = value;
                if (value != null)
                {
                    buttonTab3.Visibility = Visibility.Visible;
                    tab3.ValueChanged += new EventHandler(tab3_ValueChanged);
                }
                else
                {
                    buttonTab3.Visibility = Visibility.Collapsed;
                    //scrollViewerDockPanel3.Visibility = Visibility.Collapsed;
                    dockPanel3.Visibility = Visibility.Collapsed;
                }
                if (buttonTab3.IsChecked == true)
                    ShowDockPanel(buttonTab3);
            }
        }

        [Obfuscation(Exclude = true)]
        void tab3_ValueChanged(object sender, EventArgs e)
        {
            if (tab3.TabWidth > 0)
                dockPanel3.Width = tab3.TabWidth;
            if (tab3.TabHeight > 0)
                dockPanel3.Height = tab3.TabHeight;
            dockPanel3.HorizontalAlignment = tab3.HorizontalContentAlignment;
            dockPanel3.VerticalAlignment = tab3.VerticalContentAlignment;
            buttonTab3.Content = tab3.TabName;
            buttonTab3.Width = tab3.ButtonWidth;
            if (tab3.DockedControl != null)
            {
                dockPanel3.Children.Clear();
                dockPanel3.Children.Add(tab3.DockedControl);
            }
        }

        public TabDetails Tab4
        {
            get { return tab4; }
            set
            {
                tab4 = value;
                if (value != null)
                {
                    buttonTab4.Visibility = Visibility.Visible;
                    tab4.ValueChanged += new EventHandler(tab4_ValueChanged);
                }
                else
                {
                    buttonTab4.Visibility = Visibility.Collapsed;
                    //scrollViewerDockPanel4.Visibility = Visibility.Collapsed;
                    dockPanel4.Visibility = Visibility.Collapsed;
                }
                if (buttonTab4.IsChecked == true)
                    ShowDockPanel(buttonTab4);
            }
        }

        [Obfuscation(Exclude = true)]
        void tab4_ValueChanged(object sender, EventArgs e)
        {
            if (tab4.TabWidth > 0)
                dockPanel4.Width = tab4.TabWidth;
            if (tab4.TabHeight > 0)
                dockPanel4.Height = tab4.TabHeight;
            dockPanel4.HorizontalAlignment = tab4.HorizontalContentAlignment;
            dockPanel4.VerticalAlignment = tab4.VerticalContentAlignment;
            buttonTab4.Content = tab4.TabName;
            buttonTab4.Width = tab4.ButtonWidth;
            if (tab4.DockedControl != null)
            {
                dockPanel4.Children.Clear();
                dockPanel4.Children.Add(tab4.DockedControl);
            }
        }

        public TabDetails Tab5
        {
            get { return tab5; }
            set
            {
                tab5 = value;
                if (value != null)
                {
                    buttonTab5.Visibility = Visibility.Visible;
                    tab5.ValueChanged += new EventHandler(tab5_ValueChanged);
                }
                else
                {
                    buttonTab5.Visibility = Visibility.Collapsed;
                    //scrollViewerDockPanel5.Visibility = Visibility.Collapsed;
                    dockPanel5.Visibility = Visibility.Collapsed;
                }
                if (buttonTab5.IsChecked == true)
                    ShowDockPanel(buttonTab5);
            }
        }

        [Obfuscation(Exclude = true)]
        void tab5_ValueChanged(object sender, EventArgs e)
        {
            if (tab5.TabWidth > 0)
                dockPanel5.Width = tab5.TabWidth;
            if (tab1.TabHeight > 0)
                dockPanel5.Height = tab5.TabHeight;
            dockPanel5.HorizontalAlignment = tab5.HorizontalContentAlignment;
            dockPanel5.VerticalAlignment = tab5.VerticalContentAlignment;
            buttonTab5.Content = tab5.TabName;
            buttonTab5.Width = tab5.ButtonWidth;
            if (tab5.DockedControl != null)
            {
                dockPanel5.Children.Clear();
                dockPanel5.Children.Add(tab5.DockedControl);
            }
        }

        public TabControl()
        {
            InitializeComponent();
            contentGridControl.VerticalAlignment = VerticalAlignment.Top;
        }

        [Obfuscation(Exclude = true)]
        private void buttonTab_Click(object sender, RoutedEventArgs e)
        {
            buttonTab1.IsChecked = (sender == buttonTab1);
            buttonTab2.IsChecked = (sender == buttonTab2);
            buttonTab3.IsChecked = (sender == buttonTab3);
            buttonTab4.IsChecked = (sender == buttonTab4);
            buttonTab5.IsChecked = (sender == buttonTab5);
            if (IndexChanged != null)
                IndexChanged.Invoke(this, new EventArgs());
            ShowDockPanel(sender);
        }

        private void ShowDockPanel(object sender)
        {
            /*
            scrollViewerDockPanel1.Visibility = ((sender == buttonTab1) ?
                Visibility.Visible : Visibility.Collapsed);
            scrollViewerDockPanel2.Visibility = ((sender == buttonTab2) ?
                Visibility.Visible : Visibility.Collapsed);
            scrollViewerDockPanel3.Visibility = ((sender == buttonTab3) ?
                Visibility.Visible : Visibility.Collapsed);
            scrollViewerDockPanel4.Visibility = ((sender == buttonTab4) ?
                Visibility.Visible : Visibility.Collapsed);
            scrollViewerDockPanel5.Visibility = ((sender == buttonTab5) ?
                Visibility.Visible : Visibility.Collapsed);
             */
            dockPanel1.Visibility = ((sender == buttonTab1) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel2.Visibility = ((sender == buttonTab2) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel3.Visibility = ((sender == buttonTab3) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel4.Visibility = ((sender == buttonTab4) ?
                Visibility.Visible : Visibility.Collapsed);
            dockPanel5.Visibility = ((sender == buttonTab5) ?
                Visibility.Visible : Visibility.Collapsed);
        }

        private void SetTab(TextBlockButton buttonTab, double buttonWidth,
            FrameworkElement tab, string tabTitle, int index)
        {
            if (tabTitle == null)
            {
                buttonTab.Visibility = Visibility.Collapsed;
                buttonTab.Content = "";
                return;
            }
            buttonTab.Width = buttonWidth;
            buttonTab.Visibility = Visibility.Visible;
            buttonTab.Content = tabTitle;
            if (index == 1)
                dockPanel1.Children.Add(tab);
            if (index == 2)
                dockPanel2.Children.Add(tab);
            if (index == 3)
                dockPanel3.Children.Add(tab);
            if (index == 4)
                dockPanel4.Children.Add(tab);
            if (index == 5)
                dockPanel5.Children.Add(tab);
        }


        public void SetTabVisibility(int index, Visibility visibility)
        {
            switch (index)
            {
                case 1:
                    buttonTab1.Visibility = visibility;
                    UpdateSelected(index, visibility);
                    break;
                case 2:
                    buttonTab2.Visibility = visibility;
                    UpdateSelected(index, visibility);
                    break;
                case 3:
                    buttonTab3.Visibility = visibility;
                    UpdateSelected(index, visibility);
                    break;
                case 4:
                    buttonTab4.Visibility = visibility;
                    UpdateSelected(index, visibility);
                    break;
                case 5:
                    buttonTab5.Visibility = visibility;
                    UpdateSelected(index, visibility);
                    break;
            }
        }

        private void UpdateSelected(int index, Visibility visibility)
        {
            if ((Index != index) || (visibility == Visibility.Visible))
                return;
            if (index == 1)
            {
                index = 2;
                if (buttonTab2.Visibility != Visibility.Visible)
                {
                    index = 3;
                    if (buttonTab3.Visibility != Visibility.Visible)
                    {
                        index = 4;
                        if (buttonTab4.Visibility != Visibility.Visible)
                            index = 5;
                    }
                }
                Index = index;
            }
            else if (index == 2)
            {
                index = 1;
                if (buttonTab1.Visibility != Visibility.Visible)
                {
                    index = 3;
                    if (buttonTab3.Visibility != Visibility.Visible)
                    {
                        index = 4;
                        if (buttonTab4.Visibility != Visibility.Visible)
                            index = 5;
                    }
                }
                Index = index;
            }
            else if (index == 3)
            {
                index = 2;
                if (buttonTab2.Visibility != Visibility.Visible)
                {
                    index = 1;
                    if (buttonTab1.Visibility != Visibility.Visible)
                    {
                        index = 4;
                        if (buttonTab4.Visibility != Visibility.Visible)
                            index = 5;
                    }
                }
                Index = index;
            }
            else if (index == 4)
            {
                index = 3;
                if (buttonTab3.Visibility != Visibility.Visible)
                {
                    index = 2;
                    if (buttonTab2.Visibility != Visibility.Visible)
                    {
                        index = 1;
                        if (buttonTab1.Visibility != Visibility.Visible)
                            index = 5;
                    }
                }
                Index = index;
            }
            else if (index == 5)
            {
                index = 4;
                if (buttonTab4.Visibility != Visibility.Visible)
                {
                    index = 3;
                    if (buttonTab3.Visibility != Visibility.Visible)
                    {
                        index = 2;
                        if (buttonTab2.Visibility != Visibility.Visible)
                            index = 1;
                    }
                }
                Index = index;
            }
        }
    }
}
