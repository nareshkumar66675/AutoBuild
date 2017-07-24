using AutoBuild.Helper;
using BMC.Common.ExceptionManagement;
using BMC.Common.LogManagement;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AutoBuild.Tasks
{
    public class BuildTask
    {
        /// <summary>
        /// Executes a Given Task ID
        /// </summary>
        /// <param name="TaskInfo">Task Object</param>
        /// <returns>Return Code of Process</returns>
        public virtual int Execute(TaskInfo TaskInfo)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.WorkingDirectory = Path.GetDirectoryName(TaskInfo.Path);
                processInfo.CreateNoWindow = false;
                processInfo.FileName = Path.GetFileName(TaskInfo.Path);

                using (Process process = Process.Start(processInfo))
                {
                    LogManager.WriteLog("Started Task - " + TaskInfo.Name + "Start Time - " + process.StartTime, LogManager.enumLogLevel.Debug);
                    process.WaitForExit();
                    LogManager.WriteLog(TaskInfo.Name + " - Task Completed - Elapsed Time : " + (process.ExitTime - process.StartTime), LogManager.enumLogLevel.Debug);
                    return process.ExitCode;
                }
            }
            catch (Exception ex)

            {
                ExceptionManager.Publish(ex);
                LogManager.WriteLog("Error Occured while Executing Build Task", LogManager.enumLogLevel.Error);
                return -1;
            }
        }
    }
}
