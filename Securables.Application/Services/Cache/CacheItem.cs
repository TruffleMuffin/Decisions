using System;

namespace Securables.Application.Services.Cache
{
    /// <summary>
    /// A cacheable item
    /// </summary>
    internal class CacheItem
    {
        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the cached until date and time.
        /// </summary>
        public DateTime CachedUntil { get; set; }
    }
}