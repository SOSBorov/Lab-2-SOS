using System;
using System.Linq;

namespace TodoList
{
    public class UpdateCommand : ICommand
    {
        public int Id { get; set; }
        public string NewText { get; set; } = "";
        public string? TodosFilePath { get; set; }
        private string _previousText;

        public void Execute()
        {
            if (AppInfo.Todos == null) throw new InvalidOperationException("TodoList не инициализирован");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToUpdate = AppInfo.Todos.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (itemToUpdate != null)
            {
                _previousText = itemToUpdate.Text;
                AppInfo.Todos.Update(Id, NewText);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
            }
        }

        public void Unexecute()
        {
            if (_previousText != null)
            {
                AppInfo.Todos.Update(Id, _previousText);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
            }
        }
    }
}