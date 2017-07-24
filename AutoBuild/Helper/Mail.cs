using BMC.Common.LogManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuild.Helper
{
    public static class Mail
    {
        public static void SendMail(string Subject,string Body,string Attachment)
        {
            try
            {
                int port=25;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["MailSMTPServer"]);

                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFromAddress"]);

                var toAddress = Argument.Args.ContainsKey("-m")?Argument.Args.Where(t=>t.Key=="-m").FirstOrDefault().Value:ConfigurationManager.AppSettings["MailToAddress"];
                toAddress.Split(';').ToList<string>().ForEach((t) => mail.To.Add(t));
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(Attachment))
                    mail.Attachments.Add(new System.Net.Mail.Attachment(Attachment));

                SmtpServer.Port = int.TryParse(ConfigurationManager.AppSettings["MailPortAddress"], out port) ? port : 25;

                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFromAddress"], ConfigurationManager.AppSettings["MailPassword"]); 
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                LogManager.WriteLog("Mail Sent successfully", LogManager.enumLogLevel.Info);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error occurred while sending mail", LogManager.enumLogLevel.Error);
                throw ex;
            }
        }
    }
}
