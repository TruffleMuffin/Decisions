using System.Threading.Tasks;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes a service for retrieving environment variables for use in the <see cref="ISecurablesService"/>
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Gets the environment with the specified alias asynchronously.
        /// </summary>
        /// <param name="alias">The globally unique alias used to represent a specific environment.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        Task<dynamic> GetAsync(string alias, DecisionContext context);
    }
}