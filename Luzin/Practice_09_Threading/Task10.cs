using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice_09_Threading
{
    internal class Task10
    {
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(3);
        Mutex mutex = new Mutex();
        string value = "Данные";

        public void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread reader = new Thread(Read);
                reader.Name = "#" + i;
                reader.Start();
            }
            for (int i = 0; i < 3; i++)
            {
                Thread writer = new Thread(Write);
                writer.Name = "#" + i;
                writer.Start();
            }
        }

        public void Read()
        {
            semaphoreSlim.Wait();

            Console.WriteLine($"Читатель {Thread.CurrentThread.Name} прочитал {value}");

            Thread.Sleep(1000);

            semaphoreSlim.Release();
        }

        public void Write()
        {
            mutex.WaitOne();

            value = Thread.CurrentThread.Name + " данные";
            Console.WriteLine($"Писатель {Thread.CurrentThread.Name}: {value}");

            Thread.Sleep(1000);

            mutex.ReleaseMutex();
        }
    }
}
