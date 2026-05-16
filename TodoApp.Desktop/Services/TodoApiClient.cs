using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TodoApp.Models;

namespace TodoApp.Desktop.Services;

public class TodoApiClient
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private readonly HttpClient _http;

	public TodoApiClient(HttpClient http)
	{
		_http = http;
	}

	public void SetToken(string? token)
	{
		_http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
			? null
			: new AuthenticationHeaderValue("Bearer", token);
	}

	public async Task<List<TodoItem>> GetAllAsync()
	{
		using HttpResponseMessage response = await _http.GetAsync("/api/todos");
		if (!response.IsSuccessStatusCode)
		{
			throw await CreateApiExceptionAsync(response);
		}

		List<TodoItem>? items = await response.Content.ReadFromJsonAsync<List<TodoItem>>(JsonOptions);
		return items ?? [];
	}

	public async Task<TodoItem> CreateAsync(string text, TodoStatus status, Guid profileId)
	{
		using HttpResponseMessage response = await _http.PostAsJsonAsync("/api/todos", new
		{
			Text = text,
			Status = status,
			ProfileId = profileId
		});

		return await ReadTodoAsync(response);
	}

	public async Task<TodoItem> UpdateAsync(int id, string text, TodoStatus status)
	{
		using HttpResponseMessage response = await _http.PutAsJsonAsync($"/api/todos/{id}", new
		{
			Text = text,
			Status = status
		});

		return await ReadTodoAsync(response);
	}

	public async Task<TodoItem> SetStatusAsync(int id, TodoStatus status)
	{
		using HttpResponseMessage response = await _http.PatchAsJsonAsync($"/api/todos/{id}/status", new
		{
			Status = status
		});

		return await ReadTodoAsync(response);
	}

	public async Task DeleteAsync(int id)
	{
		using HttpResponseMessage response = await _http.DeleteAsync($"/api/todos/{id}");
		if (!response.IsSuccessStatusCode)
		{
			throw await CreateApiExceptionAsync(response);
		}
	}

	private static async Task<TodoItem> ReadTodoAsync(HttpResponseMessage response)
	{
		if (!response.IsSuccessStatusCode)
		{
			throw await CreateApiExceptionAsync(response);
		}

		TodoItem? item = await response.Content.ReadFromJsonAsync<TodoItem>(JsonOptions);
		if (item == null)
		{
			throw new InvalidOperationException("API вернул пустой ответ по задаче.");
		}

		return item;
	}

	private static async Task<ApiException> CreateApiExceptionAsync(HttpResponseMessage response)
	{
		string message = await response.Content.ReadAsStringAsync();
		if (string.IsNullOrWhiteSpace(message))
		{
			message = response.StatusCode switch
			{
				HttpStatusCode.Unauthorized => "Требуется повторный вход.",
				HttpStatusCode.NotFound => "Запись не найдена.",
				_ => $"Ошибка API: {(int)response.StatusCode}"
			};
		}

		return new ApiException(response.StatusCode, message.Trim('"'));
	}
}
