using System;
using System.Collections.Generic;
using System.Linq;

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
        public static DecisionContext As(this DecisionContext context, string sourceId)
        {
            context = context ?? new DecisionContext();
            context.SourceId = sourceId;
            return context;
        }

        /// <summary>
        /// Sets a target key/value for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The modified <see cref="DecisionContext" />
        /// </returns>
        public static DecisionContext Against(this DecisionContext context, string key, object value)
        {
            context = context ?? new DecisionContext();
            context.SetTargetProperty(key, value);
            return context;
        }

        /// <summary>
        /// Sets all target key/value pairs encoded within the string for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The modified <see cref="DecisionContext" />
        /// </returns>
        public static DecisionContext Against(this DecisionContext context, string target)
        {
            context = context ?? new DecisionContext();
            foreach (var keyValue in target.Split('|').Select(a => a.Split('=')).Select(a => new KeyValuePair<string, string>(a.First(), a.Last())))
            {
                // currently the value is a string. Try some common primitive type casts so that the Target as a better object to work with
                int intValue;
                Guid guidValue;
                if (int.TryParse(keyValue.Value, out intValue))
                {
                    context.SetTargetProperty(keyValue.Key, intValue);
                }
                else if (Guid.TryParse(keyValue.Value, out guidValue))
                {
                    context.SetTargetProperty(keyValue.Key, guidValue);
                }
                else
                {
                    // otherwise just use the string
                    context.SetTargetProperty(keyValue.Key, keyValue.Value);
                }
            }
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