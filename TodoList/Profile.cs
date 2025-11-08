using System; 
using System.Globalization;

namespace TodoList
{
    public class Profile
    {
        public string Name { get; set; } = "Default";
        public int YearOfBirth { get; set; } 

        public Profile() { }

        public Profile(string name, int yearOfBirth) 
        {
            Name = name;
            YearOfBirth = yearOfBirth;
        }

        public Profile(string name) : this(name, DateTime.Now.Year) 
        {
        }

        public string GetInfo()
        {
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - YearOfBirth;
            return $"Пользователь: {Name}, Возраст: {age} лет"; 
        }
    }
}