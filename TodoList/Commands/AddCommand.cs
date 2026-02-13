using System;

namespace TodoList
{
    public class AddCommand : ICommand, IUndo
    {
        public string? Text { get; set; }
        public bool Multiline { get; set; }
        public string? TodosFilePath { get; set; }
        private TodoItem? _addedItem;

        public void Execute()
        {
            if (AppInfo.CurrentUserTodoList == null) throw new InvalidOperationException("TodoList не инициализирован для текущего пользователя");

            if (Multiline)
            {
                AppInfo.CurrentUserTodoList.ReadFromConsoleAndAddMultiline();
                return;
            }

            if (string.IsNullOrWhiteSpace(Text))
            {
                Console.WriteLine("Введите текст задачи.");
                return;
            }

            _addedItem = AppInfo.CurrentUserTodoList.Add(Text);
        }

        public void Unexecute()
        {
            if (_addedItem != null && AppInfo.CurrentUserTodoList != null && TodosFilePath != null)
            {
                AppInfo.CurrentUserTodoList.Remove(_addedItem.Id);
                FileManager.SaveTodos(AppInfo.CurrentUserTodoList, TodosFilePath);
            }
        }
    }
}