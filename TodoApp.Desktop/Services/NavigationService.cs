using TodoApp.Desktop.ViewModels;
using TodoApp.Models;

namespace TodoApp.Desktop.Services;

public class NavigationService
{
	private readonly DesktopStateService _state;

	public NavigationService(DesktopStateService state)
	{
		_state = state;
	}

	public event Action<ViewModelBase>? CurrentViewModelChanged;

	private ViewModelBase? _currentViewModel;
	public ViewModelBase? CurrentViewModel
	{
		get => _currentViewModel;
		private set
		{
			_currentViewModel = value;
			if (value != null)
			{
				CurrentViewModelChanged?.Invoke(value);
			}
		}
	}

	public DesktopStateService State => _state;

	public void ShowLogin(string? statusMessage = null)
	{
		Navigate(new LoginViewModel(this, statusMessage));
	}

	public void ShowRegister()
	{
		Navigate(new RegisterViewModel(this));
	}

	public void ShowTodoList()
	{
		Navigate(new TodoListViewModel(this));
	}

	public void ShowAddTask()
	{
		Navigate(new AddTaskViewModel(this));
	}

	public void ShowEditTask(TodoItem task)
	{
		Navigate(new EditTaskViewModel(this, task));
	}

	private void Navigate(ViewModelBase viewModel)
	{
		CurrentViewModel = viewModel;
	}
}
