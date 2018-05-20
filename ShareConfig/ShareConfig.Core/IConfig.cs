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
        Task<Dictionary<Key, dynamic>> Read(Key key);

        /// <summary>
        /// remove configuration
        /// </summary>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        Task<bool> Remove(Key key);

        /// <summary>
        /// Write a single configuration item.
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="key">configration key</param>
        /// <param name="value">configration value</param>
        /// <returns></returns>
        Task<bool> Write<T>(string key, T value) where T : class, new();
        /// <summary>
        /// batch read the configuration items.
        /// </summary>
        /// <typeparam name="T">configration value type</typeparam>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        Task<Dictionary<string, dynamic>> Read(string key);
        /// <summary>
        /// remove configuration
        /// </summary>
        /// <param name="key">configration key</param>
        /// <returns></returns>
        Task<bool> Remove(string key);
    }
}
