using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using DBSchemaComparator.Domain.Models.Test;
using NLog;

namespace DBSchemaComparator.App.Comparator
{
    public class Xml
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string GetXml(TestNodes testNodes)
        {
            Logger.Info($"Serialization of {testNodes} to XML string");
            XmlSerializer xsSubmit = new XmlSerializer(typeof(TestNodes));
            var subReq = testNodes;
            string xmlDocument;

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xmlDocument = sww.ToString();
                    Logger.Debug($"Serialized xml: {xmlDocument}");
                }
            }
            Logger.Info("Serialization successful.");
            return xmlDocument;
        }
        public XDocument GetXDocument(TestNodes testNodes)
        {
            Logger.Info($"Serializing {testNodes} to XDocument.");

            var xmlDocument = XDocument.Parse(GetXml(testNodes));
            Logger.Debug(xmlDocument);
            return xmlDocument;
        }

        public void SaveResultTree(string path, string xmlContent)
        {
            try
            {
                Logger.Info($"Saving xml to {path}");
                Logger.Debug($"Saving xml content: {xmlContent}");
             
                var ticks = DateTime.Now.Ticks;
                var fileName = $"test-{ticks}.xml";
                string fullPath;
                fullPath = Settings.Instance.IsAbsoluteUrl(path) ? path : Path.GetFullPath(path);

                var dir = Directory.Exists(fullPath);

                    if (!dir)
                    {
                       Directory.CreateDirectory(Path.Combine(fullPath));
                       File.WriteAllText(Path.Combine(fullPath, fileName),
                       xmlContent,
                       Encoding.UTF8);
                    }
                    else 
                    {
                        File.WriteAllText(Path.Combine(fullPath, fileName),
                            xmlContent,
                            Encoding.UTF8);
                    }
            }
            catch (IOException ex)
            {
                Logger.Warn(ex, $"Unable to save xml file to {path}");
                Logger.Debug(ex, $"Error when saving content {xmlContent} to {path}");
                Environment.Exit((int)ExitCodes.UnableToExportXml);
            }
            catch (Exception ex)
            {
               Logger.Warn(ex,"Unexpected error.");
               Environment.Exit((int)ExitCodes.UnableToExportXml);
            }
            
        }
    }
}
