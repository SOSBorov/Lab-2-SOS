using System;

namespace TodoList
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; } = "Default";
        public string? LastName { get; set; }
        public int BirthYear { get; set; }

        public Profile()
        {
            Id = Guid.NewGuid();
            Login = string.Empty;
            Password = string.Empty;
        }

        public Profile(string login, string password, string firstName, string? lastName, int birthYear)
        {
            Id = Guid.NewGuid();
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            BirthYear = birthYear;
        }

        public string GetInfo()
        {
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - BirthYear;
            return $"Пользователь: {FirstName} {LastName}, Возраст: {age} лет";
        }
    }
}