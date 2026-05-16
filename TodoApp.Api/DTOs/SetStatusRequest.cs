using TodoApp.Models;

namespace TodoApp.Api.DTOs;

public class SetStatusRequest
{
	public TodoStatus Status { get; set; }
}
