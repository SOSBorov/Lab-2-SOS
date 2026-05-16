using TodoApp.Models;

namespace TodoApp.Api.DTOs;

public class CreateTodoRequest
{
	public string Text { get; set; } = string.Empty;

	public TodoStatus Status { get; set; } = TodoStatus.NotStarted;

	public Guid ProfileId { get; set; }
}
