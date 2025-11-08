using System;
using System.Globalization;
using System.IO;

namespace TodoList
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Vasilevich и Garmash");

            FileManager.EnsureDataDirectory(FileManager.DataDirectory);

            Profile profile;
            TodoList todoList;

            profile = FileManager.LoadProfile(FileManager.ProfileFilePath);

            if (profile.Name == "Default" || profile.YearOfBirth == 0)
            {
                Console.WriteLine("Профиль не найден или поврежден. Пожалуйста, введите ваши данные.");

                Console.WriteLine("Продиктуйте ваше имя и фамилию мессир: ");
                profile.Name = Console.ReadLine();

                Console.WriteLine("Продиктуйте ваш год рождения (YYYY): ");
                string yearInput = Console.ReadLine();
                DateTime birthdayDate;
                int yearOfBirth;

                while (!DateTime.TryParseExact(yearInput, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthdayDate))
                {
                    Console.WriteLine("Неверный формат года. Пожалуйста, введите год в формате YYYY:");
                    yearInput = Console.ReadLine();
                }
                yearOfBirth = birthdayDate.Year;

                profile.YearOfBirth = yearOfBirth;

                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - profile.YearOfBirth;

                Console.WriteLine($" Добавлен пользователь {profile.Name}, возраст - {age}");
                FileManager.SaveProfile(profile, FileManager.ProfileFilePath);
            }
            else
            {
                Console.WriteLine($"Добро пожаловать обратно, {profile.GetInfo()}!");
            }

            todoList = FileManager.LoadTodos(FileManager.TodosFilePath);

            Console.WriteLine("Напишите 'help' для списка команд или 'exit' для выхода.");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) break;

                input = input.Trim();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                if (input.Length == 0) continue;

                var command = CommandParser.Parse(input, todoList, profile);
                try
                {
                    command?.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
        }
    }
}