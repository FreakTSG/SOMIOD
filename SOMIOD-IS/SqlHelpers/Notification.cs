using System.Xml.Serialization;

namespace SOMIOD_IS.SqlHelpers
{
    [XmlRoot(ElementName = "Notification")]
    public class Notification
    {
        [XmlElement(ElementName = "EventType")]
        public string EventType { get; set;}

        [XmlElement(ElementName = "Content")]
        public string Content { get; set; }


        public Notification() { }
        public Notification(string eventType, string content)
        {
            this.EventType = eventType;
            this.Content = content;
        }
    }
}