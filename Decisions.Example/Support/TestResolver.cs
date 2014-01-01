using System;
using System.Collections.Generic;
using System.IO;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Services;
using Decisions.WebHost.API;

namespace Decisions.Example.Support
{
    public class TestResolver : IResolver
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private DecisionService DecisionService;
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
            DecisionService = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
            controller = new DecideController(DecisionService);
        }

        public T Get<T>()
        {
            return (T)(object)DecisionService;
        }

        public object Get(Type type)
        {
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