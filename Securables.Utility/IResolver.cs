using System;

namespace Securables.Utility
{
    /// <summary>
    /// A resolver supporting the <see cref="Injector"/> class.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <typeparam name="T">The type of the service to get</typeparam>
        /// <returns>
        /// The service instance
        /// </returns>
        T Get<T>();

        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The service instance
        /// </returns>
        object Get(Type type);
    }
}