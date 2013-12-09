using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Decisions.API;
using Decisions.Contracts;
using Decisions.Utility;
using Newtonsoft.Json.Serialization;

namespace Decisions.Example
{
    /// <summary>
    /// The Decisions Web Application
    /// </summary>
    public class WebApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Called when the first resource (such as a page) in an ASP.NET application is requested. The Application_Start method is called only one time during the life cycle of an application. You can use this method to perform startup tasks such as loading data into the cache and initializing static values.
        /// You should set only static data during application start. Do not set any instance data because it will be available only to the first instance of the HttpApplication class that is created.
        /// </summary>
        protected void Application_Start()
        {
            // Resolve dependancies via Inversion of Control
            GlobalConfiguration.Configuration.DependencyResolver = new InjectorDependencyResolver();

            // Initialise the Resolver
            Injector.Resolver = new CastleResolver();

            // Set Json Serializer to use camel case for Bankbone
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Register routing for the application
            var routes = GlobalConfiguration.Configuration.Routes;

            // Decision routes
            routes.MapHttpRoute(
                name: "DecideApi",
                routeTemplate: "Api/Decide/{namespace}/{sourceId}/{roleName}/{targetId}",
                defaults: new { controller = "Decide", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });


            Injector.Get<IDecisionService>();
            Injector.Get<DecideController>();
        }
    }
}
