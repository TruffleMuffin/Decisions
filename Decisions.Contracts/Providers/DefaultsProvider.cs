using System.Threading;

namespace Decisions.Contracts.Providers
{
    /// <summary>
    /// A simple implementation of the <see cref="IDecisionDefaultsProvider"/> that uses the Thread.CurrentPrincipal to determine a SourceId from the Identity.Name.
    /// </summary>
    public class DefaultsProvider : IDecisionDefaultsProvider
    {
        /// <summary>
        /// Gets the default source id.
        /// </summary>
        public string SourceId
        {
            get
            {
                if (Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity == null) return null;

                return Thread.CurrentPrincipal.Identity.Name;
            }
        }
    }
}
