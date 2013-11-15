namespace Securables.Contracts
{
    /// <summary>
    /// Describes a decision context, containing all relevant information for getting a <see cref="Decision"/> result from the <see cref="ISecurablesService"/>.
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
        public string Id { get { return string.Format(ID_FORMAT, Component, UserId, Role, EntityId); } }

        /// <summary>
        /// Gets or sets the name of the component which the decision should be made within.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets or sets the user id the decision should be made for.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the role the decision should be about.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the entity id which the decision should be on.
        /// </summary>
        public string EntityId { get; set; }
    }
}