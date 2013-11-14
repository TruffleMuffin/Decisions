using System.Threading.Tasks;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes a service for retrieving environment variables for use in the <see cref="ISecurablesService"/>
    /// </summary>
    internal interface IEnvironmentService
    {
        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>An environment, likely an instance of a class from an external assembly.</returns>
        Task<dynamic> GetAsync(string key);
    }
}