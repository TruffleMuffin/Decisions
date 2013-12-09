using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Decisions.Contracts;
using Decisions.Providers;

namespace Decisions.Services
{
    /// <summary>
    /// An implemenation of the <see cref="IDecisionService "/> that handles execution of decisions.
    /// </summary>
    public sealed class DecisionService : IDecisionService
    {
        private readonly ConcurrentDictionary<string, ExpressionProvider> providers = new ConcurrentDictionary<string, ExpressionProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionService" /> class.
        /// </summary>
        /// <param name="configPath">The config path.</param>
        /// <param name="service">The service.</param>
        public DecisionService(string configPath, PolicyService service)
        {
            var settings = XElement.Load(Path.GetFullPath(configPath));
            foreach (var expressionSection in settings.Elements("namespace"))
            {
                var component = expressionSection.Attribute("name").Value;
                var expressionProvider = new ExpressionProvider(expressionSection.Element("decisions"), service);
                providers.AddOrUpdate(component, expressionProvider, (key, oldValue) => expressionProvider);
            }
        }

        /// <summary>
        /// Determines the result of the specified <see cref="context" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public async Task<bool> CheckAsync(DecisionContext context)
        {
            return await Task.Run(() =>
                {
                    var expression = providers[context.Namespace].Inflate(context);
                    return expression(context);
                });
        }
    }
}
