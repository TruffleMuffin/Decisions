using System;
using System.Web.Http;
using Decisions.WebHost.Filters.Http;

namespace Decisions.Example.Support
{
    public class ValuesController : ApiController
    {
        [DecisionCheck(Using = "Example", Has = "A", On = "id={id}")]
        public string Get(int id)
        {
            return "value";
        }

        [DecisionCheck(Lazy = false, Using = "Example", Has = "Y")]
        public string Get()
        {
            throw new ApplicationException("Should never be executed");
        }
    }
}
