using System;
using System.Threading;

namespace Lab4
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

    class PhilosopherDeadlock
    {
        private readonly int _id;
        private readonly object _leftFork;
        private readonly object _rightFork;
        private readonly Random _random = new Random();
        private bool _running = true;

        public PhilosopherDeadlock(int id, object leftFork, object rightFork)
        {
            _id = id;
            _leftFork = leftFork;
            _rightFork = rightFork;
        }

        public void Run()
        {
            while (_running)
            {
                Think();
                Eat();
            }
            Console.WriteLine($"Философ {_id} завершил работу");
        }

        private void Think()
        {
            Console.WriteLine($"Философ {_id} думает");
            Thread.Sleep(_random.Next(300, 700));
        }

        private void Eat()
        {
            Console.WriteLine($"Философ {_id} хочет есть");

            lock (_leftFork)
            {
                Console.WriteLine($"Философ {_id} взял левую вилку");
                Thread.Sleep(100);

                lock (_rightFork)
                {
                    Console.WriteLine($"Философ {_id} взял правую вилку и начал есть");
                    Thread.Sleep(_random.Next(400, 800));
                    Console.WriteLine($"Философ {_id} закончил есть и положил вилки");
                }
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }

    class PhilosopherNoDeadlock
    {
        private readonly int _id;
        private readonly Semaphore _leftFork;
        private readonly Semaphore _rightFork;
        private readonly Random _random = new Random();
        private bool _running = true;

        public PhilosopherNoDeadlock(int id, Semaphore leftFork, Semaphore rightFork)
        {
            _id = id;
            _leftFork = leftFork;
            _rightFork = rightFork;
        }

        public void Run()
        {
            while (_running)
            {
                Think();
                Eat();
            }
            Console.WriteLine($"Философ {_id} завершил работу");
        }

        private void Think()
        {
            Console.WriteLine($"Философ {_id} думает");
            Thread.Sleep(_random.Next(300, 700));
        }

        private void Eat()
        {
            Console.WriteLine($"Философ {_id} хочет есть");

            bool hasTwoForks = false;

            if (_id % 2 == 0)
            {
                if (_leftFork.WaitOne(100))
                {
                    Console.WriteLine($"Философ {_id} взял левую вилку");

                    if (_rightFork.WaitOne(100))
                    {
                        Console.WriteLine($"Философ {_id} взял правую вилку");
                        hasTwoForks = true;
                    }
                    else
                    {
                        _leftFork.Release();
                        Console.WriteLine($"Философ {_id} не смог взять правую вилку, отпустил левую");
                    }
                }
            }
            else
            {
                if (_rightFork.WaitOne(100))
                {
                    Console.WriteLine($"Философ {_id} взял правую вилку");

                    if (_leftFork.WaitOne(100))
                    {
                        Console.WriteLine($"Философ {_id} взял левую вилку");
                        hasTwoForks = true;
                    }
                    else
                    {
                        _rightFork.Release();
                        Console.WriteLine($"Философ {_id} не смог взять левую вилку, отпустил правую");
                    }
                }
            }

            if (hasTwoForks)
            {
                Console.WriteLine($"Философ {_id} начал есть");
                Thread.Sleep(_random.Next(400, 800));
                Console.WriteLine($"Философ {_id} закончил есть");

                _leftFork.Release();
                _rightFork.Release();
                Console.WriteLine($"Философ {_id} положил обе вилки");
            }
            else
            {
                Thread.Sleep(_random.Next(100, 300));
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
