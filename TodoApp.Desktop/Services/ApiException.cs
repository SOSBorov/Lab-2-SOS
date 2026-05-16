using System.Net;

namespace TodoApp.Desktop.Services;

public class ApiException : Exception
{
	public ApiException(HttpStatusCode statusCode, string message)
		: base(message)
	{
		StatusCode = statusCode;
	}

	public HttpStatusCode StatusCode { get; }
}
