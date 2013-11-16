using System;
using System.Collections.Generic;
using System.IO;
using Securables.API;
using Securables.Application.Services;
using Securables.Contracts;
using Securables.Contracts.Providers;
using Securables.Tests.Support;
using Securables.Utility;

namespace Securables.Tests.Utility.Filters
{
    internal class TestResolver : IResolver
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private SecurablesService securablesService;
        private DecideController controller;

        public TestResolver()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            var policies = new Dictionary<string, AbstractPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() },
                    { "G", new DeltaPolicy {AclEnvironmentKey = "Acl"} }, 
                    { "H", new DeltaPolicy {AclEnvironmentKey = "LongRunning"} }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            securablesService = new SecurablesService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Securables.config"), policyService);
            controller = new DecideController(securablesService);
        }

        public T Get<T>()
        {
            return (T)(object)securablesService;
        }

        public object Get(Type type)
        {
            if (type == typeof(ValuesResolver)) return new ValuesResolver();
            if (type == typeof(DecideController)) return controller;

            return null;
        }

        public void Release(object instance)
        {
        }

        public bool Has(Type type)
        {
            return type == typeof(DecideController);
        }
    }
}