using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class ProfileCommand : ICommand
	{
		public bool IsLogout { get; set; }
		public bool IsSwitching { get; set; }

		public void Execute()
		{
			if (AppInfo.CurrentProfile == null)
				throw new AuthenticationException("Текущий профиль не установлен.");

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

				var newProfile = AppInfo.ProfileRepository.GetByCredentials(login ?? string.Empty, password ?? string.Empty);

				if (newProfile != null)
				{
					AppInfo.CurrentProfile = newProfile;
					AppInfo.AllProfiles = AppInfo.ProfileRepository.GetAll();

					var userTodos = AppInfo.TodoRepository.GetAllByProfile(AppInfo.CurrentProfile.Id);
					AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = new TodoList(userTodos);

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

		public void Unexecute()
		{
		}
	}
}
