using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Decisions.Contracts;
using Decisions.Services;

namespace Decisions.Castle.Installers
{
    /// <summary>
    /// Installs Decisions Environment Service implementation with all <see cref="IEnvironmentProvider"/> implementations available from the bin folder.
    /// </summary>
    public class EnvironmentServiceInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter("bin")).BasedOn<IEnvironmentProvider>().WithServiceFirstInterface());
            container.Register(Component.For<EnvironmentService>().DynamicParameters((k, p) => p.Add("providers", k.ResolveAll<IEnvironmentProvider>())));
        }
    }
}