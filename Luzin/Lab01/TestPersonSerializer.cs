using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Lab1.Tests
{
    public class TestPersonSerializer
    {
        private static string UniquePath(string fileName) =>
            Path.Combine(Path.GetTempPath(), $"Lab1_{Guid.NewGuid():N}_{fileName}");

        [Fact]
        public void SerializeToJson_And_DeserializeFromJson_RoundTrip_Works()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

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

            Assert.False(string.IsNullOrWhiteSpace(json));

            var deserialized = serializer.DeserializeFromJson(json);
            Assert.NotNull(deserialized);

            Assert.Equal("Иван", deserialized.FirstName);
            Assert.Equal("Иванов", deserialized.LastName);
            Assert.Equal(25, deserialized.Age);
            Assert.Equal("ivan@example.com", deserialized.Email);
            Assert.Equal("+79991234567", deserialized.PhoneNumber);
            Assert.Equal("123", deserialized.Id);

            Assert.DoesNotContain("Password", json, StringComparison.OrdinalIgnoreCase);
            Assert.Null(deserialized.Password);
        }

        [Fact]
        public void SaveToFile_Then_LoadFromFile_Works()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

            string filePath = UniquePath("test_person.json");

            try
            {
                var person = new Person
                {
                    FirstName = "Мария",
                    LastName = "Петрова",
                    Age = 30,
                    Email = "maria@example.com",
                    PhoneNumber = "+79998765432",
                    Id = "456"
                };

                serializer.SaveToFile(person, filePath);
                Assert.True(File.Exists(filePath));

                var loaded = serializer.LoadFromFile(filePath);
                Assert.NotNull(loaded);
                Assert.Equal("Мария", loaded.FirstName);
                Assert.Equal("Петрова", loaded.LastName);
                Assert.Equal(30, loaded.Age);
                Assert.Equal("maria@example.com", loaded.Email);
                Assert.Equal("456", loaded.Id);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                if (File.Exists(errorLog)) File.Delete(errorLog);
            }
        }

        [Fact]
        public void SaveListToFile_Then_LoadListFromFile_Works()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

            string filePath = UniquePath("test_people.json");

            try
            {
                var people = new List<Person>
                {
                    new Person { FirstName = "Алексей", LastName = "Сидоров", Age = 25, Email = "alex@example.com", Id = "1" },
                    new Person { FirstName = "Ольга", LastName = "Смирнова", Age = 22, Email = "olga@example.com", Id = "2" }
                };

                serializer.SaveListToFile(people, filePath);
                Assert.True(File.Exists(filePath));

                var loadedList = serializer.LoadListFromFile(filePath);
                Assert.NotNull(loadedList);
                Assert.Equal(2, loadedList.Count);

                Assert.Equal("Алексей", loadedList[0].FirstName);
                Assert.Equal("Сидоров", loadedList[0].LastName);
                Assert.Equal("1", loadedList[0].Id);

                Assert.Equal("Ольга", loadedList[1].FirstName);
                Assert.Equal("Смирнова", loadedList[1].LastName);
                Assert.Equal("2", loadedList[1].Id);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                if (File.Exists(errorLog)) File.Delete(errorLog);
            }
        }

        [Fact]
        public void LoadFromFile_NonExistent_Throws_FileNotFoundException()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

            string missingPath = UniquePath("does_not_exist.json");

            try
            {
                Assert.False(File.Exists(missingPath));
                Assert.Throws<FileNotFoundException>(() => serializer.LoadFromFile(missingPath));
            }
            finally
            {
                if (File.Exists(errorLog)) File.Delete(errorLog);
            }
        }

        [Fact]
        public void DeserializeFromJson_InvalidJson_Throws_JsonException()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

            try
            {
                Assert.Throws<JsonException>(() => serializer.DeserializeFromJson("{ invalid json }"));
            }
            finally
            {
                if (File.Exists(errorLog)) File.Delete(errorLog);
            }
        }

        [Fact]
        public async Task SaveToFileAsync_Then_LoadFromFileAsync_Works()
        {
            var errorLog = UniquePath("test_errors.log");
            var serializer = new PersonSerializer(errorLog);

            string filePath = UniquePath("test_async.json");

            try
            {
                var person = new Person
                {
                    FirstName = "Дмитрий",
                    LastName = "Кузнецов",
                    Age = 35,
                    Email = "dmitry@example.com",
                    Id = "789"
                };

                await serializer.SaveToFileAsync(person, filePath);
                Assert.True(File.Exists(filePath));

                var loaded = await serializer.LoadFromFileAsync(filePath);
                Assert.NotNull(loaded);
                Assert.Equal("Дмитрий", loaded.FirstName);
                Assert.Equal("Кузнецов", loaded.LastName);
                Assert.Equal(35, loaded.Age);
                Assert.Equal("dmitry@example.com", loaded.Email);
                Assert.Equal("789", loaded.Id);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                if (File.Exists(errorLog)) File.Delete(errorLog);
            }
        }
    }
}