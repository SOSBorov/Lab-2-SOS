using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace TodoApp.Desktop.Services;

public class AuthApiClient
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private readonly HttpClient _http;

	public AuthApiClient(HttpClient http)
	{
		_http = http;
	}

	public async Task<AuthResponse> LoginAsync(string email, string password)
	{
		using HttpResponseMessage response = await _http.PostAsJsonAsync("/api/auth/login", new
		{
			Email = email,
			Password = password
		});

		return await ReadResponseAsync(response);
	}

	public async Task<AuthResponse> RegisterAsync(
		string username,
		string email,
		string password,
		string firstName,
		string? lastName,
		int birthYear)
	{
		using HttpResponseMessage response = await _http.PostAsJsonAsync("/api/auth/register", new
		{
			Username = username,
			Email = email,
			Password = password,
			FirstName = firstName,
			LastName = lastName,
			BirthYear = birthYear
		});

		return await ReadResponseAsync(response);
	}

	private static async Task<AuthResponse> ReadResponseAsync(HttpResponseMessage response)
	{
		if (!response.IsSuccessStatusCode)
		{
			throw await CreateApiExceptionAsync(response);
		}

		AuthResponse? result = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
		if (result == null)
		{
			throw new InvalidOperationException("API вернул пустой ответ авторизации.");
		}

		return result;
	}

	private static async Task<ApiException> CreateApiExceptionAsync(HttpResponseMessage response)
	{
		string message = await response.Content.ReadAsStringAsync();
		if (string.IsNullOrWhiteSpace(message))
		{
			message = response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => "Ошибка авторизации.",
				HttpStatusCode.Conflict => "Такой пользователь уже существует.",
				_ => $"Ошибка API: {(int)response.StatusCode}"
			};
		}

		return new ApiException(response.StatusCode, message.Trim('"'));
	}
}
