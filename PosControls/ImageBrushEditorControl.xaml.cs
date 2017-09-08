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
    /// Interaction logic for ImageBrushEditorControl.xaml
    /// </summary>
    public partial class ImageBrushEditorControl : UserControl
    {
        #region Licensed Access Only
        static ImageBrushEditorControl()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ImageBrushEditorControl).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
        }
        #endregion

        public ImageBrushEditorControl()
        {
            InitializeComponent();
        }
    }
}
