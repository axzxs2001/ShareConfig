using System;
using System.Collections.Generic;

namespace ShareConfig.Core
{
    /// <summary>
    /// 配置接口
    /// </summary>
    public interface IConfig
    { 
        /// <summary>
        /// 写入单个配置项
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <returns></returns>
        bool Write<T>(Key key, T value) where T:class,new();
        /// <summary>
        /// 写入批量配置项
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="configs">批量配置</param>
        /// <returns></returns>
        bool Writes<T>(Dictionary<Key, T> configs) where T : class, new();
        /// <summary>
        /// 批量读取配置项
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <returns></returns>
        List<T> Read<T>(Key key) where T : class, new();

    }
}
