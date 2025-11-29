using System;

namespace TodoList
{
    public class RedoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.RedoStack.Count > 0)
            {
                ICommand commandToRedo = AppInfo.RedoStack.Pop();
                commandToRedo.Execute();
                Console.WriteLine("Последнее отмененное действие повторено.");
            }
            else
            {
                Console.WriteLine("Нет действий для повтора.");
            }
        }

        public void Unexecute() { }
    }
}