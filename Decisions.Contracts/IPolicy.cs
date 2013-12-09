namespace Decisions.Contracts
{
    /// <summary>
    /// Describes Securable policy. This extensibility point allows developers to write their own policies that can be used
    /// as part of the Decision process executed on <see cref="IDecisionService"/>.
    /// </summary>
    public interface IPolicy
    {
        /// <summary>
        /// Gets or sets the <see cref="IEnvironmentService"/>. The service can be used to retrieve Environments to process a <see cref="AbstractPolicy"/>.
        /// </summary>
        /// <remarks>Guaranteed to be set by Decisions if not set manually</remarks>
        IEnvironmentService Service { get; set; }

        /// <summary>
        /// Make a decisions on the Decision of this policy for the provided <see cref="DecisionContext"/>.
        /// </summary>
        /// <returns>True if the policy passes, otherwise false.</returns>
        bool Decide(DecisionContext context);
    }
}