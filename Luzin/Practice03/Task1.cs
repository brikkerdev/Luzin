using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice03
{
    internal class VersionedStack<T>
    {
        public Stack<T> Elements { get; set; }
        public Dictionary<int, T> Versions { get; set; }
        public int currentVersion = 0;

        public VersionedStack()
        {
            Elements = new Stack<T>();
            Versions = new Dictionary<int, T>();
        }

        public void Push(T item)
        {
            Elements.Push(item);
            currentVersion++;
            Versions[currentVersion] = item;

            Console.WriteLine($"Added {item}, verison {currentVersion}");
        }

        public T Pop()
        {
            if (Elements.Count == 0)
            {
                throw new Exception("No elements");
            }

            T item = Elements.Pop();
            Console.WriteLine($"Popped {item}");
            return item;
        }

        public T GetVersion(int version)
        {
            if (!Versions.ContainsKey(version))
            {
                throw new Exception("No such key");
            }

            Console.WriteLine(Versions[version]);
            return Versions[version];
        }
    }

    internal class Task1
    {
        public void Main(string[] args)
        {
            VersionedStack<string> stack = new VersionedStack<string>();

            stack.Push("Добавлена первая версия документа");
            stack.Push("Добавлена правка 1");

            stack.GetVersion(1);
            stack.GetVersion(2);

            stack.Pop();
            stack.Push("Очередная правка");
            stack.GetVersion(3);
        }
    }
}
