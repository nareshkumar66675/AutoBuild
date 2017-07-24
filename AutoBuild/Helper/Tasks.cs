using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoBuild.Helper
{
    [XmlRoot(ElementName = "TaskInfo")]
    public class TaskInfo
    {
        [XmlAttribute(AttributeName = "Order")]
        public int Order { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public TaskType Type { get; set; }
        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "OnSucess")]
        public string OnSucess { get; set; }
        [XmlAttribute(AttributeName = "OnFailure")]
        public string OnFailure { get; set; }
        [XmlAttribute(AttributeName = "Dependents")]
        public string Dependents { get; set; }
        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "Tasks")]
    public class Tasks
    {
        [XmlElement(ElementName = "TaskInfo")]
        public List<TaskInfo> TaskInfo { get; set; }
    }

}
