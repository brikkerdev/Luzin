using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice_Threading
{
    internal class Task9
    {
        Semaphore sem = new Semaphore(4, 4);
        Random random = new Random();

        public void Park()
        {
            sem.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} Приехал на парковку");

            Thread.Sleep(random.Next(3) * 1000);

            Console.WriteLine($"{Thread.CurrentThread.Name} Уезжает с парковки");

            sem.Release();
        }

        public void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(Park);
                thread.Name = $"Автомобиль {i}";
                thread.Start();
            }
        }
    }
}
