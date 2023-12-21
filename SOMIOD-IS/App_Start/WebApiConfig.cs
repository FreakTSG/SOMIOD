using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SOMIOD_IS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{container}/{resource}",
                defaults: new { controller = "somiod", application = RouteParameter.Optional, container = RouteParameter.Optional, resource = RouteParameter.Optional }
            );

            //config.Formatters.XmlFormatter.UseXmlSerializer = true;

            var xml = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            xml.UseXmlSerializer = true;

        }
    }
}
