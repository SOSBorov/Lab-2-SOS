using System;
using System.Linq;

namespace TodoList
{
    public class RemoveCommand : ICommand
    {
        public int Id { get; set; }
        public string? TodosFilePath { get; set; }
        private TodoItem _removedItem;

        public void Execute()
        {
            if (AppInfo.Todos == null) throw new InvalidOperationException("TodoList не инициализирован");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            _removedItem = AppInfo.Todos.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (_removedItem != null)
            {
                AppInfo.Todos.Remove(Id);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
            }
        }

        public void Unexecute()
        {
            if (_removedItem != null)
            {
                AppInfo.Todos.AddExistingItem(_removedItem);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
            }
        }
    }
}