using System;

namespace TodoList
{
    public class UndoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.UndoStack.Count > 0)
            {
                ICommand lastCommand = AppInfo.UndoStack.Pop();

                if (lastCommand is IUndo undoableCommand)
                {
                    undoableCommand.Unexecute();
                    AppInfo.RedoStack.Push(lastCommand);
                    Console.WriteLine("Последнее действие отменено.");
                }
            }
            else
            {
                Console.WriteLine("Нет действий для отмены.");
            }
        }
    }
}