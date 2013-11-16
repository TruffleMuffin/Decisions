using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Securables.Application.Providers;
using Securables.Contracts;

namespace Securables.Application.Services
{
    /// <summary>
    /// An implemenation of the <see cref="ISecurablesService "/> that handles execution of decisions.
    /// </summary>
    public sealed class SecurablesService : ISecurablesService
    {
        private readonly ConcurrentDictionary<string, ExpressionProvider> providers = new ConcurrentDictionary<string, ExpressionProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurablesService" /> class.
        /// </summary>
        /// <param name="configPath">The config path.</param>
        /// <param name="service">The service.</param>
        public SecurablesService(string configPath, PolicyService service)
        {
            var settings = XElement.Load(Path.GetFullPath(configPath));
            foreach (var expressionSection in settings.Element("components").Elements("component"))
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
                    var expression = providers[context.Component].Inflate(context);
                    return expression(context);
                });
        }
    }
}
