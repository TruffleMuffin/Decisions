using Decisions.Contracts;
using Decisions.Example.Support;
using MbUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Decisions.Tests.Utility.Filters
{
    /*
     * ****************************************************** Developer Notice **************************************************************************
     * 
     * These tests will only pass if running Visual Studio in Administrative mode, or the appropriate permissions have been applied to your user account.
     */

    [TestFixture]
    class DecisionCheckAttributeTests
    {
        private HttpSelfHostServer server;

        [SetUp]
        void SetUp()
        {
            Injector.Resolver = new TestResolver();

            var config = new HttpSelfHostConfiguration("http://localhost:40000");

            config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

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

        [Test]
        void NotAuthorized_NotLazy()
        {
            var client = new HttpClient();

            var resp = client.GetAsync(string.Format("http://localhost:40000/api/values/{0}", 2)).Result;
            Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
        }
    }
}
