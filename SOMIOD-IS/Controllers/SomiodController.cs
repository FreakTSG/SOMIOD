using SOMIOD_IS.Models;
using SOMIOD_IS.SqlHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public HttpResponseMessage Post([FromBody] Application newApp)
        {
            try
            {
                if (newApp == null)
                    throw new UnprocessableEntityException("You must provide an application with a name in the correct xml format");

                if (string.IsNullOrEmpty(newApp.Name))
                    throw new UnprocessableEntityException("You must include the name of your new application");

                DbHelper.CreateApplication(newApp.Name);
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
        [Route("api/somiod/{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            try
            {
                bool isDeleted = DbHelper.DeleteApplication(id);
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

        //Data

        //Subscription
    }
}
