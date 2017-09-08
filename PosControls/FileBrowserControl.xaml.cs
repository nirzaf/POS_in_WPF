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
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PosControls
{
    /// <summary>
    /// Interaction logic for FileBrowserControl.xaml
    /// </summary>
    public partial class FileBrowserControl : UserControl
    {
        #region Licensed Access Only
        static FileBrowserControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(FileBrowserControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion        

        public DirectoryEntry SelectedDirectoryEntry
        {
            get;
            private set;
        }

        public FileBrowserControl()
        {
            SelectedDirectoryEntry = null;
            InitializeComponent();
        }

        [Obfuscation(Exclude = true)]
        void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayDriveList();
        }

        private void DisplayDriveList()
        {
            textBlockPath.Text = "My Computer";            
            foreach (string s in Directory.GetLogicalDrives())
            {
                DriveInfo driveInfo = new DriveInfo(s);
                string name = "";
                try
                {
                    if (driveInfo.VolumeLabel != null)
                        name = driveInfo.VolumeLabel;
                }
                catch { }
                DirectoryEntry d = new DirectoryEntry(s, s, name, null,
                    Directory.GetLastWriteTime(s), "Images/drive.png", EntryType.Dir);
                listBox.Items.Add(new FileBrowserItemControl(d, FileBrowserItemClickCallback));
            }
        }

        void FileBrowserItemClickCallback(DirectoryEntry entry)
        {
            if (entry.Type == EntryType.File)
            {
                SelectedDirectoryEntry = entry;
                Window.GetWindow(this).Close();
                return;
            }
            if (entry.Type == EntryType.Dir)
            {
                listBox.ClearItems();
                if (entry.Fullpath.Length == 0)
                {
                    DisplayDriveList();
                    return;
                }
                DirectoryInfo parentDir =
                    new DirectoryInfo(entry.Fullpath + System.IO.Path.DirectorySeparatorChar + "..");
                string parentFullName = (entry.Fullpath.EndsWith(":" + System.IO.Path.DirectorySeparatorChar) &&                    
                    parentDir.FullName.EndsWith(":" + System.IO.Path.DirectorySeparatorChar) ?
                    "" : parentDir.FullName);
                listBox.Items.Add(new FileBrowserItemControl(
                    new DirectoryEntry("..", parentFullName, "", "<DIR>",
                    Directory.GetLastWriteTime(entry.Fullpath), "Images/folder.png", EntryType.Dir),
                    FileBrowserItemClickCallback));
                try
                {
                    textBlockPath.Text = entry.Fullpath;
                    List<string> folders = new List<string>(Directory.GetDirectories(entry.Fullpath));
                    folders.Sort();
                    foreach (string s in folders)
                    {
                        DirectoryInfo dir = new DirectoryInfo(s);
                        DirectoryEntry d = new DirectoryEntry(
                            dir.Name, dir.FullName, "", "<DIR>",
                            Directory.GetLastWriteTime(s),
                            "Images/folder.png", EntryType.Dir);
                        listBox.Items.Add(new FileBrowserItemControl(d,
                            FileBrowserItemClickCallback));
                    }
                    List<string> files = new List<string>(Directory.GetFiles(entry.Fullpath));
                    files.Sort();
                    foreach (string f in files)
                    {
                        FileInfo file = new FileInfo(f);
                        if (!HasAnImageExtension(file.Extension))
                            continue;
                        DirectoryEntry d = new DirectoryEntry(
                            file.Name, file.FullName, file.Extension, file.Length.ToString(),
                            file.LastWriteTime,
                            "Images/file.png", EntryType.File);
                        listBox.Items.Add(new FileBrowserItemControl(d,
                            FileBrowserItemClickCallback));
                    }
                }
                catch { /* Unauthorized access exception probably */ }
            }
        }

        private bool HasAnImageExtension(string ext)
        {
            if (ext == null)
                return false;
            ext = ext.ToLower();
            return (ext.Equals(".png") ||
                    ext.Equals(".gif") ||
                    ext.Equals(".jpg") ||
                    ext.Equals(".bmp") ||
                    ext.Equals(".tiff"));
        }
    }

    public enum EntryType
    {
        Dir,
        File
    }

    public class DirectoryEntry
    {
        private string _name;
        private string _fullpath;
        private string _ext;
        private string _size;
        private DateTime _date;
        private string _imagepath;
        private EntryType _type;

        public DirectoryEntry(string name,string fullname, string ext, string size, DateTime date, string imagepath, EntryType type)
        {
            _name = name;
            _fullpath = fullname;
            _ext = ext;
            _size = size;
            _date = date;
            _imagepath = imagepath;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public string Ext
        {
            get { return _ext; }
            set { _ext = value; }
        }

        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }
        
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        
        public string Imagepath
        {
            get { return _imagepath; }
            set { _imagepath = value; }
        }

        public EntryType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Fullpath
        {
            get { return _fullpath; }
            set { _fullpath = value; }
        }
    }
}
