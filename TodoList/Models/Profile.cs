using System;
using System.Collections.Generic;

namespace TodoList
{
	public class Profile
	{
		public Profile()
		{
			Id = Guid.NewGuid();
			Login = string.Empty;
			Password = string.Empty;
			TodoItems = new List<TodoItem>();
		}

		public Profile(string login, string password, string firstName, string? lastName, int birthYear)
		{
			Id = Guid.NewGuid();
			Login = login;
			Password = password;
			FirstName = firstName;
			LastName = lastName;
			BirthYear = birthYear;
			TodoItems = new List<TodoItem>();
		}

		public Guid Id { get; set; }

		public string Login { get; set; }

		public string Password { get; set; }

		public string FirstName { get; set; } = "Default";

		public string? LastName { get; set; }

		public int BirthYear { get; set; }

		public List<TodoItem> TodoItems { get; set; }

		public string Info => GetInfo();

		public string GetInfo()
		{
			DateTime currentDate = DateTime.Today;
			int age = currentDate.Year - BirthYear;
			return $"Пользователь: {FirstName} {LastName}, Возраст: {age} лет";
		}
	}
}
