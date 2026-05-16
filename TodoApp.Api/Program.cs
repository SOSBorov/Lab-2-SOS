var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "TodoApp.Api is running.");

app.Run();
