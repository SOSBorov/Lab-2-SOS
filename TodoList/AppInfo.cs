using System;
using System.Collections.Generic;

namespace TodoList
{
	public static class AppInfo
	{
		public static IDataStorage DataStorage { get; set; } = null!;

		public static Dictionary<Guid, TodoList> UserTodos { get; set; } = new();
		public static List<Profile> AllProfiles { get; set; } = new();
		public static Profile? CurrentProfile { get; set; }

		public static Stack<ICommand> UndoStack { get; set; } = new();
		public static Stack<ICommand> RedoStack { get; set; } = new();

		public static TodoList? CurrentUserTodoList
		{
			get
			{
				if (CurrentProfile == null || !UserTodos.ContainsKey(CurrentProfile.Id))
				{
					return null;
				}
				return UserTodos[CurrentProfile.Id];
			}
		}
	}
}