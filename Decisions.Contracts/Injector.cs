using System;

namespace Decisions.Contracts
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
            get
            {
                if (resolver == null) throw new NotImplementedException("A resolve must be specified for Decisions.Contracts.Injector.");

                return resolver;
            }
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

        /// <summary>
        /// Determines whether Resolver has a component of type registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the Resolver has the type registered; otherwise, <c>false</c>.
        /// </returns>
        public static bool Has(Type type)
        {
            return Resolver.Has(type);
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void Release(object instance)
        {
            Resolver.Release(instance);
        }
    }
}