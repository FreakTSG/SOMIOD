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
        private readonly List<string> _validEvents = new List<string>() { "CREATE", "DELETE", "BOTH" };



        #region application
        // GETAll: application

        [HttpGet]
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


        [HttpGet]
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


        [HttpPost]
        [Route("api/somiod")]
        public HttpResponseMessage Post([FromBody] Application newApp)
        {
            try
            {
                /*if (newAppName == null)
                    throw new UnprocessableEntityException("You must provide an application with a name in the correct xml format");*/

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


        [HttpPut]
        [Route("api/somiod/{application}")]
        public HttpResponseMessage Put(string application,[FromBody] Application newAppDetails)
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


        [HttpGet]
        [Route("api/somiod/{application}/containers")]
        public HttpResponseMessage GetContainers(string application)
        {
            try
            {
                if (Request.Headers.Contains("somiod-discover"))
                {
                    // Get the values of the "somiod-discover" header
                    IEnumerable<string> headerValues = Request.Headers.GetValues("somiod-discover");

                    // Assuming you only expect one value for the header, you can retrieve it like this
                    string discoverHeaderValue = headerValues.FirstOrDefault();

                    if (discoverHeaderValue == "containers")
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
                    else
                    {
                        throw new UnprocessableEntityException("Header esta a vazio ou errado");
                    }
                }
                else
                {
                    throw new UnprocessableEntityException("Header em falta ou errado");
                }
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpGet]
        [Route("api/somiod/{application}/{container}")]
        public HttpResponseMessage GetContainer(string application,string container)
        {
            try
            {
                var cont = DbHelper.GetContainer(application, container);
                return RequestHelper.CreateMessage(Request, cont);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpPost]
        [Route("api/somiod/{application}")]
        public HttpResponseMessage PostContainer(string application, [FromBody] Container container)
        {
            try
            {
                if (container == null)
                    throw new UnprocessableEntityException("You must provide a container with a name in the correct xml format");

                if (string.IsNullOrEmpty(container.Name))
                    throw new UnprocessableEntityException("You must include the name of your new container");

                DbHelper.CreateContainer(container.Name, application );
                return RequestHelper.CreateMessage(Request, "Container created");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpPut]
        [Route("api/somiod/{application}/{container}")]
        public HttpResponseMessage Put(string application,string container, [FromBody] Container newContainerDetails)
        {
            try
            {
                if (newContainerDetails == null)
                    throw new UnprocessableEntityException("You must provide an container with a name in the correct xml format");

                if (string.IsNullOrEmpty(newContainerDetails.Name))
                    throw new UnprocessableEntityException("You must include the updated name of the container");

                string newName = newContainerDetails.Name;
                DbHelper.UpdateContainer(application, container, newName);
                return Request.CreateResponse(HttpStatusCode.OK, "Container updated");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpDelete]
        [Route("api/somiod/{application}/{container}")]
        public HttpResponseMessage DeleteContainer(string application, string container)
        {
            try
            {
                DbHelper.DeleteContainer(application, container);
                return Request.CreateResponse(HttpStatusCode.OK, "Container was deleted");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        #endregion

        #region Data

        [HttpGet]
        [Route("api/somiod/{application}/{container}/data")]
        public HttpResponseMessage GetDatas(string application, string container)
        {
            try
            {
                if (Request.Headers.Contains("somiod-discover"))
                {
                    // Get the values of the "somiod-discover" header
                    IEnumerable<string> headerValues = Request.Headers.GetValues("somiod-discover");

                    // Assuming you only expect one value for the header, you can retrieve it like this
                    string discoverHeaderValue = headerValues.FirstOrDefault();

                    if (discoverHeaderValue == "data")
                    {
                        try
                        {
                            var datas = DbHelper.GetSubscriptions(application, container);
                            return RequestHelper.CreateMessage(Request, datas);
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
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpGet]
        [Route("api/somiod/{application}/{container}/data/{dataName}")]
        public HttpResponseMessage GetData(string application, string container, string dataName)
        {
            try
            {
                var datas = DbHelper.GetData(application, container, dataName);
                return RequestHelper.CreateMessage(Request, datas);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}/data")]
        public HttpResponseMessage PostData(string application, string container, [FromBody]Data newData)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            try
            {
                if(newData == null)
                    throw new UnprocessableEntityException("You must provide an data with a name in the correct xml format");
                if(string.IsNullOrEmpty(newData.Content))
                    throw new UnprocessableEntityException("You must include content for that data resource");
                DbHelper.CreateData(application, container, newData.Content);
                return RequestHelper.CreateMessage(Request, "Data created");
            }
            catch (Exception e)
            {
                if(e is BrokerException)
                    return RequestHelper.CreateMessage(Request,"Data resource was created but could not notify at least one of the subscribers Error:" +e.Message);
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}/data/{dataName}")]
        public HttpResponseMessage DeleteData(string application, string container, string dataName)
        {
            try
            {
                DbHelper.DeleteData(application, container, dataName);
                return RequestHelper.CreateMessage(Request, "Data resource was deleted");
            }
            catch (Exception e)
            {
                if(e is BrokerException)
                    return RequestHelper.CreateMessage(Request, "Data resource was deleted but could not notify at least one of the subscribers Error:" + e.Message);
                return RequestHelper.CreateError(Request, e);
            }
        }


        #endregion

        #region Subscription

        [HttpGet]
        [Route("api/somiod/{application}/{container}/sub")]
        public HttpResponseMessage GetSubscriptions(string application, string container)
        {
            try
            {
                if (Request.Headers.Contains("somiod-discover"))
                {
                    // Get the values of the "somiod-discover" header
                    IEnumerable<string> headerValues = Request.Headers.GetValues("somiod-discover");

                    // Assuming you only expect one value for the header, you can retrieve it like this
                    string discoverHeaderValue = headerValues.FirstOrDefault();

                    if (discoverHeaderValue == "sub")
                    {
                        try
                        {
                            var subscriptions = DbHelper.GetSubscriptions(application,container);
                            return RequestHelper.CreateMessage(Request, subscriptions);
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
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [HttpGet]
        [Route("api/somiod/{application}/{container}/sub/{subscription}")]
        public HttpResponseMessage GetSubscription(string application, string container, string subscription)
        {
            try
            {
                var subs = DbHelper.GetSubscription(application, container, subscription);
                return RequestHelper.CreateMessage(Request, subs);
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}/sub")]
        public HttpResponseMessage PostSubscription(string application, string container, [FromBody] Subscription newSubscription)
        {
            try
            {
                if (newSubscription == null)
                    throw new UnprocessableEntityException("You must provide a subscription with a valid url in the correct xml format");

                if (string.IsNullOrEmpty(newSubscription.Name))
                    throw new UnprocessableEntityException("You must include a name for that subscription");

                if (string.IsNullOrEmpty(newSubscription.Endpoint))
                    throw new UnprocessableEntityException("You must include an endpoint for that subscription");

                if (!_validEvents.Contains(newSubscription.Event.ToUpper()))
                    throw new UnprocessableEntityException("You must include a valid event for that subscription. Valid event types are: CREATE, DELETE, BOTH");

                DbHelper.CreateSubscription(application, container, newSubscription);
                return RequestHelper.CreateMessage(Request, "Subscription created");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        [Route("api/somiod/{application}/{container}/sub/{subscription}")]
        public HttpResponseMessage DeleteSubscription(string application, string container, string subscription)
        {
            try
            {
                DbHelper.DeleteSubscription(application, container, subscription);
                return Request.CreateResponse(HttpStatusCode.OK, "Subscription was deleted");
            }
            catch (Exception e)
            {
                return RequestHelper.CreateError(Request, e);
            }
        }

        #endregion

    }
}
