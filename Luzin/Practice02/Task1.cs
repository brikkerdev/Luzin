using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice02
{
    internal class Task1
    {
        private static void Calculate(string path, ref int fileN, ref int subDirN, ref long size)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);
                Console.WriteLine(info.FullName);

                string[] directories = Directory.GetDirectories(path);
                subDirN += directories.Length;
                foreach (string directory in directories)
                {
                    Calculate(directory, ref fileN, ref subDirN, ref size);
                }

                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    Calculate(file, ref fileN, ref subDirN, ref size);
                }
            }
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                Console.WriteLine(fileInfo.FullName);

                fileN += 1;
                size += fileInfo.Length;
            }
        }
        private static void PrintInfo(string path)
        {
            int fileN = 0;
            int subDirN = 0;
            long size = 0;

            Calculate(path, ref fileN, ref subDirN, ref size);
            Console.WriteLine($"Количество файлов: {fileN}");
            Console.WriteLine($"Количество подпапок: {subDirN}");
            Console.WriteLine($"Общий размер: {size}");
        }
        public void Main(string[] args)
        {
            string path = "C:\\School";

            PrintInfo(path);
        }
    }
}
