using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMC.Common.ExceptionManagement
{
    public static class ExceptionManager
    {
        public static void Publish(Exception ex)
        {

            var exptText = new StringBuilder();

            while (ex != null)
            {
                exptText.AppendLine(DateTime.Now + "\t"+ex.Message);
                exptText.AppendLine(DateTime.Now + "\t"+ ex.StackTrace);
                ex = ex.InnerException;
            }
            LogManagement.LogManager.WriteLog(exptText.ToString(), LogManagement.LogManager.enumLogLevel.Info);
        }
    }
}
