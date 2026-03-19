using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class AddCommand : ICommand, IUndo
	{
		public string? Text { get; set; }
		public bool Multiline { get; set; }
		private TodoItem? _addedItem;

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			if (Multiline)
			{
				_addedItem = AppInfo.CurrentUserTodoList.ReadFromConsoleAndAddMultiline();
			}
			else
			{
				if (string.IsNullOrWhiteSpace(Text))
				{
					throw new InvalidArgumentException("Текст задачи не может быть пустым.");
				}
				_addedItem = AppInfo.CurrentUserTodoList.Add(Text);
			}

			if (_addedItem != null)
			{
				Console.WriteLine("Задача успешно добавлена!");
			}
		}

		public void Unexecute()
		{
			if (_addedItem != null && AppInfo.CurrentUserTodoList != null)
			{
				AppInfo.CurrentUserTodoList.Remove(_addedItem.Id);
			}
		}
	}
}