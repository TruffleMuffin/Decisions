using System.Collections.Generic;

namespace Decisions.Example.Support
{
    /// <summary>
    /// A simple Environment providing information about a random Access Control List
    /// </summary>
    class AclEnvironment
    {
        public const string ALIAS = "Acl";

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public List<Acl> Entries { get; set; }
    }
}