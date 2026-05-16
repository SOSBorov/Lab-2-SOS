using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class TodoListViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private readonly ICollectionView _tasksView;
	private string _searchText = string.Empty;
	private TodoStatus? _selectedStatusFilter;
	private TodoStatus _selectedStatusForTask;
	private TodoItem? _selectedTask;

	public TodoListViewModel(NavigationService navigationService)
	{
		_navigationService = navigationService;
		Tasks = _navigationService.State.Tasks;
		StatusFilters = new ObservableCollection<string>(new[] { "Все", "NotStarted", "InProgress", "Completed", "Postponed", "Failed" });

		_tasksView = CollectionViewSource.GetDefaultView(Tasks);
		_tasksView.Filter = FilterTasks;

		OpenAddTaskCommand = new RelayCommand(OpenAddTask);
		OpenEditTaskCommand = new RelayCommand(OpenEditTask, () => SelectedTask != null);
		RemoveTaskCommand = new RelayCommand(RemoveSelectedTask, () => SelectedTask != null);
		ApplyStatusCommand = new RelayCommand(ApplyStatus, () => SelectedTask != null);
		RefreshCommand = new RelayCommand(RefreshTasks);
		LogoutCommand = new RelayCommand(Logout);

		RefreshTasks();
	}

	public ObservableCollection<TodoItem> Tasks { get; }
	public ObservableCollection<string> StatusFilters { get; }

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (SetProperty(ref _searchText, value))
			{
				_tasksView.Refresh();
				OnPropertyChanged(nameof(VisibleTaskCount));
			}
		}
	}

	public string SelectedFilter
	{
		get => _selectedStatusFilter?.ToString() ?? "Все";
		set
		{
			_selectedStatusFilter = Enum.TryParse<TodoStatus>(value, out var status) ? status : null;
			OnPropertyChanged();
			_tasksView.Refresh();
			OnPropertyChanged(nameof(VisibleTaskCount));
		}
	}

	public TodoStatus SelectedStatusForTask
	{
		get => _selectedStatusForTask;
		set => SetProperty(ref _selectedStatusForTask, value);
	}

	public TodoItem? SelectedTask
	{
		get => _selectedTask;
		set
		{
			if (SetProperty(ref _selectedTask, value))
			{
				if (value != null)
				{
					SelectedStatusForTask = value.Status;
				}

				(OpenEditTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
				(RemoveTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
				(ApplyStatusCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
	}

	public string CurrentProfileDisplay =>
		_navigationService.State.CurrentUserDisplay;

	public int VisibleTaskCount => _tasksView.Cast<object>().Count();

	public ICommand OpenAddTaskCommand { get; }
	public ICommand OpenEditTaskCommand { get; }
	public ICommand RemoveTaskCommand { get; }
	public ICommand ApplyStatusCommand { get; }
	public ICommand RefreshCommand { get; }
	public ICommand LogoutCommand { get; }

	private void OpenAddTask()
	{
		_navigationService.ShowAddTask();
	}

	private void OpenEditTask()
	{
		if (SelectedTask != null)
		{
			_navigationService.ShowEditTask(SelectedTask);
		}
	}

	private void RemoveSelectedTask()
	{
		if (SelectedTask == null)
		{
			return;
		}

		_navigationService.State.DeleteTask(SelectedTask);
		SelectedTask = null;
		RefreshTasks();
	}

	private void ApplyStatus()
	{
		if (SelectedTask == null)
		{
			return;
		}

		_navigationService.State.UpdateTaskStatus(SelectedTask, SelectedStatusForTask);
		SelectedTask = Tasks.FirstOrDefault(task => task.Id == SelectedTask?.Id);
		RefreshTasks();
	}

	private void RefreshTasks()
	{
		_navigationService.State.ReloadTasksForCurrentProfile();
		_tasksView.Refresh();
		OnPropertyChanged(nameof(VisibleTaskCount));
		OnPropertyChanged(nameof(CurrentProfileDisplay));
	}

	private void Logout()
	{
		_navigationService.State.Logout();
		_navigationService.ShowLogin();
	}

	private bool FilterTasks(object obj)
	{
		if (obj is not TodoItem task)
		{
			return false;
		}

		bool matchesText = string.IsNullOrWhiteSpace(SearchText) ||
			task.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

		bool matchesStatus = !_selectedStatusFilter.HasValue || task.Status == _selectedStatusFilter.Value;

		return matchesText && matchesStatus;
	}
}
