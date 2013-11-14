namespace Securables.Contracts
{
    /// <summary>
    /// The result of any given decision that Securables handles
    /// </summary>
    public enum Decision
    {
        /// <summary>
        /// The result of a decision is that it should be Permitted.
        /// </summary>
        Permit,

        /// <summary>
        /// The result of a decision is that it should be Denied.
        /// </summary>
        Deny
    }
}
