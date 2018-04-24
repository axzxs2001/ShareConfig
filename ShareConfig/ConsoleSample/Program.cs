using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace ConsoleSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //var reg = new Regex("^abc/([\\w.]*)/([\\w*.]*)/a$");
            //Console.WriteLine(reg.IsMatch("abc/pro/1.0/a"));

            //return;
            var config = new ConsulShareConfig.ConsulConfig("http://localhost:8500");
            while (true)
            {
                Console.WriteLine("1、add config  2、query config  3、query config key  4、delete config");
                switch (Console.ReadLine())
                {
                    case "1":

                        Console.WriteLine("请输入Environment");
                        var addEnvironment = Console.ReadLine();

                        Console.WriteLine("请输入Version");
                        var addVersion = Console.ReadLine();

                        Console.WriteLine("请输入Tag");
                        var addtab = Console.ReadLine();

                        var key = new ShareConfig.Core.Key { NameSpace = "abc", Environment = addEnvironment, Version = addVersion, Tag = addtab };
                        var addResult = await config.Write<ABC>(key, new ABC { Name = "ggg_" + key.ToString(), Sex = true });
                        Console.WriteLine($"add result:{addResult}");
                        break;

                    case "2":
                        Console.WriteLine("请输入Tag");
                        var writetab = Console.ReadLine();
                        var list = await config.Read<ABC>(new ShareConfig.Core.Key { Environment = "pro", NameSpace = "abc", Version = "1.0", Tag = writetab });
                        Console.WriteLine("================数据================");
                        foreach (var item in list)
                        {
                            Console.WriteLine($"{item.Name},{item.Sex}");
                        }
                        Console.WriteLine("====================================");
                        break;
                    case "3":
                        var keylist = await config.Read<ABC>(new ShareConfig.Core.Key { NameSpace = "abc", Version = "1.0",Tag="aaa" });
                        Console.WriteLine("================数据================");
                        foreach (var item in keylist)
                        {
                            Console.WriteLine($"{item.Name},{item.Sex}");
                        }
                        Console.WriteLine("====================================");
                        break;
                    case "4":
                        var removeResult = await config.Remove(new ShareConfig.Core.Key { NameSpace = "abc",Version="1.0"});
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
