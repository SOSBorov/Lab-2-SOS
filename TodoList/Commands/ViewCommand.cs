using System;
using TodoList.Exceptions;

namespace TodoList
{
	public class ViewCommand : ICommand
	{
		public bool ShowIndex { get; set; }
		public bool ShowStatus { get; set; }
		public bool ShowUpdateDate { get; set; }
		public bool ShowAll { get; set; }

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
			{
				throw new AuthenticationException("Список задач не найден. Пожалуйста, войдите в профиль.");
			}

			AppInfo.CurrentUserTodoList.ViewCustom(ShowIndex, ShowStatus, ShowUpdateDate, ShowAll);
		}

		public void Unexecute() { }
	}
}