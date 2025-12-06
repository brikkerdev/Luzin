namespace Luzin.Ind01
{
    internal class Program
    {
        private static void FindTangentRoots(double[] aValues)
        {
            double left = -Math.PI / 2;
            double right = Math.PI / 5;

            Console.WriteLine($"Корни tg x = a на промежутке ({left:F2}; {right:F2}):");

            foreach (double a in aValues)
            {
                double x = Math.Atan(a);

                if (x > left && x < right)
                {
                    Console.WriteLine($"a = {a}: x = {x:F2}");
                }
            }
        }
        static void Main(string[] args)
        {
            int numberOfA = 5;
            double[] aValues = new double[numberOfA];

            for (int i = 0; i < numberOfA; i++)
            {
                string? a = Console.ReadLine();
                aValues[i] = Convert.ToDouble(a);
            }

            FindTangentRoots(aValues);
        }
    }
}
