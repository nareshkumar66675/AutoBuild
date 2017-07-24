using AutoBuild.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace AutoBuild.Tasks
{
    class SendMailBuildTask : BuildTask
    {
        string status = string.Empty;
        public override int Execute(TaskInfo TaskInfo)
        {

            string tempZipPath = Path.Combine(Path.GetTempPath(), "BuildErrors_" + DateTime.Now.ToString("ddMMyyyy") + ".zip");

            if (!File.Exists(tempZipPath))
                tempZipPath = "";

            string htmlBody = BuildHtml(TaskInfo);
            Mail.SendMail("BMC 16.x Daily Build Status - "+ DateTime.Now.ToString("dd-MM-yyyy")+" - "+status, htmlBody, tempZipPath);
           
            return 1;
        }

        private string BuildHtml(TaskInfo TaskInfo)
        {

            TextWriter tableXMLData = new StringWriter();
            StringWriter htmlText = new StringWriter();

            /*Transform Dataset to XML*/
            ConvertTaskToDataset(TaskInfo).WriteXml(tableXMLData);

            /*Arguments for XSLT */
            XsltArgumentList argsList = new XsltArgumentList();

            if (BuildTaskExecutor.TaskList.Where(t => t.Status == TaskStatus.Completed && t.TaskInfo.Order != TaskInfo.Order 
                       && t.TaskInfo.Name != TaskInfo.Name).Count()== BuildTaskExecutor.TaskList.Count()-1)
            {
                argsList.AddParam("Status", "", "Success");
                status = "Success";
            }
            else
            {
                argsList.AddParam("Status", "", "Failed");
                status= "Failed";
            }
            
            argsList.AddParam("InstallerPath","", Path.Combine(ConfigurationManager.AppSettings["InstallerDestinationFolder"], "BMC Nightly Build - " + DateTime.Now.ToString("dd-MM-yyyy")));


            XmlReader rdr = XmlReader.Create(new StringReader(tableXMLData.ToString()));
            XmlWriter writerXML = XmlWriter.Create(htmlText);

            /* Trasnform XSLT to HTML*/
            XslCompiledTransform xsltTransformer = new XslCompiledTransform();
            xsltTransformer.Load(XmlReader.Create(new StringReader(AutoBuildResource.MailTemplate)));
            xsltTransformer.Transform(rdr, argsList, writerXML);

            return htmlText.ToString();
        }

        private DataSet ConvertTaskToDataset(TaskInfo TaskInfo)
        {
            DataSet taskDS = new DataSet("TaskStatus");
            DataTable taskDt = new DataTable("Tasks");
            taskDS.Tables.Add(taskDt);
            taskDt.Columns.Add(new DataColumn("TaskName"));
            taskDt.Columns.Add(new DataColumn("Status"));

            foreach (TaskDictionary<int> task in BuildTaskExecutor.TaskList)
            {
                if (TaskInfo.Order == task.TaskInfo.Order && TaskInfo.Name == task.TaskInfo.Name)
                    continue;
                taskDt.Rows.Add(task.TaskInfo.Description, task.Status.ToString());
            }

            return taskDS;
        }
    }
}
