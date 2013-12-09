using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Decisions.Contracts;

namespace Decisions.Utility
{
    /// <summary>
    /// An implementation of the <see cref="IDependencyResolver"/> interface which uses <see cref="Injector"/>
    /// to supply dependencies.
    /// </summary>
    public class InjectorDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// The instances that have been initialised within this scope
        /// </summary>
        private readonly List<object> instances = new List<object>();

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public object GetService(Type t)
        {
            // Return NULL here if not in the Container, calling service will revert to another mechnaism for getting the Service
            var service = Injector.Has(t) ? Injector.Get(t) : null;

            AddToScope(service);

            return service;
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type t)
        {
            var services = Injector.Has(t) ? new[] { Injector.Get(t) } : new object[0];

            AddToScope(services);

            return services;
        }

        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <returns>
        /// The dependency scope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            return new InjectorDependencyResolver();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var instance in instances)
            {
                Injector.Release(instance);
            }

            instances.Clear();
        }

        /// <summary>
        /// Adds the services to the registered instances within this scope.
        /// </summary>
        /// <param name="services">The services.</param>
        private void AddToScope(params object[] services)
        {
            if (services.Any(a => a != null))
            {
                instances.AddRange(services);
            }
        }
    }
}