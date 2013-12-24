using System.Web.Http;
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
            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
            
            Injector.Get<IDecisionService>();
            Injector.Get<DecideController>();
        }
    }
}
