using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TodoList.Exceptions;

namespace TodoList
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Работу выполнили Vasilevich и Garmash");
			InitializeDatabase();

			try
			{
				AppInfo.AllProfiles = AppInfo.ProfileRepository.GetAll();
			}
			catch (Exception ex)
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
					var userTodos = AppInfo.TodoRepository.GetAllByProfile(AppInfo.CurrentProfile.Id);
					var todoList = new TodoList(userTodos);
					AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = todoList;

					SetupAutoSave(todoList);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
					AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = new TodoList(new List<TodoItem>());
				}

				Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.GetInfo()}!");
				Console.WriteLine("Напишите 'help' для списка команд или 'exit' для выхода.");

				CommandLoop();
			}
		}

		private static void InitializeDatabase()
		{
			using var context = new AppDbContext();
			context.Database.Migrate();
			Console.WriteLine("Выбрано хранение в SQLite (todos.db).");
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

			var profile = AppInfo.ProfileRepository.GetByCredentials(login, password);

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
			AppInfo.ProfileRepository.Add(newProfile);
			AppInfo.AllProfiles = AppInfo.ProfileRepository.GetAll();
			AppInfo.CurrentProfile = newProfile;

			Console.WriteLine("Новый профиль успешно создан.");
		}

		private static void SetupAutoSave(TodoList todoList)
		{
			Action<TodoItem> addHandler = item =>
			{
				try
				{
					AppInfo.TodoRepository.Add(item);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка автосохранения: {ex.Message}");
				}
			};

			Action<TodoItem> deleteHandler = item =>
			{
				try
				{
					AppInfo.TodoRepository.Delete(item.Id);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка автосохранения: {ex.Message}");
				}
			};

			Action<TodoItem> updateHandler = item =>
			{
				try
				{
					AppInfo.TodoRepository.Update(item);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка автосохранения: {ex.Message}");
				}
			};

			Action<TodoItem> statusHandler = item =>
			{
				try
				{
					AppInfo.TodoRepository.SetStatus(item.Id, item.Status);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка автосохранения: {ex.Message}");
				}
			};

			todoList.OnTodoAdded += addHandler;
			todoList.OnTodoDeleted += deleteHandler;
			todoList.OnTodoUpdated += updateHandler;
			todoList.OnStatusChanged += statusHandler;
		}
	}
}
