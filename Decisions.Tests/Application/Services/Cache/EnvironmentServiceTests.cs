﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Decisions.Application.Services;
using Decisions.Contracts;
using Decisions.Tests.Support;
using MbUnit.Framework;

namespace Decisions.Tests.Application.Services.Cache
{
    [TestFixture]
    class EnvironmentServiceTests
    {
        private EnvironmentService service;
        private Decisions.Application.Services.Cache.EnvironmentService target;

        [SetUp]
        void SetUp()
        {
            service = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            target = new Decisions.Application.Services.Cache.EnvironmentService(service, 2);
        }

        [AsyncTest]
        async Task GetAsync_NotARecognisedAlias_Null()
        {
            var result = await target.GetAsync("SomeRandomAlias", new DecisionContext());

            Assert.IsNull(result);
        }

        [AsyncTest]
        async Task GetAsync_NotCached_ThenCached_Result_WithApproriateImprovement()
        {
            var start = DateTime.Now;

            var nonCachedResult = await target.GetAsync(LongAclEnvironment.ALIAS, new DecisionContext());

            var firstEnd = DateTime.Now;

            var cachedResult = await target.GetAsync(LongAclEnvironment.ALIAS, new DecisionContext());

            var secondEnd = DateTime.Now;

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var secondStart = DateTime.Now;

            var nonCachedResult2 = await target.GetAsync(LongAclEnvironment.ALIAS, new DecisionContext());

            var thirdEnd = DateTime.Now;

            Assert.IsInstanceOfType<LongAclEnvironment>(nonCachedResult);
            Assert.IsInstanceOfType<LongAclEnvironment>(cachedResult);
            Assert.IsInstanceOfType<LongAclEnvironment>(nonCachedResult2);

            // The long running environment has a thread.sleep for 3s on it so it should take at least this time to execute.
            Assert.IsTrue(firstEnd.Subtract(start).TotalSeconds > 3);

            // After first execution, it should be cached for 2s meaning the next call should take far less time to aquire the result.
            Assert.IsTrue(secondEnd.Subtract(firstEnd).TotalSeconds < 1);

            // After the second execution there is a wait in this thread for 3s, making sure that the cache period has expired. So now the next execution
            // should take the original amount of time as its going back to get the environment fresh.
            Assert.IsTrue(thirdEnd.Subtract(secondStart).TotalSeconds > 3);
        }
    }
}
