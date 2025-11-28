using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice02
{
    internal class Task2
    {
        public void Main(string[] args)
        {
            string sourcePath = "C:\\School\\Source";
            string backupPath = "C:\\School\\Backup\\";

            string[] files = Directory.GetFiles(sourcePath);
            List<string> copy = new List<string>();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                Console.WriteLine(fileInfo.Extension);
                if (
                    fileInfo.Exists
                    && (fileInfo.Extension == ".json" || fileInfo.Extension == ".txt")
                    && fileInfo.Length < 5242880
                )
                {
                    copy.Add(file);
                }
            }

            foreach (string file in copy)
            {
                FileInfo fileInfo = new FileInfo(file);
                string fileName = Path.GetFileNameWithoutExtension(file);

                if (File.Exists(fileInfo.FullName))
                {
                    continue;
                }

                File.Copy(file, backupPath + fileName + "_" + DateTime.Today.ToString("MM_dd_yyyy") + fileInfo.Extension);
            }
        }
    }
}
