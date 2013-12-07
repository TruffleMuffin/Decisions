using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Decisions.Contracts;

namespace Decisions.Application.Services
{
    /// <summary>
    /// A service for retrieving policies regarding a <see cref="DecisionContext"/> for use in determining its Decision via the <see cref="IDecisionService"/>.
    /// </summary>
    public sealed class PolicyService
    {
        private readonly ConcurrentDictionary<string, IPolicy> policies = new ConcurrentDictionary<string, IPolicy>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyService" /> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="service">The service.</param>
        /// <exception cref="System.ArgumentException">Some providers are required to initialize the Decisions.Application.Services.PolicyService</exception>
        public PolicyService(IEnumerable<IPolicyProvider> providers, IEnvironmentService service)
        {
            if (providers == null || providers.Any() == false) throw new ArgumentException("Some providers are required to initialize the Decisions.Application.Services.PolicyService");

            foreach (var policyList in providers.Select(policyProvider => policyProvider.GetPolicies().ToList()))
            {
                policyList.Where(a => a.Value.Service == null).ToList().ForEach(a => a.Value.Service = service);
                policyList.ForEach(a => policies.AddOrUpdate(a.Key, a.Value, (key, oldValue) => { throw new ArgumentException("A Policy with this key has already been registered.", key); }));
            }
        }

        /// <summary>
        /// Gets the <see cref="AbstractPolicy" /> with the specified key.
        /// </summary>
        /// <param name="alias">The key that can be used to lookup the policy.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public IPolicy Get(string alias)
        {
            return policies[alias];
        }
    }
}
