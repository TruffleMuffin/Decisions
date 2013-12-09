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
        /// Determines the results of the specified <see cref="contexts" />.
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
                        tasks.Add(context.Id, service.CheckAsync(context));
                    }

                    Task.WaitAll(tasks.Values.ToArray());

                    return tasks.ToDictionary(a => a.Key, a => a.Value.Result);
                });
        }
    }
}
