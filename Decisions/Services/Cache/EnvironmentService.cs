using System;
using System.Threading.Tasks;
using Decisions.Contracts;
using TruffleCache;

namespace Decisions.Services.Cache
{
    /// <summary>
    /// An implementation of <see cref="IEnvironmentService"/> that caches environments for short periods of time.
    /// </summary>
    public sealed class EnvironmentService : IEnvironmentService
    {
        private const string CACHE_KEY_FORMAT = @"{0}_{1}";
        private readonly IEnvironmentService service;
        private readonly Cache<object> cache;
        private readonly int cacheDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="cache">The cache.</param>
        public EnvironmentService(IEnvironmentService service, Cache<object> cache)
            : this(service, cache, 15)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="cacheDuration">Duration of the cache in seconds.</param>
        public EnvironmentService(IEnvironmentService service, Cache<object> cache, int cacheDuration)
        {
            this.service = service;
            this.cache = cache;
            this.cacheDuration = cacheDuration;
        }

        /// <summary>
        /// Gets the environment with the specified alias asynchronously.
        /// </summary>
        /// <param name="alias">The globally unique alias used to represent a specific environment.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<dynamic> GetAsync(string alias, DecisionContext context)
        {
            var result = cache.Get(Key(alias, context));

            if (result == null)
            {
                result = await service.GetAsync(alias, context);
                cache.Set(Key(alias, context), result, TimeSpan.FromSeconds(cacheDuration));
            }

            return result;
        }

        /// <summary>
        /// Creates a unique cache key for the alias and context
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="context">The context.</param>
        /// <returns>A unique string representing the parameters</returns>
        private static string Key(string alias, DecisionContext context)
        {
            return string.Format(CACHE_KEY_FORMAT, alias, context.Id);
        }
    }
}