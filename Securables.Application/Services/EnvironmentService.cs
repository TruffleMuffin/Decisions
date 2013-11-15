using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Application.Services
{
    /// <summary>
    /// A service for retrieving environments regarding a <see cref="DecisionContext"/> for use in determining its Decision via the <see cref="ISecurablesService"/>.
    /// </summary>
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly ConcurrentDictionary<string, IEnvironmentProvider> environments = new ConcurrentDictionary<string, IEnvironmentProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public EnvironmentService(IEnumerable<IEnvironmentProvider> providers)
        {
            if (providers == null || providers.Any() == false) throw new ArgumentException("Some providers are required to initialize the Securables.Application.Services.EnvironmentService");

            foreach (var provider in providers)
            {
                foreach (var key in provider.SupportedKeys)
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

        /// <summary>
        /// Gets the cache options for the specified alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>A <see cref="CacheOptions"/></returns>
        internal CacheOptions GetCacheOptions(string alias)
        {
            IEnvironmentProvider provider;
            if (environments.TryGetValue(alias, out provider)) return provider.Cache;
            return null;
        }
    }
}