using System;
using Decisions.Contracts;

namespace Decisions.Example.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the current user is a match
    /// </summary>
    public class CappaPolicy : AbstractPolicy
    {
        public Guid MatchUserId { get; set; }

        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync<CurrentUserEnvironment>(CurrentUserEnvironment.ALIAS, context);
            envTask.Wait();
            return envTask.Result.UserId == MatchUserId;
        }
    }
}