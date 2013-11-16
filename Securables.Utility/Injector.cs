using System;

namespace Securables.Utility
{
    /// <summary>
    /// An inversion of control injector for resolving objects.
    /// </summary>
    public class Injector
    {
        /// <summary>
        /// The resolver to use for Inversion of Control
        /// </summary>
        private static IResolver resolver;

        /// <summary>
        /// Gets or sets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public static IResolver Resolver
        {
            get { return resolver ?? (resolver = new CastleResolver()); }
            set { resolver = value; }
        }

        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>The instance.</returns>
        public static T Get<T>()
        {
            return Resolver.Get<T>();
        }

        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <param name="type">The type of the instance.</param>
        /// <returns>The instance.</returns>
        public static object Get(Type type)
        {
            return Resolver.Get(type);
        }

    }
}