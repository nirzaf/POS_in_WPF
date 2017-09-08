using System;
using System.Windows.Controls;
using System.IO;
using System.Reflection;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for ProgramLogoControl.xaml
    /// </summary>
    public partial class ProgramLogoControl : UserControl
    {
        public ProgramLogoControl()
        {
            InitializeComponent();
            try
            {
                if (File.Exists("pos.gif"))
                    gifImage.StreamSource = new FileStream("pos.gif", FileMode.Open, FileAccess.Read);
                else
                    gifImage.UriSource =
                        new Uri(@"pack://application:,,,/" 
                        + Assembly.GetExecutingAssembly().GetName().Name 
                        + ";component/" 
                        + "Resources/pos.gif", UriKind.Absolute);
            }
            catch
            {
            }
        }
    }
}
