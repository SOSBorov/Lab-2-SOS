using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoList.Exceptions;

namespace TodoList
{
	public class TodoRepository
	{
		public List<TodoItem> GetAll()
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			return context.Todos
				.AsNoTracking()
				.OrderBy(todo => todo.Id)
				.ToList();
		}

		public List<TodoItem> GetAllByProfile(Guid profileId)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			return context.Todos
				.AsNoTracking()
				.Where(todo => todo.ProfileId == profileId)
				.OrderBy(todo => todo.Id)
				.ToList();
		}

		public TodoItem? GetById(int id)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			return context.Todos
				.AsNoTracking()
				.FirstOrDefault(todo => todo.Id == id);
		}

		public void Add(TodoItem item)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			context.Todos.Add(item);
			context.SaveChanges();
		}

		public void Update(TodoItem item)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			var existingItem = context.Todos.FirstOrDefault(todo => todo.Id == item.Id);
			if (existingItem == null)
			{
				throw new TaskNotFoundException($"Задача с ID '{item.Id}' не найдена.");
			}

			existingItem.Text = item.Text;
			existingItem.Status = item.Status;
			existingItem.CreatedAt = item.CreatedAt;
			existingItem.LastUpdated = item.LastUpdated;
			existingItem.ProfileId = item.ProfileId;

			context.SaveChanges();
		}

		public void Delete(int id)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			var existingItem = context.Todos.FirstOrDefault(todo => todo.Id == id);
			if (existingItem == null)
			{
				throw new TaskNotFoundException($"Задача с ID '{id}' не найдена.");
			}

			context.Todos.Remove(existingItem);
			context.SaveChanges();
		}

		public void SetStatus(int id, TodoStatus status)
		{
			using var context = new AppDbContext();
			context.Database.EnsureCreated();

			var existingItem = context.Todos.FirstOrDefault(todo => todo.Id == id);
			if (existingItem == null)
			{
				throw new TaskNotFoundException($"Задача с ID '{id}' не найдена.");
			}

			existingItem.Status = status;
			existingItem.LastUpdated = DateTime.Now;

			context.SaveChanges();
		}
	}
}
