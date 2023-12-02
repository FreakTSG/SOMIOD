using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD_IS.Models
{
    public class Subscription : Container
    {
        
        public string Event { get; set; }

        public string Endpoint { get; set; }

        public Subscription() { }

        public Subscription(int id, string name, DateTime creationDate, int parent, string @event, string endpoint):base(id,name,creationDate,parent)

        {
            Event = @event;
            Endpoint = endpoint;
        }
       
    }
}