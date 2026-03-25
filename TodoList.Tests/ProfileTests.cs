using Xunit;
using System;
using TodoList;

namespace TodoList.Tests;

public class ProfileTests
{

	[Fact]
	public void DefaultConstructor_InitializesIdAndEmptyFields()
	{
		// Arrange & Act
		var profile = new Profile();

		// Assert
		Assert.NotEqual(Guid.Empty, profile.Id);
		Assert.Equal(string.Empty, profile.Login);
		Assert.Equal(string.Empty, profile.Password);
		Assert.Equal("Default", profile.FirstName);
		Assert.Null(profile.LastName);
		Assert.Equal(0, profile.BirthYear);
	}

	[Fact]
	public void Constructor_WithParameters_SetsAllFields()
	{
		// Arrange
		string login = "garibos";
		string password = "secret123";
		string first = "Gari";
		string last = "Brous";
		int birth = 2000;

		// Act
		var profile = new Profile(login, password, first, last, birth);

		// Assert
		Assert.Equal(login, profile.Login);
		Assert.Equal(password, profile.Password);
		Assert.Equal(first, profile.FirstName);
		Assert.Equal(last, profile.LastName);
		Assert.Equal(birth, profile.BirthYear);
		Assert.NotEqual(Guid.Empty, profile.Id);
	}

	[Fact]
	public void GetInfo_ReturnsCorrectFormattedString()
	{
		// Arrange
		var profile = new Profile("login", "pass", "Ivan", "Petrov", 2000);

		// Act
		string info = profile.GetInfo();

		// Assert
		Assert.Contains("Пользователь: Ivan Petrov", info);
		Assert.Contains("Возраст:", info);
		Assert.Contains("лет", info);
	}

	[Fact]
	public void GetInfo_CalculatesCorrectAge()
	{
		// Arrange
		int birthYear = DateTime.Today.Year - 25;
		var profile = new Profile("l", "p", "A", "B", birthYear);

		// Act
		string info = profile.GetInfo();

		// Assert
		Assert.Contains("25 лет", info);
	}

	[Fact]
	public void GetInfo_AllowsNullLastName()
	{
		// Arrange
		var profile = new Profile("l", "p", "A", null, 1990);

		// Act
		string info = profile.GetInfo();

		// Assert
		Assert.Contains("A ", info); 
		Assert.Contains("Возраст:", info);
	}
}
