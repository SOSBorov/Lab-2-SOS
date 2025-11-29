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
            if (AppInfo.Todos == null) throw new InvalidOperationException("TodoList не инициализирован");
            if (TodosFilePath == null) throw new InvalidOperationException("Путь к файлу задач не установлен");

            // Многострочный ввод пока не поддерживается для Undo/Redo
            if (Multiline)
            {
                AppInfo.Todos.ReadFromConsoleAndAddMultiline();
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
                // Не помещаем в Undo, так как сложно отменить
                return;
            }

            if (string.IsNullOrWhiteSpace(Text))
            {
                Console.WriteLine("Введите текст задачи.");
                return;
            }

            _addedItem = AppInfo.Todos.Add(Text!);
            Console.WriteLine("Задача добавлена.");

            FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
        }

        public void Unexecute()
        {
            if (_addedItem != null)
            {
                AppInfo.Todos.Remove(_addedItem.Id);
                FileManager.SaveTodos(AppInfo.Todos, TodosFilePath);
            }
        }
    }
}