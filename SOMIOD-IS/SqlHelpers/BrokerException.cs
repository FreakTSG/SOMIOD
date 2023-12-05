using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD_IS.SqlHelpers
{
    public class BrokerException :Exception
    {
        public BrokerException(string message) : base(message)
        {
        }
        public BrokerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}