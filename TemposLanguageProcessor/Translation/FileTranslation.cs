using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TemposLanguageProcessor.Types;

namespace TemposLanguageProcessor.Translation
{
    public class FileTranslation
    {
        private int processLineIndex;
        private List<string> _lines = new List<string>();

        public event EventHandler Finished;

        public string SourceFile
        {
            get;
            private set;
        }
        
        public Languages SourceLanguage
        {
            get;
            private set;
        }
        
        public string TargetFile
        {
            get;
            private set;
        }

        public Languages TargetLanguage
        {
            get;
            private set;
        }

        public Window Window
        {
            get;
            private set;
        }

        public FileTranslation(string sourceFile, Languages sourceLanguage,
            string targetFile, Languages targetLanguage)
        {
            SourceFile = sourceFile;
            SourceLanguage = sourceLanguage;
            TargetFile = targetFile;
            TargetLanguage = targetLanguage;
            GoogleTranslation.TranslationCompleted += GoogleTranslation_TranslationCompleted;
            CreateWindow();
        }

        public void Start()
        {
            ConsoleWriteLine("Checking \'" + SourceFile + "\'...");
            int line = 0;
            using (StreamReader streamReader = new StreamReader(SourceFile))
            {
                while (!streamReader.EndOfStream)
                {
                    line++;
                    string lineText = streamReader.ReadLine();
                    if (lineText == null) continue;
                    lineText = lineText.Replace(SourceLanguage.ToString(), TargetLanguage.ToString());
                    _lines.Add(lineText);
                    //if (lineText.Contains("\"") && lineText.Contains(".Add"))
                    //    ConsoleWriteLine(line + ": " + GetSourceText(lineText));
                    //else
                    //    ConsoleWriteLine(line + ": " + lineText);
                }
            }
            ProcessLines();
        }

        private void ProcessLines()
        {
            if (processLineIndex >= _lines.Count)
            {
                File.WriteAllLines(TargetFile, _lines);
                if (Finished != null)
                    Finished.Invoke(this, new EventArgs());
                return;
            }
            string currentLineText = _lines[processLineIndex];
            if (currentLineText.Contains("\"") && currentLineText.Contains(".Add"))
            {
                string sourceText = GetSourceText(currentLineText);
                GoogleTranslation.Translate(sourceText, SourceLanguage, TargetLanguage);
            }
            else
            {
                ConsoleWriteLine(processLineIndex + ": " + _lines[processLineIndex]);
                processLineIndex++;
                ProcessLines();
            }
        }

        void GoogleTranslation_TranslationCompleted(object sender, EventArgs e)
        {
            _lines[processLineIndex] = RewriteLine(_lines[processLineIndex]);
            ConsoleWriteLine(processLineIndex + ": " + _lines[processLineIndex]);
            processLineIndex++;
            ProcessLines();
        }

        private string RewriteLine(string text)
        {
            bool blocking = false;
            int quoteCount = 0;
            string result = "";
            foreach (char ch in text)
            {
                if (ch == '\"') quoteCount++;
                if (!blocking && (quoteCount == 3))
                {
                    blocking = true;
                    result += ch + GoogleTranslation.TranslatedText;
                    continue;
                }
                if (quoteCount == 4) blocking = false;                
                if (!blocking) result += ch;
            }
            return result;
        }

        private string GetSourceText(string currentLineText)
        {
            bool capture = false;
            int quoteCount = 0;
            string result = "";
            foreach (char ch in currentLineText)
            {
                if (ch == '\"') quoteCount++;
                if (!capture && (quoteCount == 3)) capture = true;
                else if (quoteCount == 4) break;
                else if (capture) result += ch;
            }
            return result;
        }

        private TextBox textBox;
        public void ConsoleWriteLine(string text)
        {
            textBox.Text += text + Environment.NewLine;
        }

        private void CreateWindow()
        {
            textBox = new TextBox();
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            Window = new Window();
            Window.Content = textBox;
            Window.Width = 700;
            Window.Height = 500;
            Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

    }
}
