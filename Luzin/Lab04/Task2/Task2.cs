using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab04
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    class Task2
    {
        private readonly Semaphore _barberSemaphore = new Semaphore(0, 1);
        private readonly Semaphore _customerSemaphore = new Semaphore(0, int.MaxValue);
        private readonly object _queueLock = new object();
        private readonly Queue<int> _waitingCustomers = new Queue<int>();
        private readonly int _maxQueueSize;
        private volatile bool _shopOpen = true;
        private int _nextCustomerId = 1;
        private bool _barberAwakened = false;

        public Task2(int maxQueueSize = 5)
        {
            _maxQueueSize = maxQueueSize;
        }

        public void Run()
        {
            Console.WriteLine("Парикмахерская открывается!");

            Thread barberThread = new Thread(BarberWork);
            Thread customerGeneratorThread = new Thread(GenerateCustomers);

            barberThread.Start();
            customerGeneratorThread.Start();

            Thread.Sleep(20000);

            _shopOpen = false;

            barberThread.Join(3000);
            customerGeneratorThread.Join(3000);

            Console.WriteLine("Парикмахерская закрылась!");
        }

        private void BarberWork()
        {
            Console.WriteLine("Парикмахер готов к работе");

            while (_shopOpen || _waitingCustomers.Count > 0)
            {
                Console.WriteLine("Парикмахер проверяет клиентов...");

                if (_waitingCustomers.Count == 0 && _shopOpen)
                {
                    Console.WriteLine("Клиентов нет, парикмахер засыпает");
                    _barberAwakened = false;
                    _barberSemaphore.WaitOne(1000);
                }
                else if (_customerSemaphore.WaitOne(0))
                {
                    int customerId;
                    lock (_queueLock)
                    {
                        customerId = _waitingCustomers.Dequeue();
                    }

                    Console.WriteLine($"Парикмахер начинает стричь клиента {customerId}");
                    Thread.Sleep(800);
                    Console.WriteLine($"Парикмахер закончил стричь клиента {customerId}");
                }
                else if (!_shopOpen)
                {
                    break;
                }
            }

            Console.WriteLine("Парикмахер уходит домой");
        }

        private void GenerateCustomers()
        {
            Random random = new Random();

            while (_shopOpen)
            {
                Thread.Sleep(random.Next(300, 800));

                lock (_queueLock)
                {
                    if (_waitingCustomers.Count < _maxQueueSize)
                    {
                        int customerId = _nextCustomerId++;
                        _waitingCustomers.Enqueue(customerId);
                        Console.WriteLine($"Клиент {customerId} пришел в парикмахерскую. В очереди: {_waitingCustomers.Count}");

                        _customerSemaphore.Release();

                        if (!_barberAwakened)
                        {
                            _barberAwakened = true;
                            _barberSemaphore.Release();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Клиент {_nextCustomerId} пришел, но очередь полна! Уходит.");
                        _nextCustomerId++;
                    }
                }
            }
        }
    }
}
