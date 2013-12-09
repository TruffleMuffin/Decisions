using Decisions.Contracts;

namespace Decisions.Example.Support
{
    /// <summary>
    /// An implementation of <see cref="AbstractPolicy"/> which always Permits.
    /// </summary>
    public class AlphaPolicy : AbstractPolicy
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaPolicy"/> class.
        /// </summary>
        public AlphaPolicy()
        {
            this.Id = 1;
        }

        /// <summary>
        /// Make a decisions on the Decision of this policy for the provided <see cref="DecisionContext" />.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool Decide(DecisionContext context)
        {
            return context.Target.id == Id;
        }
    }
}
