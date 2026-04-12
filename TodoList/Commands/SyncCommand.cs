using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoList
{
	public class SyncCommand : ICommand
	{
		public bool Push { get; set; }
		public bool Pull { get; set; }

		public void Execute()
		{
			if (!Push && !Pull)
			{
				Console.WriteLine("Использование: sync --push или sync --pull");
				return;
			}

			if (AppInfo.CurrentProfile == null)
			{
				Console.WriteLine("Вы не авторизованы.");
				return;
			}

			if (Push)
				DoPush();

			if (Pull)
				DoPull();
		}

		private void DoPush()
		{
			Console.WriteLine("Отправка данных на сервер...");

			var currentProfile = AppInfo.CurrentProfile!;
			var storage = CreateServerStorage();
			storage.SaveProfiles(AppInfo.AllProfiles);

			var todos = AppInfo.CurrentUserTodoList?.GetAllItems() ?? AppInfo.TodoRepository.GetAllByProfile(currentProfile.Id);
			storage.SaveTodos(currentProfile.Id, todos);

			Console.WriteLine("Данные успешно отправлены на сервер.");
		}

		private void DoPull()
		{
			Console.WriteLine("Получение данных с сервера...");

			var currentProfile = AppInfo.CurrentProfile!;
			var storage = CreateServerStorage();
			var profiles = storage.LoadProfiles().ToList();
			ReplaceLocalProfiles(profiles);
			AppInfo.AllProfiles = profiles;

			var todos = storage.LoadTodos(currentProfile.Id).ToList();
			ReplaceLocalTodos(currentProfile.Id, todos);
			AppInfo.UserTodos[currentProfile.Id] = new TodoList(todos);

			Console.WriteLine("Данные успешно получены с сервера.");
		}

		private static ApiDataStorage CreateServerStorage()
		{
			byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
			byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

			Console.Write("Введите адрес API сервера (Enter для http://localhost:5000/): ");
			string? baseUrl = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(baseUrl))
			{
				baseUrl = "http://localhost:5000/";
			}

			return new ApiDataStorage(baseUrl, key, iv);
		}

		private static void ReplaceLocalProfiles(List<Profile> profiles)
		{
			var existingProfiles = AppInfo.ProfileRepository.GetAll();

			foreach (var existingProfile in existingProfiles)
			{
				AppInfo.ProfileRepository.Delete(existingProfile.Id);
			}

			foreach (var profile in profiles)
			{
				AppInfo.ProfileRepository.Add(profile);
			}
		}

		private static void ReplaceLocalTodos(Guid profileId, List<TodoItem> todos)
		{
			var existingTodos = AppInfo.TodoRepository.GetAllByProfile(profileId);

			foreach (var existingTodo in existingTodos)
			{
				AppInfo.TodoRepository.Delete(existingTodo.Id);
			}

			foreach (var todo in todos)
			{
				AppInfo.TodoRepository.Add(todo);
			}
		}
	}
}
