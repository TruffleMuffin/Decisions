namespace Decisions.Contracts
{
    /// <summary>
    /// Fluent extensions <see cref="DecisionContext"/> for constructing the context nicely.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// Sets the component for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="component">The component.</param>
        /// <returns>The modified <see cref="DecisionContext"/></returns>
        public static DecisionContext For(this DecisionContext context, string component)
        {
            context = context ?? new DecisionContext();
            context.Component = component;
            return context;
        }

        /// <summary>
        /// Sets the sourceId for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceId">The sourceId.</param>
        /// <returns>The modified <see cref="DecisionContext"/></returns>
        public static DecisionContext Source(this DecisionContext context, string sourceId)
        {
            context = context ?? new DecisionContext();
            context.SourceId = sourceId;
            return context;
        }

        /// <summary>
        /// Sets the targetId for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="targetId">The targetId.</param>
        /// <returns>The modified <see cref="DecisionContext"/></returns>
        public static DecisionContext Target(this DecisionContext context, string targetId)
        {
            context = context ?? new DecisionContext();
            context.TargetId = targetId;
            return context;
        }

        /// <summary>
        /// Sets the role for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="role">The role.</param>
        /// <returns>The modified <see cref="DecisionContext"/></returns>
        public static DecisionContext On(this DecisionContext context, string role)
        {
            context = context ?? new DecisionContext();
            context.Role = role;
            return context;
        }
    }
}