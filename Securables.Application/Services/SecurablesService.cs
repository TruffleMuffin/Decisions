using Securables.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Securables.Application.Services
{
    /// <summary>
    /// An implemenation of the <see cref="ISecurablesService "/> that handles execution of decisions.
    /// </summary>
    internal class SecurablesService : ISecurablesService
    {
        /// <summary>
        /// Determines the result of the specified <see cref="context" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="Decision" /> indicating the result of the query.
        /// </returns>
        public async Task<Decision> CheckAsync(DecisionContext context)
        {
            return await Task.FromResult(Decision.Deny);
        }

        /// <summary>
        /// Determines the results of the specified <see cref="contexts" />.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        /// <returns>
        /// A set of <see cref="Decision" /> indicating the results of the query.
        /// </returns>
        public async Task<IDictionary<string, Decision>> CheckAsync(IEnumerable<DecisionContext> contexts)
        {
            return await Task.FromResult(new Dictionary<string, Decision>());
        }
    }
}
