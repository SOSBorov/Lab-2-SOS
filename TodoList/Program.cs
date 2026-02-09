using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TodoList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Vasilevich и Garmash");

            FileManager.EnsureDataDirectory(FileManager.DataDirectory);
            AppInfo.AllProfiles = FileManager.LoadProfiles(FileManager.ProfileFilePath);
            AppInfo.UserTodos = new Dictionary<Guid, TodoList>();

            while (true)
            {
                while (AppInfo.CurrentProfile == null)
                {
                    Console.Write("Войти в существующий профиль [y], создать новый [n] или выйти [exit]?: ");
                    string choice = Console.ReadLine()?.ToLower();

                    if (choice == "y") HandleLogin();
                    else if (choice == "n") HandleRegistration();
                    else if (choice == "exit")
                    {
                        Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
                        return;
                    }
                    else Console.WriteLine("Неверный ввод.");
                }

                AppInfo.CurrentUserTodosFilePath = Path.Combine(FileManager.DataDirectory, $"todos_{AppInfo.CurrentProfile.Id}.csv");
                var currentUserTodos = FileManager.LoadTodos(AppInfo.CurrentUserTodosFilePath);
                AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = currentUserTodos;

                Action<TodoItem> saveHandler = (item) =>
                    FileManager.SaveTodos(currentUserTodos, AppInfo.CurrentUserTodosFilePath);

                currentUserTodos.OnTodoAdded += saveHandler;
                currentUserTodos.OnTodoDeleted += saveHandler;
                currentUserTodos.OnTodoUpdated += saveHandler;
                currentUserTodos.OnStatusChanged += saveHandler;

                AppInfo.UndoStack = new Stack<ICommand>();
                AppInfo.RedoStack = new Stack<ICommand>();

                Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.GetInfo()}!");
                Console.WriteLine("Напишите 'help' для списка команд или 'exit' для выхода.");

                while (true)
                {
                    if (AppInfo.CurrentProfile == null)
                    {
                        currentUserTodos.OnTodoAdded -= saveHandler;
                        currentUserTodos.OnTodoDeleted -= saveHandler;
                        currentUserTodos.OnTodoUpdated -= saveHandler;
                        currentUserTodos.OnStatusChanged -= saveHandler;

                        Console.WriteLine("\nВы вышли из профиля. Выберите действие:");
                        break;
                    }

                    Console.Write("> ");
                    var input = Console.ReadLine();
                    if (input == null) continue;

                    input = input.Trim();
                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
                        return;
                    }
                    if (input.Length == 0) continue;

                    var command = CommandParser.Parse(input);
                    if (command != null)
                    {
                        try
                        {
                            command.Execute();
                            if (command is not (ViewCommand or HelpCommand or UndoCommand or RedoCommand or ProfileCommand or SearchCommand))
                            {
                                AppInfo.UndoStack.Push(command);
                                AppInfo.RedoStack.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                        }
                    }
                }
            }
        }

        private static void HandleLogin()
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            var profile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login) && p.Password.Equals(password));

            if (profile != null)
            {
                AppInfo.CurrentProfile = profile;
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
            }
        }

        private static void HandleRegistration()
        {
            string login;
            while (true)
            {
                Console.Write("Введите новый логин: ");
                login = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(login))
                {
                    Console.WriteLine("Логин не может быть пустым.");
                    continue;
                }
                if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login)))
                {
                    Console.WriteLine("Этот логин уже занят. Пожалуйста, выберите другой.");
                }
                else
                {
                    break;
                }
            }

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();
            Console.Write("Введите ваше имя: ");
            string firstName = Console.ReadLine();
            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            int birthYear;
            while (true)
            {
                Console.Write("Введите год рождения (YYYY): ");
                if (int.TryParse(Console.ReadLine(), out birthYear) && birthYear > 1900 && birthYear <= DateTime.Now.Year)
                {
                    break;
                }
                Console.WriteLine("Неверный формат года. Пожалуйста, введите год в формате YYYY.");
            }

            var newProfile = new Profile(login, password, firstName, lastName, birthYear);
            AppInfo.AllProfiles.Add(newProfile);
            FileManager.SaveProfiles(AppInfo.AllProfiles, FileManager.ProfileFilePath);

            AppInfo.CurrentProfile = newProfile;
            Console.WriteLine("Новый профиль успешно создан.");
        }
    }
}