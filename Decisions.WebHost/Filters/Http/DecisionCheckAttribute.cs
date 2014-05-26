using System.Net;
using System.Web.Http;
using Decisions.Contracts;
using Decisions.Contracts.IoC;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Decisions.WebHost.Filters.Http
{
    /// <summary>
    /// An action filter that can be used to make calls to <see cref="IDecisionService" /> in a non-blocking (for the action execution) manner.
    /// </summary>
    /// <remarks>
    /// Will block on OnActionExecuted until the results is returned.
    /// For best performance, you should attempt to place this on the action itself so it is last in the filter stack to be executed.
    /// </remarks>
    public sealed class DecisionCheckAttribute : ActionFilterAttribute
    {
        private readonly IDecisionService service;
        private const string CHECK_TASK_KEY = "DecisionCheckAttribute.CHECK_TASK";

        /// <summary>
        /// Gets the Namespace to use for the Check.
        /// </summary>
        public string Using { get; set; }

        /// <summary>
        /// Gets or sets the Role to use for the Check.
        /// </summary>
        public string Has { get; set; }

        /// <summary>
        /// Gets or sets the Target Details to use for the Check.
        /// </summary>
        public string On { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DecisionCheckAttribute"/> can lazily resolve the result of the Decision.
        /// </summary>
        public bool Lazy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionCheckAttribute" /> class.
        /// </summary>
        public DecisionCheckAttribute() :
            this(Injector.Get<IDecisionService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionCheckAttribute" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public DecisionCheckAttribute(IDecisionService service)
        {
            this.service = service;
            this.Lazy = true;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Validate();

            var task = Task.Run(() => service.CheckAsync(Resolve(actionContext.ActionArguments)));

            if (!Lazy)
            {
                Executed(task);
            }
            else
            {
                actionContext.Request.Properties.Add(CHECK_TASK_KEY, task);
            }
        }


        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Properties.ContainsKey(CHECK_TASK_KEY))
            {
                var task = (Task<bool>)actionExecutedContext.Request.Properties[CHECK_TASK_KEY];
                Executed(task);
            }
        }

        /// <summary>
        /// Throws a <see cref="HttpException"/> with 403 Forbidden if the decision result was false.
        /// </summary>
        /// <param name="checkTask">The <see cref="Task{T}"/> to check.</param>
        private static void Executed(Task<bool> checkTask)
        {
            if (checkTask.IsFaulted || checkTask.Result == false)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        /// <summary>
        /// Resolves the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        private DecisionContext Resolve(IDictionary<string, object> args)
        {
            IDictionary<string, object> target = new ExpandoObject();
            foreach (var pair in ParseOn(args))
            {
                target.Add(pair);
            }

            return DecisionContext.Create(a => a.Using(Using).Has(Has).On(target));
        }

        /// <summary>
        /// Parses the Target.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>An IEnumerable of key value pairs from the args matching the on</returns>
        /// <exception cref="System.ArgumentException">Decisions.Utility.Filters.DecisionCheckAttribute has no Target</exception>
        private IEnumerable<KeyValuePair<string, object>> ParseOn(IDictionary<string, object> args)
        {
            if (string.IsNullOrWhiteSpace(On)) yield break;

            var elements = On.Split('&');
            foreach (var element in elements.Where(a => a.Contains("=")).Select(a => a.Split('=')))
            {
                var key = element.First();
                var valueIdentifier = CleanIdentifier(element.Last());

                if (args.ContainsKey(valueIdentifier) == false)
                {
                    yield return new KeyValuePair<string, object>(key, valueIdentifier);
                }
                else
                {
                    yield return new KeyValuePair<string, object>(key, args[valueIdentifier]);
                }
            }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Decisions.Utility.Filters.DecisionCheckAttribute - Using has not been set
        /// or
        /// Decisions.Utility.Filters.DecisionCheckAttribute - Has has not been set
        /// </exception>
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Using))
            {
                throw new ArgumentException("Using has not been set");
            }
            if (string.IsNullOrWhiteSpace(Has))
            {
                throw new ArgumentException("Has has not been set");
            }
        }

        /// <summary>
        /// Cleans the identifier of non important text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string with {}</returns>
        private static string CleanIdentifier(string value)
        {
            return value.Replace("{", string.Empty).Replace("}", string.Empty);
        }
    }
}
