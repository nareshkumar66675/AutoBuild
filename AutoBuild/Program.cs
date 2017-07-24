using AutoBuild.Helper;
using BMC.Common.ExceptionManagement;
using BMC.Common.LogManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager.WriteLog("AutoBuild Process started", LogManager.enumLogLevel.Info);
                Argument.Initialize(args);
                //Iniatialize Entity Based on XML Config
                XmlHelper.Initialize();

                BuildTaskExecutor buildExecutor = new BuildTaskExecutor();

                //Executes Tasks
                buildExecutor.ExecuteTask();

                LogManager.WriteLog("AutoBuild Process Completed", LogManager.enumLogLevel.Info);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
            }
        }
    }
}
