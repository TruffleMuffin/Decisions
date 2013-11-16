using System.Threading.Tasks;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes the basic Securable policy. This extensibility point allows developers to write their own policies that can be used
    /// as part of the Decision process executed on <see cref="ISecurablesService"/>.
    /// </summary>
    public abstract class AbstractPolicy
    {
        /// <summary>
        /// Gets or sets the <see cref="IEnvironmentService"/>. The service can be used to retrieve Environments to process a <see cref="AbstractPolicy"/>.
        /// </summary>
        /// <remarks>Guaranteed to be set by Securables if not set manually</remarks>
        public IEnvironmentService Service { get; set; }
        
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
            if (Service == null) return null;

            return await Service.GetAsync(key, context);
        }
    }
}
