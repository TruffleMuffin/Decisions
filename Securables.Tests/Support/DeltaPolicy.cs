using System.Linq;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the acl entries contains a true
    /// </summary>
    class DeltaPolicy : AbstractPolicy
    {
        public string AclEnvironmentKey { get; set; }

        public override string Id
        {
            get { return "Example.Delta"; }
        }

        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync(context.Component, AclEnvironmentKey, context);
            envTask.Wait();
            var env = envTask.Result as AclEnvironment;
            return env.Entries.Any(a => a.Allow);
        }
    }
}