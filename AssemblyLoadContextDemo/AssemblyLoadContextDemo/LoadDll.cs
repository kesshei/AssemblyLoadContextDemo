using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskInterface;

namespace AssemblyLoadContextDemo
{
    /// <summary>
    /// dll文件的加载
    /// </summary>
    public class LoadDll
    {
        /// <summary>
        /// 任务实体
        /// </summary>
        public ITask _task;
        public Thread _thread;
        /// <summary>
        /// 核心程序集加载
        /// </summary>
        public AssemblyLoadContext _AssemblyLoadContext { get; set; }
        /// <summary>
        /// 获取程序集
        /// </summary>
        public Assembly _Assembly { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string filepath = string.Empty;
        /// <summary>
        /// 指定位置的插件库集合
        /// </summary>
        AssemblyDependencyResolver resolver { get; set; }

        public bool LoadFile(string filepath)
        {
            this.filepath = filepath;
            try
            {
                resolver = new AssemblyDependencyResolver(filepath);
                _AssemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), true);
                _AssemblyLoadContext.Resolving += _AssemblyLoadContext_Resolving;

                using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var _Assembly = _AssemblyLoadContext.LoadFromStream(fs);
                    var Modules = _Assembly.Modules;
                    foreach (var item in _Assembly.GetTypes())
                    {
                        if (item.GetInterface("ITask") != null)
                        {
                            _task = (ITask)Activator.CreateInstance(item);
                            break;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex) { Console.WriteLine($"LoadFile:{ex.Message}"); };
            return false;
        }

        private Assembly _AssemblyLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            Console.WriteLine($"加载{arg2.Name}");
            var path = resolver.ResolveAssemblyToPath(arg2);
            if (!string.IsNullOrEmpty(path))
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return _AssemblyLoadContext.LoadFromStream(fs);
                }
            }
            return null;
        }

        public bool StartTask()
        {
            bool RunState = false;
            try
            {
                if (_task != null)
                {
                    _thread = new Thread(new ThreadStart(_Run));
                    _thread.IsBackground = true;
                    _thread.Start();
                    RunState = true;
                }
            }
            catch (Exception ex) { Console.WriteLine($"StartTask:{ex.Message}"); };
            return RunState;
        }
        private void _Run()
        {
            try
            {
                _task.Run();
            }
            catch (Exception ex) { Console.WriteLine($"_Run 任务中断执行:{ex.Message}"); };
        }
        public bool UnLoad()
        {
            try
            {
                _thread?.Interrupt();
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"UnLoad:{ex.Message}");
            }
            finally
            {
                _thread = null;
            }
            _task = null;
            try
            {
                _AssemblyLoadContext?.Unload();
            }
            catch (Exception)
            { }
            finally
            {
                _AssemblyLoadContext = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return true;
        }
    }
}
