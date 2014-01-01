using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Decisions.Contracts;
using Decisions.Services.Cache;
using TruffleCache;
using CacheDecisionService = Decisions.Services.Cache.DecisionService;
using DecisionService = Decisions.Services.DecisionService;
using CacheEnvironmentService = Decisions.Services.Cache.EnvironmentService;
using EnvironmentService = Decisions.Services.EnvironmentService;

namespace Decisions.Castle.Installers
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
            container.Register(Component.For<Cache<Decision>>().DependsOn(Dependency.OnValue("keyPrefix", "Decisions")));
            container.Register(Component.For<Cache<object>>().DependsOn(Dependency.OnValue("keyPrefix", "Environments")));
            container.Register(Component.For<DecisionService>().DependsOn(Dependency.OnAppSettingsValue("configPath", "Decisions.ConfigurationPath")));
            
            container.Register(
                Component
                .For<IEnvironmentService>()
                .ImplementedBy<CacheEnvironmentService>()
                .DependsOn(
                    Dependency.OnValue<IEnvironmentService>(container.Resolve<EnvironmentService>()),
                    Dependency.OnAppSettingsValue("cacheDuration", "Decisions.EnvironmentCacheDuration")
                )
            );
            container.Register(
                Component
                .For<IDecisionService>()
                .ImplementedBy<CacheDecisionService>()
                .DependsOn(
                    Dependency.OnComponent<IDecisionService, DecisionService>(),
                    Dependency.OnAppSettingsValue("cacheDuration", "Decisions.DecisionsCacheDuration")
                )
            );
        }
    }
}