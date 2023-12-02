using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD_IS.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreationDate { get; set; }

        public Application() { }
        public Application(int id, string name, DateTime creationDate)
        {
            Id = id;
            Name = name;
            CreationDate = creationDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}