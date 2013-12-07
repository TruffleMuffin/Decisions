using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Decisions.Application.Services;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Tests.Support;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts
{
    [TestFixture]
    class DecisionsExtensionsTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private IDecisionService target;

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
                    { "H", new DeltaPolicy() }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
        }

        [AsyncTest]
        async Task CheckAsync_ManyDecision()
        {
            var decisions = new[]
                {
                    DecisionContext.Create().Using("Example").As("trufflemuffin").Has("A").On(new { id = 1 }),
                    DecisionContext.Create().Using("Example").As("trufflemuffin").Has("B").On(new { id = 1 }),
                    DecisionContext.Create().Using("Example").As("trufflemuffin").Has("C").On(new { id = 1 }),
                    DecisionContext.Create().Using("Example").As("trufflemuffin").Has("D").On(new { id = 1 })
                };
            var results = await target.CheckAsync(decisions);
            Assert.Count(4, results);
            Assert.AreEqual(true, results["Example/trufflemuffin/A/id%3D1"]);
            Assert.AreEqual(false, results["Example/trufflemuffin/B/id%3D1"]);
            Assert.AreEqual(true, results["Example/trufflemuffin/C/id%3D1"]);
            Assert.AreEqual(false, results["Example/trufflemuffin/D/id%3D1"]);
        }
    }
}
