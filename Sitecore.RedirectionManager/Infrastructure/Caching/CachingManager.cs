using System;
using System.Runtime.Caching;

namespace Sitecore.RedirectionManager.Infrastructure.Caching
{
    public sealed class CachingManager
    {
        private static readonly Lazy<CachingManager> lazyInstance = new Lazy<CachingManager>(() => new CachingManager());
        private MemoryCache memoryCache;
        private object syncObject = new object();

        public static CachingManager Instance
        {
            get { return lazyInstance.Value; }
        }

        private CachingManager()
        {
            memoryCache = new MemoryCache("RedirectionCache");
        }

        public void AddItem(string key, object value, DateTimeOffset timeOffset)
        {
            lock (syncObject)
            {
                memoryCache.Add(key, value, timeOffset);
            }
        }

        public void AddItem(string key, object value)
        {
            lock (syncObject)
            {
                memoryCache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        public void RemoveItem(string key)
        {
            lock (syncObject)
            {
                memoryCache.Remove(key);
            }
        }

        public object GetItem(string key, bool hasRemove)
        {
            lock (syncObject)
            {
                var res = memoryCache[key];

                if (res != null && hasRemove == true)
                {
                    memoryCache.Remove(key);
                }
                
                return res;
            }
        }
    }
}
