using System;
using TodoList.Interfaces;

namespace TodoList
{
	public class TodoItem
	{
		private readonly IClock? _clock;

		public TodoItem()
		{
			CreatedAt = DateTime.Now;
			LastUpdated = CreatedAt;
		}

		public TodoItem(IClock clock)
		{
			_clock = clock;
			CreatedAt = clock.Now;
			LastUpdated = clock.Now;
		}

		public int Id { get; set; }

		public string Text { get; set; } = string.Empty;

		public TodoStatus Status { get; set; } = TodoStatus.NotStarted;

		public DateTime CreatedAt { get; set; }

		public DateTime LastUpdated { get; set; }

		public Guid ProfileId { get; set; }

		public Profile? Profile { get; set; }

		public void UpdateText(string newText)
		{
			Text = newText;
			LastUpdated = _clock?.Now ?? DateTime.Now;
		}

		public override string ToString()
		{
			return $"({Status}) {Text} обновлено {LastUpdated:dd.MM.yyyy HH:mm}";
		}
	}
}
