using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp.Desktop.Services;
using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class EditTaskViewModel : ViewModelBase
{
	private readonly NavigationService _navigationService;
	private readonly TodoItem _sourceTask;
	private string _taskText;
	private TodoStatus _selectedStatus;
	private string _statusMessage = "Измени текст или статус задачи. После сохранения изменения уйдут в Web API.";

	public EditTaskViewModel(NavigationService navigationService, TodoItem task)
	{
		_navigationService = navigationService;
		_sourceTask = task;
		_taskText = task.Text;
		_selectedStatus = task.Status;

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
			_navigationService.State.UpdateTask(_sourceTask, TaskText, SelectedStatus);
			_navigationService.ShowTodoList();
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
