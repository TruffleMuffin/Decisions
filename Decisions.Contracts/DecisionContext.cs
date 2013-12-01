using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decisions.Contracts
{
    /// <summary>
    /// Describes a decision context, containing all relevant information for getting a Decision result from the <see cref="IDecisionsService"/>.
    /// </summary>
    public class DecisionContext
    {
        /// <summary>
        /// The string format for the Id of this instance
        /// </summary>
        private const string ID_FORMAT = "{0}/{1}/{2}/{3}";

        /// <summary>
        /// Gets or sets the Globally Unique Identifier that identifies this specific instance uniquely.
        /// </summary>
        public string Id
        {
            get
            {
                return string.Format(ID_FORMAT, Component, SourceId, Role, Uri.EscapeDataString(string.Join("|", SerializeTarget())));
            }
        }

        /// <summary>
        /// Gets or sets the name of the component which the decision should be made within.
        /// </summary>
        public string Component { get; internal set; }

        /// <summary>
        /// Gets or sets the source id the decision should be made for.
        /// </summary>
        public string SourceId { get; internal set; }

        /// <summary>
        /// Gets or sets the role the decision should be about.
        /// </summary>
        public string Role { get; internal set; }

        /// <summary>
        /// Gets or sets the target which the decision should be on.
        /// </summary>
        public dynamic Target { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="DecisionContext"/>
        /// </summary>
        /// <returns>An empty <see cref="DecisionContext"/></returns>
        public static DecisionContext Create()
        {
            return new DecisionContext();
        }

        /// <summary>
        /// Sets the target property.
        /// </summary>
        /// <param name="anonymousObject">The anonymous object.</param>
        internal void SetTargetProperty(object anonymousObject)
        {
            this.Target = anonymousObject;
        }

        /// <summary>
        /// Serializes the target into an IEnumerable of strings representing its Key/Value pairs for Properties.
        /// </summary>
        /// <returns>A string for each Property on Target</returns>
        private IEnumerable<string> SerializeTarget()
        {
            // If there is no target, do not attempt to serialize
            if (Target == null)
            {
                yield break;
            }

            foreach (var prop in Target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return prop.Name + "=" + prop.GetValue(Target, null);
            }
        }
    }
}