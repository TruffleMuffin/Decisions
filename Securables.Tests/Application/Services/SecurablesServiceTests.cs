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
        [Row("A", Decision.Permit)]
        [Row("B", Decision.Deny)]
        [Row("C", Decision.Permit)]
        [Row("D", Decision.Deny)]
        [Row("E", Decision.Permit)]
        [Row("F", Decision.Deny)]
        [Row("G", Decision.Deny)]
        [Row("H", Decision.Permit)]
        async Task CheckAsync_Decision_Expected(string alias, Decision expected)
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
            Assert.AreEqual(Decision.Permit, results["Example/gareth/A/1"]);
            Assert.AreEqual(Decision.Deny, results["Example/gareth/B/1"]);
            Assert.AreEqual(Decision.Permit, results["Example/gareth/C/1"]);
            Assert.AreEqual(Decision.Deny, results["Example/gareth/D/1"]);
        }

        [AsyncTest]
        [Row("H", Decision.Permit, 4)]
        [Row("I", Decision.Permit, 1)]
        async Task CheckAsync_Decision_Expected_TimeConstraint(string alias, Decision expected, int seconds)
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

