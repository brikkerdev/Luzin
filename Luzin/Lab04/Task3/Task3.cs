using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab04
{
    class Task3
    {
        private readonly Queue<int> _buffer = new Queue<int>();
        private readonly object _bufferLock = new object();
        private readonly Semaphore _itemsAvailable = new Semaphore(0, int.MaxValue);
        private readonly Semaphore _spacesAvailable;
        private readonly int _bufferSize;
        private bool _running = true;
        private int _itemCounter = 0;

        public Task3(int bufferSize = 5)
        {
            _bufferSize = bufferSize;
            _spacesAvailable = new Semaphore(bufferSize, bufferSize);
        }

        public void Run(int producerCount = 2, int consumerCount = 2)
        {
            Console.WriteLine($"Запуск Producer-Consumer. Размер буфера: {_bufferSize}");
            Console.WriteLine($"Производители: {producerCount}, Потребители: {consumerCount}");

            List<Thread> producers = new List<Thread>();
            List<Thread> consumers = new List<Thread>();

            for (int i = 0; i < producerCount; i++)
            {
                int producerId = i + 1;
                Thread producer = new Thread(() => ProducerWork(producerId));
                producers.Add(producer);
                producer.Start();
            }

            for (int i = 0; i < consumerCount; i++)
            {
                int consumerId = i + 1;
                Thread consumer = new Thread(() => ConsumerWork(consumerId));
                consumers.Add(consumer);
                consumer.Start();
            }

            Thread.Sleep(15000);

            Console.WriteLine("\nЗавершение работы...");
            _running = false;

            for (int i = 0; i < producerCount; i++)
            {
                _spacesAvailable.Release();
            }

            for (int i = 0; i < consumerCount; i++)
            {
                _itemsAvailable.Release();
            }

            foreach (var producer in producers)
            {
                producer.Join(1000);
            }

            foreach (var consumer in consumers)
            {
                consumer.Join(1000);
            }

            Console.WriteLine("Работа завершена!");
        }

        private void ProducerWork(int producerId)
        {
            Random random = new Random();

            while (_running)
            {
                Thread.Sleep(random.Next(300, 800));

                if (!_running) break;

                if (_spacesAvailable.WaitOne(100))
                {
                    int item;
                    lock (_bufferLock)
                    {
                        item = ++_itemCounter;
                        _buffer.Enqueue(item);
                        Console.WriteLine($"Производитель {producerId} создал товар {item}. Буфер: {_buffer.Count}/{_bufferSize}");
                    }

                    _itemsAvailable.Release();
                }
                else
                {
                    Console.WriteLine($"Производитель {producerId} ждет свободного места...");
                }
            }

            Console.WriteLine($"Производитель {producerId} завершил работу");
        }

        private void ConsumerWork(int consumerId)
        {
            Random random = new Random();

            while (_running)
            {
                Thread.Sleep(random.Next(400, 900));

                if (!_running && _itemsAvailable.WaitOne(0) == false) break;

                if (_itemsAvailable.WaitOne(100))
                {
                    int item;
                    lock (_bufferLock)
                    {
                        item = _buffer.Dequeue();
                        Console.WriteLine($"Потребитель {consumerId} забрал товар {item}. Буфер: {_buffer.Count}/{_bufferSize}");
                    }

                    _spacesAvailable.Release();

                    Thread.Sleep(random.Next(100, 300));
                }
                else if (_running)
                {
                    Console.WriteLine($"Потребитель {consumerId} ждет товары...");
                }
            }

            Console.WriteLine($"Потребитель {consumerId} завершил работу");
        }
    }
}
