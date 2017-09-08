using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PosControls.Helpers
{
    public class Logger
    {
        private StreamWriter writer;
        private FileStream file;

        static private Logger loggerInstance = null;
        static private string logFileName =
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + "log.txt";

        public static string LogFileName
        {
            get
            {
                return logFileName;
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
            loggerInstance = new Logger();
        }

        private Logger()
        {
            try
            {
                file = File.Open(logFileName, FileMode.Append);
                writer = new StreamWriter(file);
            }
            catch
            {
                
            }
        }

        static public void OpenLog()
        {
            if (loggerInstance != null)
                CloseLog();
            loggerInstance = new Logger();
        }

        static public void CloseLog()
        {
            if (loggerInstance == null)
                return;
            if (loggerInstance.writer != null)
                loggerInstance.writer.Close();
            if (loggerInstance.file != null)
                loggerInstance.file.Close();
            loggerInstance.writer = null;
            loggerInstance.file = null;
            loggerInstance = null;
        }

        static public void WriteLog(string text)
        {
            if ((loggerInstance == null) || (loggerInstance.writer == null))
                OpenLog();
            if (loggerInstance.writer != null)
            {
                loggerInstance.writer.WriteLine(DateTime.Now + ": " + text);
                loggerInstance.writer.Flush();
            }
        }
    }
}
