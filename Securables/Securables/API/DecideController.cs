using Securables.Contracts;
using System.Threading.Tasks;
using System.Web.Http;

namespace Securables.API
{
    /// <summary>
    /// Describes the Decide endpoint for the Securables API.
    /// </summary>
    public class DecideController : ApiController
    {
        /// <summary>
        /// Handles a Decision request.
        /// 
        /// HTTP Verb: GET
        /// </summary>
        /// <returns>A <see cref="Decision"/> indicating the result of the query</returns>
        public async Task<Decision> Get()
        {
            return await Task.FromResult(Decision.Deny);
        }
    }
}