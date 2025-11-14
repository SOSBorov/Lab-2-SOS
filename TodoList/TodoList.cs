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
            item.Status = TodoStatus.Completed;
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

            foreach (var item in this)
            {
                var outputBuilder = new StringBuilder();

                if (showAll || showIndex)
                {
                    outputBuilder.Append($"[{item.Id}] ");
                }

                if (showAll || showStatus)
                {
                    outputBuilder.Append(TodoItem.GetStatusSymbol(item.Status) + " ");
                }

                outputBuilder.Append(item.Text);

                if (showAll || showUpdateDate)
                {
                    outputBuilder.Append($" обновлено {item.LastUpdated:dd.MM.yyyy HH:mm}");
                }

                Console.WriteLine(outputBuilder.ToString());
            }
        }

        public List<TodoItem> GetAllItems()
        {
            return new List<TodoItem>(_items);
        }

        public void AddLoadedItem(TodoItem item)
        {
            _items.Add(item);
            if (item.Id >= _nextId)
            {
                _nextId = item.Id + 1;
            }
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