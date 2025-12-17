using System;
using System.Threading;

namespace Lab04
{
    class Task1
    {
        public void RunWithDeadlock()
        {
            Console.WriteLine("=== РЕАЛИЗАЦИЯ С DEADLOCK ===");

            PhilosopherDeadlock[] philosophers = new PhilosopherDeadlock[5];
            object[] forks = new object[5];

            for (int i = 0; i < 5; i++)
            {
                forks[i] = new object();
            }

            for (int i = 0; i < 5; i++)
            {
                philosophers[i] = new PhilosopherDeadlock(i, forks[i], forks[(i + 1) % 5]);
            }

            Thread[] threads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                int id = i;
                threads[i] = new Thread(() => philosophers[id].Run());
                threads[i].Start();
            }

            Thread.Sleep(10000);

            foreach (var philosopher in philosophers)
            {
                philosopher.Stop();
            }

            foreach (var thread in threads)
            {
                thread.Join(1000);
            }

            Console.WriteLine("Deadlock реализация завершена");
        }

        public void RunWithoutDeadlock()
        {
            Console.WriteLine("\n=== РЕАЛИЗАЦИЯ БЕЗ DEADLOCK ===");

            PhilosopherNoDeadlock[] philosophers = new PhilosopherNoDeadlock[5];
            Semaphore[] forks = new Semaphore[5];

            for (int i = 0; i < 5; i++)
            {
                forks[i] = new Semaphore(1, 1);
            }

            for (int i = 0; i < 5; i++)
            {
                philosophers[i] = new PhilosopherNoDeadlock(i, forks[i], forks[(i + 1) % 5]);
            }

            Thread[] threads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                int id = i;
                threads[i] = new Thread(() => philosophers[id].Run());
                threads[i].Start();
            }

            Thread.Sleep(10000);

            foreach (var philosopher in philosophers)
            {
                philosopher.Stop();
            }

            foreach (var thread in threads)
            {
                thread.Join(1000);
            }

            Console.WriteLine("Без deadlock реализация завершена");
        }
    }
}