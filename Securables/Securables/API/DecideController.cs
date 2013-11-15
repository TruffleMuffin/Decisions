using System;
using System.Threading.Tasks;
using System.Web.Http;
using Securables.Contracts;

namespace Securables.API
{
    /// <summary>
    /// Describes the Decide endpoint for the Securables API.
    /// </summary>
    public class DecideController : ApiController
    {
        /// <summary>
        /// Handles a Decision request.
        /// HTTP Verb: GET
        /// </summary>
        /// <param name="componentName">Name of the component the decision should be run for.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="roleName">Name of the role the user claims to have on the entity within the <see cref="componentName"/>.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>
        /// A <see cref="Decision" /> indicating the result of the query.
        /// </returns>
        public async Task<Decision> Get(string componentName, Guid userId, string roleName, string entityId)
        {
            return await Task.FromResult(Decision.Deny);
        }
    }
}