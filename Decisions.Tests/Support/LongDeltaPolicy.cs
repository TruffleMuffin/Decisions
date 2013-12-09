using System.Linq;
using Decisions.Contracts;

namespace Decisions.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the acl entries contains a true
    /// </summary>
    public class LongDeltaPolicy : AbstractPolicy
    {
        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync(LongAclEnvironment.ALIAS, context);
            envTask.Wait();
            var env = envTask.Result as LongAclEnvironment;
            return env.Entries.Any(a => a.Allow);
        }
    }
}