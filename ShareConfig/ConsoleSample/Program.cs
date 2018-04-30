using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ShareConfig.Core;
using ConsulShareConfig;

namespace ConsoleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }

        static void DataHandle()
        {
           
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
