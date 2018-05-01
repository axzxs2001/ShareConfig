using System;
using System.Collections.Generic;
using ShareConfig.Core;
using StackExchange.Redis;

namespace ShareConfig.DataPersistence.Redis
{
    public class RedisDataPersistence : Core.IDataPersistence
    {
        /// <summary>
        /// connection string
        /// </summary>
        string _connectionString;
        public RedisDataPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// read all configs
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadConfigs()
        {
            var redis = ConnectionMultiplexer.Connect(_connectionString);
            var server= redis.GetServer(_connectionString);
            var keys= server.Keys();
            var dataBase = redis.GetDatabase();
			var configDic = new Dictionary<string, string>();
            foreach(var key in keys)
            {
                configDic.Add(key, dataBase.StringGet(key));    
            }
            return configDic;

        }
        /// <summary>
        /// write all configs
        /// </summary>
        /// <param name="configs">configs</param>
        /// <returns></returns>
        public bool WriteConfigs(Dictionary<Key, dynamic> configs)
        {

            return false;
        }
    }
}
