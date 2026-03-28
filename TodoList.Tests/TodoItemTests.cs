using System;
using Moq;
using TodoList;
using TodoList.Interfaces;
using Xunit;

namespace TodoList.Tests;

public class TodoItemTests
{
	[Fact]
	public void Constructor_InitializesDatesUsingClock()
	{
		// Arrange
		var fixedTime = new DateTime(2025, 1, 1, 12, 0, 0);
		var clock = new Mock<IClock>();
		clock.Setup(c => c.Now).Returns(fixedTime);

		// Act
		var item = new TodoItem(clock.Object);

		// Assert
		Assert.Equal(fixedTime, item.CreatedAt);
		Assert.Equal(fixedTime, item.LastUpdated);
	}

	[Fact]
	public void UpdateText_UpdatesLastUpdatedUsingClock()
	{
		// Arrange
		var initialTime = new DateTime(2025, 1, 1, 12, 0, 0);
		var updatedTime = new DateTime(2025, 1, 1, 13, 0, 0);

		var clock = new Mock<IClock>();
		clock.SetupSequence(c => c.Now)
			 .Returns(initialTime)
			 .Returns(initialTime)
			 .Returns(updatedTime);

		var item = new TodoItem(clock.Object);

		// Act
		item.UpdateText("New text");

		// Assert
		Assert.Equal("New text", item.Text);
		Assert.Equal(updatedTime, item.LastUpdated);
	}

	[Fact]
	public void ToString_ReturnsFormattedString()
	{
		// Arrange
		var fixedTime = new DateTime(2024, 1, 1, 12, 30, 0);
		var clock = new Mock<IClock>();
		clock.Setup(c => c.Now).Returns(fixedTime);

		var item = new TodoItem(clock.Object)
		{
			Id = 1,
			Text = "Hello world",
			Status = TodoStatus.InProgress,
			LastUpdated = fixedTime
		};

		// Act
		string result = item.ToString();

		// Assert
		Assert.Equal("(InProgress) Hello world обновлено 01.01.2024 12:30", result);
	}
}
