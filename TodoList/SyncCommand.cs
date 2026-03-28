using System;
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

			AppInfo.DataStorage.SaveProfiles(AppInfo.AllProfiles);

			var todos = AppInfo.CurrentUserTodoList?.GetAllItems() ?? new();
			AppInfo.DataStorage.SaveTodos(AppInfo.CurrentProfile!.Id, todos);

			Console.WriteLine("Данные успешно отправлены на сервер.");
		}

		private void DoPull()
		{
			Console.WriteLine("Получение данных с сервера...");

			var profiles = AppInfo.DataStorage.LoadProfiles().ToList();
			AppInfo.AllProfiles = profiles;

			var todos = AppInfo.DataStorage.LoadTodos(AppInfo.CurrentProfile!.Id).ToList();
			AppInfo.UserTodos[AppInfo.CurrentProfile.Id] = new TodoList(todos);

			Console.WriteLine("Данные успешно получены с сервера.");
		}
	}
}
