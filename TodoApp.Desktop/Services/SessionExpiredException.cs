namespace TodoApp.Desktop.Services;

public class SessionExpiredException : Exception
{
	public SessionExpiredException(string message)
		: base(message)
	{
	}
}
