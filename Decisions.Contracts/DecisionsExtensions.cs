using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decisions.Contracts
{
    /// <summary>
    /// A set of extensions to the <see cref="IDecisionService"/> to make common tasks a little simpler to execute.
    /// </summary>
    public static class DecisionsExtensions
    {
        /// <summary>
        /// Determines the result of the specified context.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public static bool Check(this IDecisionService service, DecisionContext context)
        {
            return Task.Run(() => service.CheckAsync(context)).Result;
        }

        /// <summary>
        /// Determines the results of the specified contexts.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="contexts">The contexts.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public static IDictionary<string, bool> Check(this IDecisionService service, IEnumerable<DecisionContext> contexts)
        {
            return Task.Run(() => service.CheckAsync(contexts)).Result;
        }

        /// <summary>
        /// Determines the results of the specified contexts.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="contexts">The contexts.</param>
        /// <returns>
        /// A set of Decision indicating the results of the query.
        /// </returns>
        public static async Task<IDictionary<string, bool>> CheckAsync(this IDecisionService service, IEnumerable<DecisionContext> contexts)
        {
            return await Task.Run(() =>
                {
                    var tasks = new Dictionary<string, Task<bool>>();
                    foreach (var context in contexts)
                    {
                        // Create the task before adding so the service applies defaults to the context.
                        var result = service.CheckAsync(context);
                        tasks.Add(context.Id, result);
                    }

                    Task.WaitAll(tasks.Values.ToArray());

                    return tasks.ToDictionary(a => a.Key, a => a.Value.Result);
                });
        }
    }
}
