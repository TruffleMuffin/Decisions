using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Decisions.Contracts;
using Decisions.Utility;
using Decisions.Utility.Filters;

namespace Decisions.Example.Support
{
    public class ValuesController : ApiController
    {
        [DecisionsCheck(typeof(ValuesResolver))]
        public string Get(int id)
        {
            return "value";
        }

        [DecisionsCheck(typeof(ValuesResolver), false)]
        public string Get()
        {
            throw new ApplicationException("Should never be executed");
        }
    }

    class ValuesResolver : AbstractDecisionContextResolver
    {
        public override DecisionContext Resolve(HttpActionContext actionContext)
        {
            return Resolve(actionContext.ActionArguments);
        }

        public override DecisionContext Resolve(ActionExecutingContext actionContext)
        {
            return Resolve(actionContext.ActionParameters);
        }

        private DecisionContext Resolve(IDictionary<string, object> values)
        {
            if (values.ContainsKey("id") && (int)values["id"] == 1)
            {
                return DecisionContext.Create().Using("Example").As("trufflemuffin").Has("A").On(new { id = 1 });
            }

            return DecisionContext.Create().Using("Example").As("trufflemuffin").Has("B").On(new { id = 1 });
            
        }
    }
}
