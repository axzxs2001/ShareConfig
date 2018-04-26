using System;
using System.Collections.Generic;
using ShareConfig.Core;
using System.Data.SqlClient;

namespace ShareConfig.DataPersistence.SqlServer
{
    public class SqlServerPersistence : Core.IDataPersistence
    {
        string _connectionString;
        public SqlServerPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Dictionary<Key, dynamic> ReadConfigs() 
        {
            using(var conn=new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "";

                var reader=cmd.ExecuteReader();
                var configDic = new Dictionary<Key, dynamic>();
                while(reader.Read())
                {
                    var keyString= reader.GetString(0);
                    var valueJson = reader.GetString(1);
                    var key = ToKey(keyString);
                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(valueJson);
                    configDic.Add(key, value);

                }
                return configDic;
            }
        }
        Key ToKey(string keyString)
        {
           var keyArr= keyString.Split('/');
            var key = new Key();
            key.NameSpace = keyArr[0];
            key.Environment = keyArr[1];
            key.Version = keyArr[2];
            key.Tag = keyArr[3];
            return key;
        }
        /// <summary>
        /// if table is not exist ,creates the table.
        /// </summary>
        void CreateTable()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"";

                cmd.ExecuteNonQuery();
            }
        }

        public bool WriteConfigs(Dictionary<Key, dynamic> configs)
        {
            throw new NotImplementedException();
        }
    }
}
