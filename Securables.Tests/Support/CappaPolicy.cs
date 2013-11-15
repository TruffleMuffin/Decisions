using System;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which will Permit if the current user is a match
    /// </summary>
    class CappaPolicy : AbstractPolicy
    {
        public Guid MatchUserId { get; set; }

        public string CurrentUserEnvironmentKey { get; set; }
        
        public override bool Decide(DecisionContext context)
        {
            var envTask = GetEnvironmentAsync(context.Component, CurrentUserEnvironmentKey, context);
            envTask.Wait();
            var env = envTask.Result as CurrentUserEnvironment;
            return env.UserId == MatchUserId;
        }
    }
}