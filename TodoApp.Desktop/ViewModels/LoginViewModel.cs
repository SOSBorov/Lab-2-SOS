using System.Windows.Input;
using TodoApp.Desktop.Services;

namespace TodoApp.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private string _login = string.Empty;
	private string _password = string.Empty;
	private string _statusMessage = "Введи email или логин и пароль. После перехода на API вход идёт через JWT.";

	public LoginViewModel(NavigationService navigationService)
	{
		_navigationService = navigationService;
		LoginCommand = new RelayCommand(ExecuteLogin);
		OpenRegisterCommand = new RelayCommand(OpenRegister);
	}

	public string Login
	{
		get => _login;
		set => SetProperty(ref _login, value);
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
	public ICommand OpenRegisterCommand { get; }

	private void ExecuteLogin()
	{
		if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
		{
			StatusMessage = "Email или логин и пароль обязательны.";
			return;
		}

		bool success = _navigationService.State.Login(Login.Trim(), Password);
		if (!success)
		{
			StatusMessage = "Пользователь не найден. Проверь email и пароль.";
			return;
		}

		StatusMessage = "Вход выполнен успешно.";
		_navigationService.ShowTodoList();
	}

	private void OpenRegister()
	{
		_navigationService.ShowRegister();
	}
}
