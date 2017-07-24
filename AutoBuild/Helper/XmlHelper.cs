using BMC.Common.ExceptionManagement;
using BMC.Common.LogManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AutoBuild.Helper
{
    public static class XmlHelper
    {
        public static Tasks TaskList;
        public static void Initialize()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(Tasks));
                string xmlFilePath = ConfigurationManager.AppSettings["TaskInfoConfigPath"];
                TextReader reader = null;

                if (Validate(xmlFilePath))
                {
                    reader = new StreamReader(xmlFilePath);
                    TaskList = (Tasks)deserializer.Deserialize(reader);
                    reader.Close();
                }
                else
                {
                    throw new Exception("XML Validation Failed");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                LogManager.WriteLog("Error Occured while Initializing XML File", LogManager.enumLogLevel.Error);
                throw ex;
            }
        }


        private static bool Validate(string xmlFilePath)
        {
            try
            {
                bool isValid = true;

                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", XmlReader.Create(new StringReader(AutoBuildResource.BuildTaskConfig)));

                XDocument custOrdDoc = XDocument.Load(xmlFilePath);

                custOrdDoc.Validate(schemas, (o, e) =>
                {
                    LogManager.WriteLog(e.Message, LogManager.enumLogLevel.Error);
                    isValid = false;
                });


                return isValid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
