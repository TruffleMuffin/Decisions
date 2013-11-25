﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Decisions.Contracts;
using Decisions.Utility;
using Decisions.Utility.Filters;

namespace Decisions.Tests.Support
{
    public class ValuesController : ApiController
    {
        [DecisionsCheck(typeof(ValuesResolver))]
        public string Get(int id)
        {
            return "value";
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
            if ((int)values["id"] == 1)
            {
                return new DecisionContext { Component = "Example", Role = "A", TargetId = "1", SourceId = "gareth" };
            }

            return new DecisionContext { Component = "Example", Role = "B", TargetId = "1", SourceId = "gareth" };
            
        }
    }
}