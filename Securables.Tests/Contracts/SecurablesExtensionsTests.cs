using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MbUnit.Framework;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Contracts.Providers;
using Securables.Tests.Support;

namespace Securables.Tests.Contracts
{
    [TestFixture]
    class SecurablesExtensionsTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private ISecurablesService target;

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
            target = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Securables.config"), policyService);
        }

        [AsyncTest]
        async Task CheckAsync_ManyDecision()
        {
            var decisions = new[]
                {
                    new DecisionContext { Component = "Example", Role = "A", TargetId = "1", SourceId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "B", TargetId = "1", SourceId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "C", TargetId = "1", SourceId = "gareth" },
                    new DecisionContext { Component = "Example", Role = "D", TargetId = "1", SourceId = "gareth" }
                };
            var results = await target.CheckAsync(decisions);
            Assert.Count(4, results);
            Assert.AreEqual(true, results["Example/gareth/A/1"]);
            Assert.AreEqual(false, results["Example/gareth/B/1"]);
            Assert.AreEqual(true, results["Example/gareth/C/1"]);
            Assert.AreEqual(false, results["Example/gareth/D/1"]);
        }
    }
}