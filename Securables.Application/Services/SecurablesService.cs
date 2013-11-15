﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Securables.Application.Providers;
using Securables.Contracts;

namespace Securables.Application.Services
{
    /// <summary>
    /// An implemenation of the <see cref="ISecurablesService "/> that handles execution of decisions.
    /// </summary>
    internal class SecurablesService : ISecurablesService
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

        /// <summary>
        /// Determines the results of the specified <see cref="contexts" />.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        /// <returns>
        /// A set of Decision indicating the results of the query.
        /// </returns>
        public async Task<IDictionary<string, bool>> CheckAsync(IEnumerable<DecisionContext> contexts)
        {
            return await Task.Run(() =>
                {
                    var tasks = new Dictionary<string, Task<bool>>();
                    foreach (var context in contexts)
                    {
                        tasks.Add(context.Id, CheckAsync(context));
                    }

                    Task.WaitAll(tasks.Values.ToArray());

                    return tasks.ToDictionary(a => a.Key, a => a.Value.Result);
                });
        }
    }
}