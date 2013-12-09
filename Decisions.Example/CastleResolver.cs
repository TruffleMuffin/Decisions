using System;
using System.Threading;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Decisions.Contracts;

namespace Decisions.Example
{
    /// <summary>
    /// The IoC resolver for castle
    /// </summary>
    internal sealed class CastleResolver : IResolver
    {
        /// <summary>
        /// The windsor castle configuration containers (lazy and explicitly set)
        /// </summary>
        private static readonly Lazy<IWindsorContainer> lazyContainer = new Lazy<IWindsorContainer>(CreateContainer, LazyThreadSafetyMode.PublicationOnly);

        private static IWindsorContainer container;

        /// <summary>
        /// Gets or sets the container, which is lazily initialized if not explicitly set.
        /// </summary>
        public static IWindsorContainer Container
        {
            get { return container ?? lazyContainer.Value; }
            set { container = value; }
        }

        /// <summary>
        /// Gets or sets the config file path.
        /// </summary>
        /// <value>The config file.</value>
        /// <remarks>
        /// The path to the configuration file to load.  If this is
        /// not set then config will be loaded from application domain config,
        /// for example web.config.
        /// </remarks>
        public static string ConfigFilePath { get; set; }

        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <typeparam name="T">The type of the service to get</typeparam>
        /// <returns>The service instance</returns>
        public T Get<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Gets an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The service instance
        /// </returns>
        public object Get(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance)
        {
            Container.Release(instance);
        }

        /// <summary>
        /// Determines whether Resolver has a component of type registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the Resolver has the type registered; otherwise, <c>false</c>.
        /// </returns>
        public bool Has(Type type)
        {
            return Container.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Ensures the container.
        /// </summary>
        private static IWindsorContainer CreateContainer()
        {
            IResource config;
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                config = new ConfigResource("castle");
            }
            else
            {
                var uri = new CustomUri(string.Concat("file://", ConfigFilePath));
                config = new FileResource(uri);
            }

            return new WindsorContainer(new XmlInterpreter(config));
        }
    }
}