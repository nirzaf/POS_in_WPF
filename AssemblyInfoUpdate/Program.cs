using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace AssemblyInfoUpdate
{
    static class Program
    {
        private static readonly string[] _Projects = new string[] { "PosControls", "PosModels", "SQL Procedures", "TemPOS", "TemposLibrary", "TemposWebApplication" };
        private const string _SolutionFolder = @"F:\Programming\TemPOS";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            foreach (string projectName in _Projects)
            {
                SetBuild(_SolutionFolder + "\\" + projectName + "\\Properties\\AssemblyInfo.cs", "1.0.651");
            }
            return 0;
        }

        // [assembly: AssemblyVersion("1.0.*")]
        // [assembly: AssemblyFileVersion("1.0.*")]
 
        static void SetBuild(string fileName, string build)
        {
            if (!File.Exists(fileName))
                throw new Exception("File not found");
            string text = File.ReadAllText(fileName);
            text = ReplaceBuild(text, "AssemblyVersion(\"", build);
            text = ReplaceBuild(text, "AssemblyFileVersion(\"", build);
            File.WriteAllText(fileName, text);
        }
    

        static string ReplaceBuild(string text, string startText, string build)
        {

            int indexOf = text.LastIndexOf(startText) - startText.Length;
            string preText = text.Substring(0, indexOf);
            string endText = "\")]";
            string postText = text.Substring(indexOf);
            int endIndex = postText.IndexOf(endText);
            postText = postText.Substring(endIndex);

            return preText + build + postText;
        }
    }
}
