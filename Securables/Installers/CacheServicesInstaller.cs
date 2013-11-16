using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Securables.Application.Services;
using Securables.Contracts;
using CacheEnvironmentService = Securables.Application.Services.Cache.EnvironmentService;
using CacheSecurablesService = Securables.Application.Services.Cache.SecurablesService;

namespace Securables.Installers
{
    /// <summary>
    /// Installs Securables using Cache services as wrappers to the actual services.
    /// </summary>
    public class CacheServicesInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<SecurablesService>().DependsOn(Dependency.OnAppSettingsValue("configPath", "Securables.ConfigurationPath")));
            container.Register(
                Component
                .For<IEnvironmentService>()
                .ImplementedBy<CacheEnvironmentService>()
                .DependsOn(
                    Dependency.OnComponent<IEnvironmentService, EnvironmentService>(),
                    Dependency.OnAppSettingsValue("cacheDuration", "Securables.EnvironmentCacheDuration")
                )
            );
            container.Register(
                Component
                .For<ISecurablesService>()
                .ImplementedBy<CacheSecurablesService>()
                .DependsOn(
                    Dependency.OnComponent<ISecurablesService, SecurablesService>(),
                    Dependency.OnAppSettingsValue("cacheDuration", "Securables.SecurablesCacheDuration")
                )
            );
        }
    }
}