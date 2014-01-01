using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TruffleCache;

namespace Decisions.Tests
{
    class TestCacheStore : ICacheStore
    {
        private ConcurrentDictionary<string, CacheItem> store = new ConcurrentDictionary<string, CacheItem>();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Set(string key, object value, TimeSpan expiresIn)
        {
            var cacheItem = new CacheItem { Value = value, CachedUntil = DateTime.Now + expiresIn };
            store.AddOrUpdate(key, a => cacheItem, (o, k) => cacheItem);
            return true;
        }

        public IDictionary<string, object> Get(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            if (!store.ContainsKey(key)) return (T)((object)null);
            var result = store[key];
            if (result.CachedUntil > DateTime.Now) return (T)(result.Value);
            return (T)((object)null);
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        class CacheItem
        {
            public object Value { get; set; }
            public DateTime CachedUntil { get; set; }
        }

        public Task SetAsync(string key, object value, TimeSpan expiresIn)
        {
            return Task.FromResult(Set(key, value, expiresIn));
        }

        public Task<IDictionary<string, object>> GetAsync(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        public Task<bool> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}