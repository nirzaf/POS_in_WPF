using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PosControls.Helpers
{
    public static class StringHelper
    {
        private static string ThinString(string p)
        {
            p = p.Replace("\n", " ");
            while (p.Contains("  "))
            {
                p = p.Replace("  ", " ");
            }
            while (p.StartsWith(" "))
            {
                p = p.Substring(1);
            }
            while (p.EndsWith(" "))
            {
                p = p.Substring(0, p.Length - 1);
            }
            return p;
        }

        public static bool IsNumber(this Key key)
        {
	        if (key < Key.D0 || key > Key.D9)
	        {
	            if (key < Key.NumPad0 || key > Key.NumPad9)
	            {
	                return false;
	            }
	        }
	        return true;
        }

        public static string[] SplitMultiSpaceLine(string text, int padding = 0)
        {
            bool inToken = false;
            List<string> tokens = new List<string>();
            List<char> token = new List<char>();
            for (int i = 0; i < text.Length; i++)
            {
                if (!inToken && text[i] == ' ')
                {
                    continue;
                }
                else if (!inToken && text[i] != ' ')
                {
                    inToken = true;
                    token.Clear();
                }
                else if (inToken && text[i] == ' ')
                {
                    if (((i + 1) < text.Length) && (text[i + 1] == ' '))
                    {
                        inToken = false;
                        tokens.Add(new string(token.ToArray()));
                        continue;
                    }
                }

                token.Add(text[i]);
            }
            if (inToken)
                tokens.Add(new string(token.ToArray()));

            // Add the padding
            for (int i = 0; i < padding; i++)
            {
                tokens.Insert(0, "");
            }

            return tokens.ToArray();
        }

    }
}
