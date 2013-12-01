using Decisions.Contracts;

namespace Decisions.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which always Permits.
    /// </summary>
    public class AlphaPolicy : AbstractPolicy
    {
        /// <summary>
        /// Make a decisions on the Decision of this policy for the provided <see cref="DecisionContext" />.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool Decide(DecisionContext context)
        {
            return context.Target.id == 1;
        }
    }
}
