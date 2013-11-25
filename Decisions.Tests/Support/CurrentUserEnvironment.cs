using System;

namespace Decisions.Tests.Support
{
    /// <summary>
    /// A simple Environment providing information about the current user
    /// </summary>
    class CurrentUserEnvironment
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public Guid UserId { get; set; }
    }
}