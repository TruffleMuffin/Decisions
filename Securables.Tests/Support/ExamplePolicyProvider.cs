using System.Collections.Generic;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    class ExamplePolicyProvider : IPolicyProvider
    {
        public string Component { get { return "Example"; } }

        public IDictionary<string, AbstractPolicy> GetPolicies()
        {
            return new Dictionary<string, AbstractPolicy>
                {
                    { "A", new AlphaPolicy() },
                    { "B", new BetaPolicy() },
                    { "C", new AlphaPolicy() },
                    { "D", new AlphaPolicy() }
                };
        }
    }
}