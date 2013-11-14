using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Application.Services
{
    /// <summary>
    /// A service for retrieving environments regarding a <see cref="DecisionContext"/> for use in determining its <see cref="Decision"/> via the <see cref="ISecurablesService"/>.
    /// </summary>
    internal class EnvironmentService : IEnvironmentService
    {
        /// <summary>
        /// Gets the environment with the specified key asynchronously.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<dynamic> GetAsync(string component, string key)
        {
            return await Task.FromResult(new object());
        }
    }
}