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
        /// <returns></returns>
        Dictionary<string, string> ReadConfigs();
    

        /// <summary>
        /// write all config to database
        /// </summary>
        /// <typeparam name="T">configruation value type</typeparam>
        /// <param name="configs">all config</param>
        /// <returns></returns>
        bool WriteConfigs(Dictionary<string, dynamic> configs);

        /// <summary>
        /// Deletes the configs.
        /// </summary>
        /// <returns><c>true</c>, if configs was deleted, <c>false</c> otherwise.</returns>
        /// <param name="keys">Keys.</param>
        bool DeleteConfig(params string[] keys);
    }
}
