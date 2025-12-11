using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luzin.Practice_04
{
    internal class Task3
    {
        public static bool ValidateExpression(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            string expr = str.Replace(" ", "");

            if (!ValidateParentheses(expr))
                return false;
            Console.WriteLine("Parenthesis right");


            if (!Regex.IsMatch(expr, @"[0-9\*\+\-\/()]+"))
                return false;
            Console.WriteLine("Characters right");

            if (Regex.IsMatch(expr, @"[\+\-\/\*]{2,}"))
                return false;
            Console.WriteLine("Double operators right");

            if (Regex.IsMatch(expr, @".+[\*\+\-\/]$"))
                return false;
            Console.WriteLine("End operators");

            if (expr.Contains("()"))
                return false;
            Console.WriteLine("Empty parenthesis right");

            return true;
        }

        private static bool ValidateParentheses(string expr)
        {
            int balance = 0;

            foreach (char c in expr)
            {
                if (c == '(')
                    balance++;
                else if (c == ')')
                    balance--;

                if (balance < 0)
                    return false;
            }

            return balance == 0;
        }

        public void Main(string[] args)
        {
            string str = "(2+3)*(4-(5/2))+74";
            Console.WriteLine(ValidateExpression(str));
        }
    }
}
