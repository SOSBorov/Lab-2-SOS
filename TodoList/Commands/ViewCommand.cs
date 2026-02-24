using System;

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
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			AppInfo.CurrentUserTodoList.ViewCustom(ShowIndex, ShowStatus, ShowUpdateDate, ShowAll);
		}
	}
}