using System;
using System.Globalization;

namespace TodoList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Работу выполнили Vasilevich и Garmash");
            Console.WriteLine("Продиктуйте ваше имя и фамилию мессир: ");
            string fullname = Console.ReadLine();

            Console.WriteLine("Продиктуйте ваш год рождения: ");
            DateTime birthdayDate = DateTime.ParseExact(Console.ReadLine(), "yyyy", CultureInfo.InvariantCulture);

            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - birthdayDate.Year;

            var profile = new Profile
            {
                Name = fullname
            };

            Console.WriteLine(" Добавлен пользователь " + fullname + ", " + "возраст - " + age);

            var todoList = new TodoList();

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
