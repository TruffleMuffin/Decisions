using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    class ExampleEnvironmentProvider : IEnvironmentProvider
    {
        private readonly Dictionary<string, object> environments = new Dictionary<string, object>
            {
                { "CurrentUser", new CurrentUserEnvironment { UserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267") } }, 
                { "Acl", new AclEnvironment { Entries = new List<Acl>{ new Acl { Allow = true } } } },
                { "LongRunning", new ComplexEnvironment { Entries = new List<Acl>{ new Acl { Allow = true } } } }
            };

        public CacheOptions Cache { get { return new CacheOptions { Cacheable = true, Period = TimeSpan.FromSeconds(10) }; } }

        public string Component { get { return "Example"; } }

        public async Task<dynamic> GetAsync(string key, DecisionContext context)
        {
            if (key == "LongRunning")
            {
                return await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t => Task.FromResult(environments[key]));
            }

            return await Task.FromResult(environments[key]);
        }
    }
}