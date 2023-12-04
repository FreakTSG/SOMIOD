using System;


namespace SOMIOD_IS.SqlHelpers
{
   
    internal class ModelNotFoundException : Exception
    {
        public ModelNotFoundException(string message, bool suffix = true) : base(message + (suffix ? " not found" : ""))
        {
        }
        
    }
}