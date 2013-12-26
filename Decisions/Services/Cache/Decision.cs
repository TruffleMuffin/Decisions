using System;

namespace Decisions.Services.Cache
{
    /// <summary>
    /// Describes the result of a Decision
    /// </summary>
    [Serializable]
    public class Decision
    {
        /// <summary>
        /// Gets or sets the Decision identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the result of the Decision
        /// </summary>
        public bool Result { get; set; }
    }
}