using System.Threading.Tasks;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes the basic Securable policy. This extensibility point allows developers to write their own policies that can be used
    /// as part of the Decision process executed on <see cref="ISecurablesService"/>.
    /// </summary>
    public abstract class AbstractPolicy
    {
        private IEnvironmentService service;
        
        /// <summary>
        /// Make a decisions on the Decision of this policy for the provided <see cref="DecisionContext"/>.
        /// </summary>
        /// <returns>True if the policy passes, otherwise false.</returns>
        public abstract bool Decide(DecisionContext context);

        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The environment
        /// </returns>
        protected async Task<dynamic> GetEnvironmentAsync(string key, DecisionContext context)
        {
            return await service.GetAsync(key, context);
        }

        /// <summary>
        /// Sets the environment provider for this policy instance. The provider will be used to look up environments that the policy needs to complete its task.
        /// </summary>
        /// <param name="provider">The environment provider.</param>
        internal void SetEnvironmentProvider(IEnvironmentService provider)
        {
            this.service = provider;
        }
    }
}
