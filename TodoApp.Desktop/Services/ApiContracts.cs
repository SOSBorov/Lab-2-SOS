namespace TodoApp.Desktop.Services;

public class AuthResponse
{
	public int Id { get; set; }

	public string Username { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string Role { get; set; } = string.Empty;

	public string Token { get; set; } = string.Empty;

	public Guid ProfileId { get; set; }

	public string FirstName { get; set; } = string.Empty;

	public string? LastName { get; set; }

	public int BirthYear { get; set; }
}

public class ApiErrorResponse
{
	public string? Message { get; set; }
}
