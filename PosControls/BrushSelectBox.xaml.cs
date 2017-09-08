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
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for BrushSelectBox.xaml
    /// </summary>
    public partial class BrushSelectBox : UserControl
    {
        #region Licensed Access Only
        static BrushSelectBox()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BrushSelectBox).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        [Obfuscation(Exclude = true)]
        public event EventHandler SelectedBrushChanged;

        public string SelectedBrushName
        {
            get;
            set;
        }

        public Brush SelectedBrush
        {
            get { return borderSwatch.Background; }
            set
            {
                borderSwatch.Background = value;
                if (IsLoaded && (SelectedBrushChanged != null))
                    SelectedBrushChanged.Invoke(this, new EventArgs());
            }
        }

        public BrushSelectBox()
        {
            InitializeComponent();
        }

        public void InitializeBrush(string brushName, Brush selectedBrush)
        {
            borderSwatch.Background = selectedBrush;
            SelectedBrushName = brushName;
        }

        private ContextMenu GetColorSelectContextMenu()
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = Resources.GetEnumerator();
            while (e.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)e.Current;
                string name = entry.Key as string;
                if (name == "colorSelectContextMenu")
                {
                    contextMenu = entry.Value as ContextMenu;
                    contextMenu.Placement = PlacementMode.Top;
                    contextMenu.PlacementTarget = borderSwatch;
                    break;
                }
            }
            return contextMenu;
        }
        
        [Obfuscation(Exclude = true)]
        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            borderSwatch.ContextMenu = GetColorSelectContextMenu();
        }

        private void ShowContentMenu()
        {
            if (borderSwatch.ContextMenu != null)
            {
                borderSwatch.ContextMenu.IsOpen = true;
            }
        }

        [Obfuscation(Exclude = true)]
        private void borderSwatch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowContentMenu();
            e.Handled = true;
        }

    }
}
