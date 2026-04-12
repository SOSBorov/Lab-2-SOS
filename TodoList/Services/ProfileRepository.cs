using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoList.Exceptions;

namespace TodoList
{
	public class ProfileRepository
	{
		public List<Profile> GetAll()
		{
			using var context = new AppDbContext();

			return context.Profiles
				.AsNoTracking()
				.Include(profile => profile.TodoItems)
				.OrderBy(profile => profile.Login)
				.ToList();
		}

		public Profile? GetById(Guid id)
		{
			using var context = new AppDbContext();

			return context.Profiles
				.AsNoTracking()
				.Include(profile => profile.TodoItems)
				.FirstOrDefault(profile => profile.Id == id);
		}

		public Profile? GetByLogin(string login)
		{
			using var context = new AppDbContext();

			return context.Profiles
				.AsNoTracking()
				.Include(profile => profile.TodoItems)
				.FirstOrDefault(profile => profile.Login == login);
		}

		public Profile? GetByCredentials(string login, string password)
		{
			using var context = new AppDbContext();

			return context.Profiles
				.AsNoTracking()
				.Include(profile => profile.TodoItems)
				.FirstOrDefault(profile => profile.Login == login && profile.Password == password);
		}

		public void Add(Profile profile)
		{
			using var context = new AppDbContext();

			bool loginExists = context.Profiles.Any(existingProfile => existingProfile.Login == profile.Login);
			if (loginExists)
			{
				throw new DuplicateLoginException("Этот логин уже занят.");
			}

			context.Profiles.Add(profile);
			context.SaveChanges();
		}

		public void Update(Profile profile)
		{
			using var context = new AppDbContext();

			var existingProfile = context.Profiles.FirstOrDefault(item => item.Id == profile.Id);
			if (existingProfile == null)
			{
				throw new ProfileNotFoundException($"Профиль с ID '{profile.Id}' не найден.");
			}

			bool loginExists = context.Profiles.Any(item => item.Id != profile.Id && item.Login == profile.Login);
			if (loginExists)
			{
				throw new DuplicateLoginException("Этот логин уже занят.");
			}

			existingProfile.Login = profile.Login;
			existingProfile.Password = profile.Password;
			existingProfile.FirstName = profile.FirstName;
			existingProfile.LastName = profile.LastName;
			existingProfile.BirthYear = profile.BirthYear;

			context.SaveChanges();
		}

		public void Delete(Guid id)
		{
			using var context = new AppDbContext();

			var existingProfile = context.Profiles.FirstOrDefault(profile => profile.Id == id);
			if (existingProfile == null)
			{
				throw new ProfileNotFoundException($"Профиль с ID '{id}' не найден.");
			}

			context.Profiles.Remove(existingProfile);
			context.SaveChanges();
		}
	}
}
