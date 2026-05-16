using System.Collections.ObjectModel;
using System.Net;
using TodoApp.Models;

namespace TodoApp.Desktop.Services;

public class DesktopStateService
{
	private readonly AuthApiClient _authApiClient;
	private readonly TodoApiClient _todoApiClient;
	private string? _token;

	public DesktopStateService(AuthApiClient authApiClient, TodoApiClient todoApiClient)
	{
		_authApiClient = authApiClient;
		_todoApiClient = todoApiClient;

		Tasks = new ObservableCollection<TodoItem>();
	}

	public ObservableCollection<TodoItem> Tasks { get; }
	public Guid? CurrentProfileId { get; private set; }
	public string CurrentUserDisplay { get; private set; } = "Пользователь не выбран";
	public string CurrentEmail { get; private set; } = string.Empty;

	public bool Login(string loginOrEmail, string password)
	{
		string email = NormalizeEmail(loginOrEmail);
		AuthResponse authResponse;

		try
		{
			authResponse = _authApiClient.LoginAsync(email, password).GetAwaiter().GetResult();
		}
		catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
		{
			return false;
		}

		_token = authResponse.Token;
		_todoApiClient.SetToken(_token);
		CurrentProfileId = authResponse.ProfileId;
		CurrentEmail = authResponse.Email;
		CurrentUserDisplay = BuildCurrentUserDisplay(authResponse);

		ReloadTasksForCurrentProfile();
		return true;
	}

	public void Logout()
	{
		_token = null;
		CurrentProfileId = null;
		CurrentEmail = string.Empty;
		CurrentUserDisplay = "Пользователь не выбран";
		_todoApiClient.SetToken(null);
		Tasks.Clear();
	}

	public void RegisterProfile(string login, string password, string firstName, string? lastName, int birthYear)
	{
		string username = NormalizeUsername(login);
		string email = NormalizeEmail(login);

		ExecuteApi(() =>
			_authApiClient.RegisterAsync(username, email, password, firstName.Trim(), lastName?.Trim(), birthYear)
				.GetAwaiter()
				.GetResult());

		if (!Login(login, password))
		{
			throw new InvalidOperationException("Не удалось выполнить вход после регистрации.");
		}
	}

	public void ReloadTasksForCurrentProfile()
	{
		EnsureAuthorized();

		if (!CurrentProfileId.HasValue)
		{
			Tasks.Clear();
			return;
		}

		List<TodoItem> tasks = ExecuteApi(() => _todoApiClient.GetAllAsync()
			.GetAwaiter()
			.GetResult())
			.Where(todo => todo.ProfileId == CurrentProfileId.Value)
			.OrderBy(todo => todo.Id)
			.ToList();

		ReplaceCollection(Tasks, tasks);
	}

	public TodoItem AddTask(string text, TodoStatus status)
	{
		EnsureAuthorized();
		_ = ExecuteApi(() =>
			_todoApiClient.CreateAsync(text.Trim(), status, CurrentProfileId!.Value)
				.GetAwaiter()
				.GetResult());
		ReloadTasksForCurrentProfile();

		return Tasks.OrderByDescending(task => task.Id).First();
	}

	public void UpdateTask(TodoItem task, string newText, TodoStatus newStatus)
	{
		EnsureAuthorized();
		_ = ExecuteApi(() =>
			_todoApiClient.UpdateAsync(task.Id, newText.Trim(), newStatus)
				.GetAwaiter()
				.GetResult());
		ReloadTasksForCurrentProfile();
	}

	public void DeleteTask(TodoItem task)
	{
		EnsureAuthorized();
		ExecuteApi(() =>
			_todoApiClient.DeleteAsync(task.Id)
				.GetAwaiter()
				.GetResult());
		ReloadTasksForCurrentProfile();
	}

	public void UpdateTaskStatus(TodoItem task, TodoStatus status)
	{
		EnsureAuthorized();
		_ = ExecuteApi(() =>
			_todoApiClient.SetStatusAsync(task.Id, status)
				.GetAwaiter()
				.GetResult());
		ReloadTasksForCurrentProfile();
	}

	private void EnsureAuthorized()
	{
		if (string.IsNullOrWhiteSpace(_token) || !CurrentProfileId.HasValue)
		{
			throw new InvalidOperationException("Сначала нужно войти в приложение через API.");
		}
	}

	private void ExecuteApi(Action action)
	{
		try
		{
			action();
		}
		catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
		{
			Logout();
			throw new SessionExpiredException("Сессия истекла. Выполни вход заново.");
		}
		catch (ApiException ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	private T ExecuteApi<T>(Func<T> action)
	{
		try
		{
			return action();
		}
		catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
		{
			Logout();
			throw new SessionExpiredException("Сессия истекла. Выполни вход заново.");
		}
		catch (ApiException ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	private static string NormalizeUsername(string loginOrEmail)
	{
		if (string.IsNullOrWhiteSpace(loginOrEmail))
		{
			return string.Empty;
		}

		string trimmed = loginOrEmail.Trim();
		int atIndex = trimmed.IndexOf('@');
		return atIndex > 0 ? trimmed[..atIndex] : trimmed;
	}

	private static string NormalizeEmail(string loginOrEmail)
	{
		string trimmed = loginOrEmail.Trim();
		return trimmed.Contains('@')
			? trimmed
			: $"{trimmed}@todoapp.local";
	}

	private static string BuildCurrentUserDisplay(AuthResponse authResponse)
	{
		string fullName = $"{authResponse.FirstName} {authResponse.LastName}".Trim();
		if (string.IsNullOrWhiteSpace(fullName))
		{
			fullName = authResponse.Username;
		}

		return $"Пользователь: {fullName}, Email: {authResponse.Email}";
	}

	private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
	{
		target.Clear();
		foreach (var item in source)
		{
			target.Add(item);
		}
	}
}
