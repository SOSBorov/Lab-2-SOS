using System.Windows.Input;
using TodoApp.Desktop.Services;

namespace TodoApp.Desktop.ViewModels;

public class RegisterViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private string _login = string.Empty;
	private string _password = string.Empty;
	private string _firstName = string.Empty;
	private string _lastName = string.Empty;
	private int _birthYear = DateTime.Today.Year;
	private string _statusMessage = "Создай пользователя. После регистрации данные уйдут в Web API.";

	public RegisterViewModel(NavigationService navigationService)
	{
		_navigationService = navigationService;
		RegisterCommand = new RelayCommand(ExecuteRegister);
		BackToLoginCommand = new RelayCommand(BackToLogin);
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

	public string FirstName
	{
		get => _firstName;
		set => SetProperty(ref _firstName, value);
	}

	public string LastName
	{
		get => _lastName;
		set => SetProperty(ref _lastName, value);
	}

	public int BirthYear
	{
		get => _birthYear;
		set => SetProperty(ref _birthYear, value);
	}

	public string StatusMessage
	{
		get => _statusMessage;
		set => SetProperty(ref _statusMessage, value);
	}

	public ICommand RegisterCommand { get; }
	public ICommand BackToLoginCommand { get; }

	private void ExecuteRegister()
	{
		if (string.IsNullOrWhiteSpace(Login) ||
			string.IsNullOrWhiteSpace(Password) ||
			string.IsNullOrWhiteSpace(FirstName))
		{
			StatusMessage = "Логин, пароль и имя обязательны.";
			return;
		}

		try
		{
			_navigationService.State.RegisterProfile(Login.Trim(), Password, FirstName.Trim(), LastName?.Trim(), BirthYear);
			_navigationService.ShowTodoList();
		}
		catch (Exception ex)
		{
			StatusMessage = ex.Message;
		}
	}

	private void BackToLogin()
	{
		_navigationService.ShowLogin();
	}
}
