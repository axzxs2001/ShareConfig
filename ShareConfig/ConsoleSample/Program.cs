using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ShareConfig.Core;
using ConsulShareConfig;
using System.Collections.Generic;

namespace ConsoleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("1、Data Handle 2、Consul Handle 3、Mixed Handle 4、Exit");
                switch(Console.ReadLine())
                {
                    case "1":
                        DataHandle();
                        break;
                    case "2":
                        ConsulHandle().GetAwaiter().GetResult();
                        break;
                    case "3":
                        MixedHandle();
                        break;
                    case  "4":
                        return;
                }
            }
     
        }

        static void MixedHandle()
        {
            
        }
        /// <summary>
        /// Datas the handle.
        /// </summary>
        static void DataHandle()
        {
            /*
            first install redis in docker，exec command "docker run -p 56379:6379 redis "
            */
            while (true)
            {
                Console.WriteLine("1、Write Redis  2、Read Redis  3、Exit");
                switch (Console.ReadLine())
                {
                    case "1":
                        Write();
                        break;
                    case "2":
                        Read();
                        break;
                    case "3":
                        return;
                }
            }
        }
        /// <summary>
        /// Read Redis
        /// </summary>
        static void Read()
        {
            var dataPersistence = DataPersistenceFactory.CreateDataPersistence<ShareConfig.DataPersistence.Redis.RedisDataPersistence>("localhost:56379");
            var result = dataPersistence.ReadConfigs();
            foreach(var item in result)
            {
                Console.WriteLine($"Key:{item.Key}");
                Console.WriteLine($"Value:{item.Value}");
            }
            
        }
        /// <summary>
        /// write redis
        /// </summary>
        static void Write()
        {
            var dataPersistence = DataPersistenceFactory.CreateDataPersistence<ShareConfig.DataPersistence.Redis.RedisDataPersistence>("localhost:56379");

            var key = new Key { NameSpace = "ns", Environment = "pro", Version = "1.0", Tag = "His" };
            var value = new { Name = "Gui Suwei", Age = 18, Sex = false };
            var dic = new Dictionary<Key, dynamic>();
            dic.Add(key, value);
            var result = dataPersistence.WriteConfigs(dic);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Consuls the handle.
        /// </summary>
        /// <returns>The handle.</returns>
        static async Task ConsulHandle()
        {
            try
            {
                var config = ConfigFactory.CreateConfig<ConsulConfig>();

                while (true)
                {
                    try
                    {
                        Console.WriteLine("1、Add Config  2、Query Config  3、Delete Config");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                await AddConfig(config);
                                break;

                            case "2":
                                await QueryConfig(config);
                                break;
                            case "3":
                                await RemoveConfig(config);
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine($"Exception:{exc.Message}");
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
        }
        /// <summary>
        /// Removes the config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="config">Config.</param>
        private static async Task RemoveConfig(IConfig config)
        {
            Console.WriteLine("Input NameSpace");
            var deleteNameSpace = Console.ReadLine();

            Console.WriteLine("Input Environment");
            var deleteEnvironment = Console.ReadLine();

            Console.WriteLine("Input Version");
            var deleteVersion = Console.ReadLine();

            Console.WriteLine("Input Tag");
            var deleteTag = Console.ReadLine();

            var deleteKey = new Key { NameSpace = deleteNameSpace, Environment = deleteEnvironment, Version = deleteVersion, Tag = deleteTag };
            var removeResult = await config.Remove(deleteKey);
            Console.WriteLine($"remove result:{removeResult}");
        }
        /// <summary>
        /// Queries the config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="config">Config.</param>
        private static async Task QueryConfig(IConfig config)
        {
            Console.WriteLine("Input NameSpace");
            var queryNameSpace = Console.ReadLine();

            Console.WriteLine("Input Environment");
            var queryEnvironment = Console.ReadLine();

            Console.WriteLine("Input Version");
            var queryVersion = Console.ReadLine();

            Console.WriteLine("Input Tag");
            var queryTag = Console.ReadLine();

            var queryKey = new Key { NameSpace = queryNameSpace, Environment = queryEnvironment, Version = queryVersion, Tag = queryTag };
            var keylist = await config.Read(queryKey);
            Console.WriteLine("================query result================");
            foreach (var item in keylist)
            {
                Console.WriteLine($"Key:{item.Key.ToString()},Value:{(item.Value as MyEntity).Name},{(item.Value as MyEntity).Sex}");
            }
            Console.WriteLine("====================================");
        }
        /// <summary>
        /// Adds the config.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="config">Config.</param>
        private static async Task AddConfig(IConfig config)
        {
            Console.WriteLine("Input NameSpace");
            var addNameSpace = Console.ReadLine();

            Console.WriteLine("Input Environment");
            var addEnvironment = Console.ReadLine();

            Console.WriteLine("Input Version");
            var addVersion = Console.ReadLine();

            Console.WriteLine("Input Tag");
            var addTag = Console.ReadLine();

            var key = new Key { NameSpace = addNameSpace, Environment = addEnvironment, Version = addVersion, Tag = addTag };
            var addResult = await config.Write(key, new MyEntity { Name = "ggg_" + key.ToString(), Sex = true });
            Console.WriteLine($"add result:{addResult}");
        }
    }
    /// <summary>
    /// Config Entity
    /// </summary>
    class MyEntity
    {
        public string Name { get; set; }

        public bool Sex { get; set; }
    }
}
