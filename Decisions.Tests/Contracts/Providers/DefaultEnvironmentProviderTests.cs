using System;
using System.Threading.Tasks;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts.Providers
{
    [TestFixture]
    class DefaultEnvironmentProviderTests
    {
        private readonly IEnvironmentProvider target = new EnvironmentProviderImpl();

        [Test]
        void SupportedAliases_AreFromKeys()
        {
            var result = target.SupportedAliases;
            Assert.Count(1, result);
            Assert.Contains(result, "VALUE");
        }

        class EnvironmentProviderImpl : DefaultEnvironmentProvider
        {
            public EnvironmentProviderImpl()
                : base(typeof(Keys))
            {
            }

            public override Task<dynamic> GetAsync(string alias, DecisionContext context)
            {
                throw new NotImplementedException();
            }
        }

        static class Keys
        {
            public const string KEY = "VALUE";
        }
    }
}
