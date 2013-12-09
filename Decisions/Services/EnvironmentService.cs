using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decisions.Contracts;

namespace Decisions.Services
{
    /// <summary>
    /// A service for retrieving environments regarding a <see cref="DecisionContext"/> for use in determining its Decision via the <see cref="IDecisionService"/>.
    /// </summary>
    public sealed class EnvironmentService : IEnvironmentService
    {
        private readonly ConcurrentDictionary<string, IEnvironmentProvider> environments = new ConcurrentDictionary<string, IEnvironmentProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public EnvironmentService(IEnumerable<IEnvironmentProvider> providers)
        {
            if (providers == null || providers.Any() == false) throw new ArgumentException("Some providers are required to initialize the Decisions.Application.Services.EnvironmentService");

            foreach (var provider in providers)
            {
                foreach (var key in provider.SupportedAliases)
                {
                    environments.AddOrUpdate(key, provider, (k, oldValue) => { throw new ArgumentException("A Environment Provider with this key has already been registered.", k); });
                }
            }
        }

        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="alias">The key.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<dynamic> GetAsync(string alias, DecisionContext context)
        {
            IEnvironmentProvider provider;
            if (environments.TryGetValue(alias, out provider)) return await provider.GetAsync(alias, context);
            return null;
        }
    }
}