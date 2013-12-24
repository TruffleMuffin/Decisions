using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Decisions.Contracts.Providers
{
    /// <summary>
    /// A default implementation of the <see cref="IEnvironmentProvider"/> which will automatically provide supported aliases.
    /// </summary>
    public abstract class DefaultEnvironmentProvider : IEnvironmentProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEnvironmentProvider"/> class.
        /// </summary>
        /// <param name="supportedType">A <see cref="IReflect"/> of a object with the supported Environment Aliases on.</param>
        protected DefaultEnvironmentProvider(IReflect supportedType)
        {
            this.SupportedAliases = supportedType
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsLiteral)
                .Select(a => a.GetValue(supportedType) as string)
                .ToArray();
        }

        /// <summary>
        /// Gets the aliases that this instance supports retrieval of environments for.
        /// </summary>
        public string[] SupportedAliases { get; private set; }

        /// <summary>
        /// Gets the environment with the specified alias using the context provided.
        /// </summary>
        /// <param name="alias">The globally unique alias used to represent a specific environment.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// An environment specified to the provided information.
        /// </returns>
        public abstract Task<dynamic> GetAsync(string alias, DecisionContext context);
    }
}