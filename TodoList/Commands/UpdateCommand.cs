using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class UpdateCommand : ICommand, IUndo
	{
		public int Id { get; set; }
		public string NewText { get; set; } = "";
		private string? _previousText;

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			var itemToUpdate = AppInfo.CurrentUserTodoList.GetById(Id);

			if (itemToUpdate != null)
			{
				_previousText = itemToUpdate.Text;
				AppInfo.CurrentUserTodoList.Update(Id, NewText);
			}
		}

		public void Unexecute()
		{
			if (_previousText != null && AppInfo.CurrentProfile != null && AppInfo.CurrentUserTodoList != null)
			{
				AppInfo.CurrentUserTodoList.Update(Id, _previousText);
				AppInfo.DataStorage.SaveTodos(AppInfo.CurrentProfile.Id, AppInfo.CurrentUserTodoList.GetAllItems());
			}
		}
	}
}