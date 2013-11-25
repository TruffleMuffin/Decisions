using System.Threading.Tasks;
using System.Web.Http;
using Decisions.Contracts;

namespace Decisions.API
{
    /// <summary>
    /// Describes the Decide endpoint for the Decisions API.
    /// </summary>
    public class DecideController : ApiController
    {
        private readonly IDecisionsService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecideController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public DecideController(IDecisionsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handles a Decision request.
        /// HTTP Verb: GET
        /// </summary>
        /// <param name="componentName">Name of the component the decision should be run for.</param>
        /// <param name="sourceId">The identifier of the source of the request.</param>
        /// <param name="roleName">Name of the role the user claims to have on the entity within the <see cref="componentName"/>.</param>
        /// <param name="targetId">The identifier of the target of the request.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        public async Task<bool> Get(string componentName, string sourceId, string roleName, string targetId)
        {
            return await service.CheckAsync(Resolve(componentName, sourceId, roleName, targetId));
        }

        /// <summary>
        /// Resolves the <see cref="DecisionContext"/> based on the provided parameters.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="sourceId">The source id.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns>A <see cref="DecisionContext"/></returns>
        private static DecisionContext Resolve(string componentName, string sourceId, string roleName, string targetId)
        {
            return new DecisionContext { Component = componentName, SourceId = sourceId, Role = roleName, TargetId = targetId };
        }
    }
}