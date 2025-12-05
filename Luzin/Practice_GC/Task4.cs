using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice_GC
{
    class LogWriter : IDisposable
    {
        FileStream file;

        public LogWriter(string name)
        {
            file = File.Create(name);
            Console.WriteLine($"Создан файл {name}");
        }

        public void Dispose()
        {
            file.Dispose();
            Console.WriteLine("Dispose вызван");
            GC.SuppressFinalize(this);
        }

        ~LogWriter()
        {
            Console.WriteLine("Финализатор вызван");
            file.Dispose();
        }
    }

    internal class Task4
    {
        public void PrintGeneration(List<LogWriter> writers)
        {
            Console.WriteLine($"Текущие поколения {writers.Count} log writers");
            foreach (LogWriter writer in writers)
            {
                Console.Write($"{GC.GetGeneration(writer)},");
            }
            Console.Write('\n');
        }

        public void Main(string[] args)
        {
            int n = 10;
            List<LogWriter> list = new List<LogWriter>();

            string path = "C:\\Sharp\\text.txt";

            for (int i = 0; i < n; i++)
            {
                LogWriter logWriter = new LogWriter(path + i);
                list.Add(logWriter);

            }

            Console.WriteLine("До первой сборки мусора");
            PrintGeneration(list);
            GC.Collect();

            Console.WriteLine("После первой сборки мусора");
            PrintGeneration(list);

            for (int i = n; i < 2 * n; i++)
            {
                LogWriter logWriter = new LogWriter(path + i);
                list.Add(logWriter);
            }

            Console.WriteLine("До второй сборки мусора, добавлены еще объекты");
            PrintGeneration(list);
            GC.Collect();

            Console.WriteLine("После второй сборки мусора");
            PrintGeneration(list);

            foreach (LogWriter writer in list)
            {
                writer.Dispose();
            }
            GC.Collect();
        }
    }
}
