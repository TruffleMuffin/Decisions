using System.Collections.Generic;

namespace Securables.Tests.Support
{
    /// <summary>
    /// A simple Environment providing information about a random Access Control List
    /// </summary>
    class AclEnvironment
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public List<Acl> Entries { get; set; }
    }
}