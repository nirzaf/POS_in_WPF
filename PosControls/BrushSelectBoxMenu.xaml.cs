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
    /// Interaction logic for BrushSelectBoxMenu.xaml
    /// </summary>
    public partial class BrushSelectBoxMenu : UserControl
    {
        #region Licensed Access Only
        static BrushSelectBoxMenu()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(BrushSelectBoxMenu).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        #region Static Clipboard
        public static Brush ClipboardBrush
        {
            get;
            private set;
        }
        #endregion

        #region Properties
        public BrushSelectBox UserControl
        {
            get
            {
                DependencyObject depObject =
                    VisualTreeHelper.GetParent(ThisContextMenu.PlacementTarget);
                while (depObject != null)
                {
                    if (depObject is BrushSelectBox)
                        return depObject as BrushSelectBox;
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
                return null;
            }
        }

        public ContextMenu ThisContextMenu
        {
            get
            {
                DependencyObject depObject = VisualTreeHelper.GetParent(this);
                while (depObject != null)
                {
                    if (depObject is ContextMenu)
                        return depObject as ContextMenu;
                    depObject = VisualTreeHelper.GetParent(depObject);
                }
                return null;
            }
        }
        #endregion

        #region Initialization
        public BrushSelectBoxMenu()
        {
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        void SetupContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (ClipboardBrush != null)
                buttonPaste.IsEnabled = true;
        }
        #endregion

        #region Brush Editing
        private void NewSolidColorBrush(bool editExisting)
        {
            SolidColorBrushEditorControl control = new SolidColorBrushEditorControl();
            PosDialogWindow window = new PosDialogWindow(control,
                "Solid Color Brush Editor", 500, 260);
            if (editExisting)
                control.SelectedBrush = UserControl.SelectedBrush as SolidColorBrush;
            if (PosDialogWindow.ShowPosDialogWindow(this, window) != null)
                UserControl.SelectedBrush = control.SelectedBrush;
        }

        private void NewGradientBrush(bool editExisting)
        {
            GradientBrushEditorControl control = new GradientBrushEditorControl();
            PosDialogWindow window = new PosDialogWindow(control,
                "Gradient Brush Editor", 780, 465);
            if (editExisting)
                control.SelectedBrush = UserControl.SelectedBrush as GradientBrush;
            if (PosDialogWindow.ShowPosDialogWindow(this, window) != null)
                UserControl.SelectedBrush = control.SelectedBrush;
        }

        private void NewImageBrush(bool editExisting)
        {
            FileBrowserControl control = new FileBrowserControl();
            PosDialogWindow window = new PosDialogWindow(control,
                "Image Brush File Browser", 650, 440);
            PosDialogWindow.ShowPosDialogWindow(this, window);
            if (control.SelectedDirectoryEntry != null)
            {
                BitmapImage image = null;
                try
                {
                    image = new BitmapImage(
                        new Uri(control.SelectedDirectoryEntry.Fullpath, UriKind.Absolute));
                }
                catch (Exception)
                {
                    PosDialogWindow.ShowDialog(Window.GetWindow(this),
                        "That is not a valid image file", "Error");
                }
                if (image != null)
                    UserControl.SelectedBrush = new ImageBrush(image);
            }
        }

        private void NewVisualBrush(bool p)
        {
            FileBrowserControl control = new FileBrowserControl();
            PosDialogWindow window = new PosDialogWindow(control,
                "Video Brush File Browser", 650, 440);
            PosDialogWindow.ShowPosDialogWindow(this, window);
            if (control.SelectedDirectoryEntry != null)
            {
                MediaElement media = null;
                try
                {
                    media = new MediaElement();
                    media.Source =
                        new Uri(control.SelectedDirectoryEntry.Fullpath, UriKind.Absolute);
                    //if (!media.HasVideo)
                    //    throw new Exception("Not a video");
                }
                catch (Exception)
                {
                    media = null;
                    PosDialogWindow.ShowDialog(Window.GetWindow(this),
                        "That is not a valid video file", "Error");
                }
                if (media != null)
                    UserControl.SelectedBrush = new VisualBrush(media);
            }
        }

        private void NewDrawingBrush(bool editExisting)
        {

        }
        #endregion

        #region Event Handlers
        [Obfuscation(Exclude = true)]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.Opened += SetupContextMenu_Opened;
        }

        [Obfuscation(Exclude = true)]
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            if (UserControl.SelectedBrush == null)
                return;
            if (UserControl.SelectedBrush is SolidColorBrush)
                NewSolidColorBrush(true);
            else if (UserControl.SelectedBrush is GradientBrush)
                NewGradientBrush(true);
            else if (UserControl.SelectedBrush is ImageBrush)
                NewImageBrush(true);
            else if (UserControl.SelectedBrush is DrawingBrush)
                NewDrawingBrush(true);
            else if (UserControl.SelectedBrush is VisualBrush)
                NewVisualBrush(true);
        }

        [Obfuscation(Exclude = true)]
        private void buttonNewSolid_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            NewSolidColorBrush(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonNewGradient_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            NewGradientBrush(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonNewImage_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            NewImageBrush(false);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonNewVisualBrush_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            NewVisualBrush(false);
        }

        [Obfuscation(Exclude = true)]
        private void buttonNewDrawing_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            NewDrawingBrush(false);
        }
        
        [Obfuscation(Exclude = true)]
        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            ClipboardBrush = UserControl.SelectedBrush;
            if (ClipboardBrush == null)
                buttonPaste.IsEnabled = false;
        }

        [Obfuscation(Exclude = true)]
        private void buttonPaste_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            if (ClipboardBrush != null)
                UserControl.SelectedBrush = ClipboardBrush;
        }

        [Obfuscation(Exclude = true)]
        private void buttonResetDefault_Click(object sender, RoutedEventArgs e)
        {
            ThisContextMenu.IsOpen = false;
            if (PosDialogWindow.ShowDialog(
                "Are you sure you want to reset this brush to its default?", "Confirm Reset",
                DialogButtons.YesNo) == DialogButton.Yes)
            {
                UserControl.SelectedBrush =
                    ConfigurationObjectManager.CreateBrush(UserControl.SelectedBrushName, true);
            }
        }
        #endregion

    }
}
