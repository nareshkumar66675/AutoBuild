using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuild.Helper;
using System.Configuration;
using System.IO;
using BMC.Common.LogManagement;
using BMC.Common.ExceptionManagement;

namespace AutoBuild.Tasks
{
    class CopyInstallerBuildTask:BuildTask
    {
        public override int Execute(TaskInfo TaskInfo)
        {
            string installerSourceFolder = ConfigurationManager.AppSettings["InstallerSourceFolder"];
            string installerDestinationFolder = ConfigurationManager.AppSettings["InstallerDestinationFolder"];
            int daysToRetain ;

            if (int.TryParse(ConfigurationManager.AppSettings["DaysToRetain"], out daysToRetain) == false)
            {
                daysToRetain = 2;
                LogManager.WriteLog("Error in DaysToRetain Setting. Configuring it to Default Value - " +daysToRetain,LogManager.enumLogLevel.Warning);
            }
                
            try
            {
                if (Directory.Exists(installerSourceFolder))
                {
                    if (!Directory.Exists(installerDestinationFolder))
                        Directory.CreateDirectory(installerDestinationFolder);

                    DirectoryInfo info = new DirectoryInfo(installerDestinationFolder);
                    foreach (DirectoryInfo dir in info.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                    {
                        if (dir.CreationTime < DateTime.Now.AddDays(-daysToRetain))
                            DeleteDirectory(dir.FullName,true);//Directory.Delete(dir.FullName, true);
                    }
                    string buildPath = Path.Combine(installerDestinationFolder, "BMC Nightly Build - " + DateTime.Now.ToString("dd-MM-yyyy"));

                    if (Directory.Exists(buildPath))
                        DeleteDirectory(buildPath, true);

                    CopyDirectory(installerSourceFolder, buildPath);
                    return 1;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                LogManager.WriteLog("Error Occured while Copying installer files", LogManager.enumLogLevel.Error);
                return -1;
            }
        }


        private void CopyDirectory(string Source, string Destination)
        {
            if (!Directory.Exists(Destination))
            {
                Directory.CreateDirectory(Destination);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(Source);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                tempfile.CopyTo(Path.Combine(Destination, tempfile.Name));
            }

            DirectoryInfo[] directories = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in directories)
            {
                CopyDirectory(Path.Combine(Source, tempdir.Name), Path.Combine(Destination, tempdir.Name));
            }

        }
        private static void DeleteDirectory(string path, bool recursive)
        {
            if (recursive)
            {
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, recursive);
                }
            }

            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                var attr = File.GetAttributes(f);

                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                }
                File.Delete(f);
            }
            Directory.Delete(path);
        }
    }
}
