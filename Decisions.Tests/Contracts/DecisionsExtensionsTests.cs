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
        private IDecisionsService target;

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
            target = new DecisionsService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
        }

        [AsyncTest]
        async Task CheckAsync_ManyDecision()
        {
            var decisions = new[]
                {
                    DecisionContext.Create().Within("Example").As("gareth").Has("A").On(new { id = 1 }),
                    DecisionContext.Create().Within("Example").As("gareth").Has("B").On(new { id = 1 }),
                    DecisionContext.Create().Within("Example").As("gareth").Has("C").On(new { id = 1 }),
                    DecisionContext.Create().Within("Example").As("gareth").Has("D").On(new { id = 1 })
                };
            var results = await target.CheckAsync(decisions);
            Assert.Count(4, results);
            Assert.AreEqual(true, results["Example/gareth/A/id%3D1"]);
            Assert.AreEqual(false, results["Example/gareth/B/id%3D1"]);
            Assert.AreEqual(true, results["Example/gareth/C/id%3D1"]);
            Assert.AreEqual(false, results["Example/gareth/D/id%3D1"]);
        }
    }
}
