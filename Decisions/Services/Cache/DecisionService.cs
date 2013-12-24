using System;
using System.Threading.Tasks;
using Decisions.Contracts;
using TruffleCache;

namespace Decisions.Services.Cache
{
    /// <summary>
    /// An implementation of <see cref="IDecisionService"/> that caches decisions for short periods of time.
    /// </summary>
    public sealed class DecisionService : IDecisionService
    {
        private readonly IDecisionService service;
        private readonly Cache<Decision> cache;
        private readonly int cacheDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionService" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="cache">The cache.</param>
        public DecisionService(IDecisionService service, Cache<Decision> cache)
            : this(service, cache, 5)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionService" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="cacheDuration">Duration of the cache in seconds.</param>
        public DecisionService(IDecisionService service, Cache<Decision> cache, int cacheDuration)
        {
            this.service = service;
            this.cache = cache;
            this.cacheDuration = cacheDuration;
        }

        /// <summary>
        /// Determines the result of the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public async Task<bool> CheckAsync(DecisionContext context)
        {
            var result = cache.Get(Key(context));

            if (result == null)
            {
                result = new Decision { Id = context.Id, Result = await service.CheckAsync(context) };
                cache.Set(Key(context), result, TimeSpan.FromSeconds(cacheDuration));
            }

            return result.Result;
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
