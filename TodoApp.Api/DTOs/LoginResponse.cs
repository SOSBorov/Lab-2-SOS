namespace TodoApp.Api.DTOs;

public class LoginResponse
{
	public Guid Id { get; set; }

	public string Login { get; set; } = string.Empty;

	public string FirstName { get; set; } = string.Empty;

	public string? LastName { get; set; }

	public int BirthYear { get; set; }
}
