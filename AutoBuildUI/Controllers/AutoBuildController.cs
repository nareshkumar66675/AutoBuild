using AutoBuildUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutoBuildUI.Controllers
{
    public class AutoBuildController : ApiController
    {
        /// <summary>
        /// Executes the AutoBuild
        /// </summary>
        /// <returns>HttpResponse</returns>
        [HttpGet]
        [Route("Execute")]
        public IHttpActionResult Execute(string email)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.AppSettings["AppPath"]);
            processInfo.CreateNoWindow = false;
            processInfo.FileName = Path.GetFileName(ConfigurationManager.AppSettings["AppPath"]);
            if(!string.IsNullOrEmpty(email))
                processInfo.Arguments = "-m "+ email;
            using (Process process = Process.Start(processInfo))
            {
                return Ok();
            }
            
        }
        /// <summary>
        /// Get Process  Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProcessStatus")]
        public IHttpActionResult GetProcessStatus()
        {
            if (IsProcessOpen(Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["AppPath"])))
                return Ok<string>("Running");
            else
                return Ok<string>("Not Running");
        }
        /// <summary>
        /// Get Log Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLogData")]
        public IHttpActionResult GetLogData()
        {
            //TextReader rdr = new StringReader("");

            
            string line;
            List<string> logs = new List<string>();
            using (StreamReader rdr = new StreamReader(File.OpenRead(ConfigurationManager.AppSettings["LogPath"])))
            {
                while ((line = rdr.ReadLine()) != null)
                {
                    if (line.Contains("Start Of Log"))
                    {
                        logs.Clear();
                    }
                    logs.Add(line);
                } 
            }
            List<LogModel> parsedLog = ParseLogs(logs);
            return Ok<List<LogModel>>(parsedLog);
        }
        /// <summary>
        /// Retrieves Process Deatils
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProcessDetails")]
        public HttpResponseMessage GetProcessDetails()
        {
            string name = Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["AppPath"]);
            ProcessDetail detail= new ProcessDetail();
            if(IsProcessOpen(name))
            {
                Process proc= Process.GetProcesses().ToList<Process>().Where(t => t.ProcessName.ToUpper() == name.ToUpper()).FirstOrDefault();
                detail.TaskName = proc.ProcessName;
                detail.ElapsedTime = DateTime.Now.Subtract(proc.StartTime).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
            }
            else
            {
                detail.TaskName = name;
                detail.ElapsedTime = null;
            }
            return Request.CreateResponse<ProcessDetail>(HttpStatusCode.OK, detail);
        }
        private List<LogModel> ParseLogs(List<string> logs)
        {
            List<LogModel> parsedLog = new List<LogModel>();

            logs.ForEach(t =>
            {
                try
                {
                    DateTime temp = new DateTime();
                    string[] str = t.Split('\t');
                    if (DateTime.TryParse(t.Substring(0, 23),out temp))
                    {
                        parsedLog.Add(new LogModel(t.Substring(0, 23), t.Substring(23, t.Length - 23)));
                    }
                    else
                        parsedLog.Add(new LogModel("", t.ToString()));
                }
                catch (ArgumentOutOfRangeException)
                {
                    parsedLog.Add(new LogModel("", t.ToString()));
                }
            });

            return parsedLog;
        }
        private bool IsProcessOpen(string name)
        {
            return Process.GetProcesses().ToList<Process>().Count(t => t.ProcessName.ToUpper() == name.ToUpper()) > 0 ? true : false;
        }
    }
}
