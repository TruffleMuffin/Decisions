using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Example.Support;
using Decisions.Services;
using Decisions.Services.Cache;
using MbUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TruffleCache;
using DecisionService = Decisions.Services.DecisionService;
using EnvironmentService = Decisions.Services.EnvironmentService;

namespace Decisions.Tests.Application.Services.Cache
{
    [TestFixture]
    class DecisionsServiceTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private DecisionService service;
        private Decisions.Services.Cache.DecisionService target;

        [SetUp]
        void SetUp()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            var policies = new Dictionary<string, IPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() }, 
                    { "C", new AlphaPolicy() }, 
                    { "D", new AlphaPolicy() }, 
                    { "E", new CappaPolicy { MatchUserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267")} }, 
                    { "F", new CappaPolicy { MatchUserId = new Guid("1E9A7C0C-FC86-4516-BA42-F7232E65A12C")} }, 
                    { "G", new DeltaPolicy() }, 
                    { "H", new LongDeltaPolicy() }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            service = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
            target = new Decisions.Services.Cache.DecisionService(service, new Cache<Decision>(new TestCacheStore(), "Decisions"), 2);
        }

        [AsyncTest]
        async Task CheckAsync_Decision_Expected_TimeConstraint()
        {
            var context = DecisionContext.Create().Using("Example").As("trufflemuffin").Has("H").On(new { id = 1 });

            var start = DateTime.Now;

            var nonCachedResult = await target.CheckAsync(context);

            var firstEnd = DateTime.Now;

            var cachedResult = await target.CheckAsync(context);

            var secondEnd = DateTime.Now;

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var secondStart = DateTime.Now;

            var nonCachedResult2 = await target.CheckAsync(context);

            var thirdEnd = DateTime.Now;

            Assert.IsTrue(nonCachedResult);
            Assert.IsTrue(cachedResult);
            Assert.IsTrue(nonCachedResult2);

            // The long running decision has a thread.sleep for 3s on one of the environments used on it so it should take at least this time to execute.
            Assert.IsTrue(firstEnd.Subtract(start).TotalSeconds >= 3, "Long Running 1st: " + firstEnd.Subtract(start).TotalSeconds);

            // After first execution, it should be cached for 2s meaning the next call should take far less time to aquire the result.
            Assert.IsTrue(secondEnd.Subtract(firstEnd).TotalSeconds <= 1, "Cached Hit: " + secondEnd.Subtract(firstEnd).TotalSeconds);

            // After the second execution there is a wait in this thread for 3s, making sure that the cache period has expired. So now the next execution
            // should take the original amount of time as its going back to get the decision fresh.
            Assert.IsTrue(thirdEnd.Subtract(secondStart).TotalSeconds >= 3, "Long Running 2nd: " + thirdEnd.Subtract(secondStart).TotalSeconds);
        }
    }
}

