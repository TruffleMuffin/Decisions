using System;
using System.IO;
using System.Threading.Tasks;
using MbUnit.Framework;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Tests.Support;

namespace Securables.Tests.Application.Services
{
    [TestFixture]
    class SecurablesServiceTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private SecurablesService target;

        [SetUp]
        void SetUp()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            policyService = new PolicyService(new[] { new ExamplePolicyProvider() }, environmentService);
            target = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Securables.config"), policyService);
        }

        [AsyncTest]
        [Row("A", true)]
        [Row("B", false)]
        [Row("C", true)]
        [Row("D", false)]
        [Row("E", true)]
        [Row("F", false)]
        [Row("G", false)]
        [Row("H", true)]
        async Task CheckAsync_Decision_Expected(string alias, bool expected)
        {
            var result = await target.CheckAsync(new DecisionContext
                {
                    Component = "Example",
                    Role = alias,
                    EntityId = "1",
                    UserId = "gareth"
                });
            Assert.AreEqual(expected, result);
        }

        [AsyncTest]
        async Task CheckAsync_ManyDecision()
        {
            var decisions = new[]
                {
                    new DecisionContext { Component = "Example", Role = "A", EntityId = "1", UserId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "B", EntityId = "1", UserId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "C", EntityId = "1", UserId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "D", EntityId = "1", UserId = "gareth" }
                };
            var results = await target.CheckAsync(decisions);
            Assert.Count(4, results);
            Assert.AreEqual(true, results["Example/gareth/A/1"]);
            Assert.AreEqual(false, results["Example/gareth/B/1"]);
            Assert.AreEqual(true, results["Example/gareth/C/1"]);
            Assert.AreEqual(false, results["Example/gareth/D/1"]);
        }

        [AsyncTest]
        [Row("H", true, 4)]
        [Row("I", true, 1)]
        async Task CheckAsync_Decision_Expected_TimeConstraint(string alias, bool expected, int seconds)
        {
            DateTime start = DateTime.Now;

            var result = await target.CheckAsync(new DecisionContext
            {
                Component = "Example",
                Role = alias,
                EntityId = "1",
                UserId = "gareth"
            });

            DateTime end = DateTime.Now;

            Assert.AreEqual(expected, result);
            Assert.AreApproximatelyEqual(end, start, TimeSpan.FromSeconds(seconds));
        }
    }
}

