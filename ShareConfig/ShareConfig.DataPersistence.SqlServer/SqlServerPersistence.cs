﻿using System;
using System.Collections.Generic;
using ShareConfig.Core;
using System.Data.SqlClient;
using System.Data;

namespace ShareConfig.DataPersistence.SqlServer
{
    /// <summary>
    /// ms sql server data persistence
    /// </summary>
    public class SqlServerPersistence : Core.IDataPersistence
    {
        /// <summary>
        /// connection string
        /// </summary>
        string _connectionString;
        public SqlServerPersistence(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// read all configs
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadConfigs()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
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
        public bool WriteConfigs(Dictionary<string, dynamic> configs)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    var cmd = new SqlCommand();
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
                    //get datatable organization
                    cmd.CommandText = "SELECT [Key],[Value] FROM Configs";
                    var reader = cmd.ExecuteReader();
                    var table = new DataTable();
                    table.Load(reader);
                    foreach (var item in configs)
                    {
                        var row = table.NewRow();
                        row["key"] = item.Key;
                        row["value"] = Newtonsoft.Json.JsonConvert.SerializeObject(item.Value as object);
                        table.Rows.Add(row);
                    }

					//add all config data
                    var sqlbulk = new SqlBulkCopy(con, SqlBulkCopyOptions.UseInternalTransaction, tran);
                    sqlbulk.NotifyAfter = table.Rows.Count;
                    sqlbulk.DestinationTableName = "Configs";               
                    sqlbulk.ColumnMappings.Add(0, "Key");
                    sqlbulk.ColumnMappings.Add(1, "Value");                
                    sqlbulk.WriteToServer(table);
                    sqlbulk.Close();
                    tran.Commit();
                    return true;
                }
                catch(Exception exc)
                {
                    tran.Rollback();
                    throw exc;
                }
            }
        }
        /// <summary>
        /// Deletes the config.
        /// </summary>
        /// <returns><c>true</c>, if config was deleted, <c>false</c> otherwise.</returns>
        /// <param name="keys">Keys.</param>
        public bool DeleteConfig(params string[] keys)
        {
            var parmeterNames = new List<string>();
            var parmeterValues = new List<SqlParameter>();
            for(int i=0;i<keys.Length;i++)
            {
                parmeterNames.Add($"@p{i}");
                parmeterValues.Add(new SqlParameter { Value=keys[i],ParameterName=$"@p{i}"});
            }
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"delete FROM Configs where key in({string.Join(",",parmeterNames.ToArray())})";
                cmd.Parameters.AddRange(parmeterValues.ToArray());
                con.Open();
                var reader = cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}
