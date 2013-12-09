using System.Linq;
using Decisions.Contracts;

namespace Decisions.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the acl entries contains a true
    /// </summary>
    public class DeltaPolicy : AbstractPolicy
    {
        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync(AclEnvironment.ALIAS, context);
            envTask.Wait();
            var env = envTask.Result as AclEnvironment;
            return env.Entries.Any(a => a.Allow);
        }
    }
}