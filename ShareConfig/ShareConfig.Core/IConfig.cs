using System;
using System.Collections.Generic;

namespace ShareConfig.Core
{
    /// <summary>
    /// configration interface
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Write a single configuration item.
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="key">configration key</param>
        /// <param name="value">configration value</param>
        /// <returns></returns>
        bool Write<T>(Key key, T value) where T:class,new();
        /// <summary>
        /// write the batch configuration item
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="configs">batch configuration</param>
        /// <returns></returns>
        bool Writes<T>(Dictionary<Key, T> configs) where T : class, new();
        /// <summary>
        /// batch read the configuration items.
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        List<T> Read<T>(Key key) where T : class, new();

    }
}
