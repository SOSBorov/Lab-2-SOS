using System;
using System.Collections.Generic;
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
					try
					{
						Console.Write("Войти в существующий профиль [y], создать новый [n] или выйти [exit]?: ");
						string choice = Console.ReadLine()?.ToLower() ?? "";

						if (choice == "y") HandleLogin();
						else if (choice == "n") HandleRegistration();
						else if (choice == "exit")
						{
							Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
							return;
						}
						else if (!string.IsNullOrWhiteSpace(choice))
						{
							Console.WriteLine("Неверный ввод.");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Ошибка: {ex.Message}");
					}
				}

				AppInfo.CurrentUserTodosFilePath = Path.Combine(FileManager.DataDirectory, $"todos_{AppInfo.CurrentProfile.Id}.csv");
				var currentUserTodos = FileManager.LoadTodos(AppInfo.CurrentUserTodosFilePath);
				AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = currentUserTodos;

				Action<TodoItem> saveHandler = (item) =>
				{
					if (AppInfo.CurrentUserTodosFilePath != null)
						FileManager.SaveTodos(currentUserTodos, AppInfo.CurrentUserTodosFilePath);
				};

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
					if (string.IsNullOrWhiteSpace(input)) continue;

					input = input.Trim();
					if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
					{
						Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
						return;
					}

					try
					{
						var command = CommandParser.Parse(input);
						command.Execute();
						if (command is IUndo)
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

		private static void HandleLogin()
		{
			Console.Write("Введите логин: ");
			string? login = Console.ReadLine();
			Console.Write("Введите пароль: ");
			string? password = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
			{
				throw new AuthenticationException("Логин и пароль не могут быть пустыми.");
			}

			var profile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login) && p.Password.Equals(password));

			if (profile != null)
			{
				AppInfo.CurrentProfile = profile;
			}
			else
			{
				throw new AuthenticationException("Неверный логин или пароль.");
			}
		}

		private static void HandleRegistration()
		{
			string login;
			Console.Write("Введите новый логин: ");
			login = Console.ReadLine() ?? "";
			if (string.IsNullOrWhiteSpace(login))
				throw new InvalidArgumentException("Логин не может быть пустым.");
			if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
				throw new DuplicateLoginException($"Логин '{login}' уже занят. Пожалуйста, выберите другой.");

			string password;
			Console.Write("Введите пароль: ");
			password = Console.ReadLine() ?? "";
			if (string.IsNullOrWhiteSpace(password))
				throw new InvalidArgumentException("Пароль не может быть пустым.");

			string firstName;
			Console.Write("Введите ваше имя: ");
			firstName = Console.ReadLine() ?? "";
			if (string.IsNullOrWhiteSpace(firstName))
				throw new InvalidArgumentException("Имя не может быть пустым.");

			Console.Write("Введите вашу фамилию: ");
			string? lastName = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(lastName)) lastName = null;

			int birthYear;
			Console.Write("Введите год рождения (YYYY): ");
			string? yearInput = Console.ReadLine();
			if (!int.TryParse(yearInput, out birthYear) || birthYear <= 1900 || birthYear > DateTime.Now.Year)
				throw new InvalidArgumentException("Неверный формат года. Пожалуйста, введите корректный год (например, 1995).");

			var newProfile = new Profile(login, password, firstName, lastName, birthYear);
			AppInfo.AllProfiles.Add(newProfile);
			FileManager.SaveProfiles(AppInfo.AllProfiles, FileManager.ProfileFilePath);

			AppInfo.CurrentProfile = newProfile;
			Console.WriteLine("Новый профиль успешно создан.");
		}
	}

	public class InvalidCommandException : Exception
	{
		public InvalidCommandException(string message) : base(message) { }
	}

	public class InvalidArgumentException : Exception
	{
		public InvalidArgumentException(string message) : base(message) { }
	}

	public class TaskNotFoundException : Exception
	{
		public TaskNotFoundException(string message) : base(message) { }
	}

	public class ProfileNotFoundException : Exception
	{
		public ProfileNotFoundException(string message) : base(message) { }
	}

	public class AuthenticationException : Exception
	{
		public AuthenticationException(string message) : base(message) { }
	}

	public class DuplicateLoginException : Exception
	{
		public DuplicateLoginException(string message) : base(message) { }
	}
}