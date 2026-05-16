namespace TodoApp.Api.DTOs;

public class RegisterRequest
{
	public string Login { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public string FirstName { get; set; } = string.Empty;

	public string? LastName { get; set; }

	public int BirthYear { get; set; }
}
