using System;
using System.Collections.Generic;
using System.Text;

namespace ShareConfig.Core
{
    public class ConfigFactory
    {
        public IConfig CreateConfig<T>() where T:IConfig
        {
            return Activator.CreateInstance(typeof(T)) as IConfig;
        }
        public IConfig CreateConfig<T>(params object[] parmeters) where T : IConfig
        {
            return Activator.CreateInstance(typeof(T), parmeters) as IConfig;
        }
    }
}
