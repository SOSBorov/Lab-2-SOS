using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TodoList;
using TodoList.Exceptions;

namespace TodoList
{
	public class ApiDataStorage : IDataStorage
	{
		private readonly HttpClient _client;
		private readonly byte[] _key;
		private readonly byte[] _iv;

		public ApiDataStorage(string baseUrl, byte[] key, byte[] iv)
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri(baseUrl),
				Timeout = TimeSpan.FromSeconds(10)
			};

			_key = key;
			_iv = iv;
		}


		public void SaveProfiles(IEnumerable<Profile> profiles)
		{
			try
			{
				string json = JsonSerializer.Serialize(profiles);
				byte[] encrypted = Encrypt(json);

				using var content = new ByteArrayContent(encrypted);
				content.Headers.ContentType =
					new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

				var response = _client.PostAsync("profiles", content).Result;

				if (!response.IsSuccessStatusCode)
					throw new DataStorageException($"Ошибка отправки профилей на сервер: {(int)response.StatusCode}");
			}
			catch (Exception ex) when (ex is not DataStorageException)
			{
				throw new DataStorageException("Не удалось отправить профили на сервер.", ex);
			}
		}

		public IEnumerable<Profile> LoadProfiles()
		{
			try
			{
				var response = _client.GetAsync("profiles").Result;

				if (!response.IsSuccessStatusCode)
					return new List<Profile>();

				byte[] encrypted = response.Content.ReadAsByteArrayAsync().Result;

				if (encrypted.Length == 0)
					return new List<Profile>();

				string json = Decrypt(encrypted);

				return JsonSerializer.Deserialize<List<Profile>>(json) ?? new List<Profile>();
			}
			catch (CryptographicException ex)
			{
				throw new DataStorageException("Не удалось расшифровать данные профилей с сервера.", ex);
			}
			catch (Exception ex) when (ex is not DataStorageException)
			{
				throw new DataStorageException("Не удалось загрузить профили с сервера.", ex);
			}
		}


		public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
		{
			try
			{
				string json = JsonSerializer.Serialize(todos);
				byte[] encrypted = Encrypt(json);

				using var content = new ByteArrayContent(encrypted);
				content.Headers.ContentType =
					new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

				var response = _client.PostAsync($"todos/{userId}", content).Result;

				if (!response.IsSuccessStatusCode)
					throw new DataStorageException($"Ошибка отправки задач на сервер: {(int)response.StatusCode}");
			}
			catch (Exception ex) when (ex is not DataStorageException)
			{
				throw new DataStorageException($"Не удалось отправить задачи пользователя {userId} на сервер.", ex);
			}
		}

		public IEnumerable<TodoItem> LoadTodos(Guid userId)
		{
			try
			{
				var response = _client.GetAsync($"todos/{userId}").Result;

				if (!response.IsSuccessStatusCode)
					return new List<TodoItem>();

				byte[] encrypted = response.Content.ReadAsByteArrayAsync().Result;

				if (encrypted.Length == 0)
					return new List<TodoItem>();

				string json = Decrypt(encrypted);

				return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
			}
			catch (CryptographicException ex)
			{
				throw new DataStorageException($"Не удалось расшифровать задачи пользователя {userId} с сервера.", ex);
			}
			catch (Exception ex) when (ex is not DataStorageException)
			{
				throw new DataStorageException($"Не удалось загрузить задачи пользователя {userId} с сервера.", ex);
			}
		}


		private byte[] Encrypt(string plainText)
		{
			using var aes = Aes.Create();
			aes.Key = _key;
			aes.IV = _iv;

			using var ms = new System.IO.MemoryStream();
			using (var crypto = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
			using (var writer = new System.IO.StreamWriter(crypto, Encoding.UTF8))
			{
				writer.Write(plainText);
			}

			return ms.ToArray();
		}

		private string Decrypt(byte[] cipherBytes)
		{
			using var aes = Aes.Create();
			aes.Key = _key;
			aes.IV = _iv;

			using var ms = new System.IO.MemoryStream(cipherBytes);
			using var crypto = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
			using var reader = new System.IO.StreamReader(crypto, Encoding.UTF8);

			return reader.ReadToEnd();
		}
	}
}
