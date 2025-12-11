using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Kr04
{
    internal class Task2
    {
        public void Main(string[] args)
        {
            Box box = new Box(10f);

            Console.WriteLine("До первого collect");
            Console.WriteLine(GC.GetGeneration(box)); // 0 - объект новый
            GC.Collect();

            Console.WriteLine("После первого collect");
            Console.WriteLine(GC.GetGeneration(box)); // 1 - объект выжил после 1 очистки
            GC.Collect();

            Console.WriteLine("После второго collect");
            Console.WriteLine(GC.GetGeneration(box)); // 2 - объект выжил после >1 очистки
            GC.Collect();

            Console.WriteLine("После третьего collect");
            Console.WriteLine(GC.GetGeneration(box)); // 2 - объект выжил после >1 очистки
        }
    }
}
