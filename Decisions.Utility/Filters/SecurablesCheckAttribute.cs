using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Decisions.Contracts;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Decisions.Utility.Filters
{
    /// <summary>
    /// An action filter that can be used to make calls to <see cref="IDecisionsService" /> in a non-blocking (for the action execution) manner.
    /// </summary>
    /// <remarks>
    /// Will block on OnActionExecuted until the results is returned.
    /// For best performance, you should attempt to place this on the action itself so it is last in the filter stack to be executed.
    /// </remarks>
    public sealed class DecisionsCheckAttribute : ActionFilterAttribute, System.Web.Mvc.IActionFilter
    {
        private readonly IDecisionsService service;
        private Task<bool> checkTask;

        /// <summary>
        /// Gets or sets the <see cref="AbstractDecisionContextResolver"/>.
        /// </summary>
        private Type Resolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DecisionsCheckAttribute"/> can lazily resolve the result of the Decision.
        /// </summary>
        private bool Lazy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionsCheckAttribute" /> class.
        /// </summary>
        /// <param name="resolver">The resolver. An instance of <see cref="AbstractDecisionContextResolver" /></param>
        /// <param name="lazy">if set to <c>true</c> can lazily resolve the result of the Decision, otherwise will resolve before Action executed.</param>
        public DecisionsCheckAttribute(Type resolver, bool lazy = true) : this(resolver, lazy, Injector.Get<IDecisionsService>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionsCheckAttribute" /> class.
        /// </summary>
        /// <param name="resolver">The resolver. An instance of <see cref="AbstractDecisionContextResolver" /></param>
        /// <param name="lazy">if set to <c>true</c> can lazily resolve the result of the Decision, otherwise will resolve before Action executed.</param>
        /// <param name="service">The service.</param>
        public DecisionsCheckAttribute(Type resolver, bool lazy, IDecisionsService service)
        {
            this.service = service;
            this.Resolver = resolver;
            this.Lazy = lazy;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = (Injector.Get(Resolver) as AbstractDecisionContextResolver).Resolve(actionContext);
            checkTask = service.CheckAsync(context);
            if (!Lazy) Executed();
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (Lazy)
            {
                Executed();
            }
        }

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = (Injector.Get(Resolver) as AbstractDecisionContextResolver).Resolve(filterContext);
            checkTask = service.CheckAsync(context);
            if (!Lazy) Executed();
        }

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Lazy)
            {
                Executed();
            }
        }

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        private void Executed()
        {
            if (checkTask != null)
            {
                checkTask.Wait();
                if (checkTask.IsFaulted || checkTask.Result == false)
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}
