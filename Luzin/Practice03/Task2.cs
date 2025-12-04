using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Practice03
{
    internal class Student
    {
        public float Avg { get; set; }
        public string Name { get; set; }

        public Student(int avg, string name)
        {
            Avg = avg;
            Name = name;
        }
    }

    internal class Product
    {
        public string Name { get; set; }
        public float Price { get; set; }

        public Product(string name, float price)
        {
            Name = name;
            Price = price;
        }
    }

    internal class Task2
    {
        private static IEnumerable<T> FilterCollection<T>(IEnumerable<T> collection, Func<T, bool> condition)
        {
            foreach (T item in collection)
            {
                if (condition(item))
                {
                    yield return item;
                }
            }
        }

        public void Main(string[] args)
        {
            List<Student> students = new List<Student>()
            {
                new Student(7, "Bob"),
                new Student(9, "Alex"),
                new Student(5, "Oleg")
            };
            List<Product> products = new List<Product>()
            {
                new Product("Milk", 120),
                new Product("Bread", 47),
                new Product("Key", 1200)
            };

            var filteredStudents = FilterCollection(students, s => s.Avg > 8);

            foreach (Student student in filteredStudents)
            {
                Console.WriteLine(student.Name);
            }

            var filteredProducts = FilterCollection(products, p => p.Price > 100);

            foreach (Product product in filteredProducts)
            {
                Console.WriteLine(product.Name);
            }
        }
    }
}
