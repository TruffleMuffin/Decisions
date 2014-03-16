using System.Linq;
using Decisions.Contracts;

namespace Decisions.Example.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the acl entries contains a true
    /// </summary>
    public class DeltaPolicy : AbstractPolicy
    {
        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync<AclEnvironment>(AclEnvironment.ALIAS, context);
            envTask.Wait();
            return envTask.Result.Entries.Any(a => a.Allow);
        }
    }
}