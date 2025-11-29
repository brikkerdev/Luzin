using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice03
{
    internal class Task3
    {
        public void Main(string[] args)
        {
            SortedList<int, string> list = new SortedList<int, string>();
            SortedDictionary<int, string> dict = new SortedDictionary<int, string>();

            var random = new Random();
            var elements = new[]
            {
                (15, "Анна"),
                (3, "Борис"),
                (7, "Виктор"),
                (1, "Дмитрий"),
                (10, "Елена")
            };

            Console.WriteLine("\nSortedList:");
            foreach (var (id, name) in elements)
            {
                list.Add(id, name);
                Console.WriteLine($"ID: {id}, Name: {name}");
            }

            Console.WriteLine("\nSortedDictionary:");
            foreach (var (id, name) in elements)
            {
                dict.Add(id, name);
                Console.WriteLine($"ID: {id}, Name: {name}");
            }

            Console.WriteLine("\nСравнение скорости");
            var stopwatch = new Stopwatch();

            list = new SortedList<int, string>();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                int id = random.Next(1, 100000);
                list.TryAdd(id, $"Name_{id}");
            }

            stopwatch.Stop();
            Console.WriteLine($"SortedList: {stopwatch.ElapsedMilliseconds} мс");

            dict = new SortedDictionary<int, string>();
            stopwatch.Restart();

            for (int i = 0; i < 10000; i++)
            {
                int id = random.Next(1, 100000);
                dict.TryAdd(id, $"Name_{id}");
            }

            stopwatch.Stop();
            Console.WriteLine($"SortedDictionary: {stopwatch.ElapsedMilliseconds} мс");
        }
    }
}
