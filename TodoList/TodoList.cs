using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoList.Exceptions;

namespace TodoList
{
	public class TodoList
	{
		public event Action<TodoItem>? OnTodoAdded;
		public event Action<TodoItem>? OnTodoDeleted;
		public event Action<TodoItem>? OnTodoUpdated;
		public event Action<TodoItem>? OnStatusChanged;

		private readonly List<TodoItem> _items;
		private int _nextId = 1;
		private const int TRUNCATE_LENGTH = 70;

		public TodoList()
		{
			_items = new List<TodoItem>();
		}

		public TodoList(List<TodoItem> items)
		{
			_items = items;
			if (_items.Any())
			{
				_nextId = _items.Max(item => item.Id) + 1;
			}
		}

		public int Count => _items.Count;

		public TodoItem Add(string text)
		{
			var newItem = new TodoItem
			{
				Id = _nextId++,
				Text = text,
				LastUpdated = DateTime.Now
			};

			_items.Add(newItem);
			OnTodoAdded?.Invoke(newItem);
			return newItem;
		}

		public TodoItem ReadFromConsoleAndAddMultiline()
		{
			Console.WriteLine("Введите текст задачи. Пустая строка — завершение ввода:");

			var lines = new List<string>();
			while (true)
			{
				string? line = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(line))
					break;

				lines.Add(line);
			}

			string fullText = string.Join("\n", lines);
			return Add(fullText);
		}

		internal void AddExistingItem(TodoItem item)
		{
			if (!_items.Any(i => i.Id == item.Id))
			{
				_items.Add(item);
				_items.Sort((a, b) => a.Id.CompareTo(b.Id));
				OnTodoAdded?.Invoke(item);
			}
		}

		public void Remove(int id)
		{
			var item = _items.FirstOrDefault(i => i.Id == id);
			if (item != null)
			{
				_items.Remove(item);
				OnTodoDeleted?.Invoke(item);
			}
			else
			{
				throw new TaskNotFoundException($"Задача с ID '{id}' не найдена.");
			}
		}

		public void SetStatus(int id, TodoStatus newStatus)
		{
			var item = _items.FirstOrDefault(i => i.Id == id);
			if (item != null)
			{
				item.Status = newStatus;
				item.LastUpdated = DateTime.Now;

				OnStatusChanged?.Invoke(item);
			}
			else
			{
				throw new TaskNotFoundException($"Задача с ID '{id}' не найдена.");
			}
		}

		public void Update(int id, string newText)
		{
			var item = _items.FirstOrDefault(i => i.Id == id);
			if (item != null)
			{
				item.UpdateText(newText);
				OnTodoUpdated?.Invoke(item);
			}
			else
			{
				throw new TaskNotFoundException($"Задача с ID '{id}' не найдена.");
			}
		}

		public List<TodoItem> GetAllItems() => new List<TodoItem>(_items);

		public TodoItem? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);

		public void ViewCustom(bool showIndex, bool showStatus, bool showUpdateDate, bool showAll)
		{
			if (Count == 0)
			{
				Console.WriteLine("Список задач пуст.");
				return;
			}

			bool noFlags = !showIndex && !showStatus && !showUpdateDate && !showAll;

			foreach (var item in _items)
			{
				if (noFlags)
				{
					Console.WriteLine(item.Text);
					continue;
				}

				var prefixBuilder = new StringBuilder();
				if (showAll || showIndex) prefixBuilder.Append($"[{item.Id}] ");
				if (showAll || showStatus) prefixBuilder.Append($"({item.Status}) ");
				string prefix = prefixBuilder.ToString();

				string textToDisplay;
				if (showAll)
				{
					int availableWidth = Console.WindowWidth > prefix.Length + 1
						? Console.WindowWidth - prefix.Length - 1
						: 10;

					string padding = new string(' ', prefix.Length);
					textToDisplay = WrapText(item.Text, availableWidth, padding);
				}
				else
				{
					string firstLine = item.Text.Split('\n').FirstOrDefault() ?? string.Empty;
					bool isMultiline = item.Text.Contains('\n') &&
									   !string.IsNullOrWhiteSpace(item.Text.Replace(firstLine, ""));

					textToDisplay = firstLine.Length > TRUNCATE_LENGTH
						? firstLine.Substring(0, TRUNCATE_LENGTH) + "..."
						: firstLine;

					if (isMultiline)
					{
						textToDisplay += " [...]";
					}
				}

				Console.Write(prefix);
				Console.Write(textToDisplay);
				if (showAll || showUpdateDate)
					Console.Write($" (обновлено {item.LastUpdated:dd.MM.yyyy HH:mm})");
				Console.WriteLine();
			}
		}

		private string WrapText(string text, int maxWidth, string padding)
		{
			if (string.IsNullOrEmpty(text) || maxWidth <= 0) return string.Empty;

			var resultBuilder = new StringBuilder();
			string[] lines = text.Split('\n');

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].TrimEnd();
				var words = line.Split(' ');
				var currentLine = new StringBuilder();

				foreach (var word in words)
				{
					if (currentLine.Length > 0 && currentLine.Length + word.Length + 1 > maxWidth)
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
	}
}
