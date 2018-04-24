using Newtonsoft.Json;
using ShareConfig.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">Base Address，Default value is "http://localhost:8500"</param>
        public ConsulConfig(string baseAddress = "http://localhost:8500")
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"{baseAddress}");
        }
        /// <summary>
        /// Read Config
        /// </summary>
        /// <typeparam name="T">configuration value type</typeparam>
        /// <param name="key">configuration key</param>
        /// <returns></returns>
        public async Task<List<T>> Read<T>(Key key) where T : class, new()
        {
            var list = await ReadKey<ReadKeyResult>(new ReadKeyParmeter { DC = null, Key = key.ToString() });
            var backList = new List<T>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    backList.Add(JsonConvert.DeserializeObject<T>(item.DecodeValue));
                }
            }
            return backList;
        }

        public async Task<List<string>> ReadKeyList(Key key)
        {
            var list = await ReadKey<string>(new ReadKeyParmeter { DC = null, Key = key.ToString(), Recurse = true, Raw = true, Keys = true, Separator = "/" });
            var backList = new List<string>();
            if (list != null)
            {
                backList.AddRange(list);
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
            var result = await DeleteKey(new DeleteKeyParmeter { Key = key.ToString() });
            return result.result && result.deleteResult;
        }

        /// <summary>
        /// This endpoint returns the specified key. If no key exists at the given path, a 404 is returned instead of a 200 response.For multi-key reads, please consider using transaction.
        /// </summary>
        /// <param name="readKeyParmeter">Read Key Parmeter</param>
        /// <returns></returns>
        async Task<W[]> ReadKey<W>(ReadKeyParmeter readKeyParmeter)
        {
            var url = $"/kv/{readKeyParmeter.Key}";
            var parString = GetUrlParmeter<ReadKeyParmeter>(readKeyParmeter);
            if (!string.IsNullOrEmpty(parString))
            {
                url += $"?{parString}";
            }
            var response = await _client.GetAsync($"/{_urlPrefix}/{url.ToLower()}");
            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var entity = JsonConvert.DeserializeObject<W[]>(json);
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
       /// <typeparam name="W"></typeparam>
       /// <param name="inEntity"></param>
       /// <returns></returns>
        string GetUrlParmeter<T>(T inEntity) where T : class, new()
        {
            var parmeterString = new StringBuilder();
            foreach (var pro in inEntity.GetType().GetProperties())
            {
                //get property value
                var entityValue = pro.GetValue(inEntity);              
                if (!pro.PropertyType.IsValueType&&entityValue == null)
                {
                    continue;                   
                }
                else
                {
                    if (entityValue.ToString() == Activator.CreateInstance(pro.PropertyType).ToString())
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
            var parString = GetUrlParmeter<CreateUpdateKeyParmeter>(createUpdateKeyParmeter);
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
            var parString = GetUrlParmeter<DeleteKeyParmeter>(deleteKeyParmeter);
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
    }
}
