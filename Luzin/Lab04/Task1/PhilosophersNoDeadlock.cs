using System;
using System.Threading;

namespace Lab04
{
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