using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which always Permits.
    /// </summary>
    class AlphaPolicy : AbstractPolicy
    {
        /// <summary>
        /// Make a decisions on the <see cref="Decision" /> of this policy for the provided <see cref="DecisionContext" />.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool Decide(DecisionContext context)
        {
            return true;
        }
    }
}
