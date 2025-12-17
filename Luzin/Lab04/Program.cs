using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА 4 ===");

            // Задача 1: Обедающие философы
            Console.WriteLine("\n1. Обедающие философы:");
            var task1 = new Task1();
            task1.RunWithDeadlock();      // Демонстрация deadlock
            task1.RunWithoutDeadlock();   // Корректная работа

            // Задача 2: Спящий парикмахер
            Console.WriteLine("\n\n2. Спящий парикмахер:");
            var task2 = new Task2(5);
            task2.Run();

            // Задача 3: Производитель-Потребитель
            Console.WriteLine("\n\n3. Производитель-Потребитель:");
            var task3 = new Task3(5);
            task3.Run(producerCount: 2, consumerCount: 2);

            Console.WriteLine("\nВсе задачи завершены!");
            Console.ReadKey();
        }
    }
}
