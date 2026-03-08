using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TodoList.Exceptions;

namespace TodoList
{
	public class FileManager : IDataStorage
	{
		private readonly string _dataDirectory;
		private readonly string _profileFilePath;
		private readonly byte[] _key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
		private readonly byte[] _iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

		public FileManager(string dataDirectory)
		{
			_dataDirectory = dataDirectory;
			_profileFilePath = Path.Combine(_dataDirectory, "profiles.dat");

			if (!Directory.Exists(_dataDirectory))
			{
				Directory.CreateDirectory(_dataDirectory);
			}
		}

		public void SaveProfiles(IEnumerable<Profile> profiles)
		{
			try
			{
				using var fileStream = new FileStream(_profileFilePath, FileMode.Create, FileAccess.Write);
				using var bufferedStream = new BufferedStream(fileStream);
				using var aes = Aes.Create();
				using var encryptor = aes.CreateEncryptor(_key, _iv);
				using var cryptoStream = new CryptoStream(bufferedStream, encryptor, CryptoStreamMode.Write);
				using var writer = new StreamWriter(cryptoStream, Encoding.UTF8);

				foreach (var profile in profiles)
				{
					writer.WriteLine($"{profile.Id};{profile.Login};{profile.Password};{profile.FirstName};{profile.LastName};{profile.BirthYear}");
				}
			}
			catch (Exception ex)
			{
				throw new DataStorageException("Не удалось сохранить профили.", ex);
			}
		}

		public IEnumerable<Profile> LoadProfiles()
		{
			if (!File.Exists(_profileFilePath))
			{
				return Enumerable.Empty<Profile>();
			}

			var profiles = new List<Profile>();
			try
			{
				using var fileStream = new FileStream(_profileFilePath, FileMode.Open, FileAccess.Read);
				using var bufferedStream = new BufferedStream(fileStream);
				using var aes = Aes.Create();
				using var decryptor = aes.CreateDecryptor(_key, _iv);
				using var cryptoStream = new CryptoStream(bufferedStream, decryptor, CryptoStreamMode.Read);
				using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line)) continue;
					var parts = line.Split(';');
					if (parts.Length == 6)
					{
						var profile = new Profile
						{
							Id = Guid.Parse(parts[0]),
							Login = parts[1],
							Password = parts[2],
							FirstName = parts[3],
							LastName = parts[4],
							BirthYear = int.Parse(parts[5])
						};
						profiles.Add(profile);
					}
				}
				return profiles;
			}
			catch (CryptographicException ex)
			{
				throw new DataStorageException("Не удалось расшифровать файл профилей. Файл может быть поврежден.", ex);
			}
			catch (Exception ex)
			{
				throw new DataStorageException("Не удалось загрузить профили. Файл может быть поврежден.", ex);
			}
		}

		public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
		{
			var filePath = GetTodoFilePath(userId);
			try
			{
				using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				using var bufferedStream = new BufferedStream(fileStream);
				using var aes = Aes.Create();
				using var encryptor = aes.CreateEncryptor(_key, _iv);
				using var cryptoStream = new CryptoStream(bufferedStream, encryptor, CryptoStreamMode.Write);
				using var writer = new StreamWriter(cryptoStream, Encoding.UTF8);

				foreach (var item in todos)
				{
					string textToSave = item.Text.Replace("\r", "").Replace("\n", "\\n");
					string formattedDate = item.LastUpdated.ToString("o", CultureInfo.InvariantCulture);
					writer.WriteLine($"{item.Id};{textToSave};{item.Status};{formattedDate}");
				}
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Не удалось сохранить задачи для пользователя {userId}.", ex);
			}
		}

		public IEnumerable<TodoItem> LoadTodos(Guid userId)
		{
			var filePath = GetTodoFilePath(userId);
			if (!File.Exists(filePath))
			{
				return Enumerable.Empty<TodoItem>();
			}

			var loadedItems = new List<TodoItem>();
			try
			{
				using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				using var bufferedStream = new BufferedStream(fileStream);
				using var aes = Aes.Create();
				using var decryptor = aes.CreateDecryptor(_key, _iv);
				using var cryptoStream = new CryptoStream(bufferedStream, decryptor, CryptoStreamMode.Read);
				using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					var parts = line.Split(';');
					if (parts.Length == 4)
					{
						var item = new TodoItem
						{
							Id = int.Parse(parts[0]),
							Text = parts[1].Replace("\\n", "\n"),
							Status = Enum.Parse<TodoStatus>(parts[2], true),
							LastUpdated = DateTime.Parse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
						};
						loadedItems.Add(item);
					}
				}
				return loadedItems;
			}
			catch (CryptographicException ex)
			{
				throw new DataStorageException($"Не удалось расшифровать файл задач для пользователя {userId}. Файл может быть поврежден.", ex);
			}
			catch (Exception ex)
			{
				throw new DataStorageException($"Не удалось загрузить задачи для пользователя {userId}. Файл может быть поврежден.", ex);
			}
		}

		private string GetTodoFilePath(Guid userId)
		{
			return Path.Combine(_dataDirectory, $"todos_{userId}.dat");
		}
	}
}