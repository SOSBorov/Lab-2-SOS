using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.DTOs;
using TodoApp.Api.Services;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly PasswordHasher _passwordHasher;
	private readonly JwtTokenService _jwtTokenService;

	public AuthController(AppDbContext context, PasswordHasher passwordHasher, JwtTokenService jwtTokenService)
	{
		_context = context;
		_passwordHasher = passwordHasher;
		_jwtTokenService = jwtTokenService;
	}

	[HttpPost("register")]
	public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
	{
		bool usernameExists = await _context.Users.AnyAsync(user => user.Username == request.Username);
		if (usernameExists)
		{
			return Conflict("Пользователь с таким username уже существует.");
		}

		bool emailExists = await _context.Users.AnyAsync(user => user.Email == request.Email);
		if (emailExists)
		{
			return Conflict("Пользователь с таким email уже существует.");
		}

		User user = new()
		{
			Username = request.Username,
			Email = request.Email,
			PasswordHash = _passwordHasher.HashPassword(request.Password),
			Role = "User"
		};

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		return Created(string.Empty, MapToResponse(user, string.Empty));
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
	{
		User? user = await _context.Users.FirstOrDefaultAsync(item => item.Email == request.Email);
		if (user == null)
		{
			return Unauthorized("Неверный email или пароль.");
		}

		bool passwordMatches = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
		if (!passwordMatches)
		{
			return Unauthorized("Неверный email или пароль.");
		}

		string token = _jwtTokenService.CreateToken(user);
		return Ok(MapToResponse(user, token));
	}

	private static LoginResponse MapToResponse(User user, string token)
	{
		return new LoginResponse
		{
			Id = user.Id,
			Username = user.Username,
			Email = user.Email,
			Role = user.Role,
			Token = token
		};
	}
}
