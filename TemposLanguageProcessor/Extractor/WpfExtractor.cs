using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using System.Xml;
using System.Windows.Markup;

namespace TemposLanguageProcessor.Extractor
{
    public class WpfExtractor : Extractor
    {
        private static StringCollection _strings = new StringCollection();

        private static IEnumerable<string> GetFiles()
        {
            foreach (string projectPath in ProjectPaths)
            {
                foreach (string file in GetFiles(projectPath))
                {
                    yield return file;
                }
            }
        }

        private static IEnumerable<string> GetFiles(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                foreach (string file in GetFiles(directory))
                {
                    if (IsValidFile(file))
                        yield return file;
                }
            }
            foreach (string file in Directory.GetFiles(path))
            {
                if (IsValidFile(file))
                    yield return file;
            }
        }

        private static bool IsValidFile(string file)
        {
            return (file.ToLower().EndsWith(".xaml") &&
                !file.Contains(@"\Resources\") &&
                !file.Contains(@"\Helpers\"));
        }

        public static void Start()
        {
            foreach (string filename in GetFiles())
            {
                _strings.Clear();
                StartFile(filename);
            }
            MainWindow.WriteLine("(Done)");
        }

        public static void StartFile(string filename)
        {
            MainWindow.Write("Process \"" + filename + "\" (y/N)?");

            bool add = false;
            ManualResetEvent waiter = new ManualResetEvent(false);
            MainWindow.Yes += (sender, args) =>
            {
                MainWindow.ClearEventHandlers();
                add = true;
                waiter.Set();
            };
            MainWindow.No += (sender, args) =>
            {
                MainWindow.ClearEventHandlers();
                waiter.Set();
            };
            waiter.WaitOne();
            MainWindow.WriteLine();
            if (!add) return;
            FindMatches(filename);
            ProcessStringInsertions(filename);            
        }

        private static void ProcessStringInsertions(string filename)
        {
            string regionName = GetRegionName(filename);
            var allLinesStringsEnglish = File.ReadAllLines(StringsEnglishPath).ToList();
            int? insertionLineNumberStringsEnglish = GetRegionLineNumber(allLinesStringsEnglish, regionName) + 1;
            var allLinesStrings = File.ReadAllLines(StringsPath).ToList();
            int? insertionLineNumberStrings = GetRegionLineNumber(allLinesStrings, regionName) + 1;

            foreach (string str in _strings)
            {
                string replacementText = GetVariableName(str);
                string fullString = "\"" + str + "\"";
                insertionLineNumberStringsEnglish =
                    AddToStringsEnglish(allLinesStringsEnglish, insertionLineNumberStringsEnglish, filename, fullString, replacementText);
                insertionLineNumberStrings =
                    AddToStrings(allLinesStrings, insertionLineNumberStrings, filename, fullString, replacementText);
            }

            File.WriteAllLines(StringsEnglishPath, allLinesStringsEnglish.ToArray());
            File.WriteAllLines(StringsPath, allLinesStrings.ToArray());
        }

        private static void FindMatches(string filename)
        {
            XmlDocument document = new XmlDocument();
            //document.PreserveWhitespace = true;
            document.Load(filename);
            XmlElement xmlElement = document.DocumentElement;
            AddNamespace(document, xmlElement);
            ProcessNodes(document, xmlElement);
            if (_strings.Count > 0)
                document.Save(filename);
        }

        private static bool ProcessNodes(XmlDocument document, XmlNode rootXmlNode, int depth = 0)
        {
            bool changesMade = false;
            foreach (XmlNode node in rootXmlNode.ChildNodes)
            {
                bool isText, isHeader, isTabName;
                //MainWindow.WriteLine(":" + node.Name); 

                if (IsValidNode(node.Name, out isText, out isHeader, out isTabName))
                {
                    // Replace existing Content, Header, or Text
                    if (ReplaceAttribute(document, node))
                        changesMade = true;
                }

                // Recursive Call
                if (ProcessNodes(document, node, depth++))
                    changesMade = true;
            }
            return changesMade;
        }

        private static bool ReplaceAttribute(XmlDocument document, XmlNode node)
        {
            XmlAttribute contentAttribute = node.Attributes.GetAttribute("Content");
            XmlAttribute headerAttribute = node.Attributes.GetAttribute("Header");
            XmlAttribute textAttribute = node.Attributes.GetAttribute("Text");
            XmlAttribute tabNameAttribute = node.Attributes.GetAttribute("TabName");
            if (tabNameAttribute != null)
                ReplaceAttribute(document, node, tabNameAttribute);
            else if (contentAttribute != null)
                ReplaceAttribute(document, node, contentAttribute);
            else if (headerAttribute != null)
                ReplaceAttribute(document, node, headerAttribute);
            else if (textAttribute != null)
                ReplaceAttribute(document, node, textAttribute);
            return false;
        }

        private static bool ReplaceAttribute(XmlDocument document, XmlNode node, XmlAttribute attribute)
        {
            if (attribute.Value.Contains("t:Strings") ||
                attribute.Value.Contains("$") ||
                attribute.Value.Contains("{Binding Source={StaticResource strings}"))
                return false;

            MainWindow.Write("Extract " + node.Name + "." + attribute.Name + " Value=\"" + attribute.Value + "\" (b/y/N)?");

            bool extract = false;
            bool bind = false;
            ManualResetEvent waiter = new ManualResetEvent(false);
            MainWindow.YesBind += (sender, args) =>
            {
                MainWindow.ClearEventHandlers();
                bind = true; 
                extract = true;
                waiter.Set();
            };
            MainWindow.Yes += (sender, args) =>
            {
                MainWindow.ClearEventHandlers();
                extract = true;
                waiter.Set();
            };
            MainWindow.No += (sender, args) =>
            {
                MainWindow.ClearEventHandlers();
                waiter.Set();
            };
            waiter.WaitOne();
            MainWindow.WriteLine();
            if (extract)
            {
                if (bind)
                {
                    AddStringsResource(document);
                    ReplaceAttribute(document, node, attribute, true);
                }
                else
                {
                    ReplaceAttribute(document, node, attribute, false);
                }
                return true;
            }
            return false;
        }

        private static void ReplaceAttribute(XmlDocument document, XmlNode node, XmlAttribute attribute, bool isBinding)
        {
            string originalText = attribute.Value;
            string replacementText = GetVariableName(attribute.Value);
            string actualVariableName = replacementText;
            if (actualVariableName.StartsWith("Strings."))
                actualVariableName = actualVariableName.Substring(8);
            if (isBinding)
                attribute.Value = @"{Binding Source={StaticResource strings}, Path=" + actualVariableName + "}";
            else
                attribute.Value = @"{x:Static t:Strings." + actualVariableName + "}";
            if (!_strings.Contains(originalText))
            {
                _strings.Add(originalText);
            }
        }

        private static bool AddStringsResource(XmlDocument document)
        {
            XmlElement xmlElement = document.DocumentElement;
            XmlNode matchNode = null;
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                if (node.Name.EndsWith(".Resources"))
                {
                    matchNode = node;
                    break;
                }
            }
            if (matchNode == null)
                matchNode = BuildResourcesNode(document, xmlElement.Name);
            return AddStringsResource(document, matchNode);
        }

        private static XmlNode BuildResourcesNode(XmlDocument document, string baseElementName)
        {
            string name = baseElementName + ".Resources";
            XmlNode node = document.CreateElement(name, document.DocumentElement.NamespaceURI);
            node.Attributes.RemoveAll();
            node.InnerText = Environment.NewLine + "    ";
            document.DocumentElement.PrependChild(node);
            return node;
        }

        private static bool AddStringsResource(XmlDocument document, XmlNode matchNode)
        {
            if (matchNode.ChildNodes.GetElement("t:Strings") != null)
                return false;
            XmlNode node = document.CreateElement("t:Strings", "clr-namespace:PointOfSale.Types");
            
            XmlAttribute attribute = document.CreateAttribute("x:Key", matchNode.NamespaceURI);
            attribute.Value = "strings";
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("xmlns:x");
            attribute.Value = "http://schemas.microsoft.com/winfx/2006/xaml";
            node.Attributes.Append(attribute);

            matchNode.AppendChild(node);
            return true;
        }

        private static bool IsValidNode(string nodeName, out bool isText, out bool isHeader, out bool isTabName)
        {
            if (nodeName.Equals("Label") ||
                nodeName.Equals("Button"))
            {
                isText = false;
                isHeader = false;
                isTabName = false;
                return true;
            }
            if (nodeName.Equals("GroupBox"))
            {
                isHeader = true;
                isText = false;
                isTabName = false;
                return true;
            }
            if (nodeName.EndsWith(":TextBlockButton") ||
                nodeName.Equals("TextBlock") ||
                nodeName.EndsWith(":PushRadioButton"))
            {
                isHeader = false;
                isText = true;
                isTabName = false;
                return true;
            }
            if (nodeName.EndsWith(":TabDetails"))
            {
                isHeader = false;
                isText = false;
                isTabName = true;
                return true;
            }
            isHeader = false;
            isText = false;
            isTabName = false;
            return false;
        }

        private static void AddNamespace(XmlDocument document, XmlElement xmlElement)
        {
            XmlAttribute attribute = xmlElement.Attributes.GetAttribute("xmlns:t");
            if (attribute == null)
            {                
                XmlAttribute newAttribute = document.CreateAttribute("xmlns:t");
                newAttribute.Value = "clr-namespace:PointOfSale.Types";
                xmlElement.Attributes.Append(newAttribute);
            }
        }
    }
}
