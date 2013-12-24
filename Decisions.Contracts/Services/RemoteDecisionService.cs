using System;
using System.Net.Http;
using System.Threading.Tasks;
using Decisions.Contracts.Providers;

namespace Decisions.Contracts.Services
{
    /// <summary>
    /// A implementation of the <see cref="IDecisionService"/> which will use <see cref="HttpClient"/> to call a remote endpoint.
    /// </summary>
    public sealed class RemoteDecisionService : IDecisionService
    {
        private readonly HttpClient client;
        private readonly IDecisionDefaultsProvider defaultProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDecisionService"/> class.
        /// </summary>
        /// <param name="endpointUrl">The endpoint URL.</param>
        public RemoteDecisionService(string endpointUrl) : this(endpointUrl, new DefaultsProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDecisionService"/> class.
        /// </summary>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="defaultProvider">The default provider.</param>
        public RemoteDecisionService(string endpointUrl, IDecisionDefaultsProvider defaultProvider)
        {
            client = new HttpClient { BaseAddress = new Uri(endpointUrl) };
            this.defaultProvider = defaultProvider;
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
            // Apply Defaults
            context.SourceId = context.SourceId ?? defaultProvider.SourceId;

            // Wrap the Remote call in a Try/Catch to provide good debug information if an error occurs.
            try
            {
                var result = await client.GetStringAsync("Api/Decide/" + context.Id);
                return bool.Parse(result);
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("A GET for {0} returned error: {1}", "Api/Decide/" + context.Id, e.Message));
            }
        }
    }
}
