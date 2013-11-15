using Securables.Contracts;

namespace Securables.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which always Denies.
    /// </summary>
    class BetaPolicy : AbstractPolicy
    {
        /// <summary>
        /// Gets the Globally Unique Identifier that identifies this Policy.
        /// </summary>
        public override string Id
        {
            get { return "Example.Beta"; }
        }

        /// <summary>
        /// Make a decisions on the <see cref="Decision" /> of this policy for the provided <see cref="DecisionContext" />.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Decision Decide(DecisionContext context)
        {
            return Decision.Deny;
        }
    }
}