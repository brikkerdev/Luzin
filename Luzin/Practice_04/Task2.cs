using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Lection_Strings_Regex
{
    internal class Task2
    {
        private string Clear(string str)
        {
            StringBuilder sb = new StringBuilder();

            int opened = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == '(')
                {
                    opened += 1;
                    continue;
                }
                else if (c == ')')
                {
                    opened -= 1;
                    opened = opened < 0 ? 0 : opened;
                    continue;
                }

                if (opened == 0)
                {
                    sb.Append(c);
                }
            }

            string result = sb.ToString();
            Console.WriteLine($"Результат: {result}");
            return result;
        }

        public void Main(string[] args)
        {
            string str = "a(bc(de)f)g(h(i))";
            Console.WriteLine($"Исходные данные: {str}");
            Clear(str);
        }
    }
}
