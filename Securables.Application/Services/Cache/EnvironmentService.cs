using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Securables.Contracts;
using InnerService = Securables.Application.Services.EnvironmentService;

namespace Securables.Application.Services.Cache
{
    /// <summary>
    /// An implementation of <see cref="IEnvironmentService"/> that caches environments based on <see cref="CacheOptions"/>.
    /// </summary>
    internal class EnvironmentService : IEnvironmentService
    {
        private const string CACHE_KEY_FORMAT = @"{0}_{1}";
        private readonly InnerService service;
        private readonly ConcurrentDictionary<string, CacheItem> cache = new ConcurrentDictionary<string, CacheItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public EnvironmentService(InnerService service)
        {
            this.service = service;
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
            CacheItem item;
            if (cache.TryGetValue(Key(alias, context), out item))
            {
                if (item.CachedUntil > DateTime.Now)
                {
                    return item.Value;
                }

                cache.TryRemove(Key(alias, context), out item);
            }

            var result = await service.GetAsync(alias, context);

            if (result != null)
            {
                var options = service.GetCacheOptions(alias);
                if (options.Cacheable)
                {
                    item = new CacheItem {Value = result, CachedUntil = DateTime.Now.Add(options.Period)};
                    cache.AddOrUpdate(Key(alias, context), item, (key, oldValue) => item);
                }
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

        /// <summary>
        /// A cacheable item
        /// </summary>
        internal class CacheItem
        {
            /// <summary>
            /// Gets or sets the value of the item.
            /// </summary>
            public dynamic Value { get; set; }

            /// <summary>
            /// Gets or sets the cached until date and time.
            /// </summary>
            public DateTime CachedUntil { get; set; }
        }
    }
}
