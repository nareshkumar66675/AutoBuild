using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BMC.Common.LogManagement
{
    public static class LogManager
    {
        static string logPath;
        public enum enumLogLevel : int
        {
            Essential = 0,
            Error = 1,
            Warning = 2,
            Info = 4,
            Debug = 8
            //Raw = 16,
        }
        static LogManager()
        {
            logPath = ConfigurationManager.AppSettings["LogPath"];
            if(File.Exists(logPath))
            {
                if (new FileInfo(logPath).Length > 2000000) // 2Mb
                {
                    string fileName = Path.GetFileName(logPath);
                    string logDirectory = Path.GetDirectoryName(logPath);
                    string oldFileName = Path.Combine(logDirectory, fileName.Replace(Path.GetFileNameWithoutExtension(logPath), Path.GetFileNameWithoutExtension(logPath)+"_OLD"));

                    if (File.Exists(oldFileName))
                        File.Delete(oldFileName);

                    File.Copy(logPath, oldFileName);
                    File.Delete(logPath);
                }
            }

            File.AppendAllText(logPath, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff") + "\t" + "Start Of Log"+ "\n");
        }

        public static void WriteLog(string message, enumLogLevel level)
        {
            try
            {
                int iteration = 0;
                while (IsFileInUse(logPath) && iteration < 50)
                {
                    iteration++;
                    Thread.Sleep(100);
                }
                File.AppendAllText(logPath, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff") + "\t" + message + "\n");
            }
            catch (Exception)
            {
                //Do Nothing
            }
        }
        private static bool IsFileInUse(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read)) { }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}
