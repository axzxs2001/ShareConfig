using Newtonsoft.Json;
using ShareConfig.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsulShareConfig
{
    /// <summary>
    /// use consul as a distributed configuration center.
    /// </summary>
    public class ConsulConfig : IConfig
    {
        string _urlPrefix = "v1";
        HttpClient _client;
        IDataPersistence _dataPersistence;

        #region 构造
        /// <summary>
        /// ctor
        /// </summary>
        public ConsulConfig()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://localhost:8500");
        }
        /// <summary>
        /// ctor
        /// </summary>
        public ConsulConfig(IDataPersistence dataPersistence = null)
        {
            _dataPersistence = dataPersistence;
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://localhost:8500");
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">Base Address，Default value is "http://localhost:8500"</param>
        public ConsulConfig(string baseAddress = "http://localhost:8500", IDataPersistence dataPersistence = null)
        {
            _dataPersistence = dataPersistence;
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"{baseAddress}");
        }
        #endregion

        //TODO 同步配置与数据实现
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region Key类型 
        /// <summary>
        /// Read Config
        /// </summary>
        /// <typeparam name="T">configuration value type</typeparam>
        /// <param name="key">configuration key</param>
        /// <returns></returns>
        public async Task<Dictionary<Key, dynamic>> Read(Key key = null)
        {
            var keyString = key == null ? new Key().ToString() : key.ToString();
            var queryKey = HandleQueryKey(keyString);
            var backList = new Dictionary<Key, dynamic>();
            var list = new List<ReadKeyResult>();
            list.AddRange(await ReadKey(queryKey));
            foreach (var item in list)
            {
                var reg = new Regex($"^{keyString}$");
                if (reg.IsMatch(item.Key))
                {
                    var newKey = new Key();
                    var keyArr = item.Key.Split('/');
                    if (keyArr.Length < 4)
                    {
                        throw new Exception("");
                    }
                    newKey.NameSpace = keyArr[0];
                    newKey.Environment = keyArr[1];
                    newKey.Version = keyArr[2];
                    newKey.Tag = keyArr[3];
                    backList.Add(newKey, JsonConvert.DeserializeObject(item.DecodeValue));
                }
            }
            return backList;
        }

        /// <summary>
        /// write config
        /// </summary>
        /// <typeparam name="T">configuration value type</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="value">configuration value</param>
        /// <returns></returns>
        public async Task<bool> Write<T>(Key key, T value) where T : class, new()
        {
            //data persistence write all
            if (_dataPersistence != null)
            {
                var configs = await Read(key: new Key());
                configs.Add(key, value);
                var newConfigs = new Dictionary<string, dynamic>();
                foreach (var item in configs)
                {
                    newConfigs.Add(item.Key.ToString(), item.Value);
                }
                _dataPersistence.WriteConfigs(newConfigs);
            }
            //consul write one
            var result = await CreateUpdateKey(new CreateUpdateKeyParmeter { Key = key.ToString(), DC = null }, value);
            return result.result && result.createUpdateResult;
        }
        /// <summary>
        /// remove config
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <returns></returns>
        public async Task<bool> Remove(Key key)
        {
            return await RemoveKey(key.ToString());
        }
        #endregion
        #region string类型
        /// <summary>
        /// Read Config
        /// </summary>
        /// <typeparam name="T">configuration value type</typeparam>
        /// <param name="key">configuration key</param>
        /// <returns></returns>
        public async Task<Dictionary<string, dynamic>> Read(string key = null)
        {
            var keyString = key == null ? new Key().ToString() : key.ToString();
            var queryKey = HandleQueryKey(keyString);
            var backList = new Dictionary<string, dynamic>();
            var list = new List<ReadKeyResult>();
            list.AddRange(await ReadKey(queryKey));
            foreach (var item in list)
            {
                var reg = new Regex($"^{keyString}$");
                if (reg.IsMatch(item.Key))
                {
                    backList.Add(item.Key, JsonConvert.DeserializeObject(item.DecodeValue));
                }
            }
            return backList;
        }
        /// <summary>
        /// remove config
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <returns></returns>
        public async Task<bool> Remove(string key)
        {
            return await RemoveKey(key);
        }

        /// <summary>
        /// write config
        /// </summary>
        /// <typeparam name="T">configuration value type</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="value">configuration value</param>
        /// <returns></returns>
        public async Task<bool> Write<T>(string key, T value) where T : class, new()
        {
            //data persistence write all
            if (_dataPersistence != null)
            {
                var configs = await Read(key: "");
                configs.Add(key, value);
                _dataPersistence.WriteConfigs(configs);
            }
            //consul write one
            var result = await CreateUpdateKey(new CreateUpdateKeyParmeter { Key = key.ToString(), DC = null }, value);
            return result.result && result.createUpdateResult;
        }
        #endregion
        #region 私有成员
        /// <summary>
        /// Handle key string to consul query key
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        string HandleQueryKey(string keyString)
        {
            var keyStrArr = keyString.Replace(Key.RegxString, "").Split('/');
            var queryKey = "";
            if (string.IsNullOrEmpty(keyStrArr[0]))
            {
                queryKey = "/";
            }
            else
            {
                if (string.IsNullOrEmpty(keyStrArr[1]))
                {
                    queryKey = $"{keyStrArr[0]}/";
                }
                else
                {
                    if (string.IsNullOrEmpty(keyStrArr[2]))
                    {
                        queryKey = $"{keyStrArr[0]}/{keyStrArr[1]}/";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(keyStrArr[3]))
                        {
                            queryKey = $"{keyStrArr[0]}/{keyStrArr[1]}/{keyStrArr[2]}/";
                        }
                        else
                        {
                            queryKey = $"{keyStrArr[0]}/{keyStrArr[1]}/{keyStrArr[2]}/{keyStrArr[3]}";
                        }
                    }
                }
            }
            return queryKey;
        }
        /// <summary>
        /// Load configs from data persistence
        /// </summary>
        /// <returns></returns>
        private async Task<bool> LoadConfig()
        {
            var dataConfigs = _dataPersistence?.ReadConfigs();
            if (dataConfigs == null)
            {
                return false;
            }
            else
            {
                //read consul configs
                var consulConfigs = await Read(key: "");
                //consul configs is null
                if (consulConfigs.Count == 0)
                {
                    foreach (var config in dataConfigs)
                    {
                        var result = await CreateUpdateKey(new CreateUpdateKeyParmeter { Key = config.Key, DC = null }, config.Value);
                        if (result.result == false || result.createUpdateResult == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        async Task<List<ReadKeyResult>> ReadKey(string keyUrl)
        {
            //read  key/value in directory
            if (keyUrl.EndsWith("/", true, System.Globalization.CultureInfo.CurrentCulture))
            {
                var list = await ReadKey<string>(new ReadKeyParmeter { DC = null, Key = keyUrl, Recurse = true, Raw = true, Keys = true, Separator = "/" });

                var backList = new List<ReadKeyResult>();
                foreach (var item in list)
                {
                    var value = await ReadKey(item);
                    backList.AddRange(value);
                }
                return backList;
            }
            else//read key/value
            {
                var arr = await ReadKey<ReadKeyResult>(new ReadKeyParmeter { DC = null, Key = keyUrl });
                var backList = new List<ReadKeyResult>();
                backList.AddRange(arr);
                return backList;
            }
        }

        private async Task<bool> RemoveKey(string key)
        {
            //data persistence delete key
            if (_dataPersistence != null)
            {
                _dataPersistence.DeleteConfig(key);
            }
            //consul remove            
            var keyStrArr = key.Split(new string[] { Key.RegxString }, StringSplitOptions.RemoveEmptyEntries);
            var backResult = true;
            if (keyStrArr.Length > 0)
            {
                var list = new List<ReadKeyResult>();
                list.AddRange(await ReadKey(keyStrArr[0]));
                foreach (var item in list)
                {
                    var reg = new Regex($"^{key}$");
                    if (reg.IsMatch(item.Key))
                    {
                        var result = await DeleteKey(new DeleteKeyParmeter { Key = item.Key });
                        backResult = backResult && result.result && result.deleteResult;
                    }
                }
            }
            return backResult;
        }

        /// <summary>
        /// This endpoint returns the specified key. If no key exists at the given path, a 404 is returned instead of a 200 response.For multi-key reads, please consider using transaction.
        /// </summary> 
        /// <typeparam name="T">back value type</typeparam>
        /// <param name="readKeyParmeter">read key parmeter</param>
        /// <returns></returns>
        async Task<T[]> ReadKey<T>(ReadKeyParmeter readKeyParmeter)
        {
            var url = $"/kv/{readKeyParmeter.Key}";
            var parString = GetUrlParmeter(readKeyParmeter);
            if (!string.IsNullOrEmpty(parString))
            {
                url += $"?{parString.ToLower()}";
            }
            var response = await _client.GetAsync($"/{_urlPrefix}/{url}");
            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var entity = JsonConvert.DeserializeObject<T[]>(json);
                    return entity;
                }
                catch (JsonReaderException)
                {
                    throw new ApplicationException($"back content is error formatter:{json}");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get in parmeter with url
        /// </summary>
        /// <param name="inEntity"></param>
        /// <returns></returns>
        string GetUrlParmeter(object inEntity)
        {
            var parmeterString = new StringBuilder();
            foreach (var pro in inEntity.GetType().GetProperties())
            {
                //get property value
                var entityValue = pro.GetValue(inEntity);
                if (!pro.PropertyType.IsValueType)
                {
                    if (entityValue == null)
                    {
                        continue;
                    }
                }
                else
                {
                    var defultValue = Activator.CreateInstance(pro.PropertyType);
                    if (entityValue.ToString() == defultValue.ToString())
                    {
                        continue;
                    }
                }
                parmeterString.Append($"{pro.Name}={pro.GetValue(inEntity)}&");
            }
            return parmeterString.ToString().Trim('&');
        }
        /// <summary>
        /// Even though the return type is application/json, the value is either true or false, indicating whether the create/update succeeded.The table below shows this endpoint's support for blocking queries, consistency modes, and required ACLs.
        /// </summary>
        /// <param name="firEventParmeter">Create Update Key Parmeter</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        async Task<(bool result, bool createUpdateResult)> CreateUpdateKey(CreateUpdateKeyParmeter createUpdateKeyParmeter, object value)
        {
            var url = $"/kv/{createUpdateKeyParmeter.Key}";
            var parString = GetUrlParmeter(createUpdateKeyParmeter);
            if (!string.IsNullOrEmpty(parString))
            {
                url += $"?{parString}";
            }
            var json = JsonConvert.SerializeObject(value);
            var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            var content = new StreamContent(stream);
            var response = await _client.PutAsync($"/{_urlPrefix}/{url}", content);
            var backJson = await response.Content.ReadAsStringAsync();
            var backResult = (result: response.StatusCode == System.Net.HttpStatusCode.OK, backJson: backJson);

            if (!backResult.result)
            {
                throw new Exception(backResult.backJson);
            }
            var backEntity = JsonConvert.DeserializeObject<bool>(backResult.backJson);
            return (backResult.result, backEntity);
        }
        /// <summary>
        /// This endpoint deletes a single key or all keys sharing a prefix.
        /// </summary>
        /// <param name="deleteKeyParmeter">Delete Key Parmeter</param>
        /// <returns></returns>
        async Task<(bool result, bool deleteResult)> DeleteKey(DeleteKeyParmeter deleteKeyParmeter)
        {
            var url = $"/kv/{deleteKeyParmeter.Key}";
            var parString = GetUrlParmeter(deleteKeyParmeter);
            if (!string.IsNullOrEmpty(parString))
            {
                url += $"?{parString}";
            }
            var response = await _client.DeleteAsync($"/{_urlPrefix}/{url}");
            var backJson = await response.Content.ReadAsStringAsync();
            var backResult = (result: response.StatusCode == System.Net.HttpStatusCode.OK, backJson: backJson);
            if (!backResult.result)
            {
                throw new Exception(backResult.backJson);
            }
            var backEntity = JsonConvert.DeserializeObject<bool>(backResult.backJson);
            return (backResult.result, backEntity);
        }

        #endregion
     

    }
}
