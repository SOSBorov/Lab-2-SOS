using TodoApp.Models;

namespace TodoApp.Api.DTOs;

public class TodoItemResponse
{
	public int Id { get; set; }

	public string Text { get; set; } = string.Empty;

	public TodoStatus Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime LastUpdated { get; set; }

	public Guid ProfileId { get; set; }
}
