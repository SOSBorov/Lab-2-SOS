using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.DTOs;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
	private readonly AppDbContext _context;

	public TodosController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<TodoItemResponse>>> GetAll()
	{
		List<TodoItemResponse> todos = await _context.Todos
			.AsNoTracking()
			.OrderBy(todo => todo.Id)
			.Select(todo => MapToResponse(todo))
			.ToListAsync();

		return Ok(todos);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<TodoItemResponse>> GetById(int id)
	{
		TodoItem? todo = await _context.Todos
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.Id == id);

		if (todo == null)
		{
			return NotFound();
		}

		return Ok(MapToResponse(todo));
	}

	[HttpPost]
	public async Task<ActionResult<TodoItemResponse>> Create(CreateTodoRequest request)
	{
		TodoItem todo = new()
		{
			Id = await GetNextTodoIdAsync(),
			Text = request.Text,
			Status = request.Status,
			ProfileId = request.ProfileId,
			CreatedAt = DateTime.Now,
			LastUpdated = DateTime.Now
		};

		_context.Todos.Add(todo);
		await _context.SaveChangesAsync();

		TodoItemResponse response = MapToResponse(todo);
		return CreatedAtAction(nameof(GetById), new { id = todo.Id }, response);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<TodoItemResponse>> Update(int id, UpdateTodoRequest request)
	{
		TodoItem? todo = await _context.Todos.FirstOrDefaultAsync(item => item.Id == id);
		if (todo == null)
		{
			return NotFound();
		}

		todo.Text = request.Text;
		todo.Status = request.Status;
		todo.LastUpdated = DateTime.Now;

		await _context.SaveChangesAsync();

		return Ok(MapToResponse(todo));
	}

	[HttpPatch("{id:int}/status")]
	public async Task<ActionResult<TodoItemResponse>> SetStatus(int id, SetStatusRequest request)
	{
		TodoItem? todo = await _context.Todos.FirstOrDefaultAsync(item => item.Id == id);
		if (todo == null)
		{
			return NotFound();
		}

		todo.Status = request.Status;
		todo.LastUpdated = DateTime.Now;

		await _context.SaveChangesAsync();

		return Ok(MapToResponse(todo));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		TodoItem? todo = await _context.Todos.FirstOrDefaultAsync(item => item.Id == id);
		if (todo == null)
		{
			return NotFound();
		}

		_context.Todos.Remove(todo);
		await _context.SaveChangesAsync();

		return NoContent();
	}

	private async Task<int> GetNextTodoIdAsync()
	{
		int? maxId = await _context.Todos
			.Select(todo => (int?)todo.Id)
			.MaxAsync();

		return (maxId ?? 0) + 1;
	}

	private static TodoItemResponse MapToResponse(TodoItem todo)
	{
		return new TodoItemResponse
		{
			Id = todo.Id,
			Text = todo.Text,
			Status = todo.Status,
			CreatedAt = todo.CreatedAt,
			LastUpdated = todo.LastUpdated,
			ProfileId = todo.ProfileId
		};
	}
}
