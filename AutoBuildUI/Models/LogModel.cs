using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoBuildUI.Models
{
    public class LogModel
    {
        public string TimeStamp { get; set; }
        public string LogData { get; set; }

        public LogModel(string TimeStamp, string LogData)
        {
            this.LogData = LogData;
            this.TimeStamp = TimeStamp;
        }
    }
}