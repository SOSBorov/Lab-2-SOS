using System;
using System.Linq;

namespace TodoList
{
    public class StatusCommand : ICommand
    {
        public int Id { get; set; }
        public TodoStatus NewStatus { get; set; }
        public string? TodosFilePath { get; set; }
        private TodoStatus _previousStatus;

        public void Execute()
        {
            if (AppInfo.Todos == null) throw new InvalidOperationException("TodoList не инициализирован");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToUpdate = AppInfo.Todos.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (itemToUpdate != null)
            {
                _previousStatus = itemToUpdate.Status;
                AppInfo.Todos.SetStatus(Id, NewStatus);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
            }
        }

        public void Unexecute()
        {
            AppInfo.Todos.SetStatus(Id, _previousStatus);
            FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
        }
    }
}