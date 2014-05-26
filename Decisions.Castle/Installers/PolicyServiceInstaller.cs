using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Decisions.Castle.Providers;
using Decisions.Contracts;
using Decisions.Services;

namespace Decisions.Castle.Installers
{
    /// <summary>
    /// Installs Decisions Policy Service implementation with the <see cref="ConfigPolicyProvider"/> and all other Castle registered implementations of <see cref="IPolicyProvider"/>.
    /// </summary>
    public class PolicyServiceInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            container.Register(Component.For<IPolicyProvider>().ImplementedBy<ConfigPolicyProvider>().DependsOn(Dependency.OnAppSettingsValue("configPath", "Decisions.ConfigurationPath")));
            container.Register(Component.For<PolicyService>());
        }
    }
}