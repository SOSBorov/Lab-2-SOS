using TodoApp.Models;

namespace TodoApp.Api.DTOs;

public class UpdateTodoRequest
{
	public string Text { get; set; } = string.Empty;

	public TodoStatus Status { get; set; }
}
