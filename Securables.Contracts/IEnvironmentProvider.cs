using System.Threading.Tasks;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes a Environment Provider that can be linked into the Securables component to provide bespoke environments. This is an extensibility point
    /// which will allow developers to create for their components multiple environment providers - one for each subomponent - which return environments
    /// that will describe information that is used as part of a <see cref="AbstractPolicy"/> in order to execute it in conjunction with the <see cref="ISecurablesService"/>.
    /// </summary>
    public interface IEnvironmentProvider
    {
        /// <summary>
        /// Gets the keys that this instance supports retrieval of environments for.
        /// </summary>
        string[] SupportedKeys { get; }
        
        /// <summary>
        /// Gets the environment with the specified <see cref="alias"/> using the <see cref="context"/> provided.
        /// </summary>
        /// <param name="alias">The globally unique alias used to represent a specific environment.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment specified to the provided information.
        /// </returns>
        Task<dynamic> GetAsync(string alias, DecisionContext context);
    }
}