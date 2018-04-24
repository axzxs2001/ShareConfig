using System;
using System.Collections.Generic;
using ShareConfig.Core;

namespace ShareConfig.DataPersistence.SqlServer
{
    public class SqlServerPersistence : Core.IDataPersistence
    {
        public Dictionary<Key, T> ReadConfigs<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public bool WriteConfigs<T>(Dictionary<Key, T> configs) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
