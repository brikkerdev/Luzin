using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice_Smokers
{
    internal class Task
    {
        // 0 - бумага
        // 1 - табак
        // 2 - спички
        public bool[] items = new bool[3]
        {
            false,
            false,
            false
        };
        public bool filled = false;

        Mutex mutex = new Mutex();
        Random rand = new Random();

        public void Agent()
        {
            while (true)
            {

                mutex.WaitOne();

                if (filled)
                {
                    mutex.ReleaseMutex();
                    continue;
                }

                int n = 2;
                while (n > 0)
                {
                    int index = rand.Next(0, 3);
                    if (items[index] == true)
                    {
                        continue;
                    }
                    items[index] = true;
                    n--;
                }

                Console.Write("Агент выложил на стол: ");
                bool f = false;
                for (int i = 0; i < 3; i++)
                {
                    if (items[i] == true)
                    {
                        switch (i)
                        {
                            case 0:
                                Console.Write("Бумага");
                                break;
                            case 1:
                                Console.Write("Табак");
                                break;
                            case 2:
                                Console.Write("Спички");
                                break;
                        }
                        if (!f) Console.Write(" и ");
                        f = true;
                    }
                }
                Console.Write('\n');

                filled = true;

                Thread.Sleep(2000);
                mutex.ReleaseMutex();
            }
        }

        public void SmokerPaper()
        {
            while (true)
            {
                mutex.WaitOne();

                if (items[0] == true || !filled)
                {
                    mutex.ReleaseMutex();
                    continue;
                }

                Console.WriteLine("Курильщик с бумагой забирает компоненты...");
                Console.WriteLine("Курильщик с бумагой курит...");
                Thread.Sleep(2000);
                Console.WriteLine("Курильщик с бумагой закончил.");

                items[0] = false;
                items[1] = false;
                items[2] = false;
                filled = false;

                Thread.Sleep(2000);
                mutex.ReleaseMutex();
            }
        }

        public void SmokerTobacco()
        {
            while (true)
            {
                mutex.WaitOne();

                if (items[1] == true || !filled)
                {
                    mutex.ReleaseMutex();
                    continue;
                }

                Console.WriteLine("Курильщик с табаком забирает компоненты...");
                Console.WriteLine("Курильщик с табаком курит...");
                Thread.Sleep(2000);
                Console.WriteLine("Курильщик с табаком закончил.");

                items[0] = false;
                items[1] = false;
                items[2] = false;
                filled = false;

                Thread.Sleep(2000);
                mutex.ReleaseMutex();
            }
        }

        public void SmokerMatches()
        {
            while (true)
            {
                mutex.WaitOne();

                if (items[2] == true || !filled)
                {
                    mutex.ReleaseMutex();
                    continue;
                }

                Console.WriteLine("Курильщик со спичками забирает компоненты...");
                Console.WriteLine("Курильщик со спичками курит...");
                Thread.Sleep(2000);
                Console.WriteLine("Курильщик со спичками закончил.");

                items[0] = false;
                items[1] = false;
                items[2] = false;
                filled = false;

                Thread.Sleep(2000);
                mutex.ReleaseMutex();
            }
        }

        public void Main(string[] args)
        {
            Thread agent = new Thread(Agent);
            Thread paper = new Thread(SmokerPaper);
            Thread tobacco = new Thread(SmokerTobacco);
            Thread matches = new Thread(SmokerMatches);

            agent.Start();
            matches.Start();
            paper.Start();
            tobacco.Start();
        }
    }
}
