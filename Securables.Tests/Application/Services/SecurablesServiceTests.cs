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
            target = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Decisions.xml"), policyService);
        }

        [AsyncTest]
        [Row("A", Decision.Permit)]
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
    }
}
