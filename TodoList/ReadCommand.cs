using System;
using System.IO;

namespace TodoList
{
    public class ReadCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public string? Filename { get; set; }

        public void Execute()
        {
            if (Filename == null)
            {
                Console.WriteLine("Укажите имя файла для чтения.");
                return;
            }

            if (!File.Exists(Filename))
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            var text = File.ReadAllText(Filename);
            TodoList?.Add(text);
            Console.WriteLine("Задача из файла добавлена.");
        }
    }
}
