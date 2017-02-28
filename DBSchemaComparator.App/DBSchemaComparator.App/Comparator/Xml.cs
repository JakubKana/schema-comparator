using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DBSchemaComparator.Domain.Models.Test;

namespace DBSchemaComparator.App.Comparator
{
    public class Xml
    {


        public string GetXml(TestNodes testNodes)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(TestNodes));
            var subReq = testNodes;
            string xmlDocument;

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xmlDocument = sww.ToString(); 
                }
            }
            return xmlDocument;
        }
        public XDocument GetXDocument(TestNodes testNodes)
        {
            var xmlDocument = XDocument.Parse(GetXml(testNodes));

            return xmlDocument;
        }
    }
}
