using System;
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
                    { "D", new AlphaPolicy() },
                    { "E", new CappaPolicy{ CurrentUserEnvironmentKey = "CurrentUser", MatchUserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267") } },
                    { "F", new CappaPolicy{ CurrentUserEnvironmentKey = "CurrentUser", MatchUserId = new Guid("1E9A7C0C-FC86-4516-BA42-F7232E65A12C") } },
                    { "G", new DeltaPolicy{ AclEnvironmentKey = "Acl" } },
                    { "H", new DeltaPolicy{ AclEnvironmentKey = "LongRunning" } }
                };
        }
    }
}