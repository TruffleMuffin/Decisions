using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using Decisions.Contracts;
using Decisions.Contracts.Services;
using Decisions.Tests.Utility.Filters;
using Decisions.Utility;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts.Services
{
    /*
     * ****************************************************** Developer Notice **************************************************************************
     * 
     * These tests will only pass if running Visual Studio in Administrative mode, or the appropriate permissions have been applied to your user account.
     */

    [TestFixture]
    class RemoteDecisionsServiceTests
    {
        private HttpSelfHostServer server;
        private RemoteDecisionsService target;

        [SetUp]
        void SetUp()
        {
            Injector.Resolver = new TestResolver();
            var endpoint = "http://localhost:40000";

            target = new RemoteDecisionsService(endpoint);
            var config = new HttpSelfHostConfiguration(endpoint);
            
            config.DependencyResolver = new InjectorDependencyResolver();

            // Decision routes
            config.Routes.MapHttpRoute(
                name: "DecideApi",
                routeTemplate: "Api/Decide/{componentName}/{sourceId}/{roleName}/{targetId}",
                defaults: new { controller = "Decide", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }

        [TearDown]
        void TearDown()
        {
            Injector.Resolver = null;
            server.CloseAsync().Wait();
        }

        [AsyncTest]
        async Task Authorized()
        {
            var result = await target.CheckAsync(new DecisionContext { Component = "Example", Role = "A", TargetId = "1", SourceId = "gareth" });
            Assert.IsTrue(result);
        }

        [AsyncTest]
        async Task NotAuthorized()
        {
            var result = await target.CheckAsync(new DecisionContext { Component = "Example", Role = "B", TargetId = "1", SourceId = "gareth" });
            Assert.IsFalse(result);
        }
    }
}
