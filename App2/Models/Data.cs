using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace App2.Models
{

    [XmlRoot("Data")]
    internal class Data
    {
        [XmlElement("Content")]
        public string Content { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }


        public Data() { }

        public Data(string name,string content)
        {
            Name = name;
            Content = content;
        }
    }
}
