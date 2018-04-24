using System;
using System.Threading.Tasks;

namespace ConsoleSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConsulShareConfig.ConsulConfig("http://localhost:8500");
            while (true)
            {
                Console.WriteLine("1、add config  2、query config  3、delete config");
                switch (Console.ReadLine())
                {
                    case "1":
                        var addResult = await config.Write<ABC>(new ShareConfig.Core.Key { Environment = "pro", NameSpace = "abc", Version = "1.0", Tag = "abc" }, new ABC { Name = "ggg", Sex = true });
                        Console.WriteLine($"add result:{addResult}");
                        break;
                    case "2":
                        var list = await config.Read<ABC>(new ShareConfig.Core.Key { Environment = "pro", NameSpace = "abc", Version = "1.0", Tag = "abc" });
                        Console.WriteLine("================数据================");
                        foreach (var item in list)
                        {
                            Console.WriteLine($"{item.Name},{item.Sex}");
                        }
                        Console.WriteLine("====================================");
                        break;
                    case "3":
                        var removeResult = await config.Remove(new ShareConfig.Core.Key { Environment = "pro", NameSpace = "abc", Version = "1.0", Tag = "abc" });
                        Console.WriteLine($"remove result:{removeResult}");
                        break;
                }

            }
        }
    }

    class ABC
    {
        public string Name { get; set; }

        public bool Sex { get; set; }
    }
}
