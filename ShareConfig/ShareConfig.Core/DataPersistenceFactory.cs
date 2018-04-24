using System;
using System.Collections.Generic;
using System.Text;

namespace ShareConfig.Core
{
    public class DataPersistenceFactory
    {
        public static IDataPersistence CreateDataPersistence<T>() where T : IConfig
        {
            return Activator.CreateInstance(typeof(T)) as IDataPersistence;
        }
        public static IDataPersistence CreateDataPersistence<T>(params object[] parmeters) where T : IConfig
        {
            return Activator.CreateInstance(typeof(T), parmeters) as IDataPersistence;
        }
    }
}
