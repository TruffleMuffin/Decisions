using System.Collections.Generic;

namespace Securables.Contracts
{
    /// <summary>
    /// Describes a Policy Provider that can be linked into the Securables component to provide bespoke policies. This is an extensibility point
    /// which will allow developers to create for their components multiple policy providers - one for each subcomponent - which return policies
    /// that will describe an aspect of the security check this wish to run via the <see cref="ISecurablesService"/>.
    /// </summary>
    public interface IPolicyProvider
    {
        /// <summary>
        /// Gets or sets the name of the component which this instance provides policies for.
        /// </summary>
        string Component { get; }

        /// <summary>
        /// Gets all the policies this instance of the provider wishes to offer.
        /// </summary>
        /// <returns>A dictionary of policies, the key should be a identifier that is used to link to the decision.</returns>
        IDictionary<string, AbstractPolicy> GetPolicies();
    }
}