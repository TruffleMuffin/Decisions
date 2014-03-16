using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Decisions.Contracts;
using Decisions.Example.Support;
using Decisions.Services;
using MbUnit.Framework;
using TruffleCache;

namespace Decisions.Tests.Application.Services.Cache
{
    [TestFixture]
    class EnvironmentServiceTests
    {
        private EnvironmentService service;
        private Decisions.Services.Cache.EnvironmentService target;

        [SetUp]
        void SetUp()
        {
            service = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            target = new Decisions.Services.Cache.EnvironmentService(service, new Cache<object>(new TestCacheStore(), "Environment"), 2);
        }

        [AsyncTest]
        async Task GetAsync_NotARecognisedAlias_Null()
        {
            try
            {
                await target.GetAsync("SomeRandomAlias", new DecisionContext());
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType<NotSupportedException>(e);
            }
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
            Assert.IsTrue(firstEnd.Subtract(start).TotalSeconds >= 3, "Long Running 1st: " + firstEnd.Subtract(start).TotalSeconds);

            // After first execution, it should be cached for 2s meaning the next call should take far less time to aquire the result.
            Assert.IsTrue(secondEnd.Subtract(firstEnd).TotalSeconds <= 1, "Cached Hit: " + secondEnd.Subtract(firstEnd).TotalSeconds);

            // After the second execution there is a wait in this thread for 3s, making sure that the cache period has expired. So now the next execution
            // should take the original amount of time as its going back to get the environment fresh.
            Assert.IsTrue(thirdEnd.Subtract(secondStart).TotalSeconds >= 3, "Long Running 2nd: " + thirdEnd.Subtract(secondStart).TotalSeconds);
        }

        [Test]
        void GetAsync_WhenExecutedInParallel_RequestsDoNotBottleneckUnderlieingProvider()
        {
            var taskList = new List<Task<dynamic>>();
            for (var i = 0; i < 1000; i++)
            {
                taskList.Add(target.GetAsync(SimpleCounterEnvironment.ALIAS, new DecisionContext()));
            }

            Task.WaitAll(taskList.ToArray());

            foreach (var task in taskList)
            {
                Assert.IsInstanceOfType<SimpleCounterEnvironment>(task.Result);
                Assert.AreEqual(1, (task.Result as SimpleCounterEnvironment).Counter());
            }
        }
    }
}
