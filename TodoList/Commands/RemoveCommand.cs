using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class RemoveCommand : ICommand, IUndo
	{
		public int Id { get; set; }
		private TodoItem? _removedItem;

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			_removedItem = AppInfo.CurrentUserTodoList.GetById(Id);
			AppInfo.CurrentUserTodoList.Remove(Id);
		}

		public void Unexecute()
		{
			if (_removedItem != null && AppInfo.CurrentProfile != null && AppInfo.CurrentUserTodoList != null)
			{
				AppInfo.CurrentUserTodoList.AddExistingItem(_removedItem);
				AppInfo.DataStorage.SaveTodos(AppInfo.CurrentProfile.Id, AppInfo.CurrentUserTodoList.GetAllItems());
			}
		}
	}
}