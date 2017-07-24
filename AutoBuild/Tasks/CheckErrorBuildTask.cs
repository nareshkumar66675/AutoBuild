using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuild.Helper;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Threading;
using BMC.Common.LogManagement;
using BMC.Common.ExceptionManagement;

namespace AutoBuild.Tasks
{
    class CheckErrorBuildTask:BuildTask
    {
        public override int Execute(TaskInfo TaskInfo)
        {
            try
            {
                List<string> dbErrors = CheckDBConsolidationErrors(TaskInfo.Path);
                List<string> buildErrors = CheckBuildErrors(TaskInfo.Path);
                string tempZipPath = Path.Combine(Path.GetTempPath(), "BuildErrors_" + DateTime.Now.ToString("ddMMyyyy") + ".zip");

                if (buildErrors.Count > 0 || dbErrors.Count > 0)
                {
                    BundleErrorfiles(dbErrors.Union(buildErrors).ToList<string>());
                    LogManager.WriteLog("Check Error Task Completed. Found Errors", LogManager.enumLogLevel.Info);
                    return -1;
                }

                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);

                LogManager.WriteLog("Check Error Task Completed. No Errors Found", LogManager.enumLogLevel.Info);
                return 1;
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                LogManager.WriteLog("Error Occured while Checking Error", LogManager.enumLogLevel.Error);
                return -1;
            }       
        }

        private List<string> CheckDBConsolidationErrors(string path)
        {
            string[] fileNames = ConfigurationManager.AppSettings["DBConsolidationErrorFileNames"].Split(';');

            List<string> errorFiles = new List<string>();

            LogManager.WriteLog("Checking for DBConsolidation Error Files Started", LogManager.enumLogLevel.Info);

            foreach (string file in fileNames)
            {
                errorFiles.AddRange(Directory.GetFiles(path, file + "*").ToList<string>());
            }

            LogManager.WriteLog("Checking for DBConsolidation Error Files Completed. Found - "+errorFiles.Count+" Errors.", LogManager.enumLogLevel.Info);

            return errorFiles;
        }

        private List<string> CheckBuildErrors(string path)
        {
            string[] fileNames = ConfigurationManager.AppSettings["BuildErrorFileNames"].Split(';');

            List<string> files = new List<string>();
            List<string> errorFiles = new List<string>();

            LogManager.WriteLog("Checking for Build Error Files Started", LogManager.enumLogLevel.Info);

            foreach (string file in fileNames)
            {
                files.AddRange(Directory.GetFiles(path, file + "*").ToList<string>());
            }

            foreach (string errorFile in files)
            {
                FileInfo info = new FileInfo(errorFile);
                if (info.Length != 0)
                    errorFiles.Add(errorFile);
            }

            LogManager.WriteLog("Checking for Build Error Files Completed. Found - "+errorFiles.Count + " Errors.", LogManager.enumLogLevel.Info);

            return errorFiles;
        }

        private void BundleErrorfiles(List<string> errorFiles)
        {
            string tempPath = Path.GetTempPath();
            string dbConsolidationTemp = Path.Combine(tempPath, "AutoBuild");
            string tempZipPath = Path.Combine(tempPath, "BuildErrors_"+DateTime.Now.ToString("ddMMyyyy")+".zip");

            LogManager.WriteLog("Bundling Error Files Started", LogManager.enumLogLevel.Info);

            if (Directory.Exists(dbConsolidationTemp))
            {
                Directory.Delete(dbConsolidationTemp, true);
            }

            Directory.CreateDirectory(dbConsolidationTemp);
            foreach (string errorFile in errorFiles)
            {
                File.Copy(errorFile, Path.Combine(dbConsolidationTemp, Path.GetFileName(errorFile)), true);
            }

            if (File.Exists(tempZipPath))
                File.Delete(tempZipPath);

            ZipFile.CreateFromDirectory(dbConsolidationTemp, tempZipPath);

            LogManager.WriteLog("Bundling Error Files Completed", LogManager.enumLogLevel.Info);

        }
    }
}
