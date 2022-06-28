using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskInterface;

namespace PrintDateLib
{
    public class PrintDate : ITask
    {
        public void Run()
        {
            while (true)
            {
                Console.WriteLine($"PrintDate:{DateTime.Now}");
                Thread.Sleep(1 * 1000);
            }
        }
    }
}
