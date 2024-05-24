using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Trackings
{
    public class CorsModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Headers", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "*");

            // Handle preflight requests
            if (context.Request.HttpMethod == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                context.Response.End();
            }
        }

        public void Dispose() { }
    }
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
