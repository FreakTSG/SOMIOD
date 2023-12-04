using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using SOMIOD_IS.Controllers;

namespace SOMIOD_IS.SqlHelpers
{
    public static class RequestHelper
    {
        public static HttpResponseMessage CreateError(HttpRequestMessage request, Exception e)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;

            if(e is ModelNotFoundException)
                httpStatusCode = HttpStatusCode.NotFound;
            if (e is UnprocessableEntityException)
                httpStatusCode = (HttpStatusCode)422;
            return request.CreateResponse(httpStatusCode, e.Message);
        }

        public static HttpResponseMessage CreateMessage(HttpRequestMessage request, object message)
        {
            var xmlDocument = XmlHelper.Serialize(message);
            return request.CreateResponse(HttpStatusCode.OK, xmlDocument,"application/xml");
        }
    }
}