using System.Net.Http;
using TodoApp.Desktop.Services;

namespace TodoApp.Desktop.ViewModels;

public class MainViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private ViewModelBase? _currentViewModel;

	public MainViewModel()
	{
		HttpClient authHttpClient = new()
		{
			BaseAddress = new Uri("http://localhost:5249")
		};

		HttpClient todoHttpClient = new()
		{
			BaseAddress = new Uri("http://localhost:5249")
		};

		var state = new DesktopStateService(
			new AuthApiClient(authHttpClient),
			new TodoApiClient(todoHttpClient));

		_navigationService = new NavigationService(state);
		_navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;
		_navigationService.ShowLogin();
	}

	public ViewModelBase? CurrentViewModel
	{
		get => _currentViewModel;
		private set => SetProperty(ref _currentViewModel, value);
	}
}
