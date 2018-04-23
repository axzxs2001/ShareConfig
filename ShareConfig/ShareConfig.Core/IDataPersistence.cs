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
        /// read all configs from database
        /// </summary>
        /// <returns></returns>
        Dictionary<Key, dynamic> ReadConfigs();

        /// <summary>
        /// write all configs to database
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        bool WriteConfigs(Dictionary<Key, dynamic> configs);
    }
}
