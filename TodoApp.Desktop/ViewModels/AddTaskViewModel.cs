using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class AddTaskViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private string _taskText = string.Empty;
	private TodoStatus _selectedStatus = TodoStatus.NotStarted;
	private string _statusMessage = "Заполни форму новой задачи. После сохранения запись уйдёт в Web API.";

	public AddTaskViewModel(NavigationService navigationService)
	{
		_navigationService = navigationService;
		AvailableStatuses = new ObservableCollection<TodoStatus>(Enum.GetValues<TodoStatus>());
		SaveCommand = new RelayCommand(SaveTask);
		CancelCommand = new RelayCommand(Cancel);
	}

	public string TaskText
	{
		get => _taskText;
		set => SetProperty(ref _taskText, value);
	}

	public TodoStatus SelectedStatus
	{
		get => _selectedStatus;
		set => SetProperty(ref _selectedStatus, value);
	}

	public string StatusMessage
	{
		get => _statusMessage;
		set => SetProperty(ref _statusMessage, value);
	}

	public ObservableCollection<TodoStatus> AvailableStatuses { get; }

	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }

	private void SaveTask()
	{
		if (string.IsNullOrWhiteSpace(TaskText))
		{
			StatusMessage = "Текст задачи не должен быть пустым.";
			return;
		}

		try
		{
			_navigationService.State.AddTask(TaskText, SelectedStatus);
			_navigationService.ShowTodoList();
		}
		catch (SessionExpiredException ex)
		{
			_navigationService.ShowLogin(ex.Message);
		}
		catch (Exception ex)
		{
			StatusMessage = ex.Message;
		}
	}

	private void Cancel()
	{
		_navigationService.ShowTodoList();
	}
}
