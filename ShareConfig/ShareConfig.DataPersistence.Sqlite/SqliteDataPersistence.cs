using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using ShareConfig.Core;

namespace ShareConfig.DataPersistence.Sqlite
{
    public class SqliteDataPersistence
    {
        /// <summary>
        /// connection string
        /// </summary>
        string _connectionString;
        public SqliteDataPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// read all configs
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadConfigs()
        {          
            using (var con = new SQLiteConnection(_connectionString))
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT [KEY],[VALUE] FROM Configs";
                con.Open();
                var reader = cmd.ExecuteReader();
                var configDic = new Dictionary<string, string>();
                while (reader.Read())
                {
                    var keyString = reader.GetString(0);
                    var valueJson = reader.GetString(1);
                    configDic.Add(keyString, valueJson);

                }
                return configDic;
            }
        }
        /// <summary>
        /// write all configs
        /// </summary>
        /// <param name="configs">configs</param>
        /// <returns></returns>
        public bool WriteConfigs(Dictionary<Key, dynamic> configs)
        {
            using (var con = new SQLiteConnection(_connectionString))
            {
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    var cmd = new SQLiteCommand();
                    cmd.Connection = con;
                    cmd.Transaction = tran;
                    //创建Configs表
                    cmd.CommandText = @"IF object_id('Configs') is  null
BEGIN
CREATE TABLE [dbo].[Configs](
    [Key] [varchar](50) NOT NULL,
    [Value] [text] NULL,
CONSTRAINT [PK_Configs] PRIMARY KEY CLUSTERED 
(
    [Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
";
                    cmd.ExecuteNonQuery();
                    //delete all config data
                    cmd.CommandText = "DELETE FROM Configs";
                    cmd.ExecuteNonQuery();                  

                    //add all config data
                    foreach(var item in configs)
                    {
                        cmd.CommandText = "INSERT INTO Configs(Key,Value) Values(@Key,@Value)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SQLiteParameter("@Key", item.Key.ToString()));
                        cmd.Parameters.Add(new SQLiteParameter(parameterName: "@Value",value: Newtonsoft.Json.JsonConvert.SerializeObject(item.Value as object)));
                        cmd.ExecuteNonQuery();
                    }                  
                    tran.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    tran.Rollback();
                    throw exc;
                }

            }
        }
    }
}
