using System;
using System.Collections.Generic;
using System.IO;
using Decisions.API;
using Decisions.Application.Services;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Tests.Support;
using Decisions.Utility;

namespace Decisions.Tests.Utility.Filters
{
    internal class TestResolver : IResolver
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private DecisionsService DecisionsService;
        private DecideController controller;

        public TestResolver()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            var policies = new Dictionary<string, IPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() },
                    { "G", new DeltaPolicy() }, 
                    { "H", new LongDeltaPolicy() }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            DecisionsService = new DecisionsService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
            controller = new DecideController(DecisionsService);
        }

        public T Get<T>()
        {
            return (T)(object)DecisionsService;
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