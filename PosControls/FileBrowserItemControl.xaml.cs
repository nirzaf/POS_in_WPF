using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.IO;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for FileBrowserItemControl.xaml
    /// </summary>
    public partial class FileBrowserItemControl : UserControl, ISelectable
    {
        private bool isSelected = false;
        private DirectoryEntry directoryEntry;

        public delegate void DirectoryEntryCallback(DirectoryEntry entry);
        public delegate void FileBrowserItemSelectedCallback(FileBrowserItemControl item);

        public bool IsSelectable
        {
            get { return true; }
            set { }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                borderControl.Background = (!isSelected ?
                    ConfigurationManager.ListItemBackgroundBrush :
                    ConfigurationManager.ListItemSelectedBackgroundBrush);
                textBlockVolumnName.Foreground =
                    textBlockFileName.Foreground = (!isSelected ?
                    ConfigurationManager.ListItemForegroundBrush :
                    ConfigurationManager.ListItemSelectedForegroundBrush);
            }
        }

        public DirectoryEntryCallback ClickCallback
        {
            get;
            private set;
        }

        public DirectoryEntry DirectoryEntry
        {
            get { return directoryEntry; }
            private set
            {
                directoryEntry = value;
                textBlockFileName.Text = directoryEntry.Name;
                textBlockVolumnName.Text = ((directoryEntry.Size == null) ? 
                    directoryEntry.Ext : "");
                iconImage.Source = GetIcon(directoryEntry.Imagepath);
            }
        }

        public FileBrowserItemControl(DirectoryEntry entry,
            DirectoryEntryCallback clickCallback)
        {
            InitializeComponent();
            DirectoryEntry = entry;
            ClickCallback = clickCallback;
        }

        [Obfuscation(Exclude = true)]
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsSelected)
            {
                e.Handled = true;
                ClickCallback(DirectoryEntry);
            }
        }

        private ImageSource GetIcon(string imagePath)
        {
            Uri iconUri = new Uri("pack://application:,,,/Resources/" + imagePath, UriKind.Absolute);
            try
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = iconUri;
                img.EndInit();
                return img;
            }
            catch
            {
                return null;
            }
        }
    }
}
