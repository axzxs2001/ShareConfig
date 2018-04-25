using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<bool> Write<T>(Key key, T value) where T : class, new();
        /// <summary>
        /// batch read the configuration items.
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        Task<Dictionary<Key,T>> Read<T>(Key key) where T : class, new();

        /// <summary>
        /// remove configuration
        /// </summary>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        Task<bool> Remove(Key key);

    }
}
