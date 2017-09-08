using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemPOS.Types
{
    public struct PrinterEscapeCodes
    {
#if DEBUG
        public const string ESC = "";
        public const string SetBold = "";
        public const string SetUnderline = "";
        public const string SetItalic = "";
        public const string SetCenter = " ";
        public const string SetRight = " ";
        public const string ResetFormatting = "";
        public static string SetSize(int size)
        {
            string result = "";
            for (int i = 0; i < size; i++)
                result += " ";
            return result;
        }
#else
        public const string ESC = "\x1B|";
        public const string SetBold = ESC + "bC";
        public const string SetUnderline = ESC + "uC";
        public const string SetItalic = ESC + "iC";
        public const string SetCenter = ESC + "cA";
        public const string SetRight = ESC + "rA";
        public const string ResetFormatting = ESC + "N";
        public static string SetSize(int size)
        {
            return ESC + size + "C";
        }
#endif
    }
}
