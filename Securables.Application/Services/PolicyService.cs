using Securables.Contracts;
using System.Threading.Tasks;

namespace Securables.Application.Services
{
    /// <summary>
    /// A service for retrieving policies regarding a <see cref="DecisionContext"/> for use in determining its <see cref="Decision"/> via the <see cref="ISecurablesService"/>.
    /// </summary>
    internal class PolicyService
    {
        /// <summary>
        /// Gets the <see cref="AbstractPolicy" /> with the specified key asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public async Task<AbstractPolicy> GetAsync(string key)
        {
            return await Task.FromResult((AbstractPolicy)null);
        }
    }
}
