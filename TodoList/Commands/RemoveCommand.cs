using System.Windows.Input;
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
            if (AppInfo.CurrentUserTodoList == null) throw new InvalidOperationException("TodoList не инициализирован для текущего пользователя");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            _removedItem = AppInfo.CurrentUserTodoList.GetById(Id);

            if (_removedItem != null)
            {
                AppInfo.CurrentUserTodoList.Remove(Id);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }

        public void Unexecute()
        {
            if (_removedItem != null)
            {
                AppInfo.CurrentUserTodoList.AddExistingItem(_removedItem);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }
    }
}