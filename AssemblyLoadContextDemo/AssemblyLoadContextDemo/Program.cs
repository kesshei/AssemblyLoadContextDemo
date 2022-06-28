using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AssemblyLoadContextDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "AssemblyLoadContext Dll热插拔 测试 by 蓝总创精英团队";
            var list = new List<LoadDll>();
            Console.WriteLine("开始加载DLL");
            list.Add(Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"DLL", "PrintDateLib.dll")));
            list.Add(Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DLL", "PrintStrLib.dll")));
            foreach (var item in list)
            {
                item.StartTask();
            }
            Console.WriteLine("开启了任务!");
            SpinWait.SpinUntil(() => false, 5 * 1000);
            foreach (var item in list)
            {
                var s = item.UnLoad();
                SpinWait.SpinUntil(() => false, 2 * 1000);
                Console.WriteLine($"任务卸载：{s}");
            }
            Console.WriteLine("热插拔插件任务 测试完毕");
            Console.ReadLine();
        }
        public static LoadDll Load(string filePath)
        {
            var load = new LoadDll();
            load.LoadFile(filePath);
            return load;
        }
    }
}
