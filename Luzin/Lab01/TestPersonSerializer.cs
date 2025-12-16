using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab1
{
    public class TestPersonSerializer
    {
        public static void RunTests()
        {
            var serializer = new PersonSerializer("test_errors.log");

            Console.WriteLine("=== Testing PersonSerializer ===");

            try
            {
                TestSerializeDeserialize(serializer);
                TestFileOperations(serializer);
                TestListOperations(serializer);
                TestErrorHandling(serializer);
                TestAsyncOperations(serializer).Wait();

                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
            }
        }

        private static void TestSerializeDeserialize(PersonSerializer serializer)
        {
            Console.WriteLine("\n1. Testing Serialize/Deserialize:");

            var person = new Person
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Age = 25,
                Email = "ivan@example.com",
                PhoneNumber = "+79991234567",
                Id = "123",
                Password = "secret"
            };

            string json = serializer.SerializeToJson(person);
            Console.WriteLine($"Serialized JSON: {json.Substring(0, Math.Min(100, json.Length))}...");

            Person deserialized = serializer.DeserializeFromJson(json);
            Console.WriteLine($"Deserialized: {deserialized.FirstName} {deserialized.LastName}, Age: {deserialized.Age}");

            if (deserialized.Password != null)
                Console.WriteLine("ERROR: Password should not be serialized!");
        }

        private static void TestFileOperations(PersonSerializer serializer)
        {
            Console.WriteLine("\n2. Testing File Operations:");

            var person = new Person
            {
                FirstName = "Мария",
                LastName = "Петрова",
                Age = 30,
                Email = "maria@example.com",
                PhoneNumber = "+79998765432",
                Id = "456"
            };

            string filePath = "test_person.json";
            serializer.SaveToFile(person, filePath);
            Console.WriteLine($"Saved to file: {filePath}");

            Person loaded = serializer.LoadFromFile(filePath);
            Console.WriteLine($"Loaded from file: {loaded.FirstName} {loaded.LastName}");

            File.Delete(filePath);
        }

        private static void TestListOperations(PersonSerializer serializer)
        {
            Console.WriteLine("\n3. Testing List Operations:");

            var people = new List<Person>
        {
            new Person { FirstName = "Алексей", LastName = "Сидоров", Age = 25, Email = "alex@example.com", Id = "1" },
            new Person { FirstName = "Ольга", LastName = "Смирнова", Age = 22, Email = "olga@example.com", Id = "2" }
        };

            string filePath = "test_people.json";
            serializer.SaveListToFile(people, filePath);
            Console.WriteLine($"Saved list to file: {filePath}");

            List<Person> loadedList = serializer.LoadListFromFile(filePath);
            Console.WriteLine($"Loaded {loadedList.Count} people from file");

            File.Delete(filePath);
        }

        private static void TestErrorHandling(PersonSerializer serializer)
        {
            Console.WriteLine("\n4. Testing Error Handling:");

            try
            {
                serializer.LoadFromFile("non_existent_file.json");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Correctly caught FileNotFoundException");
            }

            try
            {
                serializer.DeserializeFromJson("{ invalid json }");
            }
            catch (JsonException)
            {
                Console.WriteLine("Correctly caught JsonException");
            }
        }

        private static async Task TestAsyncOperations(PersonSerializer serializer)
        {
            Console.WriteLine("\n5. Testing Async Operations:");

            var person = new Person
            {
                FirstName = "Дмитрий",
                LastName = "Кузнецов",
                Age = 35,
                Email = "dmitry@example.com",
                Id = "789"
            };

            string filePath = "test_async.json";
            await serializer.SaveToFileAsync(person, filePath);
            Console.WriteLine("Async save completed");

            Person loaded = await serializer.LoadFromFileAsync(filePath);
            Console.WriteLine($"Async load completed: {loaded.FirstName} {loaded.LastName}");

            File.Delete(filePath);
        }
    }
}
