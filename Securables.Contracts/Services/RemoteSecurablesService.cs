using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Securables.Contracts.Services
{
    /// <summary>
    /// A implementation of the <see cref="ISecurablesService"/> which will use <see cref="HttpClient"/> to call a remote endpoint.
    /// </summary>
    public sealed class RemoteSecurablesService : ISecurablesService
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSecurablesService"/> class.
        /// </summary>
        /// <param name="endpointUrl">The endpoint URL.</param>
        public RemoteSecurablesService(string endpointUrl)
        {
            client = new HttpClient { BaseAddress = new Uri(endpointUrl) };
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
            return bool.Parse(await client.GetStringAsync("Api/Decide/" + context.Id));
        }
    }
}
