using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Application.Services
{
    /// <summary>
    /// A service for retrieving environments regarding a <see cref="DecisionContext"/> for use in determining its <see cref="Decision"/> via the <see cref="ISecurablesService"/>.
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
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<dynamic> GetAsync(string key, DecisionContext context)
        {
            return await environments[key].GetAsync(key, context);
        }
    }
}