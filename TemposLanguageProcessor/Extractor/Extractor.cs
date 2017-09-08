using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.Xml;

namespace TemposLanguageProcessor.Extractor
{
    public class Extractor
    {
        public const string SolutionPath = @"K:\Viipe.com\TemPOS";
        public const string TypesPath = @"K:\Viipe.com\TemPOS\PointOfSale\Types";
        public const string StringsEnglishPath = @"K:\Viipe.com\TemPOS\PointOfSale\Types\Strings.English.cs";
        public const string StringsPath = @"K:\Viipe.com\TemPOS\PointOfSale\Types\Strings.cs";

        public static readonly string[] ProjectPaths = 
        {
            @"K:\Viipe.com\TemPOS\PointOfSale"//,
            //@"K:\Viipe.com\TemPOS\PosControls"
        };

        protected static string GetRegionName(string filename)
        {
            if (filename.Contains(Path.DirectorySeparatorChar))
                filename = Path.GetFileName(filename);
            if (filename.ToLower().EndsWith(".cs"))
                filename = filename.Substring(0, filename.Length - 3);
            if (filename.ToLower().EndsWith(".xaml"))
                filename = filename.Substring(0, filename.Length - 5);
            return filename;
        }

        protected static string GetVariableName(string str)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo; 
            string preString = textInfo.ToTitleCase(str                
                .Replace(":", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("~", "")
                .Replace("`", "")
                .Replace("\"", "")
                .Replace(";", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("+", "")
                .Replace("-", " ")
                .Replace("=", "")
                .Replace("\\", "")
                .Replace("|", "")
                .Replace("/", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("!", "")
                .Replace("@", "")
                .Replace("#", "Number")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("^", "")
                .Replace("&", "")
                .Replace("*", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("?", "")).Replace(" ", "");
            if ((preString.Length == 0) || string.IsNullOrWhiteSpace(preString.Replace("Number", "")))
                return null;
            if (preString[0] == '0' || preString[0] == '1' || preString[0] == '2' || preString[0] == '3' ||
                preString[0] == '4' || preString[0] == '5' || preString[0] == '6' || preString[0] == '7' ||
                preString[0] == '8' || preString[0] == '9')
                return "Strings.Number" + preString;
            return "Strings." + preString;
        }

        protected static int? AddToStrings(List<string> allLines, int? insertionLineNumber, string filename,
            string str, string replacementText)
        {
            bool insertedRegion = false;
            string variableName = replacementText.Substring(8);

            if (insertionLineNumber == null)
            {
                insertedRegion = true;
                insertionLineNumber = allLines.Count - 2;
                allLines.Insert(insertionLineNumber.Value, "");
                insertionLineNumber++;
                allLines.Insert(insertionLineNumber.Value, "        #region " + GetRegionName(filename));
                insertionLineNumber++;
            }

            allLines.Insert(insertionLineNumber.Value,
                "        public static string " + variableName);
            allLines.Insert(insertionLineNumber.Value + 1, "        {");
            allLines.Insert(insertionLineNumber.Value + 2,
                "            get { return GetString(\"" + variableName + "\"); }");
            allLines.Insert(insertionLineNumber.Value + 3, "        }");
            insertionLineNumber += 4;
            if (insertedRegion)
            {
                allLines.Insert(insertionLineNumber.Value, "        #endregion");
            }
            return insertionLineNumber.Value;
        }

        protected static int AddToStringsEnglish(List<string> allLines, int? insertionLineNumber, string filename,
            string str, string replacementText)
        {
            bool insertedRegion = false;
            string variableName = replacementText.Substring(8);

            if (insertionLineNumber == null)
            {
                insertedRegion = true;
                insertionLineNumber = allLines.Count - 3;
                allLines.Insert(insertionLineNumber.Value, "");
                insertionLineNumber++;
                allLines.Insert(insertionLineNumber.Value, "            #region " + GetRegionName(filename));
                insertionLineNumber++;
            }
            allLines.Insert(insertionLineNumber.Value,
                "            English.Add(\"" + variableName + "\", " + str + ");");
            insertionLineNumber++;
            if (insertedRegion)
            {
                allLines.Insert(insertionLineNumber.Value, "            #endregion");
            }
            return insertionLineNumber.Value;
        }

        protected static int? GetRegionLineNumber(List<string> allLines, string regionName)
        {
            int? result = null;
            for (int i = 0; i < allLines.Count; i++)
            {
                string line = allLines[i].Trim();
                if (line.ToLower().Equals("#region " + regionName.ToLower()))
                    return i;
            }
            return result;
        }

    }

    public static class ExtractorExtensions
    {
        public static XmlAttribute GetAttribute(this XmlAttributeCollection xmlAttributes, string name)
        {
            foreach (XmlAttribute attribute in xmlAttributes)
            {
                if (attribute.Name.Equals(name))
                    return attribute;
            }
            return null;
        }

        public static XmlNode GetElement(this XmlNodeList xmlNodeList, string name)
        {
            foreach (XmlNode node in xmlNodeList)
            {
                if (node.Name.Equals(name))
                    return node;
            }
            return null;
        }
    }
}
