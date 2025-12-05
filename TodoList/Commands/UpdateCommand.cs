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
            if (AppInfo.CurrentUserTodoList == null) throw new InvalidOperationException("TodoList не инициализирован для текущего пользователя");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToUpdate = AppInfo.CurrentUserTodoList.GetById(Id);

            if (itemToUpdate != null)
            {
                _previousText = itemToUpdate.Text;
                AppInfo.CurrentUserTodoList.Update(Id, NewText);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }

        public void Unexecute()
        {
            if (_previousText != null)
            {
                AppInfo.CurrentUserTodoList.Update(Id, _previousText);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }
    }
}