using Moq;
using TodoList.Interfaces;
using TodoList.Exceptions;

namespace TodoList.Tests;

public class FileManagerTests : IDisposable
{
	private readonly string _tempDirectory;

	public FileManagerTests()
	{
		_tempDirectory = Path.Combine(Path.GetTempPath(), "TodoListTests", Guid.NewGuid().ToString());
		Directory.CreateDirectory(_tempDirectory);
	}

	[Fact]
	public void LoadProfiles_ProfileFileDoesNotExist_ReturnsEmptyCollection()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);

		// Act
		var profiles = fileManager.LoadProfiles();

		// Assert
		Assert.Empty(profiles);
	}

	[Fact]
	public void SaveProfiles_ValidProfiles_LoadProfilesReturnsSameData()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);
		var profiles = new[]
		{
			new Profile("user1", "pass1", "Ivan", "Petrov", 2000),
			new Profile("user2", "pass2", "Anna", "Sidorova", 1998)
		};

		// Act
		fileManager.SaveProfiles(profiles);
		var loadedProfiles = fileManager.LoadProfiles().ToList();

		// Assert
		Assert.Equal(2, loadedProfiles.Count);
		Assert.Equal(profiles[0].Id, loadedProfiles[0].Id);
		Assert.Equal("user1", loadedProfiles[0].Login);
		Assert.Equal("Ivan", loadedProfiles[0].FirstName);
		Assert.Equal(profiles[1].Id, loadedProfiles[1].Id);
		Assert.Equal("user2", loadedProfiles[1].Login);
		Assert.Equal("Anna", loadedProfiles[1].FirstName);
	}

	[Fact]
	public void SaveTodos_ValidTodos_LoadTodosReturnsSameData()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);
		var userId = Guid.NewGuid();
		var firstTime = new DateTime(2025, 1, 1, 10, 30, 0, DateTimeKind.Local);
		var secondTime = new DateTime(2025, 1, 2, 15, 45, 0, DateTimeKind.Local);

		var firstClock = new Mock<IClock>();
		firstClock.Setup(c => c.Now).Returns(firstTime);

		var secondClock = new Mock<IClock>();
		secondClock.Setup(c => c.Now).Returns(secondTime);

		var todos = new[]
		{
			new TodoItem(firstClock.Object)
			{
				Id = 1,
				Text = "Первая задача",
				Status = TodoStatus.InProgress,
				LastUpdated = firstTime
			},
			new TodoItem(secondClock.Object)
			{
				Id = 2,
				Text = "Список\\nпокупок".Replace("\\n", "\n"),
				Status = TodoStatus.Completed,
				LastUpdated = secondTime
			}
		};

		// Act
		fileManager.SaveTodos(userId, todos);
		var loadedTodos = fileManager.LoadTodos(userId).ToList();

		// Assert
		Assert.Equal(2, loadedTodos.Count);
		Assert.Equal(1, loadedTodos[0].Id);
		Assert.Equal("Первая задача", loadedTodos[0].Text);
		Assert.Equal(TodoStatus.InProgress, loadedTodos[0].Status);
		Assert.Equal(firstTime, loadedTodos[0].LastUpdated);
		Assert.Equal(2, loadedTodos[1].Id);
		Assert.Equal("Список\nпокупок", loadedTodos[1].Text);
		Assert.Equal(TodoStatus.Completed, loadedTodos[1].Status);
		Assert.Equal(secondTime, loadedTodos[1].LastUpdated);
	}

	[Fact]
	public void LoadTodos_TodoFileDoesNotExist_ReturnsEmptyCollection()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);

		// Act
		var todos = fileManager.LoadTodos(Guid.NewGuid());

		// Assert
		Assert.Empty(todos);
	}

	[Fact]
	public void LoadProfiles_CorruptedProfileFile_ThrowsDataStorageException()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);
		var profileFilePath = Path.Combine(_tempDirectory, "profiles.dat");
		File.WriteAllText(profileFilePath, "broken data");

		// Act & Assert
		Assert.Throws<DataStorageException>(() => fileManager.LoadProfiles().ToList());
	}

	[Fact]
	public void LoadTodos_CorruptedTodoFile_ThrowsDataStorageException()
	{
		// Arrange
		var fileManager = new FileManager(_tempDirectory);
		var userId = Guid.NewGuid();
		var todoFilePath = Path.Combine(_tempDirectory, $"todos_{userId}.dat");
		File.WriteAllText(todoFilePath, "broken data");

		// Act & Assert
		Assert.Throws<DataStorageException>(() => fileManager.LoadTodos(userId).ToList());
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDirectory))
		{
			Directory.Delete(_tempDirectory, true);
		}
	}
}
