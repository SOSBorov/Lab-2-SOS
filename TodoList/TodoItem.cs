using System;
using TodoList.Interfaces;

namespace TodoList;

public class TodoItem
{
	private readonly IClock _clock;

	public TodoItem()
		: this(new SystemClock())
	{
	}

	public TodoItem(IClock clock)
	{
		_clock = clock;
		var now = _clock.Now;
		CreatedAt = now;
		LastUpdated = now;
	}

	public int Id { get; set; }
	public string Text { get; set; } = string.Empty;
	public TodoStatus Status { get; set; } = TodoStatus.NotStarted;
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdated { get; set; }

	public override string ToString()
	{
		return $"({Status}) {Text} обновлено {LastUpdated:dd.MM.yyyy HH:mm}";
	}
}
