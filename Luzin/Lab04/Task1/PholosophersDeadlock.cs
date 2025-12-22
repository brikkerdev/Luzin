using System;
using System.Threading;

namespace Lab04
{
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
}