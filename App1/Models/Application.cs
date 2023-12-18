using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App1.Models
{
    [XmlRoot("ArrayofApplications")]
    public abstract class ApplicationList
    {
        [XmlElement("Application")]
        public List<Application> Applications { get; set; }
    }

    [XmlRoot("Application")]

    public class Application
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        public Application(string name)
        {
            Name = name;
        }
    }
}
