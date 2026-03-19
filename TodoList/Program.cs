using System;
using System.Collections.Generic;
using System.Linq;
using TodoList.Exceptions;

namespace TodoList
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Работу выполнили Vasilevich и Garmash");

			const string dataDirectory = "Data";
			AppInfo.DataStorage = new FileManager(dataDirectory);

			try
			{
				AppInfo.AllProfiles = AppInfo.DataStorage.LoadProfiles().ToList();
			}
			catch (DataStorageException ex)
			{
				Console.WriteLine($"Критическая ошибка при загрузке профилей: {ex.Message}");
				Console.WriteLine("Работа приложения будет завершена.");
				return;
			}

			while (true)
			{
				while (AppInfo.CurrentProfile == null)
				{
					HandleAuth();
				}

				if (AppInfo.CurrentProfile == null) continue;

				try
				{
					var userTodos = AppInfo.DataStorage.LoadTodos(AppInfo.CurrentProfile.Id);
					var todoList = new TodoList(userTodos.ToList());
					AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = todoList;

					SetupAutoSave(AppInfo.CurrentProfile.Id, todoList);
				}
				catch (DataStorageException ex)
				{
					Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
					AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = new TodoList(new List<TodoItem>());
				}

				Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.GetInfo()}!");
				Console.WriteLine("Напишите 'help' для списка команд или 'exit' для выхода.");

				CommandLoop();
			}
		}

		private static void HandleAuth()
		{
			try
			{
				Console.Write("Войти в существующий профиль [y], создать новый [n] или выйти [exit]?: ");
				string? choice = Console.ReadLine()?.ToLower();

				if (choice == "y") HandleLogin();
				else if (choice == "n") HandleRegistration();
				else if (choice == "exit")
				{
					Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
					Environment.Exit(0);
				}
				else Console.WriteLine("Неверный ввод.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Произошла ошибка: {ex.Message}");
			}
		}

		private static void CommandLoop()
		{
			while (AppInfo.CurrentProfile != null)
			{
				Console.Write("> ");
				var input = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(input)) continue;

				if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("\nСпасибо за использование приложения. До свидания!");
					Environment.Exit(0);
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

		private static void HandleLogin()
		{
			Console.Write("Введите логин: ");
			string? login = Console.ReadLine();
			Console.Write("Введите пароль: ");
			string? password = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
				throw new AuthenticationException("Логин и пароль не могут быть пустыми.");

			var profile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login) && p.Password.Equals(password));

			if (profile != null) AppInfo.CurrentProfile = profile;
			else throw new AuthenticationException("Неверный логин или пароль.");
		}

		private static void HandleRegistration()
		{
			Console.Write("Введите новый логин: ");
			string login = Console.ReadLine() ?? "";
			if (string.IsNullOrWhiteSpace(login)) throw new InvalidArgumentException("Логин не должен быть пустым.");
			if (AppInfo.AllProfiles.Any(p => p.Login.Equals(login)))
				throw new AuthenticationException("Этот логин уже занят.");

			Console.Write("Введите пароль: ");
			string password = Console.ReadLine() ?? "";
			Console.Write("Введите ваше имя: ");
			string firstName = Console.ReadLine() ?? "";
			Console.Write("Введите вашу фамилию: ");
			string lastName = Console.ReadLine() ?? "";
			Console.Write("Введите год рождения (YYYY): ");
			int.TryParse(Console.ReadLine(), out int birthYear);

			var newProfile = new Profile(login, password, firstName, lastName, birthYear);
			AppInfo.AllProfiles.Add(newProfile);
			AppInfo.DataStorage.SaveProfiles(AppInfo.AllProfiles);
			AppInfo.CurrentProfile = newProfile;

			Console.WriteLine("Новый профиль успешно создан.");
		}

		private static void SetupAutoSave(Guid userId, TodoList todoList)
		{
			Action<TodoItem> saveHandler = _ =>
			{
				try
				{
					AppInfo.DataStorage.SaveTodos(userId, todoList.GetAllItems());
				}
				catch (DataStorageException ex)
				{
					Console.WriteLine($"Ошибка автосохранения: {ex.Message}");
				}
			};

			todoList.OnTodoAdded += saveHandler;
			todoList.OnTodoDeleted += saveHandler;
			todoList.OnTodoUpdated += saveHandler;
			todoList.OnStatusChanged += saveHandler;
		}
	}
}