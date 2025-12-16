using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Lection_Strings_Regex
{
    internal class Task1
    {
        private string Encode(string str)
        {
            StringBuilder sb = new StringBuilder();

            char current = str[0];
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == current)
                {
                    count++;
                }
                else
                {
                    sb.Append(current + count.ToString());
                    count = 1;
                }
                current = c;
            }
            sb.Append(current + count.ToString());

            string result = sb.ToString();

            Console.WriteLine($"Encoded result: {result}");
            return result;
        }

        private string Decode(string str)
        {
            StringBuilder sb = new StringBuilder();

            char current = str[0];
            int count = 0;
            bool started = false;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (Char.IsLetter(c) && started)
                {
                    sb.Append(current, count);
                    current = c;
                    count = 0;
                }
                else if (Char.IsDigit(c))
                {
                    count = count * 10 + c - '0';
                }
                else
                {
                    started = true;
                    current = c;
                    count = 0;
                }
            }
            sb.Append(current, count);

            string result = sb.ToString();

            Console.WriteLine($"Decoded result: {result}");
            return result;
        }

        public void Main(string[] args)
        {
            string word = "AAABBCCDAA";
            Decode(Encode(word));
        }
    }
}
