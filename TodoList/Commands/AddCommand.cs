using System.Windows.Input;
using System;

namespace TodoList
{
    public class AddCommand : ICommand
    {
        public string? Text { get; set; }
        public bool Multiline { get; set; }
        public string? TodosFilePath { get; set; }
        private TodoItem _addedItem;

        public void Execute()
        {
            if (AppInfo.CurrentUserTodoList == null) throw new InvalidOperationException("TodoList не инициализирован для текущего пользователя");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            if (Multiline)
            {
                AppInfo.CurrentUserTodoList.ReadFromConsoleAndAddMultiline();
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
                return;
            }

            if (string.IsNullOrWhiteSpace(Text))
            {
                Console.WriteLine("Введите текст задачи.");
                return;
            }

            _addedItem = AppInfo.CurrentUserTodoList.Add(Text!);
            Console.WriteLine("Задача добавлена.");

            FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
        }

        public void Unexecute()
        {
            if (_addedItem != null)
            {
                AppInfo.CurrentUserTodoList.Remove(_addedItem.Id);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }
    }
}