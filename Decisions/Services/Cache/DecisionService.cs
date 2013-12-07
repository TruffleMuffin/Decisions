using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Decisions.Contracts;

namespace Decisions.Services.Cache
{
    /// <summary>
    /// An implementation of <see cref="IDecisionService"/> that caches decisions for short periods of time.
    /// </summary>
    public sealed class DecisionService : IDecisionService
    {
        private readonly IDecisionService service;
        private readonly ConcurrentDictionary<string, CacheItem> cache = new ConcurrentDictionary<string, CacheItem>();
        private readonly int cacheDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionService"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public DecisionService(IDecisionService service)
            : this(service, 5)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionService"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="cacheDuration">Duration of the cache in seconds.</param>
        public DecisionService(IDecisionService service, int cacheDuration)
        {
            this.service = service;
            this.cacheDuration = cacheDuration;
        }

        /// <summary>
        /// Determines the result of the specified <see cref="context" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public async Task<bool> CheckAsync(DecisionContext context)
        {
            CacheItem item;
            if (cache.TryGetValue(Key(context), out item))
            {
                if (item.CachedUntil > DateTime.Now)
                {
                    return (bool)item.Value;
                }

                cache.TryRemove(Key(context), out item);
            }

            var result = await service.CheckAsync(context);

            item = new CacheItem { Value = result, CachedUntil = DateTime.Now.AddSeconds(cacheDuration) };
            cache.AddOrUpdate(Key(context), item, (key, oldValue) => item);

            return result;
        }

        /// <summary>
        /// Creates a unique cache key for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A unique string representing the parameters</returns>
        private static string Key(DecisionContext context)
        {
            return context.Id;
        }
    }
}
