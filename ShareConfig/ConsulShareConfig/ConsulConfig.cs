using ShareConfig.Core;
using System;
using System.Collections.Generic;

namespace ConsulShareConfig
{
    /// <summary>
    /// use consul as a distributed configuration center.
    /// </summary>
    public class ConsulConfig : IConfig
    {
        Uri _uri;
        public ConsulConfig(Uri uri)
        {           
            _uri = uri;
        }
        public ConsulConfig(string uri)
        {
            _uri = new Uri(uri);
        }
        public List<T> Read<T>(Key key) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public bool Write<T>(Key key, T value) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public bool Writes<T>(Dictionary<Key, T> configs) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
