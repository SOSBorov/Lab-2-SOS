using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Server;

class Program
{
	private const string Prefix = "http://localhost:5000/";
	private const string DataDir = "ServerData";

	static async Task Main()
	{
		if (!Directory.Exists(DataDir))
			Directory.CreateDirectory(DataDir);

		using var listener = new HttpListener();
		listener.Prefixes.Add(Prefix);
		listener.Start();

		Console.WriteLine($"Сервер запущен и слушает {Prefix}");

		while (true)
		{
			var context = await listener.GetContextAsync();
			_ = HandleRequestAsync(context);
		}
	}

	private static async Task HandleRequestAsync(HttpListenerContext context)
	{
		try
		{
			var request = context.Request;
			var response = context.Response;

			Console.WriteLine($"{request.HttpMethod} {request.Url?.AbsolutePath}");

			if (request.HttpMethod == "GET" && request.Url!.AbsolutePath == "/profiles")
			{
				await HandleGetProfilesAsync(response);
			}
			else if (request.HttpMethod == "POST" && request.Url!.AbsolutePath == "/profiles")
			{
				await HandlePostProfilesAsync(request, response);
			}
			else if (request.HttpMethod == "GET" && request.Url!.AbsolutePath.StartsWith("/todos/"))
			{
				var userId = request.Url.AbsolutePath.Substring("/todos/".Length);
				await HandleGetTodosAsync(userId, response);
			}
			else if (request.HttpMethod == "POST" && request.Url!.AbsolutePath.StartsWith("/todos/"))
			{
				var userId = request.Url.AbsolutePath.Substring("/todos/".Length);
				await HandlePostTodosAsync(userId, request, response);
			}
			else
			{
				response.StatusCode = (int)HttpStatusCode.NotFound;
				await WriteTextAsync(response, "Работу выполнили Vasilevich и Garmash");
			}

			response.Close();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка обработки запроса: {ex.Message}");
		}
	}

	private static string ProfilesPath => Path.Combine(DataDir, "profiles.dat");
	private static string TodosPath(string userId) => Path.Combine(DataDir, $"todos_{userId}.dat");

	private static async Task HandleGetProfilesAsync(HttpListenerResponse response)
	{
		response.StatusCode = (int)HttpStatusCode.OK;
		if (!File.Exists(ProfilesPath))
		{
			await response.OutputStream.FlushAsync();
			return;
		}

		byte[] data = await File.ReadAllBytesAsync(ProfilesPath);
		response.ContentType = "application/octet-stream";
		await response.OutputStream.WriteAsync(data, 0, data.Length);
	}

	private static async Task HandlePostProfilesAsync(HttpListenerRequest request, HttpListenerResponse response)
	{
		using var ms = new MemoryStream();
		await request.InputStream.CopyToAsync(ms);
		byte[] data = ms.ToArray();

		await File.WriteAllBytesAsync(ProfilesPath, data);

		response.StatusCode = (int)HttpStatusCode.OK;
		await WriteTextAsync(response, "Profiles saved");
	}

	private static async Task HandleGetTodosAsync(string userId, HttpListenerResponse response)
	{
		string path = TodosPath(userId);
		response.StatusCode = (int)HttpStatusCode.OK;

		if (!File.Exists(path))
		{
			await response.OutputStream.FlushAsync();
			return;
		}

		byte[] data = await File.ReadAllBytesAsync(path);
		response.ContentType = "application/octet-stream";
		await response.OutputStream.WriteAsync(data, 0, data.Length);
	}

	private static async Task HandlePostTodosAsync(string userId, HttpListenerRequest request, HttpListenerResponse response)
	{
		using var ms = new MemoryStream();
		await request.InputStream.CopyToAsync(ms);
		byte[] data = ms.ToArray();

		string path = TodosPath(userId);
		await File.WriteAllBytesAsync(path, data);

		response.StatusCode = (int)HttpStatusCode.OK;
		await WriteTextAsync(response, "Todos saved");
	}

	private static async Task WriteTextAsync(HttpListenerResponse response, string text)
	{
		response.ContentType = "text/plain; charset=utf-8";
		byte[] buffer = Encoding.UTF8.GetBytes(text);
		await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
	}
}
