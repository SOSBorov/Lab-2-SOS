using Xunit;
using System;
using TodoList;

namespace TodoList.Tests;

public class TodoItemTests
{
	[Fact]
	public void Constructor_InitializesFieldsCorrectly()
	{
		// Arrange & Act
		var item = new TodoItem();

		// Assert
		Assert.Equal(0, item.Id);
		Assert.Equal(string.Empty, item.Text);
		Assert.Equal(TodoStatus.NotStarted, item.Status);

		Assert.True((DateTime.Now - item.CreatedAt).TotalSeconds < 1);
		Assert.True((DateTime.Now - item.LastUpdated).TotalSeconds < 1);
	}

	[Fact]
	public void Properties_SetCorrectValues()
	{
		// Arrange
		var item = new TodoItem();

		// Act
		item.Id = 10;
		item.Text = "Test";
		item.Status = TodoStatus.Completed;
		var now = DateTime.Now;
		item.CreatedAt = now;
		item.LastUpdated = now;

		// Assert
		Assert.Equal(10, item.Id);
		Assert.Equal("Test", item.Text);
		Assert.Equal(TodoStatus.Completed, item.Status);
		Assert.Equal(now, item.CreatedAt);
		Assert.Equal(now, item.LastUpdated);
	}

	[Fact]
	public void ToString_ReturnsFormattedString()
	{
		// Arrange
		var item = new TodoItem
		{
			Id = 1,
			Text = "Hello world",
			Status = TodoStatus.InProgress,
			LastUpdated = new DateTime(2024, 1, 1, 12, 30, 0)
		};

		// Act
		string result = item.ToString();

		// Assert
		Assert.Equal("(InProgress) Hello world обновлено 01.01.2024 12:30", result);
	}
}
