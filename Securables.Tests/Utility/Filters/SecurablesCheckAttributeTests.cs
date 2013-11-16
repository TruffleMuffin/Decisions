using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;
using MbUnit.Framework;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Contracts.Providers;
using Securables.Tests.Support;
using Securables.Utility;

namespace Securables.Tests.Utility.Filters
{
    /*
     * ****************************************************** Developer Notice **************************************************************************
     * 
     * These tests will only pass if running Visual Studio in Administrative mode, or the appropriate permissions have been applied to your user account.
     */

    [TestFixture]
    class SecurablesCheckAttributeTests
    {
        private HttpSelfHostServer server;

        [SetUp]
        void SetUp()
        {
            Injector.Resolver = new TestResolver();

            var config = new HttpSelfHostConfiguration("http://localhost:40000");

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }

        [TearDown]
        void TearDown()
        {
            Injector.Resolver = null;

            server.CloseAsync().Wait();
        }

        [Test]
        void Authorized()
        {
            var client = new HttpClient();

            var resp = client.GetAsync(string.Format("http://localhost:40000/api/values/{0}", 1)).Result;
            resp.EnsureSuccessStatusCode();

            var result = resp.Content.ReadAsAsync<string>().Result;
            Assert.AreEqual("value", result);
        }

        [Test]
        void NotAuthorized()
        {
            var client = new HttpClient();

            var resp = client.GetAsync(string.Format("http://localhost:40000/api/values/{0}", 2)).Result;
            Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        class TestResolver : IResolver
        {
            private PolicyService policyService;
            private EnvironmentService environmentService;
            private SecurablesService securablesService;

            public TestResolver()
            {
                environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
                var policies = new Dictionary<string, AbstractPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() },
                    { "G", new DeltaPolicy {AclEnvironmentKey = "Acl"} }, 
                    { "H", new DeltaPolicy {AclEnvironmentKey = "LongRunning"} }
                };
                policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
                securablesService = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Securables.config"), policyService);
            }

            public T Get<T>()
            {
                return (T)(object)securablesService; ;
            }

            public object Get(Type type)
            {
                return new ValuesResolver();
            }
        }
    }
}
