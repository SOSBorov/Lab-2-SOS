using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoList
{
    public class TodoList
    {
        private readonly List<TodoItem> _items = new();
        private int _nextId = 1;

        public void Add(string text)
        {
            _items.Add(new TodoItem { Id = _nextId++, Text = text });
        }

        public void ReadFromConsoleAndAddMultiline()
        {
            Console.WriteLine("Введите задачу (введите !end чтобы завершить ввод):");
            string line;
            var builder = new System.Text.StringBuilder();
            while ((line = Console.ReadLine() ?? "") != "!end")
            {
                builder.AppendLine(line);
            }
            Add(builder.ToString().Trim());
            Console.WriteLine("Многострочная задача добавлена.");
        }

        public void Remove(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                Console.WriteLine("Задача с таким ID не найдена.");
                return;
            }
            _items.Remove(item);
            Console.WriteLine("Задача удалена.");
        }

        public void MarkComplete(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                Console.WriteLine("Задача с таким ID не найдена.");
                return;
            }
            item.IsCompleted = true;
            item.LastUpdated = DateTime.Now;
            Console.WriteLine("Задача отмечена как выполненная.");
        }

        public void Update(int id, string newText)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                Console.WriteLine("Задача не найдена.");
                return;
            }
            item.Text = newText;
            item.LastUpdated = DateTime.Now;
            Console.WriteLine("Задача обновлена.");
        }

        public void ViewCustom(bool showIndex, bool showStatus, bool showUpdateDate, bool showAll)
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            foreach (var item in _items)
            {
                string output = item.ToString();

                if (showIndex || showAll)
                    output = $"[{item.Id}] " + output;

                Console.WriteLine(output);
            }
        }
    }
}
