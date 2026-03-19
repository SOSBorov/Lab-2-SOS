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
			if (itemToUpdate == null)
				throw new TaskNotFoundException($"Задача с ID '{Id}' не найдена.");

			_previousText = itemToUpdate.Text;

			AppInfo.CurrentUserTodoList.Update(Id, NewText);

			Console.WriteLine("Задача обновлена.");
		}

		public void Unexecute()
		{
			if (_previousText != null && AppInfo.CurrentUserTodoList != null)
			{
				AppInfo.CurrentUserTodoList.Update(Id, _previousText);
			}
		}
	}
}