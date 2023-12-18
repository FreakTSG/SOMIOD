using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App1.Models
{
    [XmlRoot("Notification")]
    public class Notification
    {
        [XmlElement("EventType")]
        public string EventType { get; set; }


        [XmlElement("Content")]
        public string Content { get; set; }

        public Notification() { }

        public Notification(string eventType, string content)
        {
            EventType = eventType;
            Content = content;
        }
    }
}
