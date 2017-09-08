using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TemPOS.Helpers
{
    public class Logger
    {
        private StreamWriter _writer;
        private FileStream _file;

        static private Logger _loggerInstance = null;
        static private readonly string _logFileName =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
            Path.DirectorySeparatorChar + "TemPOS" + Path.DirectorySeparatorChar + "log.txt";

        public static string LogFileName
        {
            get
            {
                return _logFileName;
            }
        }

        public static bool LogFileExists
        {
            get
            {
                return (File.Exists(Logger.LogFileName));
            }
        }

        static Logger()
        {
            _loggerInstance = new Logger();
        }

        private Logger()
        {
            try
            {
                _file = File.Open(_logFileName, FileMode.Append);
                _writer = new StreamWriter(_file);
            }
            catch
            {
                
            }
        }

        static public void OpenLog()
        {
            if (_loggerInstance != null)
                CloseLog();
            _loggerInstance = new Logger();
        }

        static public void CloseLog()
        {
            if (_loggerInstance == null)
                return;
            if (_loggerInstance._writer != null)
                _loggerInstance._writer.Close();
            if (_loggerInstance._file != null)
                _loggerInstance._file.Close();
            _loggerInstance._writer = null;
            _loggerInstance._file = null;
            _loggerInstance = null;
        }

        static public void WriteLog(string text)
        {
            if ((_loggerInstance == null) || (_loggerInstance._writer == null))
                OpenLog();
            if ((_loggerInstance == null) || (_loggerInstance._writer == null))
                return;
            _loggerInstance._writer.WriteLine(DateTime.Now + ": " + text);
            _loggerInstance._writer.Flush();
        }
    }
}
