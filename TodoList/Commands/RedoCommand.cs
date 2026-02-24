using System;

namespace TodoList
{
	public class RedoCommand : ICommand
	{
		public void Execute()
		{
			if (AppInfo.RedoStack.Count == 0)
			{
				throw new InvalidOperationException("Нет действий для повтора.");
			}

			ICommand commandToRedo = AppInfo.RedoStack.Pop();
			commandToRedo.Execute();
			AppInfo.UndoStack.Push(commandToRedo);
			Console.WriteLine("Последнее отмененное действие повторено.");
		}
	}
}