using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Decisions.Contracts
{
    /// <summary>
    /// Describes a decision context, containing all relevant information for getting a Decision result from the <see cref="IDecisionsService"/>.
    /// </summary>
    public class DecisionContext
    {
        private readonly ConcurrentDictionary<string, object> targetEntries = new ConcurrentDictionary<string, object>();

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
                return string.Format(ID_FORMAT, Component, SourceId, Role, Uri.EscapeDataString(string.Join("|", targetEntries.Select(a => a.Key + "=" + a.Value))));
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
        public dynamic Target
        {
            get
            {
                var expandoObject = new ExpandoObject();
                var collection = (ICollection<KeyValuePair<string, object>>)expandoObject;

                foreach (var keyValue in targetEntries)
                {
                    collection.Add(keyValue);
                }

                return expandoObject;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="DecisionContext"/>
        /// </summary>
        /// <returns>An empty <see cref="DecisionContext"/></returns>
        public static DecisionContext Create()
        {
            return new DecisionContext();
        }

        /// <summary>
        /// Adds a Target property
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal void SetTargetProperty(string key, object value)
        {
            targetEntries.AddOrUpdate(key, a => value, (currentKey, oldItem) => value);
        }
    }
}