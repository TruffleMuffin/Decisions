using System.Collections.Generic;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes a operations that can be performed using Securables.
    /// </summary>
    public interface ISecurablesService
    {
        /// <summary>
        /// Determines the result of the specified <see cref="context"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="Decision" /> indicating the result of the query.
        /// </returns>
        Decision CheckAsync(DecisionContext context);

        /// <summary>
        /// Determines the results of the specified <see cref="contexts"/>.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        /// <returns>
        /// A set of <see cref="Decision" /> indicating the results of the query.
        /// </returns>
        IDictionary<string, Decision> CheckAsync(IEnumerable<DecisionContext> contexts);
    }
}
