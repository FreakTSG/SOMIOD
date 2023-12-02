using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD_IS.Models
{
    public class Container : Application
    {

        public int Parent { get; set; }

        public Container() { }
        public Container(int id, string name, DateTime creationDate, int parent) : base(id,name,creationDate)
        {
           Parent = parent;
        }
    }
}