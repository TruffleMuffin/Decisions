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
        /// Gets the Globally Unique Identifier that identifies this Policy.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Make a decisions on the <see cref="Decision"/> of this policy for the provided <see cref="DecisionContext"/>.
        /// </summary>
        /// <returns></returns>
        public abstract Decision Decide(DecisionContext context);

        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The environment</returns>
        protected async Task<dynamic> GetEnvironmentAsync(string key)
        {
            return await service.GetAsync(key);
        }

        /// <summary>
        /// Sets the environment provider.
        /// </summary>
        /// <param name="service">The service.</param>
        internal void SetEnvironmentProvider(IEnvironmentService service)
        {
            this.service = service;
        }
    }
}
