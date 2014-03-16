using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Decisions.Contracts;

namespace Decisions.Example.Support
{
    public class ExampleEnvironmentProvider : IEnvironmentProvider
    {
        private readonly Dictionary<string, object> environments = new Dictionary<string, object>
            {
                { CurrentUserEnvironment.ALIAS, new CurrentUserEnvironment { UserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267") } }, 
                { AclEnvironment.ALIAS, new AclEnvironment { Entries = new List<Acl>{ new Acl { Allow = false } } } },
                { LongAclEnvironment.ALIAS, new LongAclEnvironment { Entries = new List<Acl>{ new Acl { Allow = true } } } },
                { SimpleCounterEnvironment.ALIAS, new SimpleCounterEnvironment(false) }
            };

        public string[] SupportedAliases { get { return environments.Keys.ToArray(); } }

        public string Component { get { return "Example"; } }

        public async Task<dynamic> GetAsync(string alias, DecisionContext context)
        {
            if (alias == LongAclEnvironment.ALIAS || alias == SimpleCounterEnvironment.ALIAS)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }

            if (alias == SimpleCounterEnvironment.ALIAS)
            {
                return new SimpleCounterEnvironment(true);
            }

            return await Task.FromResult(environments[alias]);
        }
    }
}