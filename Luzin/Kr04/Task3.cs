using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Kr04
{
    internal class Task3
    {
        private void Print()
        {
            int N = Convert.ToInt32(Console.ReadLine());
            int sum = 0;
            for (int i = 1; i <= N; i++)
            {
                sum += i;
            }
            Console.WriteLine(sum);
        }

        public void Main(string[] args)
        {
            Thread thread = new Thread(Print);
            thread.Start();
        }
    }
}
