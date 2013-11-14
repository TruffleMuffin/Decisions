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
        private readonly ConcurrentDictionary<string, IEnvironmentProvider> environmentProviders = new ConcurrentDictionary<string, IEnvironmentProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public EnvironmentService(IEnumerable<IEnvironmentProvider> providers)
        {
            if (providers == null || providers.Any() == false) throw new ArgumentException("Some providers are required to initialize the Securables.Application.Services.EnvironmentService");

            foreach (var environmentProvider in providers)
            {
                // Note: Avoid foreach closure trap changing variable assignment to be unpredictable
                var provider = environmentProvider;
                environmentProviders.AddOrUpdate(environmentProvider.Component, provider, (key, oldValue) => provider);
            }
        }

        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<dynamic> GetAsync(string component, string key, DecisionContext context)
        {
            return await environmentProviders[component].GetAsync(key, context);
        }
    }
}