using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Luzin.Lab01
{
    public class Person : IDisposable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonPropertyName("personId")]
        public string Id { get; set; }

        [JsonInclude]
        private DateTime _birthDate;

        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty");
                if (!value.Contains("@"))
                    throw new ArgumentException("Email must contain '@'");
                _email = value;
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Birth date cannot be in the future");
                _birthDate = value;
            }
        }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [JsonIgnore]
        public bool IsAdult => Age >= 18;

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _disposed = true;
            }
        }

        ~Person()
        {
            Dispose(false);
        }
    }
}