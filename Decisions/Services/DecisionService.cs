using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Decisions.Contracts;
using Decisions.Exceptions;
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

            // Validate the XML Formatting to prevent bad errors being thrown which have poor debugging information due to their nature
            if (settings.Elements("namespace").Any() == false) throw new ConfigurationMalformedException("No namespace elements could be found.");

            foreach (var expressionSection in settings.Elements("namespace"))
            {
                if (expressionSection.HasAttributes == false || expressionSection.Attributes().Any(a => a.Name == "name") == false)
                {
                    throw new ConfigurationMalformedException("One of the namespace elements does not have a name attribute.");
                }

                var component = expressionSection.Attribute("name").Value;
                var expressionProvider = new ExpressionProvider(expressionSection.Element("decisions"), service);
                providers.AddOrUpdate(component, expressionProvider, (key, oldValue) => expressionProvider);
            }
        }

        /// <summary>
        /// Determines the result of the specified context.
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
