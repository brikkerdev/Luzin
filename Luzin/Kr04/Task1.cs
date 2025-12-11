using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luzin.Kr04
{
    internal class Box
    {
        private float _size;
        public float Size => _size;

        public Box(float size)
        {
            _size = size;
        }

        public float GetArea()
        {
            return _size * _size * _size;
        }
    }

    internal class Task1
    {
        public void ReflexObject(object obj)
        {
            Type type = obj.GetType();

            Console.WriteLine($"Тип объекта: {type.Name}");

            PropertyInfo[] propertyInfo = type.GetProperties();
            foreach (PropertyInfo property in propertyInfo)
            {
                Console.WriteLine($"Свойство {property.Name}, тип {property.PropertyType}");
            }

            MethodInfo[] methodInfo = type.GetMethods();
            foreach (MethodInfo method in methodInfo)
            {
                Console.WriteLine($"Метод {method.Name}");
            }
        }


        public void Main(string[] args)
        {
            ReflexObject(new Box(10));
            ReflexObject("100");
        }
    }
}
