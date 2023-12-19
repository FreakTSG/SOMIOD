using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App2.Models
{

    [XmlRoot("Data")]
    internal class Data : Container
    {
        [XmlElement("Content")]
        public string Content { get; set; }

        public Data(string name, string content,string parent) : base(name, parent)
        {
            Content = content;
        }
    }
}
