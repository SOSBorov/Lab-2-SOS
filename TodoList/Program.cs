using System;
using System.Globalization;
using System.IO; 

namespace TodoList
{
    class Program
    {
        private static readonly string DataDirectory = "Data";
        private static readonly string ProfileFilePath = Path.Combine(DataDirectory, "profile.txt");
        private static readonly string TodosFilePath = Path.Combine(DataDirectory, "todo.csv");

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Vasilevich и Garmash");


            FileManager.EnsureDataDirectory(DataDirectory);

            Profile profile;
            TodoList todoList;


            profile = FileManager.LoadProfile(ProfileFilePath);


            if (string.IsNullOrEmpty(profile.Name) || profile.Name == "Default")
            {
                Console.WriteLine("Продиктуйте ваше имя и фамилию мессир: ");
                profile.Name = Console.ReadLine();

                Console.WriteLine("Продиктуйте ваш год рождения (YYYY): ");
                string yearInput = Console.ReadLine();
                DateTime birthdayDate;


                while (!DateTime.TryParseExact(yearInput, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthdayDate))
                {
                    Console.WriteLine("Неверный формат года. Пожалуйста, введите год в формате YYYY:");
                    yearInput = Console.ReadLine();
                }

                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - birthdayDate.Year;

                Console.WriteLine($" Добавлен пользователь {profile.Name}, возраст - {age}");
                FileManager.SaveProfile(profile, ProfileFilePath);  
            }
            else
            {
                
                Console.WriteLine($"Добро пожаловать обратно, {profile.Name}!");
            }


           
            todoList = FileManager.LoadTodos(TodosFilePath);


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
                    
                    FileManager.SaveTodos(todoList, TodosFilePath);
                    FileManager.SaveProfile(profile, ProfileFilePath);
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