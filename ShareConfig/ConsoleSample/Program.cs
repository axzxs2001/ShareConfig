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
            DataHandle();
        }

        static void DataHandle()
        {
            /*
            首先在Docker中安装redis，然后用docker run -p 56379:6379 redis 运行docekr中的redis
            */
            while (true)
            {
                Console.WriteLine("1、写入Redis  2、读取Redis  3、退出");
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
        static void Write()
        {
            var dataPersistence = DataPersistenceFactory.CreateDataPersistence<ShareConfig.DataPersistence.Redis.RedisDataPersistence>("localhost:56379");

            var key = new Key { NameSpace = "ns", Environment = "pro", Version = "1.0", Tag = "His" };
            var value = new { Name = "桂素伟", Age = 18, Sex = false };
            var dic = new Dictionary<Key, dynamic>();
            dic.Add(key, value);
            var result = dataPersistence.WriteConfigs(dic);
            Console.WriteLine(result);
        }


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
                                break;

                            case "2":
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
                                break;
                            case "3":
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
    }

    class MyEntity
    {
        public string Name { get; set; }

        public bool Sex { get; set; }
    }
}
