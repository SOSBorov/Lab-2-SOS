using System;
using System.IO;
using System.Linq;

namespace TodoList;

public class ProfileCommand : ICommand
{
	public bool IsLogout { get; set; }
	public bool IsSwitching { get; set; }

	public void Execute()
	{
		if (AppInfo.CurrentProfile == null) throw new InvalidOperationException("Профиль не инициализирован");

		if (IsLogout)
		{
			Console.WriteLine($"Пользователь {AppInfo.CurrentProfile.Login} вышел из системы.");
			AppInfo.CurrentProfile = null;
			return;
		}

		if (IsSwitching)
		{
			Console.WriteLine("Переключение профиля...");
			AppInfo.CurrentProfile = null;
			return;
		}

		Console.WriteLine(AppInfo.CurrentProfile.GetInfo());
		Console.Write("Сменить профиль? [y/n]: ");
		string? choice = Console.ReadLine()?.ToLower();

		if (choice == "y")
		{
			Console.Write("Введите логин: ");
			string? login = Console.ReadLine();
			Console.Write("Введите пароль: ");
			string? password = Console.ReadLine();

			var newProfile = AppInfo.AllProfiles.FirstOrDefault(p => p.Login.Equals(login) && p.Password.Equals(password));

			if (newProfile != null)
			{
				AppInfo.CurrentProfile = newProfile;

				AppInfo.CurrentUserTodosFilePath = Path.Combine(FileManager.DataDirectory, $"todos_{AppInfo.CurrentProfile.Id}.csv");
				var currentUserTodos = FileManager.LoadTodos(AppInfo.CurrentUserTodosFilePath);
				AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = currentUserTodos;

				AppInfo.UndoStack.Clear();
				AppInfo.RedoStack.Clear();

				Console.WriteLine("Профиль успешно изменен.");
				Console.WriteLine($"\nДобро пожаловать, {AppInfo.CurrentProfile.GetInfo()}!");
			}
			else
			{
				Console.WriteLine("Неверный логин или пароль.");
				Console.WriteLine("Смена профиля отменена. Текущий профиль оставлен без изменений.");
			}
		}
		else
		{
			Console.WriteLine("Профиль оставлен без изменений.");
		}
	}
}
