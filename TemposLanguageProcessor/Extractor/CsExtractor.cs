using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Threading;

namespace TemposLanguageProcessor.Extractor
{
    public class CsExtractor : Extractor
    {
        public const string UsingStatment = "using PointOfSale.Types;";

        public static void Start()
        {
            foreach (string filename in GetFiles())
            {
                StartFile(filename);
            }
            MainWindow.WriteLine("(Done)");
        }

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
            return (file.ToLower().EndsWith(".cs") &&
                !file.ToLower().EndsWith(".g.cs") &&
                !file.ToLower().EndsWith(".g.i.cs") &&
                !file.Contains(@"\Strings.") &&
                !file.Contains(@"\Properties\"));
        }

        private static void StartFile(string filename)
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

            string fileText;
            bool isCanceled;
            StringCollection resultList = FindMatches(filename, out fileText, out isCanceled);
            if ((resultList.Count == 0) || isCanceled)
            {
                if (isCanceled)
                    MainWindow.WriteLine();
                return;
            }

            if (!string.IsNullOrEmpty(fileText))
            {
                string regionName = GetRegionName(filename);
                var allLinesStringsEnglish = File.ReadAllLines(StringsEnglishPath).ToList();
                int? insertionLineNumberStringsEnglish = GetRegionLineNumber(allLinesStringsEnglish, regionName) + 1;
                var allLinesStrings = File.ReadAllLines(StringsPath).ToList();
                int? insertionLineNumberStrings = GetRegionLineNumber(allLinesStrings, regionName) + 1;

                // Replace strings with variable names
                foreach (string str in resultList)
                {
                    string replacementText = GetVariableName(str);
                    fileText = fileText.Replace(str, replacementText);
                    insertionLineNumberStringsEnglish =
                        AddToStringsEnglish(allLinesStringsEnglish, insertionLineNumberStringsEnglish, filename, str, replacementText);
                    insertionLineNumberStrings =
                        AddToStrings(allLinesStrings, insertionLineNumberStrings, filename, str, replacementText);
                }

                File.WriteAllLines(StringsEnglishPath, allLinesStringsEnglish.ToArray());
                File.WriteAllLines(StringsPath, allLinesStrings.ToArray());


                // Write changes
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.ASCII))
                {
                    writer.Write(fileText);
                    writer.Flush();
                }

                // Add Using
                var allLines = File.ReadAllLines(filename).ToList();
                AddUsing(allLines);
                File.WriteAllLines(filename, allLines.ToArray());
            }
        }

        private static void AddUsing(List<string> allLines)
        {
            int lastUsingLine = 0;
            for (int i = 0; i < allLines.Count; i++)
            {
                if (allLines[i].TrimEnd().Equals(UsingStatment))
                    return;
                if ((i < allLines.Count - 1) && allLines[i + 1].StartsWith("using"))
                    lastUsingLine = i;
            }
            allLines.Insert(lastUsingLine, UsingStatment);
        }

        private static StringCollection FindMatches(string filename, out string fileText, out bool canceled)
        {
            StringCollection processedList = new StringCollection();
            StringCollection resultList = new StringCollection();
            fileText = "";
            bool localCanceled = false;
            canceled = localCanceled;

            using (StreamReader reader = new StreamReader(filename))
            {
                fileText = reader.ReadToEnd();
                Regex regex = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");
                Match matchResult = regex.Match(fileText);
                while (matchResult.Success)
                {
                    string varName = GetVariableName(matchResult.Value);
                    if (processedList.Contains(matchResult.Value) ||
                        string.IsNullOrWhiteSpace(varName) ||
                        matchResult.Value.Replace(" ", "").Equals("\"\""))
                    {
                        matchResult = matchResult.NextMatch();
                        continue;
                    }
                    processedList.Add(matchResult.Value);
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
                    MainWindow.Cancel += (sender, args) =>
                    {
                        MainWindow.ClearEventHandlers();
                        localCanceled = true;
                        waiter.Set();
                    };
                    MainWindow.Write("Extract " + matchResult.Value + " (y/N)? ");
                    waiter.WaitOne();
                    if (localCanceled)
                    {
                        canceled = true;
                        return new StringCollection();
                    }
                    if (add)
                        resultList.Add(matchResult.Value);
                    MainWindow.WriteLine();
                    matchResult = matchResult.NextMatch();
                }
            }
            return resultList;
        }
    }
}
