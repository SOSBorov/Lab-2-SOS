using System;
using System.Collections.Generic; 
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

            AppInfo.CurrentProfile = FileManager.LoadProfile(FileManager.ProfileFilePath);
            AppInfo.Todos = FileManager.LoadTodos(FileManager.TodosFilePath);
            AppInfo.UndoStack = new Stack<ICommand>();
            AppInfo.RedoStack = new Stack<ICommand>();

            if (AppInfo.CurrentProfile.Name == "Default" || AppInfo.CurrentProfile.YearOfBirth == 0)
            {
                Console.WriteLine("Профиль не найден или поврежден. Пожалуйста, введите ваши данные.");

                Console.WriteLine("Продиктуйте ваше имя и фамилию мессир: ");
                AppInfo.CurrentProfile.Name = Console.ReadLine();

                Console.WriteLine("Продиктуйте ваш год рождения (YYYY): ");
                string yearInput = Console.ReadLine();
                DateTime birthdayDate;
                while (!DateTime.TryParseExact(yearInput, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthdayDate))
                {
                    Console.WriteLine("Неверный формат года. Пожалуйста, введите год в формате YYYY:");
                    yearInput = Console.ReadLine();
                }
                AppInfo.CurrentProfile.YearOfBirth = birthdayDate.Year;

                Console.WriteLine($" Добавлен пользователь {AppInfo.CurrentProfile.Name}, возраст - {DateTime.Today.Year - AppInfo.CurrentProfile.YearOfBirth}");
                FileManager.SaveProfile(AppInfo.CurrentProfile, FileManager.ProfileFilePath);
            }
            else
            {
                Console.WriteLine($"Добро пожаловать обратно, {AppInfo.CurrentProfile.GetInfo()}!");
            }

            Console.WriteLine("Напишите 'help' для списка команд или 'exit' для выхода.");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) break;

                input = input.Trim();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                if (input.Length == 0) continue;

                var command = CommandParser.Parse(input);
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