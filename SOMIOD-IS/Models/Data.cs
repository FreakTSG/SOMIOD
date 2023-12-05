using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD_IS.Models
{
    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public int Parent { get; set; }
        public Data() { }

        public Data(int id, string name, string content, DateTime creationDate, int parent)
        {
            {
                Id = id;
                Name = name;
                Content = content;
                CreationDate = creationDate.ToString("yyyy-MM-dd HH:mm:ss");
                Parent = parent;
            }
        }
        
    }



    
    
}