using System.Threading.Tasks;

namespace Decisions.Contracts
{
    /// <summary>
    /// Describes a operations that can be performed using Decisions.
    /// </summary>
    public interface IDecisionService
    {
        /// <summary>
        /// Determines the result of the specified <see cref="context"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        Task<bool> CheckAsync(DecisionContext context);
    }
}
