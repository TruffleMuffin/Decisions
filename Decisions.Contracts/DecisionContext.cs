using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Decisions.Contracts.IoC;

namespace Decisions.Contracts
{
    /// <summary>
    /// Describes a decision context, containing all relevant information for getting a Decision result from the <see cref="IDecisionService"/>.
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
                return string.Format(ID_FORMAT, Namespace, SourceId, Role, Uri.EscapeDataString(string.Join("|", SerializeTarget())));
            }
        }

        /// <summary>
        /// Gets or sets the name of the namespace which the decision should be made within.
        /// </summary>
        public string Namespace { get; internal set; }

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
        /// Applies the provided operation onto the <see cref="DecisionContext" />.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns>
        /// The modified <see cref="DecisionContext" />
        /// </returns>
        public static DecisionContext Create(Func<DecisionContext, DecisionContext> lambda)
        {
            return lambda(Create());
        }

        /// <summary>
        /// Checks the specified context against the <see cref="IDecisionService" />.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        public static async Task<bool> Check(Func<DecisionContext, DecisionContext> lambda)
        {
            return await Injector.Get<IDecisionService>().CheckAsync(lambda(Create()));
        }

        /// <summary>
        /// Sets the target property.
        /// </summary>
        /// <param name="anonymousObject">The anonymous object.</param>
        internal void SetTargetProperty(object anonymousObject)
        {
            if (!(anonymousObject is ExpandoObject))
            {
                IDictionary<string, object> expando = new ExpandoObject();

                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(anonymousObject.GetType()))
                {
                    expando.Add(property.Name, property.GetValue(anonymousObject));
                }

                this.Target = expando as ExpandoObject;
            }
            else
            {
                this.Target = anonymousObject;
            }
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

            // If this is a System.Dynamic.ExpandoObject then we need to use casting as Reflection can't find the Properties
            if (Target is ExpandoObject)
            {
                foreach (var prop in (ICollection<KeyValuePair<String, Object>>)Target)
                {
                    yield return prop.Key + "=" + prop.Value;
                }
            }

            // If the Target is another type, we can use reflection to find all the set properties
            foreach (var prop in Target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                yield return prop.Name + "=" + prop.GetValue(Target, null);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is DecisionContext)) return false;

            return (obj as DecisionContext).Id == Id;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }
    }
}