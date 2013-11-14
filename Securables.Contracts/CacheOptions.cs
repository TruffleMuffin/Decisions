using System;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes options for caching.
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity providing this instance has cacheable results.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cacheable; otherwise, <c>false</c>.
        /// </value>
        public bool Cacheable { get; set; }

        /// <summary>
        /// Gets or sets the period items should be cached for if <see cref="Cacheable"/> is true.
        /// </summary>
        public TimeSpan Period { get; set; }
    }
}