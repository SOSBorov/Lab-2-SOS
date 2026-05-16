using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "TodoApp.Api is running.");

app.Run();
