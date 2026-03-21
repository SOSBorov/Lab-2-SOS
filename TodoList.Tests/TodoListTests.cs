using Xunit;
using TodoList;
using TodoList.Exceptions;

namespace TodoList.Tests;

public class TodoListTests
{

	[Fact]
	public void Add_WithValidText_AddsItemAndIncrementsId()
	{
		// Arrange
		var list = new TodoList();

		// Act
		var item = list.Add("Test task");

		// Assert
		Assert.Equal(1, item.Id);
		Assert.Equal("Test task", item.Text);
		Assert.Equal(1, list.Count);
	}

	[Fact]
	public void Add_RaisesOnTodoAddedEvent()
	{
		// Arrange
		var list = new TodoList();
		TodoItem? received = null;
		list.OnTodoAdded += i => received = i;

		// Act
		var item = list.Add("Hello");

		// Assert
		Assert.NotNull(received);
		Assert.Equal(item.Id, received!.Id);
	}

	[Fact]
	public void AddExistingItem_AddsOnlyIfIdNotExists()
	{
		// Arrange
		var list = new TodoList();
		var item1 = list.Add("A");
		var item2 = new TodoItem { Id = 5, Text = "B" };

		// Act
		list.AddExistingItem(item2);

		// Assert
		Assert.Equal(2, list.Count);
		Assert.Equal(5, list.GetById(5)!.Id);
	}

	[Fact]
	public void AddExistingItem_DoesNotAddIfIdExists()
	{
		// Arrange
		var list = new TodoList();
		var item1 = list.Add("A");
		var duplicate = new TodoItem { Id = 1, Text = "Duplicate" };

		// Act
		list.AddExistingItem(duplicate);

		// Assert
		Assert.Equal(1, list.Count);
		Assert.Equal("A", list.GetById(1)!.Text);
	}

	[Fact]
	public void Remove_ExistingItem_RemovesItem()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Task");

		// Act
		list.Remove(item.Id);

		// Assert
		Assert.Equal(0, list.Count);
		Assert.Null(list.GetById(item.Id));
	}

	[Fact]
	public void Remove_RaisesOnTodoDeletedEvent()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Task");
		TodoItem? received = null;
		list.OnTodoDeleted += i => received = i;

		// Act
		list.Remove(item.Id);

		// Assert
		Assert.NotNull(received);
		Assert.Equal(item.Id, received!.Id);
	}

	[Fact]
	public void Remove_NonExistingItem_Throws()
	{
		// Arrange
		var list = new TodoList();

		// Act & Assert
		Assert.Throws<TaskNotFoundException>(() => list.Remove(999));
	}

	[Fact]
	public void Update_ExistingItem_UpdatesTextAndLastUpdated()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Old");
		var oldDate = item.LastUpdated;

		// Act
		list.Update(item.Id, "New");

		// Assert
		var updated = list.GetById(item.Id)!;
		Assert.Equal("New", updated.Text);
		Assert.True(updated.LastUpdated > oldDate);
	}

	[Fact]
	public void Update_RaisesOnTodoUpdatedEvent()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Old");
		TodoItem? received = null;
		list.OnTodoUpdated += i => received = i;

		// Act
		list.Update(item.Id, "New");

		// Assert
		Assert.NotNull(received);
		Assert.Equal(item.Id, received!.Id);
	}

	[Fact]
	public void Update_NonExistingItem_Throws()
	{
		// Arrange
		var list = new TodoList();

		// Act & Assert
		Assert.Throws<TaskNotFoundException>(() => list.Update(999, "Text"));
	}

	[Fact]
	public void SetStatus_ExistingItem_UpdatesStatusAndLastUpdated()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Task");
		var oldDate = item.LastUpdated;

		// Act
		list.SetStatus(item.Id, TodoStatus.Completed);

		// Assert
		var updated = list.GetById(item.Id)!;
		Assert.Equal(TodoStatus.Completed, updated.Status);
		Assert.True(updated.LastUpdated > oldDate);
	}

	[Fact]
	public void SetStatus_RaisesOnStatusChangedEvent()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Task");
		TodoItem? received = null;
		list.OnStatusChanged += i => received = i;

		// Act
		list.SetStatus(item.Id, TodoStatus.Completed);

		// Assert
		Assert.NotNull(received);
		Assert.Equal(item.Id, received!.Id);
	}

	[Fact]
	public void SetStatus_NonExistingItem_Throws()
	{
		// Arrange
		var list = new TodoList();

		// Act & Assert
		Assert.Throws<TaskNotFoundException>(() => list.SetStatus(999, TodoStatus.Completed));
	}

	[Fact]
	public void GetById_ReturnsCorrectItem()
	{
		// Arrange
		var list = new TodoList();
		var item = list.Add("Task");

		// Act
		var result = list.GetById(item.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(item.Id, result!.Id);
	}

	[Fact]
	public void GetAllItems_ReturnsCopyOfList()
	{
		// Arrange
		var list = new TodoList();
		list.Add("A");
		list.Add("B");

		// Act
		var items = list.GetAllItems();

		// Assert
		Assert.Equal(2, items.Count);
		Assert.NotSame(items, list.GetAllItems());
	}
}
