using System;
using System.Collections.Generic;
using System.Text;

namespace ShareConfig.Core
{
    /// <summary>
    /// data persistence
    /// </summary>
    public interface IDataPersistence
    {
       
        /// <summary>
        /// read all config from database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Dictionary<Key, T> ReadConfigs<T>() where T:class,new();
    

        /// <summary>
        /// write all config to database
        /// </summary>
        /// <typeparam name="T">configruation value type</typeparam>
        /// <param name="configs">all config</param>
        /// <returns></returns>
        bool WriteConfigs<T>(Dictionary<Key, T> configs) where T : class, new();
    }
}
