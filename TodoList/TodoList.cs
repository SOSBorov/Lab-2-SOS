using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoList
{
    public class TodoList
    {
        public event Action<TodoItem>? OnTodoAdded;
        public event Action<TodoItem>? OnTodoDeleted;
        public event Action<TodoItem>? OnTodoUpdated;
        public event Action<TodoItem>? OnStatusChanged;

        private readonly List<TodoItem> _items = new();
        private int _nextId = 1;
        private const int TRUNCATE_LENGTH = 70;

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

        public TodoItem Add(string text)
        {
            var newItem = new TodoItem { Id = _nextId++, Text = text };
            _items.Add(newItem);

            OnTodoAdded?.Invoke(newItem);

            return newItem;
        }

        internal void AddExistingItem(TodoItem item)
        {
            if (!_items.Any(i => i.Id == item.Id))
            {
                _items.Add(item);
                _items.Sort((a, b) => a.Id.CompareTo(b.Id));
            }
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
                var removedItem = _items[indexToRemove];
                _items.RemoveAt(indexToRemove);
                Console.WriteLine("Задача удалена.");

                OnTodoDeleted?.Invoke(removedItem);
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

                OnStatusChanged?.Invoke(_items[indexToUpdate]);
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

                OnTodoUpdated?.Invoke(_items[indexToUpdate]);
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
                var prefixBuilder = new StringBuilder();
                if (showAll || showIndex) prefixBuilder.Append($"[{item.Id}] ");
                if (showAll || showStatus) prefixBuilder.Append($"({item.Status}) ");
                string prefix = prefixBuilder.ToString();

                string textToDisplay;
                if (showAll)
                {
                    int availableWidth = Console.WindowWidth - prefix.Length - 1;
                    if (availableWidth < 10) availableWidth = 10;
                    string padding = new string(' ', prefix.Length);
                    textToDisplay = WrapText(item.Text, availableWidth, padding);
                }
                else
                {
                    string firstLine = item.Text.Split('\n').FirstOrDefault() ?? string.Empty;
                    bool isMultiline = item.Text.Contains('\n') && !string.IsNullOrWhiteSpace(item.Text.Replace(firstLine, ""));

                    if (firstLine.Length > TRUNCATE_LENGTH)
                    {
                        textToDisplay = firstLine.Substring(0, TRUNCATE_LENGTH) + "...";
                    }
                    else
                    {
                        textToDisplay = firstLine;
                    }

                    if (isMultiline)
                    {
                        textToDisplay += " [...]";
                    }
                }

                Console.Write(prefix);
                Console.Write(textToDisplay);

                if (showAll || showUpdateDate) Console.Write($" (обновлено {item.LastUpdated:dd.MM.yyyy HH:mm})");

                Console.WriteLine();
            }
        }

        private string WrapText(string text, int maxWidth, string padding)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var resultBuilder = new StringBuilder();
            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                var words = line.Split(' ');
                var currentLine = new StringBuilder();

                foreach (var word in words)
                {
                    if (currentLine.Length + word.Length + 1 > maxWidth)
                    {
                        resultBuilder.AppendLine(currentLine.ToString().TrimEnd());
                        currentLine.Clear().Append(padding);
                    }
                    currentLine.Append(word + " ");
                }
                resultBuilder.Append(currentLine.ToString().TrimEnd());

                if (i < lines.Length - 1)
                {
                    resultBuilder.AppendLine();
                    resultBuilder.Append(padding);
                }
            }
            return resultBuilder.ToString();
        }

        public List<TodoItem> GetAllItems()
        {
            return new List<TodoItem>(_items);
        }

        public TodoItem? GetById(int id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }
    }
}