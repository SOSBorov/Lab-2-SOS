using System;

namespace TodoList
{
    public class AddCommand : ICommand
    {
        public string? Text { get; set; }
        public bool Multiline { get; set; }
        public TodoList? TodoList { get; set; }
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null)
                throw new InvalidOperationException("TodoList не установлен");
            if (TodosFilePath == null)
                throw new InvalidOperationException("Путь к файлу задач не установлен");

            bool addedSuccessfully = false; 

            if (Multiline)
            {
                TodoList.ReadFromConsoleAndAddMultiline();
                addedSuccessfully = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    Console.WriteLine("Введите текст задачи.");
                    return;
                }
                TodoList.Add(Text!);
                Console.WriteLine("Задача добавлена.");
                addedSuccessfully = true;
            }

            if (addedSuccessfully)
            {
                FileManager.SaveTodos(TodoList, TodosFilePath);
            }
        }
    }
}