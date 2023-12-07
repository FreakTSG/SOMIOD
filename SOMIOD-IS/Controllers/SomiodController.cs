using SOMIOD_IS.Models;
using SOMIOD_IS.SqlHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Caching;
using System.Web.Http;

namespace SOMIOD_IS.Controllers
{

    public class SomiodController : ApiController
    {


        #region application
        // GETAll: application
        [Route("api/somiod")]
        public HttpResponseMessage GetApplications()
        {
            try { 
            if (Request.Headers.Contains("somiod-discover"))
            {
                // Get the values of the "somiod-discover" header
                IEnumerable<string> headerValues = Request.Headers.GetValues("somiod-discover");

                // Assuming you only expect one value for the header, you can retrieve it like this
                string discoverHeaderValue = headerValues.FirstOrDefault();

                if(discoverHeaderValue == "application") { 

                try
                {
                    var applications = DbHelper.GetApplications();
                    return RequestHelper.CreateMessage(Request, applications);
                }
                catch (Exception e)
                {
                    return RequestHelper.CreateError(Request, e);
                }
                }
                else
                {
                    throw new UnprocessableEntityException("Header esta a vazio ou errado");
                }
            }
            else
            {
                throw new UnprocessableEntityException("Header em falta ou errado");
            }

            }catch  (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}")]
        public HttpResponseMessage GetApplication(string application)
        {
            try
            {
                var app = DbHelper.GetApplication(application);
                return RequestHelper.CreateMessage(Request, app);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod")]
        public HttpResponseMessage Post([FromBody] String newAppName)
        {
            try
            {
                /*if (newAppName == null)
                    throw new UnprocessableEntityException("You must provide an application with a name in the correct xml format");*/

                if (string.IsNullOrEmpty(newAppName))
                    throw new UnprocessableEntityException("You must include the name of your new application");

                DbHelper.CreateApplication(newAppName);
                return RequestHelper.CreateMessage(Request, "Application created");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}")]
        public HttpResponseMessage Put(string application, [FromBody] Application newAppDetails)
        {
            try
            {
                if (newAppDetails == null)
                    throw new UnprocessableEntityException("You must provide an application with a name in the correct xml format");

                if (string.IsNullOrEmpty(newAppDetails.Name))
                    throw new UnprocessableEntityException("You must include the updated name of the application");

                string newName = newAppDetails.Name;
                DbHelper.UpdateApplication(application, newName);
                return Request.CreateResponse(HttpStatusCode.OK, "Application updated");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        // DELETE: application
        [HttpDelete]
        [Route("api/somiod/{application}")]
        public IHttpActionResult DeleteApplication(string application)
        {
            try
            {
                bool isDeleted = DbHelper.DeleteApplication(application);
                if (!isDeleted)
                {
                    return NotFound(); // Return 404 if the application doesn't exist
                }

                return Ok(); // Return 200 OK if the deletion was successful
            }
            catch (Exception e)
            {
                return InternalServerError(e); // Return 500 in case of an exception
            }
        }

        #endregion

        //Container

        #region Container

        [Route("api/somiod/{application}/containers")]
        public HttpResponseMessage GetContainers(string application)
        {
            try
            {
                var containers = DbHelper.GetContainers(application);
                return RequestHelper.CreateMessage(Request, containers);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}")]
        public HttpResponseMessage GetContainer(string appname,string container)
        {
            try
            {
                var cont = DbHelper.GetContainer(appname,container);
                return RequestHelper.CreateMessage(Request, cont);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}")]
        public HttpResponseMessage Put(string container, [FromBody] Container newContainerDetails)
        {
            try
            {
                if (newContainerDetails == null)
                    throw new UnprocessableEntityException("You must provide an container with a name in the correct xml format");

                if (string.IsNullOrEmpty(newContainerDetails.Name))
                    throw new UnprocessableEntityException("You must include the updated name of the container");

                string newName = newContainerDetails.Name;
                DbHelper.UpdateApplication(container, newName);
                return Request.CreateResponse(HttpStatusCode.OK, "Container updated");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpDelete]
        [Route("api/somiod/{application}/{id:int}")]
        public IHttpActionResult DeleteContainer(int id)
        {
            try
            {
                bool isDeleted = DbHelper.DeleteContainer(id);
                if (!isDeleted)
                {
                    return NotFound(); 
                }

                return Ok(); 
            }
            catch (Exception e)
            {
                return InternalServerError(e); 
            }
        }

        #endregion

        #region Data
        //Data



        #endregion

        #region Subscription
        //Subscription

        #endregion

    }
}
