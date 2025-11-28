using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoList
{
    public class TodoList : IEnumerable<TodoItem>
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

        public void Remove(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                Console.WriteLine("Задача удалена.");
            }
            else
            {
                Console.WriteLine("Задачи с таким номером не существует.");
            }
        }

        public void SetStatus(int index, TodoStatus newStatus)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items[index].Status = newStatus;
                _items[index].LastUpdated = DateTime.Now;
                Console.WriteLine($"Статус задачи #{_items[index].Id} изменен на '{newStatus}'.");
            }
            else
            {
                Console.WriteLine("Задачи с таким номером не существует.");
            }
        }

        public void Update(int index, string newText)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items[index].Text = newText;
                _items[index].LastUpdated = DateTime.Now;
                Console.WriteLine("Задача обновлена.");
            }
            else
            {
                Console.WriteLine("Задачи с таким номером не существует.");
            }
        }

        public void ViewCustom(bool showIndex, bool showStatus, bool showUpdateDate, bool showAll)
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            foreach (var item in this)
            {
                var outputBuilder = new StringBuilder();
                if (showAll || showIndex) outputBuilder.Append($"[{item.Id}] ");
                if (showAll || showStatus) outputBuilder.Append($"({item.Status}) ");

                string formattedText = item.Text.Replace("\r", "").Replace("\n", " | ");
                outputBuilder.Append(formattedText);

                if (showAll || showUpdateDate) outputBuilder.Append($" обновлено {item.LastUpdated:dd.MM.yyyy HH:mm}");
                Console.WriteLine(outputBuilder.ToString());
            }
        }

        public List<TodoItem> GetAllItems()
        {
            return new List<TodoItem>(_items);
        }

        public IEnumerator<TodoItem> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}