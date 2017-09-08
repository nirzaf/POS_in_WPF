using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using mshtml;
using TemposLanguageProcessor.Types;

namespace TemposLanguageProcessor.Translation
{
    public class GoogleTranslation
    {
        private static System.Windows.Forms.WebBrowser webBrowser;
        private static bool _navigated;

        public static event EventHandler TranslationCompleted;

        public static bool IsTranslating
        {
            get;
            private set;
        }

        public static string OriginalText
        {
            get;
            private set;
        }

        public static string TranslatedText
        {
            get;
            private set;
        }

        static GoogleTranslation()
        {
            webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.StatusTextChanged += webBrowser_StatusTextChanged;
        }

        public static void Translate(string sourceText, Languages sourceLanguage,
            Languages targetLanguage)
        {
            if (IsTranslating) return;
            string serverUri = "http://translate.google.com/#" +
                sourceLanguage.GetAbbreviatedCode() + "/" +
                targetLanguage.GetAbbreviatedCode() +
                "/" + EscapeUriString(sourceText); 
            OriginalText = sourceText;
            IsTranslating = true;
            _navigated = false;
            webBrowser.Navigate(serverUri);
        }

        private static string EscapeUriString(string sourceText)
        {
            return Uri.EscapeUriString(sourceText).Replace("?", "%3F");
        }

        static void webBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            _navigated = true;
        }

        static void webBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            if (!IsTranslating || !_navigated) return;
            string currentElementText = GetTranslatedText();
            if (!string.IsNullOrWhiteSpace(currentElementText))
            {
                _navigated = false;
                IsTranslating = false;
                TranslatedText = currentElementText;
                if (TranslationCompleted != null)
                    TranslationCompleted.Invoke(null, new EventArgs());
            }
        }

        private static string GetTranslatedText()
        {
            if (webBrowser.Document == null) return null;

            IHTMLDocument2 htmlDocument = webBrowser.Document.DomDocument as IHTMLDocument2;
            if (htmlDocument == null) return null;
            
            string text = "";
            foreach (IHTMLElement element in htmlDocument.all)
            {
                if (!element.ToString().Equals("mshtml.HTMLHtmlElementClass")) continue;
                if (!string.IsNullOrWhiteSpace(element.innerText))
                {
                    string tempText = element.innerText;
                    string searchText = "Alpha" + Environment.NewLine + Environment.NewLine +
                                        Environment.NewLine + Environment.NewLine;
                    int startIndex = tempText.IndexOf(searchText);
                    if (startIndex >= 0)
                    {
                        tempText = tempText.Substring(startIndex + searchText.Length);
                        int endOfLine = tempText.IndexOf("\n");
                        text += tempText.Substring(0, endOfLine - 1);
                    }
                }
            }

            return text;
        }

        public static void DoLanguageTranslations()
        {
            var rootPath = @"K:\Viipe.com\TemPOS";
            var fileTranslation = new FileTranslation(
                rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                rootPath + @"\PointOfSale\Types\Strings.Spanish.cs", Languages.Spanish);
            fileTranslation.Finished += (sender, args) =>
            {
                fileTranslation = new FileTranslation(
                    rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                    rootPath + @"\PointOfSale\Types\Strings.French.cs", Languages.French);
                fileTranslation.Finished += (sender1, args1) =>
                {
                    fileTranslation = new FileTranslation(
                        rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                        rootPath + @"\PointOfSale\Types\Strings.German.cs", Languages.German);
                    fileTranslation.Finished += (sender2, args2) =>
                    {
                        fileTranslation = new FileTranslation(
                            rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                            rootPath + @"\PointOfSale\Types\Strings.Italian.cs", Languages.Italian);
                        fileTranslation.Finished += (sender3, args3) =>
                        {
                            fileTranslation = new FileTranslation(
                                rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                                rootPath + @"\PointOfSale\Types\Strings.Dutch.cs", Languages.Dutch);
                            fileTranslation.Finished += (sender4, args4) =>
                            {
                                fileTranslation = new FileTranslation(
                                    rootPath + @"\PointOfSale\Types\Strings.English.cs", Languages.English,
                                    rootPath + @"\PointOfSale\Types\Strings.Portuguese.cs", Languages.Portuguese);
                                fileTranslation.Finished += (sender5, args5) =>
                                {
                                    MessageBox.Show("Finished");
                                };
                            };
                        };
                    };
                };
            };
            fileTranslation.Start();
            //fileTranslation.Window.Show();
            //MessageBox.Show("UI Thread Holder");
            fileTranslation.Window.ShowDialog();
        }

    }
}