using System;

namespace TodoList
{
	public class UpdateCommand : ICommand, IUndo
	{
		public int Id { get; set; }
		public string NewText { get; set; } = "";
		public string? TodosFilePath { get; set; }
		private string? _previousText;

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			var itemToUpdate = AppInfo.CurrentUserTodoList.GetById(Id);

			if (itemToUpdate != null)
			{
				_previousText = itemToUpdate.Text;
			}

			AppInfo.CurrentUserTodoList.Update(Id, NewText);
		}

		public void Unexecute()
		{
			if (_previousText != null && AppInfo.CurrentUserTodoList != null && TodosFilePath != null)
			{
				AppInfo.CurrentUserTodoList.Update(Id, _previousText);
				FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
			}
		}
	}
}