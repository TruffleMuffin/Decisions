using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Decisions.Application.Services;
using Decisions.Contracts;
using EnvironmentService = Decisions.Application.Services.Cache.EnvironmentService;

namespace Decisions.Installers
{
    /// <summary>
    /// Installs Decisions using Cache services as wrappers to the actual services.
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
            container.Register(Component.For<DecisionService>().DependsOn(Dependency.OnAppSettingsValue("configPath", "Decisions.ConfigurationPath")));
            container.Register(
                Component
                .For<IEnvironmentService>()
                .ImplementedBy<EnvironmentService>()
                .DependsOn(
                    Dependency.OnComponent<IEnvironmentService, Application.Services.EnvironmentService>(),
                    Dependency.OnAppSettingsValue("cacheDuration", "Decisions.EnvironmentCacheDuration")
                )
            );
            container.Register(
                Component
                .For<IDecisionService>()
                .ImplementedBy<Application.Services.Cache.DecisionService>()
                .DependsOn(
                    Dependency.OnComponent<IDecisionService, DecisionService>(),
                    Dependency.OnAppSettingsValue("cacheDuration", "Decisions.DecisionsCacheDuration")
                )
            );
        }
    }
}