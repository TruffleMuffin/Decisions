using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MbUnit.Framework;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Contracts.Providers;
using AlphaPolicy = Securables.Tests.Support.AlphaPolicy;
using BetaPolicy = Securables.Tests.Support.BetaPolicy;
using CappaPolicy = Securables.Tests.Support.CappaPolicy;
using DeltaPolicy = Securables.Tests.Support.DeltaPolicy;
using ExampleEnvironmentProvider = Securables.Tests.Support.ExampleEnvironmentProvider;

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
            var policies = new Dictionary<string, IPolicy>
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
            target = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Securables.config"), policyService);
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
                    TargetId = "1",
                    SourceId = "gareth"
                });
            Assert.AreEqual(expected, result);
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
                TargetId = "1",
                SourceId = "gareth"
            });

            DateTime end = DateTime.Now;

            Assert.AreEqual(expected, result);
            Assert.AreApproximatelyEqual(end, start, TimeSpan.FromSeconds(seconds));
        }
    }
}

