using System;

namespace TodoList
{
    public class AddCommand : ICommand
    {
        public string? Text { get; set; }
        public bool Multiline { get; set; }
        public TodoList? TodoList { get; set; }

        public void Execute()
        {
            if (TodoList == null)
                throw new InvalidOperationException("TodoList не установлен");

            if (Multiline)
            {
                TodoList.ReadFromConsoleAndAddMultiline();
                return;
            }

            if (string.IsNullOrWhiteSpace(Text))
            {
                Console.WriteLine("Введите текст задачи.");
                return;
            }

            TodoList.Add(Text!);
            Console.WriteLine("Задача добавлена.");
        }
    }
}
