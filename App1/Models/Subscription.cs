using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App1.Models
{
    [XmlRoot("Subscription")]
    internal class Subscription : Container
    {
        [XmlElement("Event")]
        public string Event { get; set; }

        [XmlElement("Endpoint")]
        public string Endpoint { get; set; }
        public Subscription(string name, string parent, string eventType, string endpoint) : base(name, parent)
        {
            Event = eventType;
            Endpoint = endpoint;
        }
    }
}
