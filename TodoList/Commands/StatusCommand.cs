using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class StatusCommand : ICommand, IUndo
	{
		public int Id { get; set; }
		public TodoStatus NewStatus { get; set; }
		private TodoStatus _previousStatus;

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			var itemToUpdate = AppInfo.CurrentUserTodoList.GetById(Id);

			if (itemToUpdate != null)
			{
				_previousStatus = itemToUpdate.Status;
				AppInfo.CurrentUserTodoList.SetStatus(Id, NewStatus);
			}
		}

		public void Unexecute()
		{
			if (AppInfo.CurrentProfile != null && AppInfo.CurrentUserTodoList != null)
			{
				AppInfo.CurrentUserTodoList.SetStatus(Id, _previousStatus);
				AppInfo.DataStorage.SaveTodos(AppInfo.CurrentProfile.Id, AppInfo.CurrentUserTodoList.GetAllItems());
			}
		}
	}
}