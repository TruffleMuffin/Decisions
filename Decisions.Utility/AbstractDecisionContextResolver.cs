using System.Web.Http.Controllers;
using System.Web.Mvc;
using Decisions.Contracts;
using Decisions.Utility.Filters;

namespace Decisions.Utility
{
    /// <summary>
    /// Describes the basic DecisionContextParser. This extensibility point allows for bespoke implementations to create from a <see cref="HttpActionContext"/>
    /// a <see cref="DecisionContext"/> that is used on a <see cref="DecisionsCheckAttribute"/> execution.
    /// </summary>
    public abstract class AbstractDecisionContextResolver
    {
        /// <summary>
        /// Parses the specified action context to resolve the appropriate <see cref="DecisionContext"/> for the request.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>A <see cref="DecisionContext"/></returns>
        public abstract DecisionContext Resolve(HttpActionContext actionContext);

        /// <summary>
        /// Parses the specified action context to resolve the appropriate <see cref="DecisionContext"/> for the request.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>A <see cref="DecisionContext"/></returns>
        public abstract DecisionContext Resolve(ActionExecutingContext actionContext);
    }
}