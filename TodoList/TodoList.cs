using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoList
{
    public class TodoList
    {
        private readonly List<TodoItem> _items = new();
        private int _nextId = 1;

        public TodoList() { }

        public TodoList(List<TodoItem> items)
        {
            _items = items;
            if (_items.Any())
            {
                _nextId = _items.Max(item => item.Id) + 1;
            }
        }

        public int Count => _items.Count;

        public TodoItem this[int index] => _items[index];

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
            int indexToRemove = _items.FindIndex(item => item.Id == id);

            if (indexToRemove != -1)
            {
                _items.RemoveAt(indexToRemove);
                Console.WriteLine("Задача удалена.");
            }
            else
            {
                Console.WriteLine("Задача с таким ID не найдена.");
            }
        }

        public void SetStatus(int id, TodoStatus newStatus)
        {
            int indexToUpdate = _items.FindIndex(item => item.Id == id);

            if (indexToUpdate != -1)
            {
                _items[indexToUpdate].Status = newStatus;
                _items[indexToUpdate].LastUpdated = DateTime.Now;
                Console.WriteLine($"Статус задачи #{id} изменен на '{newStatus}'.");
            }
            else
            {
                Console.WriteLine("Задача с таким ID не найдена.");
            }
        }

        public void Update(int id, string newText)
        {
            int indexToUpdate = _items.FindIndex(item => item.Id == id);

            if (indexToUpdate != -1)
            {
                _items[indexToUpdate].Text = newText;
                _items[indexToUpdate].LastUpdated = DateTime.Now;
                Console.WriteLine("Задача обновлена.");
            }
            else
            {
                Console.WriteLine("Задача не найдена.");
            }
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
                var outputBuilder = new StringBuilder();
                if (showAll || showIndex) outputBuilder.Append($"[{item.Id}] ");
                if (showAll || showStatus) outputBuilder.Append($"({item.Status}) ");
                outputBuilder.Append(item.Text);
                if (showAll || showUpdateDate) outputBuilder.Append($" обновлено {item.LastUpdated:dd.MM.yyyy HH:mm}");
                Console.WriteLine(outputBuilder.ToString());
            }
        }

        public List<TodoItem> GetAllItems()
        {
            return new List<TodoItem>(_items);
        }
    }
}