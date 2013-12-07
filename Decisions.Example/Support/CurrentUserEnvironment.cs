using System;

namespace Decisions.Example.Support
{
    /// <summary>
    /// A simple Environment providing information about the current user
    /// </summary>
    class CurrentUserEnvironment
    {
        public const string ALIAS = "CurrentUser";

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public Guid UserId { get; set; }
    }
}