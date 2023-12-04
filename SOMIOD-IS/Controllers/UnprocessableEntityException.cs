using System;


namespace SOMIOD_IS.Controllers
{
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException(string message) :base (message)
        {
        }
    }
}