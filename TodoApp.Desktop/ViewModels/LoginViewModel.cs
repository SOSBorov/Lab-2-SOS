using System.Windows.Input;
using TodoApp.Desktop.Services;

namespace TodoApp.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private string _email = string.Empty;
	private string _password = string.Empty;
	private string _statusMessage = "Введи email или логин и пароль. После перехода на API вход идёт через JWT.";

	public LoginViewModel(NavigationService navigationService, string? statusMessage = null)
	{
		_navigationService = navigationService;
		LoginCommand = new RelayCommand(ExecuteLogin);
		RegisterCommand = new RelayCommand(OpenRegister);

		if (!string.IsNullOrWhiteSpace(statusMessage))
		{
			StatusMessage = statusMessage;
		}
	}

	public string Email
	{
		get => _email;
		set => SetProperty(ref _email, value);
	}

	public string Password
	{
		get => _password;
		set => SetProperty(ref _password, value);
	}

	public string StatusMessage
	{
		get => _statusMessage;
		set => SetProperty(ref _statusMessage, value);
	}

	public ICommand LoginCommand { get; }
	public ICommand RegisterCommand { get; }

	private void ExecuteLogin()
	{
		if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
		{
			StatusMessage = "Email или логин и пароль обязательны.";
			return;
		}

		try
		{
			bool success = _navigationService.State.Login(Email.Trim(), Password);
			if (!success)
			{
				StatusMessage = "Пользователь не найден. Проверь email и пароль.";
				return;
			}

			StatusMessage = "Вход выполнен успешно.";
			_navigationService.ShowTodoList();
		}
		catch (Exception ex)
		{
			StatusMessage = ex.Message;
		}
	}

	private void OpenRegister()
	{
		_navigationService.ShowRegister();
	}
}
