using AutoBuild.Helper;
using AutoBuild.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoBuild
{
    public enum TaskType
    {
        [XmlEnum(Name ="BATCH")]
        Batch,
        [XmlEnum(Name = "EXECUTABLE")]
        Executable,
        [XmlEnum(Name = "CUSTOM")]
        Custom
    }
    public static class BuildTaskFactory
    {
        public static BuildTask GetBuildTask(TaskInfo TaskInfo)
        {
            switch (TaskInfo.Type)
            {
                case TaskType.Batch:
                    return new BuildTask();
                case TaskType.Executable:
                    return new BuildTask();
                case TaskType.Custom:
                    switch (TaskInfo.Name.ToUpper())
                    {
                        case "SENDMAIL":
                            return new SendMailBuildTask();
                        case "CHECKERROR":
                            return new CheckErrorBuildTask();
                        case "COPYINSTALLER":
                            return new CopyInstallerBuildTask();
                        default:
                            return new BuildTask();
                    }
                default:
                    return new BuildTask();
            }
        }

    }
}
