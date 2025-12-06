using System.Windows.Input;
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
            if (AppInfo.CurrentUserTodoList == null) throw new InvalidOperationException("TodoList не инициализирован для текущего пользователя");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToUpdate = AppInfo.CurrentUserTodoList.GetById(Id);

            if (itemToUpdate != null)
            {
                _previousStatus = itemToUpdate.Status;
                AppInfo.CurrentUserTodoList.SetStatus(Id, NewStatus);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }

        public void Unexecute()
        {
            AppInfo.CurrentUserTodoList.SetStatus(Id, _previousStatus);
            FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
        }
    }
}