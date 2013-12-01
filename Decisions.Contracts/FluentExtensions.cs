using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public static DecisionContext Within(this DecisionContext context, string component)
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
        /// <param name="anonymousObject">The anonymous object.</param>
        /// <returns>
        /// The modified <see cref="DecisionContext" />
        /// </returns>
        public static DecisionContext On(this DecisionContext context, object anonymousObject)
        {
            context = context ?? new DecisionContext();
            context.SetTargetProperty(anonymousObject);
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
        public static DecisionContext On(this DecisionContext context, string target)
        {
            context = context ?? new DecisionContext();
            
            var expandoObject = new ExpandoObject();
            var collection = (ICollection<KeyValuePair<string, object>>)expandoObject;

            foreach (var keyValue in target.Split('|').Select(a => a.Split('=')).Select(a => new KeyValuePair<string, string>(a.First(), a.Last())))
            {
                // currently the value is a string. Try some common primitive type casts so that the Target as a better object to work with
                int intValue;
                Guid guidValue;
                if (int.TryParse(keyValue.Value, out intValue))
                {
                    collection.Add(new KeyValuePair<string, object>(keyValue.Key, intValue));
                }
                else if (Guid.TryParse(keyValue.Value, out guidValue))
                {
                    collection.Add(new KeyValuePair<string, object>(keyValue.Key, guidValue));
                }
                else
                {
                    // otherwise just use the string
                    collection.Add(new KeyValuePair<string, object>(keyValue.Key, keyValue.Value));
                }
            }
            context.SetTargetProperty(expandoObject);

            return context;
        }

        /// <summary>
        /// Sets the role for the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="role">The role.</param>
        /// <returns>The modified <see cref="DecisionContext"/></returns>
        public static DecisionContext Has(this DecisionContext context, string role)
        {
            context = context ?? new DecisionContext();
            context.Role = role;
            return context;
        }
    }
}