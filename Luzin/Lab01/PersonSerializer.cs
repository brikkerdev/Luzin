using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace Lab1
{
    public class PersonSerializer
    {
        private readonly JsonSerializerOptions _options;
        private static readonly object _fileLock = new object();
        private readonly string _errorLogPath;

        public PersonSerializer(string errorLogPath = "errors.log")
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            _errorLogPath = errorLogPath;
        }

        private void LogError(string error)
        {
            lock (_fileLock)
            {
                try
                {
                    File.AppendAllText(_errorLogPath, $"{DateTime.Now}: {error}{Environment.NewLine}", Encoding.UTF8);
                }
                catch
                {
                }
            }
        }

        public string SerializeToJson(Person person)
        {
            try
            {
                return JsonSerializer.Serialize(person, _options);
            }
            catch (Exception ex)
            {
                LogError($"SerializeToJson error: {ex.Message}");
                throw;
            }
        }

        public Person DeserializeFromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Person>(json, _options);
            }
            catch (Exception ex)
            {
                LogError($"DeserializeFromJson error: {ex.Message}");
                throw;
            }
        }

        public void SaveToFile(Person person, string filePath)
        {
            try
            {
                string json = SerializeToJson(person);
                lock (_fileLock)
                {
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                LogError($"SaveToFile error ({filePath}): {ex.Message}");
                throw;
            }
        }

        public Person LoadFromFile(string filePath)
        {
            try
            {
                string json;
                lock (_fileLock)
                {
                    if (!File.Exists(filePath))
                        throw new FileNotFoundException($"File not found: {filePath}");
                    json = File.ReadAllText(filePath, Encoding.UTF8);
                }
                return DeserializeFromJson(json);
            }
            catch (Exception ex)
            {
                LogError($"LoadFromFile error ({filePath}): {ex.Message}");
                throw;
            }
        }

        public async Task SaveToFileAsync(Person person, string filePath)
        {
            try
            {
                string json = SerializeToJson(person);
                byte[] encodedText = Encoding.UTF8.GetBytes(json);

                using (var sourceStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            }
            catch (Exception ex)
            {
                LogError($"SaveToFileAsync error ({filePath}): {ex.Message}");
                throw;
            }
        }

        public async Task<Person> LoadFromFileAsync(string filePath)
        {
            try
            {
                using (var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    var sb = new StringBuilder();
                    byte[] buffer = new byte[0x1000];
                    int numRead;
                    while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                        sb.Append(text);
                    }
                    return DeserializeFromJson(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogError($"LoadFromFileAsync error ({filePath}): {ex.Message}");
                throw;
            }
        }

        public void SaveListToFile(List<Person> people, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(people, _options);
                lock (_fileLock)
                {
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                LogError($"SaveListToFile error ({filePath}): {ex.Message}");
                throw;
            }
        }

        public List<Person> LoadListFromFile(string filePath)
        {
            try
            {
                string json;
                lock (_fileLock)
                {
                    if (!File.Exists(filePath))
                        throw new FileNotFoundException($"File not found: {filePath}");
                    json = File.ReadAllText(filePath, Encoding.UTF8);
                }
                return JsonSerializer.Deserialize<List<Person>>(json, _options) ?? new List<Person>();
            }
            catch (Exception ex)
            {
                LogError($"LoadListFromFile error ({filePath}): {ex.Message}");
                throw;
            }
        }
    }
}