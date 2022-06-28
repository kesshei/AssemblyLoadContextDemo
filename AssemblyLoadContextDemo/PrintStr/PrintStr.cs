using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterface;

namespace PrintStrLib
{
    public class PrintStr : ITask
    {
        public void Run()
        {
            int a = 0;
            while (true)
            {
                Console.WriteLine($"PrintStr:{a}");
                a++;
                Thread.Sleep(1 * 1000);
            }
        }
    }
}
