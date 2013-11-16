using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MbUnit.Framework;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Contracts.Providers;
using Securables.Tests.Support;
using EnvironmentService = Securables.Application.Services.EnvironmentService;
using SecurablesService = Securables.Application.Services.SecurablesService;
using CacheSecurablesService = Securables.Application.Services.Cache.SecurablesService;

namespace Securables.Tests.Application.Services.Cache
{
    [TestFixture]
    class SecurablesServiceTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private SecurablesService service;
        private CacheSecurablesService target;

        [SetUp]
        void SetUp()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            var policies = new Dictionary<string, AbstractPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() }, 
                    { "C", new AlphaPolicy() }, 
                    { "D", new AlphaPolicy() }, 
                    { "E", new CappaPolicy {CurrentUserEnvironmentKey = "CurrentUser", MatchUserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267")} }, 
                    { "F", new CappaPolicy {CurrentUserEnvironmentKey = "CurrentUser", MatchUserId = new Guid("1E9A7C0C-FC86-4516-BA42-F7232E65A12C")} }, 
                    { "G", new DeltaPolicy {AclEnvironmentKey = "Acl"} }, 
                    { "H", new DeltaPolicy {AclEnvironmentKey = "LongRunning"} }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            service = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Securables.config"), policyService);
            target = new CacheSecurablesService(service, 2);
        }

        [AsyncTest]
        async Task CheckAsync_Decision_Expected_TimeConstraint()
        {
            var context = new DecisionContext { Component = "Example", Role = "H", TargetId = "1", SourceId = "gareth" };

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
            Assert.IsTrue(firstEnd.Subtract(start).TotalSeconds > 3);

            // After first execution, it should be cached for 2s meaning the next call should take far less time to aquire the result.
            Assert.IsTrue(secondEnd.Subtract(firstEnd).TotalSeconds < 1);

            // After the second execution there is a wait in this thread for 3s, making sure that the cache period has expired. So now the next execution
            // should take the original amount of time as its going back to get the decision fresh.
            Assert.IsTrue(thirdEnd.Subtract(secondStart).TotalSeconds > 3);
        }
    }
}

