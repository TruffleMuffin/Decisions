namespace Decisions.Contracts
{
    /// <summary>
    /// Describes a Defaults provider for the <see cref="IDecisionService"/>
    /// </summary>
    public interface IDecisionDefaultsProvider
    {
        /// <summary>
        /// Gets the default source id.
        /// </summary>
        string SourceId { get; }
    }
}