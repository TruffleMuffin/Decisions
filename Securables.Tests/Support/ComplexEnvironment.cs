using System.Collections.Generic;

namespace Securables.Tests.Support
{
    /// <summary>
    /// A simple Environment providing information which is complex to retrieve. Should be used for emulating long running or CPU bound EnvironmentProvider.
    /// </summary>
    class ComplexEnvironment
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public List<Acl> Entries { get; set; }
    }
}