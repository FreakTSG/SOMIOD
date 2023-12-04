using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SOMIOD_IS.SqlHelpers
{
    public static class XmlHelper
    {
        public static XmlDocument Serialize(object obj)
        {
            var xmlDocument = new XmlDocument();
            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var xmlWriter = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlWriter, obj);
                xmlWriter.Position = 0;
                xmlDocument.Load(xmlWriter);
                return xmlDocument;

            }
            
        }
    }
}